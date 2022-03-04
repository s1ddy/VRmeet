using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.XR.Cardboard;
using Photon.Pun;
public class LobbyManager : MonoBehaviourPunCallbacks
{

    private bool devMode;


    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        devMode = Application.platform != RuntimePlatform.Android;

        if (!devMode)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Screen.brightness = 1.0f;

            // Checks if the device parameters are stored and scans them if not.
            if (!Api.HasDeviceParams())
            {
                Api.ScanDeviceParams();
            }
        }

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "0.1";
        }

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

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
