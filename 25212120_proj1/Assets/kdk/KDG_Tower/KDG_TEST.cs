using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDG_TEST : MonoBehaviour
{
    public PlayerInventory playerInventory; // PlayerInventory를 참조할 변수

    void Start()
    {
        if (playerInventory != null)
        {
            // 각 마법의 스킬트리 상태 확인
            bool isWoodSkillTreeActive = playerInventory.playerMagics[0].IsWoodSkillTreeActive();
            bool isFireSkillTreeActive = playerInventory.playerMagics[1].IsFireSkillTreeActive();
            bool isIceSkillTreeActive = playerInventory.playerMagics[2].IsIceSkillTreeActive();
            bool isSandSkillTreeActive = playerInventory.playerMagics[3].IsSandSkillTreeActive();

        }
    }
}