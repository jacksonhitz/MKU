using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerInteract : MonoBehaviour
{
    void LateUpdate()
    {
        Highlight();
    }

    void Highlight()
    {
        Assert.IsNotNull(Camera.main);
        Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, 30f)) return;

        Interactable hovered = GetInteractable(hit.collider);
        if (hovered == null) return;
        hovered.isHovering = true;
        if (DOTween.Restart(hovered) == 0)
        {
            DOTween.Sequence()
                .AppendInterval(0.1f)
                .AppendCallback(() => hovered.isHovering = false)
                .SetId(hovered);
        }
    }

    public void Interact()
    {
        Assert.IsNotNull(Camera.main);
        Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, 30f)) return;
        
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
