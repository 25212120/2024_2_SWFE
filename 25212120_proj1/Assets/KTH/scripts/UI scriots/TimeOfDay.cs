using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDay : MonoBehaviour
{
    public Light directionalLight;
    public float dayDuration = 120f; // 하루가 120초라고 가정

    private float time;

    void Update()
    {
        time += Time.deltaTime;
        float timeNormalized = (time % dayDuration) / dayDuration;

        // 낮과 밤을 조정
        directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timeNormalized * 360f) - 90f, 170f, 0));
    }
}
