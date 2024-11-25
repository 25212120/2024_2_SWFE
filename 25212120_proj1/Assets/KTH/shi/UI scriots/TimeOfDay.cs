using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDay : MonoBehaviour
{
    public Light directionalLight;
    public float dayDuration; 

    private float time;

    private void Start()
    {
        //dayDuration = GameManager.instance.dayDuration;
    }

    void Update()
    {
        time += Time.deltaTime;
        float timeNormalized = (time % dayDuration) / dayDuration;

        // ³·°ú ¹ãÀ» Á¶Á¤
        directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timeNormalized * 360f) - 90f, 170f, 0));
    }
}
