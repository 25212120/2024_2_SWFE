using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class clork : MonoBehaviour
{
    public Image clockHand; // 시계의 시계침 이미지
    public float dayDuration = 60f; // 낮과 밤의 전체 주기 (초 단위)

    private float time; // 현재 시간 (0 ~ 1 범위, 0: 자정, 0.5: 정오, 1: 자정)

    void Start()
    {
        time = 0f;
        UpdateClockUI();
    }

    void Update()
    {
        // 낮밤 주기에 따라 시간 경과 (0에서 1까지 증가)
        time += Time.deltaTime / dayDuration;
        if (time >= 1f)
        {
            time -= 1f; // 1을 초과하면 다시 0으로 돌아감
        }

        UpdateClockUI();
    }

    private void UpdateClockUI()
    {
        // 시계침 회전 (360도 기준)
        if (clockHand != null)
        {
            clockHand.rectTransform.rotation = Quaternion.Euler(0f, 0f, -time * 360f);
        }
    }
}

