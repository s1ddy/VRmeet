using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice;

public class Player : MonoBehaviourPun
{
    public float speed;

    [SerializeField]
    private float sens = 25f;

    public Animator anim;

    private bool devMode;

    private MeetingManager mm;


    private float xRot;

    private Transform cam;

    [SerializeField]
    private List<Avatar> avatarList = new List<Avatar>();

    // Start is called before the first frame update
    void Start()
    {
        cam = transform.GetChild(0);
        mm = FindObjectOfType<MeetingManager>();
        devMode = Application.platform != RuntimePlatform.Android;
        anim = GetComponent<Animator>();
        if (!photonView.IsMine)
        {
            cam.gameObject.SetActive(false);
            
        }
        else
        {
            cam.gameObject.SetActive(true);
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
            if (Input.GetButton("Fire1"))
            {
                MoveForward();
            }
            else
            {
                anim.SetFloat("Speed", 0);
            }

            if (devMode)
            {
                CameraLookAround();
            }
        }

    }

    private void MoveForward()
    {
        anim.SetFloat("Speed", 1);
        transform.position += new Vector3(cam.forward.x, 0, cam.forward.z) * Time.deltaTime * speed;
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
