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
    public bool MagicSkillTree_Wood = false;
    public bool MagicSkillTree_Fire = false;
    public bool MagicSkillTree_Ice = false;
    public bool MagicSkillTree_Sand = false;

    public SkillTree skilltree;
    // 마법 레벨업 시 효과 증가
    public void LevelUp()
    {
        level++;
        effect.MagicAttack += 5f;
        maxExperience *= 2.0f;
        experience = 0f;
        Debug.Log($"{magicType} 마법 레벨업! 현재 레벨: {level}, 공격력: {effect.MagicAttack}");
        if (level >= 5)
        {
            UnlockSkillTree();
            
        }
    }
    private void UnlockSkillTree()
    {
        switch (magicType)
        {
            case PlayerMagicType.Wood:
                MagicSkillTree_Wood = true;
                Debug.Log("Wood 마법 스킬 트리 활성화!");
                skilltree.UnlockSkillTree(magicType);
                OnSkillTreeUnlocked?.Invoke(PlayerMagicType.Wood);
                break;
            case PlayerMagicType.Fire:
                MagicSkillTree_Fire = true;
                Debug.Log("Fire 마법 스킬 트리 활성화!");
                skilltree.UnlockSkillTree(magicType);
                OnSkillTreeUnlocked?.Invoke(PlayerMagicType.Fire);

                break;
            case PlayerMagicType.Ice:
                MagicSkillTree_Ice = true;
                Debug.Log("Ice 마법 스킬 트리 활성화!");
                skilltree.UnlockSkillTree(magicType);
                OnSkillTreeUnlocked?.Invoke(PlayerMagicType.Ice);
                break;
            case PlayerMagicType.Sand:
                MagicSkillTree_Sand = true;
                Debug.Log("Sand 마법 스킬 트리 활성화!");
                skilltree.UnlockSkillTree(magicType);
                OnSkillTreeUnlocked?.Invoke(PlayerMagicType.Sand);
                break;
            default:
                break;
        }
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




    private void Awake()
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

        ApplyMagicEffect(playerMagics[0]);
        ApplyMagicEffect(playerMagics[1]);
        ApplyMagicEffect(playerMagics[2]);
        ApplyMagicEffect(playerMagics[3]);

        Debug.Log("마법이 초기화되었습니다.");
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

    public void Update()
    {
    }
}

