using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameras : MonoBehaviour
{
    string playerPrefabName;
    private PhotonView pv;
    GameObject player;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        if (GameSettings.IsMultiplayer == true)
        {
            playerPrefabName = PhotonNetwork.IsMasterClient ? "Player 1(Clone)" : "Player 2(Clone)";
        }
        else
        {
            playerPrefabName = "Player 1(Clone)";
        }

        player = GameObject.Find(playerPrefabName);
    }

    private void Start()
    {
        if (pv.IsMine == true)
        {
            if (playerPrefabName == "Player 1(Clone)")
            {
                player = GameObject.Find(playerPrefabName);

            }
            else
            {
                player = GameObject.Find(playerPrefabName);
            }

            CinemachineVirtualCamera[] virtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();
            foreach (var cam in virtualCameras)
            {
                if (cam.CompareTag("IsoCam") || cam.CompareTag("TopCam"))
                {
                    cam.Follow = player.transform;
                }
            }
        }
    }
}
