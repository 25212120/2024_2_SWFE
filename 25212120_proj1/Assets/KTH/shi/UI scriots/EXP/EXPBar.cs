using UnityEngine;
using UnityEngine.UI;

public class EXPBar : MonoBehaviour
{
    [SerializeField] private Slider EXPBarSlider; // HP 바 슬라이더
    [SerializeField] private ExpManager player; // 플레이어 스크립트 참
    

    private void Start()
    {
        if (EXPBarSlider == null)
        {
            Debug.LogError("HP 바 슬라이더가 설정되지 않았습니다.");
        }

        if (player == null)
        {
            Debug.LogError("플레이어 스크립트가 설정되지 않았습니다.");
        }
        float maxEXP = player.EXPMAX();
        // HP 바의 최대값을 플레이어의 최대 HP로 설정
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
        // 플레이어의 현재 HP 값을 슬라이더에 반영
        EXPBarSlider.value = player.GetCurrentEXP()/ maxEXP;
    }
}


