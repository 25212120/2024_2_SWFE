using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MagicEffect
{
    public float MagicAttack;    // 마법 공격력
}

[System.Serializable]
public class PlayerMagic
{
    public PlayerMagicType magicType;   // 마법 종류
    public MagicEffect effect;          // 마법 효과
    public int level = 1;               // 마법의 레벨
    public float experience = 0f;       // 마법 경험치
    public float maxExperience = 100f;  // 마법 최대 경험치

    // 마법 레벨업 시 효과 증가
    public void LevelUp()
    {
        level++;
        effect.MagicAttack += 5f;
        maxExperience *= 2.0f;
        experience = 0f;

        Debug.Log($"{magicType} 마법 레벨업! 현재 레벨: {level}, 공격력: {effect.MagicAttack}");
    }

    // 경험치 추가 및 레벨업 처리
    public void AddExperience(float amount)
    {
        experience += amount;

        if (experience >= maxExperience)
        {
            experience = 0f;
            LevelUp();  // 레벨업
        }
    }
}

public class PlayerInventory : MonoBehaviour
{
    public PlayerMagic[] playerMagics = new PlayerMagic[4];  // 4개의 마법 슬롯
    private PlayerStat playerstat;

    // 이전에 적용한 마법을 추적할 변수들
    private PlayerMagic previousMagic1;
    private PlayerMagic previousMagic2;


    private void Start()
    {
        playerstat = GetComponent<PlayerStat>();

        if (playerstat == null || playerstat.statData == null)
        {
            Debug.LogError("PlayerStat 컴포넌트를 찾을 수 없습니다!");
            return;
        }

        // 초기 마법 설정 (예시)
        playerMagics[0] = new PlayerMagic { magicType = PlayerMagicType.Wood, effect = new MagicEffect { MagicAttack = 1 } };
        playerMagics[1] = new PlayerMagic { magicType = PlayerMagicType.Fire, effect = new MagicEffect { MagicAttack = 1 } };
        playerMagics[2] = new PlayerMagic { magicType = PlayerMagicType.Ice, effect = new MagicEffect { MagicAttack = 1 } };
        playerMagics[3] = new PlayerMagic { magicType = PlayerMagicType.Sand, effect = new MagicEffect { MagicAttack = 1 } };

        Debug.Log("마법이 초기화되었습니다.");
    }

    // 마법을 교체하는 메서드
    public void ChangeMagic(PlayerMagicType magicType1, PlayerMagicType magicType2)
    {
        int magic1Index = (int)magicType1;
        int magic2Index = (int)magicType2;

        // magic1과 magic2가 동일한 경우, 하나만 적용
        if (magic1Index == magic2Index && magic1Index != -1)
        {
            ApplyMagicEffect(playerMagics[magic1Index]);
        }
        else
        {
            // 이전에 적용했던 마법 효과 제거
            if (previousMagic1 != null)
                RemoveMagicEffect(previousMagic1);
            if (previousMagic2 != null)
                RemoveMagicEffect(previousMagic2);

            // magic1과 magic2가 다르면 각각 적용
            if (magic1Index != -1)
            {
                ApplyMagicEffect(playerMagics[magic1Index]);
                previousMagic1 = playerMagics[magic1Index]; // 이전 마법1 저장
            }

            if (magic2Index != -1)
            {
                ApplyMagicEffect(playerMagics[magic2Index]);
                previousMagic2 = playerMagics[magic2Index]; // 이전 마법2 저장
            }
        }
    }

    // 마법 효과를 적용
    private void ApplyMagicEffect(PlayerMagic magic)
    {
        switch (magic.magicType)
        {
            case PlayerMagicType.Sand:
                playerstat.statData.baseMagicAttack_Sand += magic.effect.MagicAttack;
                break;
            case PlayerMagicType.Fire:
                playerstat.statData.baseMagicAttack_Fire += magic.effect.MagicAttack;
                break;
            case PlayerMagicType.Ice:
                playerstat.statData.baseMagicAttack_Ice += magic.effect.MagicAttack;
                break;
            case PlayerMagicType.Wood:
                playerstat.statData.baseMagicAttack_Wood += magic.effect.MagicAttack;
                break;
            default:
                break;
        }

        Debug.Log($"{magic.magicType} 마법 효과 적용: 공격력 {magic.effect.MagicAttack}");
    }

    // 마법 효과를 제거하는 메소드
    private void RemoveMagicEffect(PlayerMagic magic)
    {
        switch (magic.magicType)
        {
            case PlayerMagicType.Sand:
                playerstat.statData.baseMagicAttack_Sand -= magic.effect.MagicAttack;
                break;
            case PlayerMagicType.Fire:
                playerstat.statData.baseMagicAttack_Fire -= magic.effect.MagicAttack;
                break;
            case PlayerMagicType.Ice:
                playerstat.statData.baseMagicAttack_Ice -= magic.effect.MagicAttack;
                break;
            case PlayerMagicType.Wood:
                playerstat.statData.baseMagicAttack_Wood -= magic.effect.MagicAttack;
                break;
            default:
                break;
        }

        Debug.Log($"{magic.magicType} 마법 효과 제거: 공격력 {magic.effect.MagicAttack}");
    }

    public void Update()
    {
    }
}

