using UnityEngine;

public class PlayerItems : MonoBehaviour
{
    public Item leftItem;
    public Item rightItem;
    public float throwForce;

    PlayerController controller;


    void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    public void UseLeft()
    {
        if (leftItem == null)
            controller.combat.Punch(controller.left);
        else
            leftItem.Invoke("Use", 0f);
    }

    public void UseRight()
    {
        if (rightItem == null)
            controller.combat.Punch(controller.right);
        else
            rightItem.Invoke("Use", 0f);
    }

    public void Left()
    {
        if (leftItem == null) GrabCheck(controller.left);
        else Throw(leftItem);
    }
    public void Right()
    {
        if (rightItem == null) GrabCheck(controller.right);
        else Throw(rightItem);
    }

    void GrabCheck(Transform hand)
    {
        Ray ray = controller.cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, 30f))
        {
            if (hit.collider.TryGetComponent(out Item item))
            {
                Grab(item, hand);
            }
        }
    }

    void Grab(Item item, Transform hand)
    {
        if (hand == controller.left) leftItem = item;
        else rightItem = item;
        item.transform.SetParent(hand);
        item.PlayerGrabbed();

        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    public void Throw(Item item)
    {
        item.Thrown();
        item.rb.AddForce(controller.cam.transform.forward * throwForce, ForceMode.Impulse);

        if (item == leftItem) leftItem = null;
        else rightItem = null;
    }
}
