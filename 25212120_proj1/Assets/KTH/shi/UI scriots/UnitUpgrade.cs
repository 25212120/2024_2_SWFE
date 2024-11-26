using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitUpgrade : MonoBehaviour
{
    public List<Button> skillButtons; // ���� ���� ��ų ��ư�� (Inspector���� �Ҵ�)
    public List<Image> highlightImages; // �� ��ų�� ���� ���̶���Ʈ �̹����� (Inspector���� �Ҵ�)

    private int highlightedSkillIndex = -1; // ���� ���̶���Ʈ�� ��ų �ε��� (-1�� ���õ��� ������ �ǹ�)

    private Unit_WithSword unit2; // �� ����
    private Unit_WithWand unit1;  // ������ ����

    public TextMeshProUGUI unitlevel2; // �� ���� ���� ǥ�� UI
    public TextMeshProUGUI unitlevel1; // ������ ���� ���� ǥ�� UI

    private bool unitname = true; // ���õ� ������ ��Ÿ�� (true = �� ����, false = ������ ����)

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

        // ���� ã�� (���� �ִ� ���� ���� ����)
        FindUnitsInScene();
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

        unitname = (index == 0); // 0�̸� �� ����, 1�̸� ������ ����
    }

    void PurchaseButtonClick()
    {
        if (unitname)
        {
            if (unit1 != null)
            {
                unit1.Upgrade();
            }
            else
            {
                Debug.LogWarning("Sword Unit is not available in the scene.");
            }
        }
        else
        {
            if (unit2 != null)
            {
                unit2.Upgrade();
            }
            else
            {
                Debug.LogWarning("Wand Unit is not available in the scene.");
            }
        }
    }

    public int SelectedIndex()
    {
        return highlightedSkillIndex;
    }

    void Update()
    {
        // �������� ���� ������ �ִ��� Ȯ���Ͽ� ���� ������Ʈ
        if (unit1 != null)
        {
            unitlevel1.text = $"Level: {unit1.UnitLevel}";
        }
        else
        {
            unitlevel1.text = "Not Available";
        }

        if (unit2 != null)
        {
            unitlevel2.text = $"Level: {unit2.UnitLevel}";
        }
        else
        {
            unitlevel2.text = "Not Available";
        }

        
    }

    // ������ ���� ã��
    private void FindUnitsInScene()
    {
        unit1 = FindObjectOfType<Unit_WithWand>();
        unit2 = FindObjectOfType<Unit_WithSword>();

        if (unit1 == null)
        {
            Debug.LogWarning("Sword Unit not found in the scene.");
        }

        if (unit2 == null)
        {
            Debug.LogWarning("Wand Unit not found in the scene.");
        }
    }
}
