using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EXPBar : MonoBehaviour
{
    [SerializeField] private Slider EXPBarSlider; // HP 바 슬라이더
    [SerializeField] public ExpManager player; // 플레이어 스크립트 참

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
        // 플레이어의 현재 HP 값을 슬라이더에 반영
        EXPBarSlider.value = player.GetCurrentEXP()/ maxEXP;
    }
}


