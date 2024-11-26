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


    private void Start()
    {
        pv = GetComponent<PhotonView>();
        
        if (GameSettings.IsMultiplayer == true) {
            playerName = PhotonNetwork.IsMasterClient ? "Player 1(Clone)" : "Player 2(Clone)";
        }
        else
        {
            playerName = "Player 1(Clone)";
        }

        if (GameSettings.IsMultiplayer == false)
        {
            UICanvas = GameObject.FindWithTag("UICanvas");
     

            ShowUIOnEnter[] showUIOnEnters = FindObjectsOfType<ShowUIOnEnter>();
            foreach (var script in showUIOnEnters)
            {
                script.player = gameObject;
            }

            Transform profile = UICanvas.transform.Find("Profile");

            profile.GetComponentInChildren<HealthBar>().player = gameObject.GetComponent<PlayerStat>();
            profile.GetComponentInChildren<EXPBar>().player = gameObject.GetComponent<ExpManager>();
            profile.GetComponentInChildren<EXPLEVEL>().player = gameObject.GetComponent<PlayerStat>();
        }
        else
        {
            if (pv.IsMine == true)
            {
                UICanvas = GameObject.FindWithTag("UICanvas");

                ShowUIOnEnter[] showUIOnEnters = FindObjectsOfType<ShowUIOnEnter>();
                foreach (var script in showUIOnEnters)
                {
                    script.player = gameObject;
                }

                Transform profile = UICanvas.transform.Find("Profile");
                profile.GetComponentInChildren<HealthBar>().player = gameObject.GetComponent<PlayerStat>();
                profile.GetComponentInChildren<EXPBar>().player = gameObject.GetComponent<ExpManager>();
                profile.GetComponentInChildren<EXPLEVEL>().player = gameObject.GetComponent<PlayerStat>();
            }
        }   
    }



}
