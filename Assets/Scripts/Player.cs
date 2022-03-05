using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Linq;

public class Player : MonoBehaviourPun, IPunObservable
{
    public float speed;

    [SerializeField]
    private float sens = 25f;

    public Animator anim;

    private bool devMode;

    private Rigidbody rb;

    private MeetingManager mm;

    private Whiteboard whiteboardObj;

    private Vector2 lastWhiteboardDrawPos;

    private float xRot;

    private bool touchedWhiteboardLastFrame = false;

    private Transform cam;

    private Vector2 whiteboardDrawPos;

    [SerializeField]
    private List<GameObject> headList = new List<GameObject>();

    private GameObject head;

    [SerializeField]
    private List<Avatar> avatarList = new List<Avatar>();

    // Start is called before the first frame update
    void Start()
    {
        cam = transform.GetChild(0).GetChild(0);
        mm = FindObjectOfType<MeetingManager>();
        devMode = Application.platform != RuntimePlatform.Android;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        if (!photonView.IsMine)
        {
            cam.gameObject.SetActive(false);
            GetComponent<Recorder>().enabled = false;
        }
        else
        {
            cam.gameObject.SetActive(true);
            GetComponent<Speaker>().enabled = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
            //anim = transform.GetChild(1).GetChild(mm.localCharacterIndex).GetComponent<Animator>();

        }


    }

    void SetLayerRecursively(GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    void SetCharacter()
    {
        
        if (!photonView.IsMine)
        {
            foreach (Transform child in transform)
            {
                SetLayerRecursively(child.gameObject, 0);
            }
        }

        if (gameObject.GetPhotonView().Owner.CustomProperties != null)
        {
            var cn = (int)gameObject.GetPhotonView().Owner.CustomProperties["Character"];

            for (int i = 0; i < 5; i++)
            {
                if (cn == i)
                {
                    transform.GetChild(1).GetChild(i).gameObject.SetActive(true);
                    anim.avatar = avatarList[i];
                    head = headList[i];
                }
                else
                {
                    transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        SetCharacter();
        if (photonView.IsMine)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (speed == 0)
                {
                    Unsit();
                }
            }
            
            
            

            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 15, LayerMask.GetMask("Chair", "Whiteboard")))
            {
                if (hit.collider.gameObject.tag == "Chair")
                {
                    transform.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Light>().color = mm.hitRet;
                    if (Input.GetButtonDown("Fire1"))
                    {
                        Sit(hit.collider.gameObject);
                    }
                }
                else if (hit.collider.gameObject.tag == "Whiteboard")
                {
                    transform.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Light>().color = mm.hitRet;
                    if (Input.GetButton("Fire1"))
                    {
                        var whiteb = hit.transform.gameObject.GetComponent<Whiteboard>();
                        whiteboardObj = whiteb;
                        whiteboardDrawPos = new Vector2(hit.textureCoord.x, hit.textureCoord.y);

                        var x = (int)(2048 * hit.textureCoord.x);
                        var y = (int)(1024 * hit.textureCoord.y);

                        if (y < 0 || y > 1024 || x < 0 || x > 2048) return;

                        if (touchedWhiteboardLastFrame)
                        {
                            whiteboardObj.texture.SetPixels(x, y, 20, 20, Enumerable.Repeat(Color.black, 400).ToArray());

                            for (float f = 0.01f; f < 1.00f; f += 0.02f)
                            {
                                var xLerp = (int)Mathf.Lerp(lastWhiteboardDrawPos.x, x, f);
                                var yLerp = (int)Mathf.Lerp(lastWhiteboardDrawPos.y, y, f);
                                whiteboardObj.texture.SetPixels(xLerp, yLerp, 20, 20, Enumerable.Repeat(Color.black, 400).ToArray());
                            }

                            whiteboardObj.texture.Apply();
                        }

                        lastWhiteboardDrawPos = new Vector2(x, y);
                        touchedWhiteboardLastFrame = true;
                        return;
                    }
                }
                else
                {
                    
                    transform.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Light>().color = mm.defaultRet;
                }

            } else
            {
                transform.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Light>().color = mm.defaultRet;
            }

            whiteboardObj = null;
            touchedWhiteboardLastFrame = false;
        }

    }

    private void FixedUpdate()
    {
        if (Input.GetButton("Fire1") && !touchedWhiteboardLastFrame)
        {

            MoveForward();
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }

    }

    private void LateUpdate()
    {
        if (devMode)
        {
            CameraLookAround();
        }
        if (photonView.IsMine)
        {
            head.transform.eulerAngles = cam.eulerAngles;
        }
    }

    private void Unsit()
    {
        anim.SetBool("Sitting", false);
        speed = 4;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        Vector3 posC = transform.position;
        posC.y = 1;
        transform.position = posC;
    }

    private void Sit(GameObject chair)
    {
        anim.SetBool("Sitting", true);
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        Vector3 chairPos = chair.transform.position;
        chairPos.y = 1.35f;
        transform.position = chairPos;
        speed = 0;
    }

    private void MoveForward()
    {
        anim.SetFloat("Speed", 1);
        Vector3 inputPos = new Vector3(cam.forward.x, 0, cam.forward.z);
        transform.position += inputPos * speed * Time.deltaTime;
    }

    private void CameraLookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * sens;
        float mouseY = Input.GetAxis("Mouse Y") * sens;
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        cam.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.Rotate(Vector3.up * mouseX);

        /*
        if (anim.GetBool("Sitting"))
        {
            head.transform.Rotate(Vector3.up * mouseX);
            head.transform.localEulerAngles = new Vector3(xRot, head.transform.localEulerAngles.y, head.transform.localEulerAngles.z);
        } else
        {
            transform.Rotate(Vector3.up * mouseX);
            head.transform.localEulerAngles = new Vector3(xRot, 0, head.transform.localEulerAngles.z);
        }*/

       
        


    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        /*if (stream.IsWriting)
        {
            
            stream.SendNext(whiteboardObj.texture)
        }*/
    }
}
