using UnityEngine;
using UnityEngine.EventSystems;

public class DropSkillSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // ��ӵ� �������� �����ɴϴ�.
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem != null)
        {
            // ��ӵ� �������� �θ� ���� �������� ����
            droppedItem.transform.SetParent(transform);

            // ������ RectTransform ��������
            RectTransform slotRectTransform = GetComponent<RectTransform>();

            // ��ӵ� �������� RectTransform ��������
            RectTransform droppedRectTransform = droppedItem.GetComponent<RectTransform>();

            // ��ӵ� �������� ũ�⸦ ���� ũ�⿡ ���߱�
            droppedRectTransform.sizeDelta = slotRectTransform.sizeDelta;
            droppedRectTransform.anchoredPosition = Vector2.zero; // ���� �߾ӿ� ��ġ
        }
    }
}
