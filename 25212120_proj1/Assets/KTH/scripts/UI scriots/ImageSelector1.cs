using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageSelector1 : MonoBehaviour
{
    public Image displaySkillSlot; // �ϳ��� ��ų ���Կ� ����� Image ������Ʈ (Inspector���� �Ҵ�)
    public List<Button> skillButtons; // ���� ���� ��ų ��ư�� (Inspector���� �Ҵ�)
    public List<GameObject> highlightObjects; // �� ��ų�� ���� ���̶���Ʈ ������Ʈ�� (Inspector���� �Ҵ�)
    public List<Image> skillImagesToChange; // ������ �̹��� ����Ʈ (Inspector���� �Ҵ�)

    private int highlightedSkillIndex = -1; // ���� ���̶���Ʈ�� ��ų �ε��� (-1�� ���õ��� ������ �ǹ�)

    void Start()
    {
        // ��� ���̶���Ʈ ������Ʈ�� �ʱ�ȭ (��Ȱ��ȭ)
        foreach (GameObject highlightObject in highlightObjects)
        {
            highlightObject.SetActive(false);
        }
    }

    void Update()
    {
        // Ű���� �Է� ó�� (0, 1, 2, 3, 4, 5 Ű)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivateHighlight(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActivateHighlight(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ActivateHighlight(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ActivateHighlight(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ActivateHighlight(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ActivateHighlight(5);
        }
    }

    // ���̶���Ʈ ������Ʈ�� Ȱ��ȭ�ϴ� �Լ�
    private void ActivateHighlight(int index)
    {
        if (index < 0 || index >= highlightObjects.Count)
        {
            Debug.Log("Invalid index for highlight: " + index);
            return;
        }

        // ���� ���̶���Ʈ ������Ʈ�� ��Ȱ��ȭ
        if (highlightedSkillIndex != -1)
        {
            highlightObjects[highlightedSkillIndex].SetActive(false);
        }

        // ���ο� ���̶���Ʈ ������Ʈ�� Ȱ��ȭ
        highlightedSkillIndex = index;
        highlightObjects[highlightedSkillIndex].SetActive(true);

        // �ε����� �ش��ϴ� �̹����� displaySkillSlot ����
        if (index < skillImagesToChange.Count)
        {
            Image selectedSkillImage = skillImagesToChange[index];
            if (selectedSkillImage != null && displaySkillSlot != null)
            {
                displaySkillSlot.sprite = selectedSkillImage.sprite;
                Debug.Log("Display skill slot image updated to index: " + index);
            }
        }

        Debug.Log("Highlight activated for index: " + index);
    }
}
