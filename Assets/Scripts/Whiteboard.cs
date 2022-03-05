using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    public Texture2D texture;

    // Start is called before the first frame update
    void Start()
    {
        texture = new Texture2D(2048, 1024);
        GetComponent<Renderer>().material.mainTexture = texture;
    }
}
