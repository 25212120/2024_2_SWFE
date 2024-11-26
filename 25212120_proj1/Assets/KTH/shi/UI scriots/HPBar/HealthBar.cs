using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider hpBarSlider; // HP �� �����̴�
    [SerializeField] public PlayerStat player; // �÷��̾� ��ũ��Ʈ ��

    private void Awake()
    {
        StartCoroutine(initializePlayer());
    }

    IEnumerator initializePlayer()
    {
        while (GameManager.instance == null)
        {
            Debug.Log("GameManager.instance�� ��ٸ��� ��...");
            yield return null;
        }
        Debug.Log("GameManager.instance�� �ʱ�ȭ�Ǿ����ϴ�.");

        while (GameManager.instance.player == null)
        {
            Debug.Log("GameManager.instance.player�� ��ٸ��� ��...");
            yield return null;
        }
        Debug.Log("GameManager.instance.player�� �Ҵ�Ǿ����ϴ�.");

        player = GameManager.instance.player.GetComponent<PlayerStat>();
        if (player == null)
        {
            Debug.LogError("PlayerStat ������Ʈ�� ������ �� �����ϴ�.");
            yield break;
        }

        if (hpBarSlider == null)
        {
            Debug.LogError("hpBarSlider�� �Ҵ���� �ʾҽ��ϴ�.");
            yield break;
        }

        hpBarSlider.maxValue = player.GetMaxHp();
        hpBarSlider.value = player.GetCurrentHP();
    }



    private void Update()
    {
        if (player != null)
        {
            UpdateHPBar();
        }
    }

    private void UpdateHPBar()
    {
        // �÷��̾��� ���� HP ���� �����̴��� �ݿ�
        hpBarSlider.value = player.GetCurrentHP();
    }
}


