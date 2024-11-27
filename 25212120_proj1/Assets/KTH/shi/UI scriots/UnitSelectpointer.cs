using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectpointer : MonoBehaviour
{
    public GameObject upgradeUI;                // ���׷��̵� UI
    public List<Button> unitButtons;           // ���� ���� ���� ��ư�� (Inspector���� �Ҵ�)
    public List<Image> highlightImages;        // �� ���ֿ� ���� ���̶���Ʈ �̹����� (Inspector���� �Ҵ�)

    private int highlightedUnitIndex = -1;     // ���� ���̶���Ʈ�� ���� �ε��� (-1�� ���õ��� ������ �ǹ�)
    private Unit_SpawnManager selectedspawnManager; // ���� ���õ� ���� ���� �Ŵ���

    public Button purchaseButton;              // "����" ��ư (Inspector���� �Ҵ�)

    void Awake()
    {
        // �������� ���׷��̵� UI ã��
        if (upgradeUI == null)
        {
            upgradeUI = GameObject.Find("UnitBuyTab");
        }

        // �������� ���� ��ư�� ã��
        if (unitButtons == null || unitButtons.Count == 0)
        {
            unitButtons = new List<Button>();

            Button unit1Button = GameObject.Find("unit1_Button").GetComponent<Button>();
            Button unit2Button = GameObject.Find("unit2_Button").GetComponent<Button>();

            if (unit1Button != null) unitButtons.Add(unit1Button);
            if (unit2Button != null) unitButtons.Add(unit2Button);
        }

        // �������� ���̶���Ʈ �̹��� ã��
        if (highlightImages == null || highlightImages.Count == 0)
        {
            highlightImages = new List<Image>();

            Image unit1Highlight = GameObject.Find("unit1_highlight").GetComponent<Image>();
            Image unit2Highlight = GameObject.Find("unit2_highlight").GetComponent<Image>();

            if (unit1Highlight != null) highlightImages.Add(unit1Highlight);
            if (unit2Highlight != null) highlightImages.Add(unit2Highlight);
        }

        // �������� ���� ��ư ã��
        if (purchaseButton == null)
        {
            purchaseButton = GameObject.Find("PurchaseButton").GetComponent<Button>();
        }

        upgradeUI?.SetActive(false); // ���� ���� �� UI ��Ȱ��ȭ
    }

    void Update()
    {
        // ESC Ű�� UI �ݱ�
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUpgradeUI();
        }
    }

    void OnMouseDown()
    {
        Unit_SpawnManager spawnManager = GetComponent<Unit_SpawnManager>();

        if (spawnManager != null)
        {
            selectedspawnManager = spawnManager; // ���õ� ���� �Ŵ��� ����
            OpenUpgradeUI();                     // UI ����
        }
        else
        {
            Debug.LogWarning("This object is not a valid spawn manager.");
        }
    }

    void Start()
    {
        

        // �� ��ư�� Ŭ�� �̺�Ʈ�� �������� ����
        for (int i = 0; i < unitButtons.Count; i++)
        {
            int index = i; // ���� ������ �ε����� �����Ͽ� Ŭ���� ���� ����
            unitButtons[i].onClick.AddListener(() => OnUnitButtonClick(index));
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
    void OnUnitButtonClick(int index)
    {
        if (selectedspawnManager == null)
        {
            Debug.LogWarning("No spawn manager selected.");
            return;
        }

        // ���� ���̶���Ʈ �̹����� ��Ȱ��ȭ
        if (highlightedUnitIndex != -1)
        {
            highlightImages[highlightedUnitIndex].gameObject.SetActive(false);
        }

        // ���ο� ���̶���Ʈ �̹����� Ȱ��ȭ
        highlightedUnitIndex = index;
        highlightImages[highlightedUnitIndex].gameObject.SetActive(true);

        // ���õ� ������ ���� �Ŵ����� �ݿ�
        selectedspawnManager.SpawnUnitSelect = (index == 1);
    }

    void PurchaseButtonClick()
    {
        if (selectedspawnManager != null)
        {
            selectedspawnManager.Spawn(); // ���õ� ���� �Ŵ������� ���� ����
        }
        else
        {
            Debug.LogWarning("No spawn manager selected for spawning.");
        }
    }

    public int SelectedIndex()
    {
        return highlightedUnitIndex;
    }

    private void OpenUpgradeUI()
    {
        if (upgradeUI != null)
        {
            // UI Ȱ��ȭ
            upgradeUI.SetActive(true);

            // ���� ��ư Ȱ��ȭ
            purchaseButton.interactable = true;
        }
        else
        {
            Debug.LogWarning("Upgrade UI is not assigned.");
        }
    }

    public void CloseUpgradeUI()
    {
        if (upgradeUI != null)
        {
            upgradeUI.SetActive(false); // UI ��Ȱ��ȭ
        }

        if (purchaseButton != null)
        {
            purchaseButton.gameObject.SetActive(false); // ��ư ��Ȱ��ȭ
        }

        selectedspawnManager = null; // ���õ� ���� �Ŵ��� ����
    }
}
