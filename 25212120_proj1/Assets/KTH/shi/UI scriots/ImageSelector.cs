using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageSelector : MonoBehaviour
{
    public Button displaySkillSlot1; // ù ��° ��ų ���Կ� ����� Button ������Ʈ
    public Button displaySkillSlot2; // �� ��° ��ų ���Կ� ����� Button ������Ʈ

    [System.Serializable]
    public class SkillButtonInfo
    {
        public Button button; // ���� ��ư
        public Image highlightImage; // ���̶���Ʈ �̹���
        public PlayerMagicType magicType; // ���� Ÿ��
    }

    public List<SkillButtonInfo> skillButtonInfos; // ���� ��ư ���� ����Ʈ
    public PlayerInputManager playerInputManager; // PlayerInputManager ���� (Inspector���� �Ҵ�)
    public PlayerInventory playerInventory; // PlayerInventory ���� (Inspector���� �Ҵ�)

    private int highlightedSkillIndex = -1; // ���� ���̶���Ʈ�� ��ų �ε��� (-1�� ���õ��� ������ �ǹ�)
    private PlayerStateType slot1Skill = PlayerStateType.None;
    private PlayerStateType slot2Skill = PlayerStateType.None;

    void Start()
    {
        // ��ų Ʈ�� Ȱ��ȭ �̺�Ʈ ����
        playerInventory.OnSkillTreeUnlocked += OnSkillTreeUnlocked;

        // �� ��ų ��ư�� Ŭ�� �̺�Ʈ�� �ʱ� ���� ����
        for (int i = 0; i < skillButtonInfos.Count; i++)
        {
            int index = i; // ���� ������ �ε����� �����Ͽ� Ŭ���� ���� ����
            SkillButtonInfo info = skillButtonInfos[i];

            // Ŭ�� �̺�Ʈ ����
            info.button.onClick.AddListener(() => OnSkillButtonClick(index));

            // ��ų Ʈ�� Ȱ��ȭ ���ο� ���� ��ư Ȱ��ȭ/��Ȱ��ȭ
            bool isUnlocked = IsSkillTreeUnlocked(info.magicType);
            info.button.interactable = isUnlocked;

            // ���̶���Ʈ �̹��� ��Ȱ��ȭ
            info.highlightImage.gameObject.SetActive(false);
        }

        // ���� ��ư�� Ŭ�� �̺�Ʈ �߰�
        displaySkillSlot1.onClick.AddListener(() => AssignSkillToSlot(1));
        displaySkillSlot2.onClick.AddListener(() => AssignSkillToSlot(2));

        Debug.Log("Start method executed, event listeners are set.");
    }

    // ��ų Ʈ�� Ȱ��ȭ ���� Ȯ�� �Լ�
    private bool IsSkillTreeUnlocked(PlayerMagicType magicType)
    {
        switch (magicType)
        {
            case PlayerMagicType.Wood:
                return playerInventory.MagicSkillTree_Wood;
            case PlayerMagicType.Fire:
                return playerInventory.MagicSkillTree_Fire;
            case PlayerMagicType.Ice:
                return playerInventory.MagicSkillTree_Ice;
            case PlayerMagicType.Sand:
                return playerInventory.MagicSkillTree_Sand;
            default:
                return false;
        }
    }

    // ��ų Ʈ�� Ȱ��ȭ �� ȣ��Ǵ� �Լ�
    private void OnSkillTreeUnlocked(PlayerMagicType magicType)
    {
        // �ش� ���� Ÿ���� ��ư�� ã�� Ȱ��ȭ
        foreach (var info in skillButtonInfos)
        {
            if (info.magicType == magicType)
            {
                info.button.interactable = true;
                Debug.Log($"{magicType} ���� ��ư�� Ȱ��ȭ�Ǿ����ϴ�.");
                break;
            }
        }
    }

    // ��ư Ŭ�� �� ����Ǵ� �Լ�
    void OnSkillButtonClick(int index)
    {
        // ���� ���̶���Ʈ �̹����� ��Ȱ��ȭ
        if (highlightedSkillIndex != -1)
        {
            skillButtonInfos[highlightedSkillIndex].highlightImage.gameObject.SetActive(false);
        }

        // ���ο� ���̶���Ʈ �̹����� Ȱ��ȭ
        highlightedSkillIndex = index;
        skillButtonInfos[highlightedSkillIndex].highlightImage.gameObject.SetActive(true);

        Debug.Log("Skill button clicked, index: " + index);
    }

    // ���Կ� ��ų�� �Ҵ��ϴ� �Լ�
    public void AssignSkillToSlot(int slotNumber)
    {
        if (highlightedSkillIndex == -1)
        {
            Debug.Log("No skill selected to assign to slot.");
            return; // ���õ� ��ų�� ������ �ƹ� �۾��� ���� ����
        }

        // ���õ� ��ų ���� ��������
        SkillButtonInfo selectedSkillInfo = skillButtonInfos[highlightedSkillIndex];
        PlayerStateType selectedSkill = ConvertMagicTypeToStateType(selectedSkillInfo.magicType);

        // �ߺ��� ��ų �Ҵ� ����
        if ((slotNumber == 1 && selectedSkill == slot2Skill) || (slotNumber == 2 && selectedSkill == slot1Skill))
        {
            Debug.Log("Skill is already assigned to the other slot.");
            return;
        }

        // PlayerInputManager�� Magic1Swap �Ǵ� Magic2Swap ȣ��
        if (slotNumber == 1)
        {
            playerInputManager.Magic1Swap(selectedSkill);
            slot1Skill = selectedSkill;
            UpdateSlotImage(displaySkillSlot1, selectedSkillInfo);
        }
        else if (slotNumber == 2)
        {
            playerInputManager.Magic2Swap(selectedSkill);
            slot2Skill = selectedSkill;
            UpdateSlotImage(displaySkillSlot2, selectedSkillInfo);
        }

        // ��ų�� ���Կ� �Ҵ��� �� ���̶���Ʈ�� ��Ȱ��ȭ
        skillButtonInfos[highlightedSkillIndex].highlightImage.gameObject.SetActive(false);
        highlightedSkillIndex = -1;
    }

    // ������ �̹����� ������Ʈ�ϴ� �Լ�
    private void UpdateSlotImage(Button slotButton, SkillButtonInfo skillInfo)
    {
        Image selectedSkillImage = skillInfo.button.GetComponent<Image>();
        if (selectedSkillImage != null)
        {
            slotButton.GetComponent<Image>().sprite = selectedSkillImage.sprite;
            Debug.Log($"Assigned skill to slot with sprite: {selectedSkillImage.sprite.name}");
        }
        else
        {
            Debug.Log("Selected Skill Image or Sprite is null");
        }
    }

    // ���� Ÿ���� �÷��̾� ���� Ÿ������ ��ȯ�ϴ� �Լ�
    private PlayerStateType ConvertMagicTypeToStateType(PlayerMagicType magicType)
    {
        switch (magicType)
        {
            case PlayerMagicType.Wood:
                return PlayerStateType.WoodMagic;
            case PlayerMagicType.Fire:
                return PlayerStateType.FireMagic;
            case PlayerMagicType.Ice:
                return PlayerStateType.IceMagic;
            case PlayerMagicType.Sand:
                return PlayerStateType.SandMagic;
            default:
                return PlayerStateType.None;
        }
    }
}
