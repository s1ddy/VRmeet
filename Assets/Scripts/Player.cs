using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPun
{
    public float speed;

    [SerializeField]
    private float sens = 25f;

    private Animator anim;

    private bool devMode;

    private MeetingManager mm;


    private float xRot;

    private Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = transform.GetChild(0);
        mm = FindObjectOfType<MeetingManager>();
        devMode = Application.platform != RuntimePlatform.Android;
        if (!photonView.IsMine)
        {
            cam.gameObject.SetActive(false);
        }
        else
        {
            cam.gameObject.SetActive(true);
            anim = transform.GetChild(1).GetChild(mm.localCharacterIndex).GetComponent<Animator>();
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
                SetLayerRecursively(child.gameObject, 6);
            }
        }
        var cn = (int)gameObject.GetPhotonView().Owner.CustomProperties["Character"];
        foreach (Transform child in transform.GetChild(1))
        {
            child.gameObject.SetActive(false);
        }
        transform.GetChild(1).GetChild(cn).gameObject.SetActive(true);
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
