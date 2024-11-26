using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectpointer : MonoBehaviour
{
    public List<Button> skillButtons; // ���� ���� ��ų ��ư�� (Inspector���� �Ҵ�)
    public List<Image> highlightImages; // �� ��ų�� ���� ���̶���Ʈ �̹����� (Inspector���� �Ҵ�)

    private int highlightedSkillIndex = -1; // ���� ���̶���Ʈ�� ��ų �ε��� (-1�� ���õ��� ������ �ǹ�)
    public Unit_SpawnManager spawnManager;

    public Button purchaseButton; // "����" ��ư (Inspector���� �Ҵ�)
    void Start()
    {
        
        // �� ��ư�� Ŭ�� �̺�Ʈ�� �������� ����
        for (int i = 0; i < skillButtons.Count; i++)
        {
            int index = i; // ���� ������ �ε����� �����Ͽ� Ŭ���� ���� ����
            skillButtons[i].onClick.AddListener(() => OnSkillButtonClick(index));
        }



        // ��� ���̶���Ʈ �̹����� �ʱ�ȭ (��Ȱ��ȭ)
        foreach (Image highlightImage in highlightImages)
        {
            highlightImage.gameObject.SetActive(false);
        }

        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(PurchaseButtonClick);
        }
        else
        {
            Debug.LogError("purchaseButton�� �Ҵ���� �ʾҽ��ϴ�.");
        }
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
        if (index == 0) { spawnManager.SpawnUnitSelect = false; }
        else if (index == 1) { spawnManager.SpawnUnitSelect = true; }
        
        
    }
    void PurchaseButtonClick()
    {
        spawnManager.Spawn();
    }
    public int SelectedIndex()
    {
        return highlightedSkillIndex;
    }
}
