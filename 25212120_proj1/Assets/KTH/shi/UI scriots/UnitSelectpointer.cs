using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectpointer : MonoBehaviour
{
    public GameObject upgradeUI;                // 업그레이드 UI
    public List<Button> unitButtons;           // 여러 개의 유닛 버튼들 (Inspector에서 할당)
    public List<Image> highlightImages;        // 각 유닛에 대한 하이라이트 이미지들 (Inspector에서 할당)

    private int highlightedUnitIndex = -1;     // 현재 하이라이트된 유닛 인덱스 (-1은 선택되지 않음을 의미)
    private Unit_SpawnManager selectedspawnManager; // 현재 선택된 유닛 스폰 매니저

    public Button purchaseButton;              // "구매" 버튼 (Inspector에서 할당)

    void Awake()
    {
        // 동적으로 업그레이드 UI 찾기
        if (upgradeUI == null)
        {
            upgradeUI = GameObject.Find("UnitBuyTab");
        }

        // 동적으로 유닛 버튼들 찾기
        if (unitButtons == null || unitButtons.Count == 0)
        {
            unitButtons = new List<Button>();

            Button unit1Button = GameObject.Find("unit1_Button").GetComponent<Button>();
            Button unit2Button = GameObject.Find("unit2_Button").GetComponent<Button>();

            if (unit1Button != null) unitButtons.Add(unit1Button);
            if (unit2Button != null) unitButtons.Add(unit2Button);
        }

        // 동적으로 하이라이트 이미지 찾기
        if (highlightImages == null || highlightImages.Count == 0)
        {
            highlightImages = new List<Image>();

            Image unit1Highlight = GameObject.Find("unit1_highlight").GetComponent<Image>();
            Image unit2Highlight = GameObject.Find("unit2_highlight").GetComponent<Image>();

            if (unit1Highlight != null) highlightImages.Add(unit1Highlight);
            if (unit2Highlight != null) highlightImages.Add(unit2Highlight);
        }

        // 동적으로 구매 버튼 찾기
        if (purchaseButton == null)
        {
            purchaseButton = GameObject.Find("PurchaseButton").GetComponent<Button>();
        }

        upgradeUI?.SetActive(false); // 게임 시작 시 UI 비활성화
    }

    void Update()
    {
        // ESC 키로 UI 닫기
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
            selectedspawnManager = spawnManager; // 선택된 스폰 매니저 저장
            OpenUpgradeUI();                     // UI 열기
        }
        else
        {
            Debug.LogWarning("This object is not a valid spawn manager.");
        }
    }

    void Start()
    {
        

        // 각 버튼에 클릭 이벤트를 동적으로 연결
        for (int i = 0; i < unitButtons.Count; i++)
        {
            int index = i; // 로컬 변수로 인덱스를 저장하여 클로저 문제 방지
            unitButtons[i].onClick.AddListener(() => OnUnitButtonClick(index));
        }

        // 모든 하이라이트 이미지를 초기화 (비활성화)
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
            Debug.LogError("purchaseButton이 할당되지 않았습니다.");
        }
    }

    // 버튼 클릭 시 실행되는 함수
    void OnUnitButtonClick(int index)
    {
        if (selectedspawnManager == null)
        {
            Debug.LogWarning("No spawn manager selected.");
            return;
        }

        // 이전 하이라이트 이미지를 비활성화
        if (highlightedUnitIndex != -1)
        {
            highlightImages[highlightedUnitIndex].gameObject.SetActive(false);
        }

        // 새로운 하이라이트 이미지를 활성화
        highlightedUnitIndex = index;
        highlightImages[highlightedUnitIndex].gameObject.SetActive(true);

        // 선택된 유닛을 스폰 매니저에 반영
        selectedspawnManager.SpawnUnitSelect = (index == 1);
    }

    void PurchaseButtonClick()
    {
        if (selectedspawnManager != null)
        {
            selectedspawnManager.Spawn(); // 선택된 스폰 매니저에서 유닛 생성
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
            // UI 활성화
            upgradeUI.SetActive(true);

            // 구매 버튼 활성화
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
            upgradeUI.SetActive(false); // UI 비활성화
        }

        if (purchaseButton != null)
        {
            purchaseButton.gameObject.SetActive(false); // 버튼 비활성화
        }

        selectedspawnManager = null; // 선택된 스폰 매니저 해제
    }
}
