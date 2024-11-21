using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageSelector : MonoBehaviour
{
    public Button displaySkillSlot1; // 첫 번째 스킬 슬롯에 사용할 Button 컴포넌트
    public Button displaySkillSlot2; // 두 번째 스킬 슬롯에 사용할 Button 컴포넌트
    public List<Button> skillButtons; // 여러 개의 스킬 버튼들 (Inspector에서 할당)
    public List<Image> highlightImages; // 각 스킬에 대한 하이라이트 이미지들 (Inspector에서 할당)

    private int highlightedSkillIndex = -1; // 현재 하이라이트된 스킬 인덱스 (-1은 선택되지 않음을 의미)

    void Start()
    {
        // 각 버튼에 클릭 이벤트를 동적으로 연결
        for (int i = 0; i < skillButtons.Count; i++)
        {
            int index = i; // 로컬 변수로 인덱스를 저장하여 클로저 문제 방지
            skillButtons[i].onClick.AddListener(() => OnSkillButtonClick(index));
        }

        // 슬롯 버튼에 클릭 이벤트 추가
        displaySkillSlot1.onClick.AddListener(() => AssignSkillToSlot(1));
        displaySkillSlot2.onClick.AddListener(() => AssignSkillToSlot(2));

        // 모든 하이라이트 이미지를 초기화 (비활성화)
        foreach (Image highlightImage in highlightImages)
        {
            highlightImage.gameObject.SetActive(false);
        }

        Debug.Log("Start method executed, event listeners are set.");
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

        Debug.Log("Skill button clicked, index: " + index);
    }

    // 슬롯에 스킬을 할당하는 함수 (슬롯 1 또는 슬롯 2에 할당)
    public void AssignSkillToSlot(int slotNumber)
    {
        if (highlightedSkillIndex == -1)
        {
            Debug.Log("No skill selected to assign to slot.");
            return; // 선택된 스킬이 없으면 아무 작업도 하지 않음
        }

        Image selectedSkillImage = skillButtons[highlightedSkillIndex].GetComponent<Image>();
        if (selectedSkillImage != null)
        {
            if (slotNumber == 1 && displaySkillSlot1 != null)
            {
                displaySkillSlot1.GetComponent<Image>().sprite = selectedSkillImage.sprite;
                Debug.Log("Assigned to Slot 1 with sprite: " + selectedSkillImage.sprite.name);
            }
            else if (slotNumber == 2 && displaySkillSlot2 != null)
            {
                displaySkillSlot2.GetComponent<Image>().sprite = selectedSkillImage.sprite;
                Debug.Log("Assigned to Slot 2 with sprite: " + selectedSkillImage.sprite.name);
            }
        }
        else
        {
            Debug.Log("Selected Skill Image or Sprite is null");
        }

        // 스킬을 슬롯에 할당한 후 하이라이트를 비활성화
        highlightImages[highlightedSkillIndex].gameObject.SetActive(false);
        highlightedSkillIndex = -1;
    }
}
