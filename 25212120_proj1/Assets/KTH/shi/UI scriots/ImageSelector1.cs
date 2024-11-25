using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageSelector1 : MonoBehaviour
{
    public Image displaySkillSlot; // 하나의 스킬 슬롯에 사용할 Image 컴포넌트 (Inspector에서 할당)
    public List<Button> skillButtons; // 여러 개의 스킬 버튼들 (Inspector에서 할당)
    public List<GameObject> highlightObjects; // 각 스킬에 대한 하이라이트 오브젝트들 (Inspector에서 할당)
    public List<Image> skillImagesToChange; // 변경할 이미지 리스트 (Inspector에서 할당)

    private int highlightedSkillIndex = -1; // 현재 하이라이트된 스킬 인덱스 (-1은 선택되지 않음을 의미)

    void Start()
    {
        // 모든 하이라이트 오브젝트를 초기화 (비활성화)
        foreach (GameObject highlightObject in highlightObjects)
        {
            highlightObject.SetActive(false);
        }
    }

    void Update()
    {
        // 키보드 입력 처리 (0, 1, 2, 3, 4, 5 키)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivateHighlight(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActivateHighlight(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ActivateHighlight(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ActivateHighlight(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ActivateHighlight(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ActivateHighlight(5);
        }
    }

    // 하이라이트 오브젝트를 활성화하는 함수
    private void ActivateHighlight(int index)
    {
        if (index < 0 || index >= highlightObjects.Count)
        {
            Debug.Log("Invalid index for highlight: " + index);
            return;
        }

        // 이전 하이라이트 오브젝트를 비활성화
        if (highlightedSkillIndex != -1)
        {
            highlightObjects[highlightedSkillIndex].SetActive(false);
        }

        // 새로운 하이라이트 오브젝트를 활성화
        highlightedSkillIndex = index;
        highlightObjects[highlightedSkillIndex].SetActive(true);

        // 인덱스에 해당하는 이미지로 displaySkillSlot 변경
        if (index < skillImagesToChange.Count)
        {
            Image selectedSkillImage = skillImagesToChange[index];
            if (selectedSkillImage != null && displaySkillSlot != null)
            {
                displaySkillSlot.sprite = selectedSkillImage.sprite;
                Debug.Log("Display skill slot image updated to index: " + index);
            }
        }

        Debug.Log("Highlight activated for index: " + index);
    }
}
