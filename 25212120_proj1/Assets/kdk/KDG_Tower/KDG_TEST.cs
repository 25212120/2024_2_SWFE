using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDG_TEST : MonoBehaviour
{
    public PlayerInventory playerInventory; // PlayerInventory�� ������ ����

    void Start()
    {
        if (playerInventory != null)
        {
            // �� ������ ��ųƮ�� ���� Ȯ��
            bool isWoodSkillTreeActive = playerInventory.playerMagics[0].IsWoodSkillTreeActive();
            bool isFireSkillTreeActive = playerInventory.playerMagics[1].IsFireSkillTreeActive();
            bool isIceSkillTreeActive = playerInventory.playerMagics[2].IsIceSkillTreeActive();
            bool isSandSkillTreeActive = playerInventory.playerMagics[3].IsSandSkillTreeActive();

        }
    }
}