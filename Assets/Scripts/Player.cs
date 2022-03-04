using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPun
{
    public float speed;

    public float sens = 25f;


    private bool devMode;

    private float xRot;

    private Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = transform.GetChild(0);
        devMode = Application.platform != RuntimePlatform.Android;
        if (!photonView.IsMine)
        {
            cam.gameObject.SetActive(false);
        }
        else
        {
            cam.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            MoveForward();
        }

        if (devMode)
        {
            CameraLookAround();
        }
    }

    private void MoveForward()
    {
        transform.position += new Vector3(cam.forward.x, 0, cam.forward.z) * Time.deltaTime * speed;
    }

    private void CameraLookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        cam.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }
}
