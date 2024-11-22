using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private float visibleDuration = 2.0f; // HP 바가 보이는 시간
    [SerializeField] private Monster_1 enemy; // 플레이어 스크립트 참
    private BaseMonster monster;
 
    private Coroutine hideCoroutine;

    void Start()
    {

        healthSlider.maxValue = enemy.GetMaxHP();
        healthSlider.value = enemy.GetCurrentHP();
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
        healthSlider.value = enemy.GetCurrentHP();
        
    }

}
