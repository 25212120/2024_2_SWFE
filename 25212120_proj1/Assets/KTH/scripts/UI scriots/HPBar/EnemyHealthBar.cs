using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private StatData statData;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private float visibleDuration = 2.0f; // HP �ٰ� ���̴� �ð�

    private Coroutine hideCoroutine;

    void Start()
    {
        healthSlider.maxValue = statData.hpMax;
        healthSlider.value = statData.HpCurrent;
        healthSlider.gameObject.SetActive(false); // ó���� ��Ȱ��ȭ
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
