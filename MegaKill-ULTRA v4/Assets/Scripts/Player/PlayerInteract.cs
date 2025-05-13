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

    public void Interact()
    {
        Ray ray = controller.cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, 30f))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            interactable?.Interact();
        }
    }
}
