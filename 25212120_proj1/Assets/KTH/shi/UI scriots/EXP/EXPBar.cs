using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EXPBar : MonoBehaviour
{
    [SerializeField] private Slider EXPBarSlider; // HP �� �����̴�
    [SerializeField] public ExpManager player; // �÷��̾� ��ũ��Ʈ ��

    private void Awake()
    {
        StartCoroutine(initializePlayer());
    }

    IEnumerator initializePlayer()
    {
        while (GameManager.instance.player == null)
        {
            yield return null;
        }
        Debug.Log("canvas");
        player = GameManager.instance.player.GetComponent<ExpManager>();
        float maxEXP = player.EXPMAX();
        EXPBarSlider.value = player.GetCurrentEXP();
    }


    private void Update()
    {
        if (player != null)
        {
            UpdateEXPBar();
            player.AddExp(10);
        }
    }

    private void UpdateEXPBar()
    {
        float maxEXP = player.EXPMAX();
        // �÷��̾��� ���� HP ���� �����̴��� �ݿ�
        EXPBarSlider.value = player.GetCurrentEXP()/ maxEXP;
    }
}


