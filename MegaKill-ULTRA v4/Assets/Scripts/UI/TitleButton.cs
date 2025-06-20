using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    public float moveDistance = 20f;

    public float animationSpeed = 20f;
    private bool isHovered;
    private Vector2 originalPosition;

    private RectTransform rectTransform;
    private Vector2 targetPosition;

    private GameObject triggerArea;
    private RectTransform triggerRect;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        CreateTriggerArea();
        targetPosition = originalPosition + Vector2.right * moveDistance;
    }

    private void OnDisable()
    {
        rectTransform.DOKill();
        rectTransform.anchoredPosition = originalPosition;
    }

    // Keep these for the actual button, but they won't be the primary detection
    public void OnPointerEnter(PointerEventData eventData)
    {
        TriggerHoverEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TriggerHoverExit();
    }

    private void CreateTriggerArea()
    {
        triggerArea = new GameObject("HoverTrigger");
        triggerArea.transform.SetParent(transform.parent);

        triggerRect = triggerArea.AddComponent<RectTransform>();
        var triggerImage = triggerArea.AddComponent<RawImage>();
        triggerImage.color = new Color(0, 0, 0, 0);
        triggerRect.anchoredPosition = originalPosition;

        // Extend the trigger area to both left and right sides
        float totalWidth = rectTransform.sizeDelta.x + (moveDistance * 2); // Extend on both sides
        triggerRect.sizeDelta = new Vector2(totalWidth, rectTransform.sizeDelta.y);

        // Keep the original pivot to maintain consistent positioning
        triggerRect.pivot = rectTransform.pivot;
        triggerRect.anchorMin = rectTransform.anchorMin;
        triggerRect.anchorMax = rectTransform.anchorMax;

        // Center the trigger rect by adjusting its position based on the pivot
        // The moveDistance is only applied horizontally
        float pivotOffsetX = totalWidth * (rectTransform.pivot.x - 0.5f);
        Vector2 centeredPosition = originalPosition - new Vector2(moveDistance - pivotOffsetX, 0);
        triggerRect.anchoredPosition = centeredPosition;

        var triggerHover = triggerArea.AddComponent<TriggerAreaHover>();
        triggerHover.parentButton = this;

        triggerArea.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }

    public void TriggerHoverEnter()
    {
        if (isHovered)
        {
            return;
        }

        isHovered = true;
        rectTransform.DOKill();
        rectTransform
            .DOAnchorPos(targetPosition, animationSpeed)
            .SetSpeedBased()
            .SetEase(Ease.OutSine);
        //TODO: play hover sound
    }

    public void TriggerHoverExit()
    {
        if (!isHovered)
        {
            return;
        }

        isHovered = false;
        rectTransform.DOKill();
        rectTransform
            .DOAnchorPos(originalPosition, animationSpeed)
            .SetSpeedBased()
            .SetEase(Ease.InSine);
    }
}

// Helper class for the trigger area
public class TriggerAreaHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]
    public TitleButton parentButton;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (parentButton)
        {
            parentButton.TriggerHoverEnter();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (parentButton)
        {
            parentButton.TriggerHoverExit();
        }
    }
}
