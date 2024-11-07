using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarBeamHandler : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        impulseSource.GenerateImpulse();
    }
}
