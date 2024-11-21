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

    // ���� ������ �� ȿ�� ����
    public void LevelUp()
    {
        level++;
        effect.MagicAttack += 5f;
        maxExperience *= 2.0f;
        experience = 0f;

        Debug.Log($"{magicType} ���� ������! ���� ����: {level}, ���ݷ�: {effect.MagicAttack}");
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


    private void Start()
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

        Debug.Log("������ �ʱ�ȭ�Ǿ����ϴ�.");
    }

    // ������ ��ü�ϴ� �޼���
    public void ChangeMagic(PlayerMagicType magicType1, PlayerMagicType magicType2)
    {
        int magic1Index = (int)magicType1;
        int magic2Index = (int)magicType2;

        // magic1�� magic2�� ������ ���, �ϳ��� ����
        if (magic1Index == magic2Index && magic1Index != -1)
        {
            ApplyMagicEffect(playerMagics[magic1Index]);
        }
        else
        {
            // ������ �����ߴ� ���� ȿ�� ����
            if (previousMagic1 != null)
                RemoveMagicEffect(previousMagic1);
            if (previousMagic2 != null)
                RemoveMagicEffect(previousMagic2);

            // magic1�� magic2�� �ٸ��� ���� ����
            if (magic1Index != -1)
            {
                ApplyMagicEffect(playerMagics[magic1Index]);
                previousMagic1 = playerMagics[magic1Index]; // ���� ����1 ����
            }

            if (magic2Index != -1)
            {
                ApplyMagicEffect(playerMagics[magic2Index]);
                previousMagic2 = playerMagics[magic2Index]; // ���� ����2 ����
            }
        }
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

    // ���� ȿ���� �����ϴ� �޼ҵ�
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

        Debug.Log($"{magic.magicType} ���� ȿ�� ����: ���ݷ� {magic.effect.MagicAttack}");
    }

    public void Update()
    {
    }
}

