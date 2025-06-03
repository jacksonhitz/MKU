using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    PlayerController controller;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        Highlight();
    }

    void Highlight()
    {
        Ray ray = controller.cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        Interactable hovered = null;
        if (Physics.Raycast(ray, out RaycastHit hit, 30f))
            hovered = GetInteractable(hit.collider);

        foreach (Interactable interactable in InteractionManager.Instance.interactables)
        {
            if (interactable != null)
                interactable.isHovering = (interactable == hovered);
        }
    }

    public void Interact()
    {
        Ray ray = controller.cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, 30f))
        {
            Interactable interactable = GetInteractable(hit.collider);
            if (interactable != null)
                interactable.Interact();
        }
    }

    Interactable GetInteractable(Collider col)
    {
        Interactable interactable = col.GetComponentInParent<Interactable>();
        if (interactable == null)
            interactable = col.GetComponent<Interactable>();
        if (interactable == null)
            interactable = col.GetComponentInChildren<Interactable>();
        if (interactable == null)
            interactable = col.transform.root.GetComponentInChildren<Interactable>();
        return interactable;
    }
}
