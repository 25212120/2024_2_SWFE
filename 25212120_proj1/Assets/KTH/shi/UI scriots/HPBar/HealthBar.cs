using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider hpBarSlider; // HP 바 슬라이더
    [SerializeField] public PlayerStat player; // 플레이어 스크립트 참

    private void Awake()
    {
        StartCoroutine(initializePlayer());
    }

    IEnumerator initializePlayer()
    {
        while (GameManager.instance == null)
        {
            Debug.Log("GameManager.instance를 기다리는 중...");
            yield return null;
        }
        Debug.Log("GameManager.instance가 초기화되었습니다.");

        while (GameManager.instance.player == null)
        {
            Debug.Log("GameManager.instance.player를 기다리는 중...");
            yield return null;
        }
        Debug.Log("GameManager.instance.player가 할당되었습니다.");

        player = GameManager.instance.player.GetComponent<PlayerStat>();
        if (player == null)
        {
            Debug.LogError("PlayerStat 컴포넌트를 가져올 수 없습니다.");
            yield break;
        }

        if (hpBarSlider == null)
        {
            Debug.LogError("hpBarSlider가 할당되지 않았습니다.");
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
        // 플레이어의 현재 HP 값을 슬라이더에 반영
        hpBarSlider.value = player.GetCurrentHP();
    }
}


