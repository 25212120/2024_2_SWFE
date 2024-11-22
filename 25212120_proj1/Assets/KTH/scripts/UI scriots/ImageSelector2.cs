using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageSelector2 : MonoBehaviour
{
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



        // 모든 하이라이트 이미지를 초기화 (비활성화)
        foreach (Image highlightImage in highlightImages)
        {
            highlightImage.gameObject.SetActive(false);
        }

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


}
