using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Google.XR.Cardboard;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MeetingManager : MonoBehaviour
{

    private bool devMode;

    public int localCharacterIndex = 0;

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
        localCharacterIndex = PhotonNetwork.LocalPlayer.ActorNumber;
        GameObject playerT = PhotonNetwork.Instantiate("Player", transform.position, Quaternion.identity);
        Hashtable h = new Hashtable();
        h.Add("Character", localCharacterIndex);
        PhotonNetwork.SetPlayerCustomProperties(h);
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
