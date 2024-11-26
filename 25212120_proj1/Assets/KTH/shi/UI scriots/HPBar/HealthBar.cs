using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider hpBarSlider; // HP �� �����̴�
    [SerializeField] public PlayerStat player; // �÷��̾� ��ũ��Ʈ ��

    private void Start()
    {
        if (hpBarSlider == null)
        {
            Debug.LogError("HP �� �����̴��� �������� �ʾҽ��ϴ�.");
        }

        if (player == null)
        {
            Debug.LogError("�÷��̾� ��ũ��Ʈ�� �������� �ʾҽ��ϴ�.");
        }

        // HP ���� �ִ밪�� �÷��̾��� �ִ� HP�� ����
        hpBarSlider.maxValue = player.GetMaxHp();
        hpBarSlider.value = player.GetCurrentHP();
    }

    private void Update()
    {
        UpdateHPBar();
        
    }

    private void UpdateHPBar()
    {
        // �÷��̾��� ���� HP ���� �����̴��� �ݿ�
        hpBarSlider.value = player.GetCurrentHP();
    }
}


