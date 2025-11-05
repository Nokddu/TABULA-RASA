using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IDragHandler
{
    private RectTransform windowToDrag;

    private Canvas canvas;

    void Start()
    {
        // 스크립트가 시작될 때, 이 오브젝트의 부모를 찾아 windowToDrag에 자동으로 할당
        windowToDrag = transform.parent.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 자동으로 찾아온 windowToDrag를 움직임
        windowToDrag.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}