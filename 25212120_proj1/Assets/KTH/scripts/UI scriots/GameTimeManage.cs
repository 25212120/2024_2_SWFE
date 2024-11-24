using System.Collections;
using UnityEngine;
using TMPro;


public class GameTimeManage : MonoBehaviour
{
    public float dayDuration = 10f; // �Ϸ翡 �ش��ϴ� �ð� (�� ����)
    private int dayCount = 0;
    public TextMeshProUGUI dayText; // UI �ؽ�Ʈ ������Ʈ ����

    void Start()
    {
        // ���� �ð��� ������ �Ϸ簡 ī��Ʈ�ǵ��� �ݺ� ȣ��
        StartCoroutine(CountDays());
    }

    IEnumerator CountDays()
    {
        while (true)
        {
            yield return new WaitForSeconds(dayDuration);
            dayCount++;
            Debug.Log("Day: " + dayCount);
            dayText.text = "Day: " + dayCount.ToString(); // �ؽ�Ʈ UI�� ǥ��
        }
    }
}
