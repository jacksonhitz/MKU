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
            hovered = hit.collider.GetComponentInParent<Interactable>();

        foreach (Interactable interactable in InteractionManager.Instance.interactables)
            interactable.isHovering = (interactable == hovered);
    }

    public void Interact()
    {
        Ray ray = controller.cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, 30f))
        {
            Interactable interactable = hit.collider.GetComponentInParent<Interactable>();
            if (interactable != null)
                interactable.Interact();
        }
    }
}
