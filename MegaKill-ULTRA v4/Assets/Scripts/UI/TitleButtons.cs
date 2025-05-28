using UnityEngine;
using UnityEngine.EventSystems;

public class TitleButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    SoundManager soundManager;

    [Header("Hover Settings")]
    public float moveDistance = 20f;
    public float animationSpeed = 5f;
    
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isAnimating = false;
    private bool isHovered = false;
    
    private GameObject triggerArea;
    private RectTransform triggerRect;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        targetPosition = originalPosition;
        
        CreateTriggerArea();
    }
    
    void CreateTriggerArea()
    {
        triggerArea = new GameObject("HoverTrigger");
        triggerArea.transform.SetParent(transform.parent); 
        
        triggerRect = triggerArea.AddComponent<RectTransform>();
        triggerRect.anchoredPosition = originalPosition;
        triggerRect.sizeDelta = new Vector2(rectTransform.sizeDelta.x + moveDistance, rectTransform.sizeDelta.y);
        triggerRect.anchorMin = rectTransform.anchorMin;
        triggerRect.anchorMax = rectTransform.anchorMax;
        triggerRect.pivot = rectTransform.pivot;
        
        var triggerHover = triggerArea.AddComponent<TriggerAreaHover>();
        triggerHover.parentButton = this;
        
        triggerArea.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }
    
    void Update()
    {
        if (isAnimating)
        {
            rectTransform.anchoredPosition = Vector3.Lerp(
                rectTransform.anchoredPosition, 
                targetPosition, 
                animationSpeed * Time.unscaledDeltaTime
            );
            
            if (Vector3.Distance(rectTransform.anchoredPosition, targetPosition) < 0.1f)
            {
                rectTransform.anchoredPosition = targetPosition;
                isAnimating = false;
            }
        }
    }
    
    public void TriggerHoverEnter()
    {
        if (!isHovered)
        {
            isHovered = true;
            targetPosition = originalPosition + Vector3.right * moveDistance;
            isAnimating = true;
            
            soundManager.GiveDrug();
            // Call your sound manager here
            // Example: SoundManager.Instance.PlayButtonHover();
            // Replace with your actual sound manager call
        }
    }
    
    public void TriggerHoverExit()
    {
        if (isHovered)
        {
            isHovered = false;
            targetPosition = originalPosition;
            isAnimating = true;
        }
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
    
    void OnDestroy()
    {
        if (triggerArea != null)
        {
            DestroyImmediate(triggerArea);
        }
    }
}

// Helper class for the trigger area
public class TriggerAreaHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public TitleButtons parentButton;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (parentButton != null)
            parentButton.TriggerHoverEnter();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (parentButton != null)
            parentButton.TriggerHoverExit();
    }
}