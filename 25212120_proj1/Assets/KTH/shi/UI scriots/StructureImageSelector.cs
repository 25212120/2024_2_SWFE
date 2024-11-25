using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StructureImageSelector : MonoBehaviour
{
    public List<Button> skillButtons; // ���� ���� ��ų ��ư�� (Inspector���� �Ҵ�)
    public List<Image> highlightImages; // �� ��ų�� ���� ���̶���Ʈ �̹����� (Inspector���� �Ҵ�)
    public List<string> turretPrefabNames; // �� ��ư�� �����Ǵ� ������ �̸��� (Inspector���� �Ҵ�)
    public HighlightArea highlightArea; // SetTurretPrefab �Լ��� ���� ��ũ��Ʈ (Inspector���� �Ҵ�)
    public Button purchaseButton; // "����" ��ư (Inspector���� �Ҵ�)

    private int highlightedSkillIndex = -1; // ���� ���̶���Ʈ�� ��ų �ε��� (-1�� ���õ��� ������ �ǹ�)
    private string selectedPrefabName = ""; // ���õ� ������ �̸��� ������ ����

    void Start()
    {
        // �� ��ư�� Ŭ�� �̺�Ʈ�� �������� ����
        for (int i = 0; i < skillButtons.Count; i++)
        {
            int index = i; // ���� ������ �ε����� �����Ͽ� Ŭ���� ���� ����
            skillButtons[i].onClick.AddListener(() => OnSkillButtonClick(index));
        }

        // "����" ��ư�� Ŭ�� �̺�Ʈ ����
        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(OnPurchaseButtonClick);
        }
        else
        {
            Debug.LogError("purchaseButton�� �Ҵ���� �ʾҽ��ϴ�.");
        }

        // ��� ���̶���Ʈ �̹����� �ʱ�ȭ (��Ȱ��ȭ)
        foreach (Image highlightImage in highlightImages)
        {
            highlightImage.gameObject.SetActive(false);
        }
    }

    // ����Ʈ ��ư Ŭ�� �� ����Ǵ� �Լ�
    void OnSkillButtonClick(int index)
    {
        // ���� ���̶���Ʈ �̹����� ��Ȱ��ȭ
        if (highlightedSkillIndex != -1 && highlightedSkillIndex < highlightImages.Count)
        {
            highlightImages[highlightedSkillIndex].gameObject.SetActive(false);
        }

        // ���ο� ���̶���Ʈ �̹����� Ȱ��ȭ
        if (index < highlightImages.Count)
        {
            highlightedSkillIndex = index;
            highlightImages[highlightedSkillIndex].gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("highlightImages ����Ʈ�� �ε����� ������ ������ϴ�.");
        }

        Debug.Log("Skill button clicked, index: " + index);

        // ������ �̸��� ����
        if (turretPrefabNames != null && index >= 0 && index < turretPrefabNames.Count)
        {
            selectedPrefabName = turretPrefabNames[index];
            Debug.Log("Selected prefab name: " + selectedPrefabName);
        }
        else
        {
            Debug.LogError("turretPrefabNames ����Ʈ�� ����ְų� �ε����� ������ ������ϴ�.");
        }
    }

    // "����" ��ư Ŭ�� �� ����Ǵ� �Լ�
    void OnPurchaseButtonClick()
    {
        if (!string.IsNullOrEmpty(selectedPrefabName))
        {
            CallSetTurretPrefab(selectedPrefabName);
        }
        else
        {
            Debug.LogWarning("���õ� �������� �����ϴ�. ���� ����Ʈ���� �����ϼ���.");
        }
    }

    // SetTurretPrefab �Լ��� ȣ���ϴ� �Լ�
    void CallSetTurretPrefab(string turretPrefabName)
    {
        if (highlightArea != null)
        {
            highlightArea.SetTurretPrefab(turretPrefabName);
            Debug.Log("SetTurretPrefab �Լ��� ȣ���߽��ϴ�: " + turretPrefabName);
        }
        else
        {
            Debug.LogError("highlightArea�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    public int SelectedIndex()
    {
        return highlightedSkillIndex;
    }
}
