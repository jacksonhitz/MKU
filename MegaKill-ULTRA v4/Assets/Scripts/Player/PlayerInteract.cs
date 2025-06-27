using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
        InputManager.PlayerActionMap.Interact.performed += InteractOnPerformed;
        InputManager.PlayerActionMap.Highlight.performed += HighlightOnPerformed;
    }

    private void OnDestroy()
    {
        InputManager.PlayerActionMap.Interact.performed -= InteractOnPerformed;
        InputManager.PlayerActionMap.Highlight.performed -= HighlightOnPerformed;
    }

    private void HighlightOnPerformed(InputAction.CallbackContext obj)
    {
        InteractionManager.Instance.isHighlightAll = true;
    }

    private void InteractOnPerformed(InputAction.CallbackContext obj)
    {
        Interact();
    }

    private void LateUpdate()
    {
        Highlight();
    }

    void Highlight()
    {
        Assert.IsNotNull(_camera);
        Ray ray = new(_camera.transform.position, _camera.transform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, 30f))
            return;

        Interactable hovered = GetInteractable(hit.collider);
        if (hovered == null)
            return;
        hovered.isHovering = true;
        if (DOTween.Restart(hovered) == 0)
        {
            DOTween
                .Sequence()
                .AppendInterval(0.1f)
                .AppendCallback(() => hovered.isHovering = false)
                .SetId(hovered);
        }
    }

    private void Interact()
    {
        Assert.IsNotNull(_camera);
        Ray ray = new(_camera.transform.position, _camera.transform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, 30f))
            return;

        Interactable interactable = GetInteractable(hit.collider);
        if (interactable != null)
            interactable.Interact();
    }

    Interactable GetInteractable(Collider col)
    {
        // TODO: This needs to be fixed as it results in highlights of distant children
        // Instead we need to define a better way to grab a reference to the interact/prefab root
        Interactable interactable = col.GetComponentInParent<Interactable>();
        if (interactable == null)
            interactable = col.GetComponent<Interactable>();
        if (interactable == null)
            interactable = col.GetComponentInChildren<Interactable>();
        return interactable;
    }
}
