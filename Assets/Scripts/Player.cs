using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed; 
    private bool devMode;

    private Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = transform.GetChild(0);
        devMode = Application.platform != RuntimePlatform.Android;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            MoveForward();
        }
    }

    private void MoveForward()
    {
        transform.position += new Vector3(cam.forward.x, 0, cam.forward.z) * Time.deltaTime * speed;
    }
}
