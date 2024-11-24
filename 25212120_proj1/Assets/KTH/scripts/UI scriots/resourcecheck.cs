using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class resourcecheck : MonoBehaviour
{
    public List<GameObject> resourcePanels; // UI 패널 리스트
    public GameObject resourceUIParent; // 자원 UI 창의 부모 오브젝트
    private int selectedIndex = -1; // 선택된 자원 창의 인덱스 (-1은 선택 안 됨을 의미)

    void Start()
    {
        // 자원 UI 창을 처음에 비활성화
        if (resourceUIParent != null)
        {
            resourceUIParent.SetActive(false);
        }
        else
        {
            Debug.LogError("resourceUIParent is not assigned in the Inspector");
        }

        // 모든 패널을 처음에 비활성화
        foreach (GameObject panel in resourcePanels)
        {
            panel.SetActive(false);
        }
    }

    void Update()
    {
        // Tab 키를 누른 동안 자원 UI 창 활성화
        if (Input.GetKey(KeyCode.Tab))
        {
            resourceUIParent.SetActive(true);

            // DownArrow 키로 자원 창 선택
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SelectNextResourcePanel();
            }

            // UpArrow 키로 자원 창 선택
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
        // 이전에 선택한 패널의 비활성화
        if (selectedIndex >= 0 && selectedIndex < resourcePanels.Count)
        {
            DeselectResourcePanel(selectedIndex);
        }

        // 다음 인덱스 계산 (순환)
        selectedIndex = (selectedIndex + 1) % resourcePanels.Count;

        // 현재 선택된 패널을 활성화
        SelectResourcePanel(selectedIndex);
    }

    private void SelectPreviousResourcePanel()
    {
        // 이전에 선택한 패널의 비활성화
        if (selectedIndex >= 0 && selectedIndex < resourcePanels.Count)
        {
            DeselectResourcePanel(selectedIndex);
        }

        // 이전 인덱스 계산 (순환)
        selectedIndex = (selectedIndex - 1 + resourcePanels.Count) % resourcePanels.Count;

        // 현재 선택된 패널을 활성화
        SelectResourcePanel(selectedIndex);
    }

    private void SelectResourcePanel(int index)
    {
        // 패널을 활성화
        GameObject panel = resourcePanels[index];
        panel.SetActive(true);
    }

    private void DeselectResourcePanel(int index)
    {
        // 패널을 비활성화
        GameObject panel = resourcePanels[index];
        panel.SetActive(false);
    }

    private void ClearAllBorders()
    {
        // 모든 패널 비활성화
        foreach (GameObject panel in resourcePanels)
        {
            panel.SetActive(false);
        }
        selectedIndex = -1;
    }
}

