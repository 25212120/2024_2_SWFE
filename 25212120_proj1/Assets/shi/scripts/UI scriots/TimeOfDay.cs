using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDay : MonoBehaviour
{
    public Light directionalLight;
    public float dayDuration = 120f; // �Ϸ簡 120�ʶ�� ����

    private float time;

    void Update()
    {
        time += Time.deltaTime;
        float timeNormalized = (time % dayDuration) / dayDuration;

        // ���� ���� ����
        directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timeNormalized * 360f) - 90f, 170f, 0));
    }
}
