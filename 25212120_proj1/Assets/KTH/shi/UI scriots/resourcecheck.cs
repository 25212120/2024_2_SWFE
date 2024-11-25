using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class resourcecheck : MonoBehaviour
{
    public List<GameObject> resourcePanels; // UI �г� ����Ʈ
    public GameObject resourceUIParent; // �ڿ� UI â�� �θ� ������Ʈ
    private int selectedIndex = -1; // ���õ� �ڿ� â�� �ε��� (-1�� ���� �� ���� �ǹ�)

    void Start()
    {
        // �ڿ� UI â�� ó���� ��Ȱ��ȭ
        if (resourceUIParent != null)
        {
            resourceUIParent.SetActive(false);
        }
        else
        {
            Debug.LogError("resourceUIParent is not assigned in the Inspector");
        }

        // ��� �г��� ó���� ��Ȱ��ȭ
        foreach (GameObject panel in resourcePanels)
        {
            panel.SetActive(false);
        }
    }

    void Update()
    {
        // Tab Ű�� ���� ���� �ڿ� UI â Ȱ��ȭ
        if (Input.GetKey(KeyCode.Tab))
        {
            resourceUIParent.SetActive(true);

            // DownArrow Ű�� �ڿ� â ����
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SelectNextResourcePanel();
            }

            // UpArrow Ű�� �ڿ� â ����
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SelectPreviousResourcePanel();
            }
        }
        else
        {
            resourceUIParent.SetActive(false);
            ClearAllBorders();
        }
    }

    private void SelectNextResourcePanel()
    {
        // ������ ������ �г��� ��Ȱ��ȭ
        if (selectedIndex >= 0 && selectedIndex < resourcePanels.Count)
        {
            DeselectResourcePanel(selectedIndex);
        }

        // ���� �ε��� ��� (��ȯ)
        selectedIndex = (selectedIndex + 1) % resourcePanels.Count;

        // ���� ���õ� �г��� Ȱ��ȭ
        SelectResourcePanel(selectedIndex);
    }

    private void SelectPreviousResourcePanel()
    {
        // ������ ������ �г��� ��Ȱ��ȭ
        if (selectedIndex >= 0 && selectedIndex < resourcePanels.Count)
        {
            DeselectResourcePanel(selectedIndex);
        }

        // ���� �ε��� ��� (��ȯ)
        selectedIndex = (selectedIndex - 1 + resourcePanels.Count) % resourcePanels.Count;

        // ���� ���õ� �г��� Ȱ��ȭ
        SelectResourcePanel(selectedIndex);
    }

    private void SelectResourcePanel(int index)
    {
        // �г��� Ȱ��ȭ
        GameObject panel = resourcePanels[index];
        panel.SetActive(true);
    }

    private void DeselectResourcePanel(int index)
    {
        // �г��� ��Ȱ��ȭ
        GameObject panel = resourcePanels[index];
        panel.SetActive(false);
    }

    private void ClearAllBorders()
    {
        // ��� �г� ��Ȱ��ȭ
        foreach (GameObject panel in resourcePanels)
        {
            panel.SetActive(false);
        }
        selectedIndex = -1;
    }
}

