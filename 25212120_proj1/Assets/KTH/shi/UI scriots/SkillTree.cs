using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    public Image lockImage_Wood1;
    public Image lockImage_Wood2;

    public Image lockImage_Fire1;
    public Image lockImage_Fire2;

    public Image lockImage_Ice1;
    public Image lockImage_Ice2;

    public Image lockImage_Sand1;
    public Image lockImage_Sand2;

    public PlayerInventory playerInventory;

    void Start()
    {
        if (playerInventory != null)
        {
            UpdateSkillTreeUI();
        }
        else
        {
            Debug.LogError("PlayerInventory가 설정되지 않았습니다.");
        }
    }

    void UpdateSkillTreeUI()
    {
        lockImage_Wood1.enabled = !playerInventory.playerMagics[0].IsWoodSkillTreeActive();
        lockImage_Wood2.enabled = !playerInventory.playerMagics[0].IsWoodSkillTreeActive();

        lockImage_Fire1.enabled = !playerInventory.playerMagics[1].IsFireSkillTreeActive();
        lockImage_Fire2.enabled = !playerInventory.playerMagics[1].IsFireSkillTreeActive();

        lockImage_Ice1.enabled = !playerInventory.playerMagics[2].IsIceSkillTreeActive();
        lockImage_Ice2.enabled = !playerInventory.playerMagics[2].IsIceSkillTreeActive();

        lockImage_Sand1.enabled = !playerInventory.playerMagics[3].IsSandSkillTreeActive();
        lockImage_Sand2.enabled = !playerInventory.playerMagics[3].IsSandSkillTreeActive();
    }
}
