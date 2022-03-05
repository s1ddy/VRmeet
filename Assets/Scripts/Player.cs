using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;

public class Player : MonoBehaviourPun
{
    public float speed;

    [SerializeField]
    private float sens = 25f;

    public Animator anim;

    private bool devMode;

    private MeetingManager mm;

    private Rigidbody rb;


    private float xRot;

    private Transform cam;

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
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        if (!photonView.IsMine)
        {
            cam.gameObject.SetActive(false);
            GetComponent<Recorder>().enabled = false;
        }
        else
        {
            cam.gameObject.SetActive(true);
            GetComponent<Speaker>().enabled = false;
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
                    //head = headList[i];
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
            
            
            if (devMode)
            {
                CameraLookAround();
            }

            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5, 1<<7))
            {
                if (hit.collider.gameObject.tag == "Chair")
                {
                    transform.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Light>().color = mm.hitRet;
                    if (Input.GetButtonDown("Fire1"))
                    {
                        Sit(hit.collider.gameObject);
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


        }

    }

    private void FixedUpdate()
    {
        if (Input.GetButton("Fire1"))
        {

            MoveForward();
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }

    }

    private void Unsit()
    {
        anim.SetBool("Sitting", false);
        speed = 4;
        Vector3 posC = transform.position;
        posC.y = 1;
        transform.position = posC;
    }

    private void Sit(GameObject chair)
    {
        anim.SetBool("Sitting", true);
        Vector3 chairPos = chair.transform.position;
        chairPos.y = 1.35f;
        transform.position = chairPos;
        speed = 0;
    }

    private void MoveForward()
    {
        anim.SetFloat("Speed", 1);
        Vector3 inputPos = new Vector3(cam.forward.x, 0, cam.forward.z);
        rb.MovePosition(transform.position + (inputPos * speed * Time.deltaTime));
    }

    private void CameraLookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * sens;
        float mouseY = Input.GetAxis("Mouse Y") * sens;
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        cam.localRotation = Quaternion.Euler(xRot, 0, 0);
        //head.transform.localRotation = cam.localRotation;
        transform.Rotate(Vector3.up * mouseX);
    }
}
