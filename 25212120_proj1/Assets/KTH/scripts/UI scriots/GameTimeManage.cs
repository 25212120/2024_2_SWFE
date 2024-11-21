using System.Collections;
using UnityEngine;
using TMPro;


public class GameTimeManage : MonoBehaviour
{
    public float dayDuration = 10f; // 하루에 해당하는 시간 (초 단위)
    private int dayCount = 0;
    public TextMeshProUGUI dayText; // UI 텍스트 컴포넌트 연결

    void Start()
    {
        // 일정 시간이 지나면 하루가 카운트되도록 반복 호출
        StartCoroutine(CountDays());
    }

    IEnumerator CountDays()
    {
        while (true)
        {
            yield return new WaitForSeconds(dayDuration);
            dayCount++;
            Debug.Log("Day: " + dayCount);
            dayText.text = "Day: " + dayCount.ToString(); // 텍스트 UI에 표시
        }
    }
}
