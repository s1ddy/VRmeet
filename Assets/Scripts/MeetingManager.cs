using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Google.XR.Cardboard;

public class MeetingManager : MonoBehaviour
{

    private bool devMode;

    // Start is called before the first frame update
    void Start()
    {
        devMode = Application.platform != RuntimePlatform.Android;

        if (!devMode)
        {
            if (!Api.HasDeviceParams())
            {
                Api.ScanDeviceParams();
            }
        }

        PhotonNetwork.Instantiate("Player", transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (!devMode)
        {
            if (Api.IsGearButtonPressed)
            {
                Api.ScanDeviceParams();
            }

            if (Api.IsCloseButtonPressed)
            {
                Application.Quit();
            }

            if (Api.HasNewDeviceParams())
            {
                Api.ReloadDeviceParams();
            }

            Api.UpdateScreenParams();
        }
    }
}
