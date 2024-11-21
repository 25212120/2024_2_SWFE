using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageSelector : MonoBehaviour
{
    public Button displaySkillSlot1; // ù ��° ��ų ���Կ� ����� Button ������Ʈ
    public Button displaySkillSlot2; // �� ��° ��ų ���Կ� ����� Button ������Ʈ
    public List<Button> skillButtons; // ���� ���� ��ų ��ư�� (Inspector���� �Ҵ�)
    public List<Image> highlightImages; // �� ��ų�� ���� ���̶���Ʈ �̹����� (Inspector���� �Ҵ�)

    private int highlightedSkillIndex = -1; // ���� ���̶���Ʈ�� ��ų �ε��� (-1�� ���õ��� ������ �ǹ�)

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

        Image selectedSkillImage = skillButtons[highlightedSkillIndex].GetComponent<Image>();
        if (selectedSkillImage != null)
        {
            if (slotNumber == 1 && displaySkillSlot1 != null)
            {
                displaySkillSlot1.GetComponent<Image>().sprite = selectedSkillImage.sprite;
                Debug.Log("Assigned to Slot 1 with sprite: " + selectedSkillImage.sprite.name);
            }
            else if (slotNumber == 2 && displaySkillSlot2 != null)
            {
                displaySkillSlot2.GetComponent<Image>().sprite = selectedSkillImage.sprite;
                Debug.Log("Assigned to Slot 2 with sprite: " + selectedSkillImage.sprite.name);
            }
        }
        else
        {
            Debug.Log("Selected Skill Image or Sprite is null");
        }

        // ��ų�� ���Կ� �Ҵ��� �� ���̶���Ʈ�� ��Ȱ��ȭ
        highlightImages[highlightedSkillIndex].gameObject.SetActive(false);
        highlightedSkillIndex = -1;
    }
}
