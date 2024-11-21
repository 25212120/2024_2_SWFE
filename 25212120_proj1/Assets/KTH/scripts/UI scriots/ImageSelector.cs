using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageSelector : MonoBehaviour
{
    public Button displaySkillSlot1; // ù ��° ��ų ���Կ� ����� Button ������Ʈ
    public Button displaySkillSlot2; // �� ��° ��ų ���Կ� ����� Button ������Ʈ
    public List<Button> skillButtons; // ���� ���� ��ų ��ư�� (Inspector���� �Ҵ�)
    public List<Image> highlightImages; // �� ��ų�� ���� ���̶���Ʈ �̹����� (Inspector���� �Ҵ�)
    public PlayerInputManager playerInputManager; // PlayerInputManager ���� (Inspector���� �Ҵ�)

    private int highlightedSkillIndex = -1; // ���� ���̶���Ʈ�� ��ų �ε��� (-1�� ���õ��� ������ �ǹ�)
    private PlayerStateType slot1Skill = PlayerStateType.None;
    private PlayerStateType slot2Skill = PlayerStateType.None;

    void Start()
    {

        // �� ��ư�� Ŭ�� �̺�Ʈ�� �������� ����
        for (int i = 0; i < skillButtons.Count; i++)
        {
            int index = i; // ���� ������ �ε����� �����Ͽ� Ŭ���� ���� ����
            skillButtons[i].onClick.AddListener(() => OnSkillButtonClick(index));
        }

        // ���� ��ư�� Ŭ�� �̺�Ʈ �߰�
        displaySkillSlot1.onClick.AddListener(() => AssignSkillToSlot(1));
        displaySkillSlot2.onClick.AddListener(() => AssignSkillToSlot(2));

        // ��� ���̶���Ʈ �̹����� �ʱ�ȭ (��Ȱ��ȭ)
        foreach (Image highlightImage in highlightImages)
        {
            highlightImage.gameObject.SetActive(false);
        }

        Debug.Log("Start method executed, event listeners are set.");
    }

    // ��ư Ŭ�� �� ����Ǵ� �Լ�
    void OnSkillButtonClick(int index)
    {
        // ���� ���̶���Ʈ �̹����� ��Ȱ��ȭ
        if (highlightedSkillIndex != -1)
        {
            highlightImages[highlightedSkillIndex].gameObject.SetActive(false);
        }

        // ���ο� ���̶���Ʈ �̹����� Ȱ��ȭ
        highlightedSkillIndex = index;
        highlightImages[highlightedSkillIndex].gameObject.SetActive(true);

        Debug.Log("Skill button clicked, index: " + index);
    }

    // ���Կ� ��ų�� �Ҵ��ϴ� �Լ� (���� 1 �Ǵ� ���� 2�� �Ҵ�)
    public void AssignSkillToSlot(int slotNumber)
    {
        if (highlightedSkillIndex == -1)
        {
            Debug.Log("No skill selected to assign to slot.");
            return; // ���õ� ��ų�� ������ �ƹ� �۾��� ���� ����
        }

        // ��ų Ÿ�� ���� (�ӽ÷� �ε��� ���� ����Ͽ� ����)
        PlayerStateType selectedSkill = (PlayerStateType)highlightedSkillIndex + 13;

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
            UpdateSlotImage(displaySkillSlot1, highlightedSkillIndex);
        }
        else if (slotNumber == 2)
        {
            playerInputManager.Magic2Swap(selectedSkill);
            slot2Skill = selectedSkill;
            UpdateSlotImage(displaySkillSlot2, highlightedSkillIndex);
        }

        // ��ų�� ���Կ� �Ҵ��� �� ���̶���Ʈ�� ��Ȱ��ȭ
        highlightImages[highlightedSkillIndex].gameObject.SetActive(false);
        highlightedSkillIndex = -1;
    }

    // ������ �̹����� ������Ʈ�ϴ� �Լ�
    private void UpdateSlotImage(Button slotButton, int skillIndex)
    {
        Image selectedSkillImage = skillButtons[skillIndex].GetComponent<Image>();
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
}
