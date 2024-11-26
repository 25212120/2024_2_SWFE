using UnityEngine;
using UnityEngine.UI;

public class EXPBar : MonoBehaviour
{
    [SerializeField] private Slider EXPBarSlider; // HP �� �����̴�
    [SerializeField] private ExpManager player; // �÷��̾� ��ũ��Ʈ ��
    

    private void Start()
    {
        if (EXPBarSlider == null)
        {
            Debug.LogError("HP �� �����̴��� �������� �ʾҽ��ϴ�.");
        }

        if (player == null)
        {
            Debug.LogError("�÷��̾� ��ũ��Ʈ�� �������� �ʾҽ��ϴ�.");
        }
        float maxEXP = player.EXPMAX();
        // HP ���� �ִ밪�� �÷��̾��� �ִ� HP�� ����
        EXPBarSlider.value = player.GetCurrentEXP();
    }

    private void Update()
    {
        UpdateEXPBar();
        player.AddExp(10);
    }

    private void UpdateEXPBar()
    {
        float maxEXP = player.EXPMAX();
        // �÷��̾��� ���� HP ���� �����̴��� �ݿ�
        EXPBarSlider.value = player.GetCurrentEXP()/ maxEXP;
    }
}


