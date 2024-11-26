using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider hpBarSlider; // HP 바 슬라이더
    [SerializeField] public PlayerStat player; // 플레이어 스크립트 참

    private void Start()
    {
        if (hpBarSlider == null)
        {
            Debug.LogError("HP 바 슬라이더가 설정되지 않았습니다.");
        }

        if (player == null)
        {
            Debug.LogError("플레이어 스크립트가 설정되지 않았습니다.");
        }

        // HP 바의 최대값을 플레이어의 최대 HP로 설정
        hpBarSlider.maxValue = player.GetMaxHp();
        hpBarSlider.value = player.GetCurrentHP();
    }

    private void Update()
    {
        UpdateHPBar();
        
    }

    private void UpdateHPBar()
    {
        // 플레이어의 현재 HP 값을 슬라이더에 반영
        hpBarSlider.value = player.GetCurrentHP();
    }
}


