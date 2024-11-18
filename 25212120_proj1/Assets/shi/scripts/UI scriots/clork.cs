using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class clork : MonoBehaviour
{
    public Image clockHand; // �ð��� �ð�ħ �̹���
    public float dayDuration = 60f; // ���� ���� ��ü �ֱ� (�� ����)

    private float time; // ���� �ð� (0 ~ 1 ����, 0: ����, 0.5: ����, 1: ����)

    void Start()
    {
        time = 0f;
        UpdateClockUI();
    }

    void Update()
    {
        // ���� �ֱ⿡ ���� �ð� ��� (0���� 1���� ����)
        time += Time.deltaTime / dayDuration;
        if (time >= 1f)
        {
            time -= 1f; // 1�� �ʰ��ϸ� �ٽ� 0���� ���ư�
        }

        UpdateClockUI();
    }

    private void UpdateClockUI()
    {
        // �ð�ħ ȸ�� (360�� ����)
        if (clockHand != null)
        {
            clockHand.rectTransform.rotation = Quaternion.Euler(0f, 0f, -time * 360f);
        }
    }
}

