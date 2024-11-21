using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private StatData statData;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private float visibleDuration = 2.0f; // HP 바가 보이는 시간

    private Coroutine hideCoroutine;

    void Start()
    {
        healthSlider.maxValue = statData.hpMax;
        healthSlider.value = statData.HpCurrent;
        healthSlider.gameObject.SetActive(false); // 처음엔 비활성화
    }

    public void ShowHealthBar()
    {
        healthSlider.gameObject.SetActive(true);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(visibleDuration);
        healthSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        healthSlider.value = statData.HpCurrent;
    }

}
