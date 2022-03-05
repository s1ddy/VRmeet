using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayer : MonoBehaviour
{
    public float speed;

    [SerializeField]
    private float sens = 25f;

    public Animator anim;

    private bool devMode;

    private LobbyManager lm;

    private float xRot;

    private Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = transform.GetChild(0).GetChild(0);
        cam.gameObject.SetActive(true);
        lm = FindObjectOfType<LobbyManager>();
        devMode = Application.platform != RuntimePlatform.Android;
        anim = GetComponent<Animator>();
        transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5, 1 << 7))
        {
            if (hit.collider.gameObject.tag == "Chair")
            {
                transform.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Light>().color = lm.hitRet;
                if (Input.GetButtonDown("Fire1"))
                {
                    
                }
            }
            else
            {
                transform.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Light>().color = lm.defaultRet;
            }

        }
        else
        {
            transform.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Light>().color = lm.defaultRet;
        }
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

    }
}
