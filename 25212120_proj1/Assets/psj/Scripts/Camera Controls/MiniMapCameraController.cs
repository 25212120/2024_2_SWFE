using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MiniMapCameraController : MonoBehaviour
{
    public GameObject player;
    private float smoothSpeed = 0.125f;


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
