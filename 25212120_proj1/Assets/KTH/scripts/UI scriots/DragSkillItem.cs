using UnityEngine;
using UnityEngine.EventSystems;

public class DragSkillItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent; // ���� �θ� Transform�� ����

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>(); // �θ� Canvas�� �����ɴϴ�.
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; // �巡�� ���� �� ���� �θ� ����
        canvasGroup.alpha = 0.6f; // ���� ����
        canvasGroup.blocksRaycasts = false; // Raycast ���� (����� �������� Ȯ���ϱ� ����)
    }

    public void OnDrag(PointerEventData eventData)
    {
        // �巡�� ���� �������� ���콺 ������ ��ġ�� �̵�
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // �巡�� ���� �� ���� ���� �� Raycast ���
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        // �ùٸ� ���Կ� ��ӵ��� �ʾ����� ���� ��ġ�� ���ư���
        if (transform.parent == originalParent)
        {
            rectTransform.anchoredPosition = Vector2.zero; // ���� ��ġ�� ���ư���
        }
    }
}
