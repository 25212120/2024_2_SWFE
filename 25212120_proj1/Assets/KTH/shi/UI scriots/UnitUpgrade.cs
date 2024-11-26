using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitUpgrade : MonoBehaviour
{
    public List<Button> skillButtons; // 여러 개의 스킬 버튼들 (Inspector에서 할당)
    public List<Image> highlightImages; // 각 스킬에 대한 하이라이트 이미지들 (Inspector에서 할당)

    private int highlightedSkillIndex = -1; // 현재 하이라이트된 스킬 인덱스 (-1은 선택되지 않음을 의미)

    private Unit_WithSword unit2; // 검 유닛
    private Unit_WithWand unit1;  // 마법사 유닛

    public TextMeshProUGUI unitlevel2; // 검 유닛 레벨 표시 UI
    public TextMeshProUGUI unitlevel1; // 마법사 유닛 레벨 표시 UI

    private bool unitname = true; // 선택된 유닛을 나타냄 (true = 검 유닛, false = 마법사 유닛)

    public Button purchaseButton; // "구매" 버튼 (Inspector에서 할당)

    void Start()
    {
        // 각 버튼에 클릭 이벤트를 동적으로 연결
        for (int i = 0; i < skillButtons.Count; i++)
        {
            int index = i; // 로컬 변수로 인덱스를 저장하여 클로저 문제 방지
            skillButtons[i].onClick.AddListener(() => OnSkillButtonClick(index));
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

        // 유닛 찾기 (씬에 있는 유닛 동적 참조)
        FindUnitsInScene();
    }

    // 버튼 클릭 시 실행되는 함수
    void OnSkillButtonClick(int index)
    {
        // 이전 하이라이트 이미지를 비활성화
        if (highlightedSkillIndex != -1)
        {
            highlightImages[highlightedSkillIndex].gameObject.SetActive(false);
        }

        // 새로운 하이라이트 이미지를 활성화
        highlightedSkillIndex = index;
        highlightImages[highlightedSkillIndex].gameObject.SetActive(true);

        unitname = (index == 0); // 0이면 검 유닛, 1이면 마법사 유닛
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
        // 동적으로 씬에 유닛이 있는지 확인하여 레벨 업데이트
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

    // 씬에서 유닛 찾기
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
