using UnityEngine;
using UnityEngine.EventSystems;

public class DropSkillSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // 드롭된 아이템을 가져옵니다.
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem != null)
        {
            // 드롭된 아이템의 부모를 현재 슬롯으로 변경
            droppedItem.transform.SetParent(transform);

            // 슬롯의 RectTransform 가져오기
            RectTransform slotRectTransform = GetComponent<RectTransform>();

            // 드롭된 아이템의 RectTransform 가져오기
            RectTransform droppedRectTransform = droppedItem.GetComponent<RectTransform>();

            // 드롭된 아이템의 크기를 슬롯 크기에 맞추기
            droppedRectTransform.sizeDelta = slotRectTransform.sizeDelta;
            droppedRectTransform.anchoredPosition = Vector2.zero; // 슬롯 중앙에 배치
        }
    }
}
