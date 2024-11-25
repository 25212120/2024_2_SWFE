using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StructureImageSelector : MonoBehaviour
{
    public List<Button> skillButtons; // 여러 개의 스킬 버튼들 (Inspector에서 할당)
    public List<Image> highlightImages; // 각 스킬에 대한 하이라이트 이미지들 (Inspector에서 할당)
    public List<string> turretPrefabNames; // 각 버튼에 대응되는 프리팹 이름들 (Inspector에서 할당)
    public HighlightArea highlightArea; // SetTurretPrefab 함수를 가진 스크립트 (Inspector에서 할당)
    public Button purchaseButton; // "구매" 버튼 (Inspector에서 할당)

    private int highlightedSkillIndex = -1; // 현재 하이라이트된 스킬 인덱스 (-1은 선택되지 않음을 의미)
    private string selectedPrefabName = ""; // 선택된 프리팹 이름을 저장할 변수

    void Start()
    {
        // 각 버튼에 클릭 이벤트를 동적으로 연결
        for (int i = 0; i < skillButtons.Count; i++)
        {
            int index = i; // 로컬 변수로 인덱스를 저장하여 클로저 문제 방지
            skillButtons[i].onClick.AddListener(() => OnSkillButtonClick(index));
        }

        // "구매" 버튼에 클릭 이벤트 연결
        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(OnPurchaseButtonClick);
        }
        else
        {
            Debug.LogError("purchaseButton이 할당되지 않았습니다.");
        }

        // 모든 하이라이트 이미지를 초기화 (비활성화)
        foreach (Image highlightImage in highlightImages)
        {
            highlightImage.gameObject.SetActive(false);
        }
    }

    // 리스트 버튼 클릭 시 실행되는 함수
    void OnSkillButtonClick(int index)
    {
        // 이전 하이라이트 이미지를 비활성화
        if (highlightedSkillIndex != -1 && highlightedSkillIndex < highlightImages.Count)
        {
            highlightImages[highlightedSkillIndex].gameObject.SetActive(false);
        }

        // 새로운 하이라이트 이미지를 활성화
        if (index < highlightImages.Count)
        {
            highlightedSkillIndex = index;
            highlightImages[highlightedSkillIndex].gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("highlightImages 리스트의 인덱스가 범위를 벗어났습니다.");
        }

        Debug.Log("Skill button clicked, index: " + index);

        // 프리팹 이름을 저장
        if (turretPrefabNames != null && index >= 0 && index < turretPrefabNames.Count)
        {
            selectedPrefabName = turretPrefabNames[index];
            Debug.Log("Selected prefab name: " + selectedPrefabName);
        }
        else
        {
            Debug.LogError("turretPrefabNames 리스트가 비어있거나 인덱스가 범위를 벗어났습니다.");
        }
    }

    // "구매" 버튼 클릭 시 실행되는 함수
    void OnPurchaseButtonClick()
    {
        if (!string.IsNullOrEmpty(selectedPrefabName))
        {
            CallSetTurretPrefab(selectedPrefabName);
        }
        else
        {
            Debug.LogWarning("선택된 프리팹이 없습니다. 먼저 리스트에서 선택하세요.");
        }
    }

    // SetTurretPrefab 함수를 호출하는 함수
    void CallSetTurretPrefab(string turretPrefabName)
    {
        if (highlightArea != null)
        {
            highlightArea.SetTurretPrefab(turretPrefabName);
            Debug.Log("SetTurretPrefab 함수를 호출했습니다: " + turretPrefabName);
        }
        else
        {
            Debug.LogError("highlightArea가 할당되지 않았습니다.");
        }
    }

    public int SelectedIndex()
    {
        return highlightedSkillIndex;
    }
}
