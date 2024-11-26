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
        upgradeUI.SetActive(false); // ���� ���� �� UI ��Ȱ��ȭ
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
