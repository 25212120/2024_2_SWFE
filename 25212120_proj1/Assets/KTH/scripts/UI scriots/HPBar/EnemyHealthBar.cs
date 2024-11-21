using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private float visibleDuration = 2.0f; // HP �ٰ� ���̴� �ð�
    [SerializeField] private Monster_1 enemy; // �÷��̾� ��ũ��Ʈ ��
    private BaseMonster monster;
 
    private Coroutine hideCoroutine;

    void Start()
    {

        healthSlider.maxValue = enemy.GetMaxHP();
        healthSlider.value = enemy.GetCurrentHP();
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
        healthSlider.value = enemy.GetCurrentHP();
        
    }

}
