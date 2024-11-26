using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCameraController : MonoBehaviour
{
    string playerPrefabName = PhotonNetwork.IsMasterClient ? "P1(Clone)" : "P2(Clone)";
    GameObject player;
    private float smoothSpeed = 0.125f;

    private void Start()
    {
        if (playerPrefabName == "P1(Clone)")
        {
            player = GameObject.Find(playerPrefabName);
            
        }
        else
        {
            player = GameObject.Find(playerPrefabName);

        }
    }

    private void LateUpdate()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        Vector3 desiredPosition = player.transform.position;
        desiredPosition.y = transform.position.y;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }


}
