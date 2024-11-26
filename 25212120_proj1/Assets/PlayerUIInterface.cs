using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIInterface : MonoBehaviour
{
    private PhotonView pv;
    private GameObject UICanvas;
    private GameObject HPCanvas;
    string playerName;
    GameObject player;


    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (GameSettings.IsMultiplayer == true) {
            playerName = PhotonNetwork.IsMasterClient ? "Player 1(Clone)" : "Player 2(Clone)";
            player = GameObject.Find(playerName);
        }
        else
        {
            player = GameObject.Find("Player 1(Clone)");
        }

        if (GameSettings.IsMultiplayer == false)
        {
            UICanvas = GameObject.FindWithTag("UICanvas");
            HPCanvas = GameObject.FindWithTag("HPCanvas");

            ShowUIOnEnter[] showUIOnEnters = FindObjectsOfType<ShowUIOnEnter>();
            foreach (var script in showUIOnEnters)
            {
                script.player = player;
            }

            UICanvas.GetComponentInChildren<HealthBar>().player = player.GetComponent<PlayerStat>();
            UICanvas.GetComponentInChildren<EXPBar>().player = player.GetComponent<ExpManager>();
            UICanvas.GetComponentInChildren<EXPLEVEL>().player = player.GetComponent<PlayerStat>();
        }
        else
        {
            if (pv.IsMine == true)
            {
                UICanvas = GameObject.FindWithTag("UICanvas");
                HPCanvas = GameObject.FindWithTag("HPCanvas");

                ShowUIOnEnter[] showUIOnEnters = FindObjectsOfType<ShowUIOnEnter>();
                foreach (var script in showUIOnEnters)
                {
                    script.player = player;
                }
                UICanvas.GetComponentInChildren<HealthBar>().player = player.GetComponent<PlayerStat>();
                UICanvas.GetComponentInChildren<EXPBar>().player = player.GetComponent<ExpManager>();
                UICanvas.GetComponentInChildren<EXPLEVEL>().player = player.GetComponent<PlayerStat>();
            }
        }   
    }



}
