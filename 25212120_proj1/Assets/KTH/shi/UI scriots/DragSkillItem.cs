using UnityEngine;
using UnityEngine.EventSystems;

public class DragSkillItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent; // 원래 부모 Transform을 저장

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>(); // 부모 Canvas를 가져옵니다.
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; // 드래그 시작 시 현재 부모를 저장
        canvasGroup.alpha = 0.6f; // 투명도 조절
        canvasGroup.blocksRaycasts = false; // Raycast 차단 (드롭이 가능한지 확인하기 위해)
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 중인 아이템을 마우스 포인터 위치로 이동
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 종료 시 투명도 복구 및 Raycast 허용
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        // 올바른 슬롯에 드롭되지 않았으면 원래 위치로 돌아가기
        if (transform.parent == originalParent)
        {
            rectTransform.anchoredPosition = Vector2.zero; // 원래 위치로 돌아가기
        }
    }
}
