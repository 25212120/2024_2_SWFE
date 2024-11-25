using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageSelector : MonoBehaviour
{
    public Button displaySkillSlot1; // 첫 번째 스킬 슬롯에 사용할 Button 컴포넌트
    public Button displaySkillSlot2; // 두 번째 스킬 슬롯에 사용할 Button 컴포넌트

    [System.Serializable]
    public class SkillButtonInfo
    {
        public Button button; // 마법 버튼
        public Image highlightImage; // 하이라이트 이미지
        public PlayerMagicType magicType; // 마법 타입
    }

    public List<SkillButtonInfo> skillButtonInfos; // 마법 버튼 정보 리스트
    public PlayerInputManager playerInputManager; // PlayerInputManager 참조 (Inspector에서 할당)
    public PlayerInventory playerInventory; // PlayerInventory 참조 (Inspector에서 할당)

    private int highlightedSkillIndex = -1; // 현재 하이라이트된 스킬 인덱스 (-1은 선택되지 않음을 의미)
    private PlayerStateType slot1Skill = PlayerStateType.None;
    private PlayerStateType slot2Skill = PlayerStateType.None;

    void Start()
    {
        // 스킬 트리 활성화 이벤트 구독
        playerInventory.OnSkillTreeUnlocked += OnSkillTreeUnlocked;

        // 각 스킬 버튼에 클릭 이벤트와 초기 설정 적용
        for (int i = 0; i < skillButtonInfos.Count; i++)
        {
            int index = i; // 로컬 변수로 인덱스를 저장하여 클로저 문제 방지
            SkillButtonInfo info = skillButtonInfos[i];

            // 클릭 이벤트 설정
            info.button.onClick.AddListener(() => OnSkillButtonClick(index));

            // 스킬 트리 활성화 여부에 따라 버튼 활성화/비활성화
            bool isUnlocked = IsSkillTreeUnlocked(info.magicType);
            info.button.interactable = isUnlocked;

            // 하이라이트 이미지 비활성화
            info.highlightImage.gameObject.SetActive(false);
        }

        // 슬롯 버튼에 클릭 이벤트 추가
        displaySkillSlot1.onClick.AddListener(() => AssignSkillToSlot(1));
        displaySkillSlot2.onClick.AddListener(() => AssignSkillToSlot(2));

        Debug.Log("Start method executed, event listeners are set.");
    }

    // 스킬 트리 활성화 여부 확인 함수
    private bool IsSkillTreeUnlocked(PlayerMagicType magicType)
    {
        switch (magicType)
        {
            case PlayerMagicType.Wood:
                return playerInventory.MagicSkillTree_Wood;
            case PlayerMagicType.Fire:
                return playerInventory.MagicSkillTree_Fire;
            case PlayerMagicType.Ice:
                return playerInventory.MagicSkillTree_Ice;
            case PlayerMagicType.Sand:
                return playerInventory.MagicSkillTree_Sand;
            default:
                return false;
        }
    }

    // 스킬 트리 활성화 시 호출되는 함수
    private void OnSkillTreeUnlocked(PlayerMagicType magicType)
    {
        // 해당 마법 타입의 버튼을 찾아 활성화
        foreach (var info in skillButtonInfos)
        {
            if (info.magicType == magicType)
            {
                info.button.interactable = true;
                Debug.Log($"{magicType} 마법 버튼이 활성화되었습니다.");
                break;
            }
        }
    }

    // 버튼 클릭 시 실행되는 함수
    void OnSkillButtonClick(int index)
    {
        // 이전 하이라이트 이미지를 비활성화
        if (highlightedSkillIndex != -1)
        {
            skillButtonInfos[highlightedSkillIndex].highlightImage.gameObject.SetActive(false);
        }

        // 새로운 하이라이트 이미지를 활성화
        highlightedSkillIndex = index;
        skillButtonInfos[highlightedSkillIndex].highlightImage.gameObject.SetActive(true);

        Debug.Log("Skill button clicked, index: " + index);
    }

    // 슬롯에 스킬을 할당하는 함수
    public void AssignSkillToSlot(int slotNumber)
    {
        if (highlightedSkillIndex == -1)
        {
            Debug.Log("No skill selected to assign to slot.");
            return; // 선택된 스킬이 없으면 아무 작업도 하지 않음
        }

        // 선택된 스킬 정보 가져오기
        SkillButtonInfo selectedSkillInfo = skillButtonInfos[highlightedSkillIndex];
        PlayerStateType selectedSkill = ConvertMagicTypeToStateType(selectedSkillInfo.magicType);

        // 중복된 스킬 할당 방지
        if ((slotNumber == 1 && selectedSkill == slot2Skill) || (slotNumber == 2 && selectedSkill == slot1Skill))
        {
            Debug.Log("Skill is already assigned to the other slot.");
            return;
        }

        // PlayerInputManager의 Magic1Swap 또는 Magic2Swap 호출
        if (slotNumber == 1)
        {
            playerInputManager.Magic1Swap(selectedSkill);
            slot1Skill = selectedSkill;
            UpdateSlotImage(displaySkillSlot1, selectedSkillInfo);
        }
        else if (slotNumber == 2)
        {
            playerInputManager.Magic2Swap(selectedSkill);
            slot2Skill = selectedSkill;
            UpdateSlotImage(displaySkillSlot2, selectedSkillInfo);
        }

        // 스킬을 슬롯에 할당한 후 하이라이트를 비활성화
        skillButtonInfos[highlightedSkillIndex].highlightImage.gameObject.SetActive(false);
        highlightedSkillIndex = -1;
    }

    // 슬롯의 이미지를 업데이트하는 함수
    private void UpdateSlotImage(Button slotButton, SkillButtonInfo skillInfo)
    {
        Image selectedSkillImage = skillInfo.button.GetComponent<Image>();
        if (selectedSkillImage != null)
        {
            slotButton.GetComponent<Image>().sprite = selectedSkillImage.sprite;
            Debug.Log($"Assigned skill to slot with sprite: {selectedSkillImage.sprite.name}");
        }
        else
        {
            Debug.Log("Selected Skill Image or Sprite is null");
        }
    }

    // 마법 타입을 플레이어 상태 타입으로 변환하는 함수
    private PlayerStateType ConvertMagicTypeToStateType(PlayerMagicType magicType)
    {
        switch (magicType)
        {
            case PlayerMagicType.Wood:
                return PlayerStateType.WoodMagic;
            case PlayerMagicType.Fire:
                return PlayerStateType.FireMagic;
            case PlayerMagicType.Ice:
                return PlayerStateType.IceMagic;
            case PlayerMagicType.Sand:
                return PlayerStateType.SandMagic;
            default:
                return PlayerStateType.None;
        }
    }
}
