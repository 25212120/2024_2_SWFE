using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MagicEffect
{
    public float MagicAttack;    // ���� ���ݷ�
}

[System.Serializable]
public class PlayerMagic
{
    public PlayerMagicType magicType;   // ���� ����
    public MagicEffect effect;          // ���� ȿ��
    public int level = 1;               // ������ ����
    public float experience = 0f;       // ���� ����ġ
    public float maxExperience = 100f;  // ���� �ִ� ����ġ
    public bool MagicSkillTree_Wood = false;
    public bool MagicSkillTree_Fire = false;
    public bool MagicSkillTree_Ice = false;
    public bool MagicSkillTree_Sand = false;

    public SkillTree skilltree;
    // ���� ������ �� ȿ�� ����
    public void LevelUp()
    {
        level++;
        effect.MagicAttack += 5f;
        maxExperience *= 2.0f;
        experience = 0f;
        Debug.Log($"{magicType} ���� ������! ���� ����: {level}, ���ݷ�: {effect.MagicAttack}");
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
                Debug.Log("Wood ���� ��ų Ʈ�� Ȱ��ȭ!");
                skilltree.UnlockSkillTree(magicType);
                OnSkillTreeUnlocked?.Invoke(PlayerMagicType.Wood);
                break;
            case PlayerMagicType.Fire:
                MagicSkillTree_Fire = true;
                Debug.Log("Fire ���� ��ų Ʈ�� Ȱ��ȭ!");
                skilltree.UnlockSkillTree(magicType);
                OnSkillTreeUnlocked?.Invoke(PlayerMagicType.Fire);

                break;
            case PlayerMagicType.Ice:
                MagicSkillTree_Ice = true;
                Debug.Log("Ice ���� ��ų Ʈ�� Ȱ��ȭ!");
                skilltree.UnlockSkillTree(magicType);
                OnSkillTreeUnlocked?.Invoke(PlayerMagicType.Ice);
                break;
            case PlayerMagicType.Sand:
                MagicSkillTree_Sand = true;
                Debug.Log("Sand ���� ��ų Ʈ�� Ȱ��ȭ!");
                skilltree.UnlockSkillTree(magicType);
                OnSkillTreeUnlocked?.Invoke(PlayerMagicType.Sand);
                break;
            default:
                break;
        }
    }

    // ����ġ �߰� �� ������ ó��
    public void AddExperience(float amount)
    {
        experience += amount;

        if (experience >= maxExperience)
        {
            experience = 0f;
            LevelUp();  // ������
        }
    }
}

public class PlayerInventory : MonoBehaviour
{
    public PlayerMagic[] playerMagics = new PlayerMagic[4];  // 4���� ���� ����
    private PlayerStat playerstat;

    // ������ ������ ������ ������ ������
    private PlayerMagic previousMagic1;
    private PlayerMagic previousMagic2;




    private void Awake()
    {
        playerstat = GetComponent<PlayerStat>();

        if (playerstat == null || playerstat.statData == null)
        {
            Debug.LogError("PlayerStat ������Ʈ�� ã�� �� �����ϴ�!");
            return;
        }
        // �ʱ� ���� ���� (����)
        playerMagics[0] = new PlayerMagic { magicType = PlayerMagicType.Wood, effect = new MagicEffect { MagicAttack = 1 } };
        playerMagics[1] = new PlayerMagic { magicType = PlayerMagicType.Fire, effect = new MagicEffect { MagicAttack = 1 } };
        playerMagics[2] = new PlayerMagic { magicType = PlayerMagicType.Ice, effect = new MagicEffect { MagicAttack = 1 } };
        playerMagics[3] = new PlayerMagic { magicType = PlayerMagicType.Sand, effect = new MagicEffect { MagicAttack = 1 } };

        ApplyMagicEffect(playerMagics[0]);
        ApplyMagicEffect(playerMagics[1]);
        ApplyMagicEffect(playerMagics[2]);
        ApplyMagicEffect(playerMagics[3]);

        Debug.Log("������ �ʱ�ȭ�Ǿ����ϴ�.");
    }


    // ���� ȿ���� ����
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

        Debug.Log($"{magic.magicType} ���� ȿ�� ����: ���ݷ� {magic.effect.MagicAttack}");
    }

    public void Update()
    {
    }
}

