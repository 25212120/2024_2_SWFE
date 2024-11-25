using System.Collections;
using UnityEngine;
using TMPro;


public class GameTimeManage : MonoBehaviour
{
    public float dayDuration;
    private int dayCount = 0;
    public Light directionalLight;
    public TextMeshProUGUI dayText;

    void Start()
    {
        //dayDuration = GameManager.instance.dayDuration;
        StartCoroutine(CountDays());
    }

    IEnumerator CountDays()
    {
        while (true)
        {
            yield return new WaitForSeconds(dayDuration);
            dayCount++;
            Debug.Log("Day: " + dayCount);
            dayText.text = "Day: " + dayCount.ToString();
        }
    }


    private float time;
    void Update()
    {
        time += Time.deltaTime;
        float timeNormalized = (time % dayDuration) / dayDuration;

        directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timeNormalized * 360f) - 90f, 170f, 0));
    }
}
