using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItems : ValidatedMonoBehaviour
{
    public Item leftItem;
    public Item rightItem;
    public float throwForce;

    [SerializeField, Self]
    PlayerController controller;

    private void Awake()
    {
        InputManager.PlayerActionMap.EquipLeft.performed += LeftEquip;
        InputManager.PlayerActionMap.EquipRight.performed += RightEquip;
        InputManager.PlayerActionMap.ThrowLeft.performed += LeftThrow;
        InputManager.PlayerActionMap.RightThrow.performed += RightThrow;
        InputManager.PlayerActionMap.UseRight.performed += UseRight;
        InputManager.PlayerActionMap.UseLeft.performed += UseLeft;
    }

    private void OnDestroy()
    {
        InputManager.PlayerActionMap.EquipLeft.performed -= LeftEquip;
        InputManager.PlayerActionMap.EquipRight.performed -= RightEquip;
        InputManager.PlayerActionMap.ThrowLeft.performed -= LeftThrow;
        InputManager.PlayerActionMap.RightThrow.performed -= RightThrow;
        InputManager.PlayerActionMap.UseRight.performed -= UseRight;
        InputManager.PlayerActionMap.UseLeft.performed -= UseLeft;
    }

    private void UseLeft(InputAction.CallbackContext callbackContext)
    {
        if (leftItem)
            leftItem.UseCheck();
    }

    private void UseRight(InputAction.CallbackContext callbackContext)
    {
        if (rightItem)
            rightItem.UseCheck();
    }

    private void LeftThrow(InputAction.CallbackContext callbackContext)
    {
        if (leftItem)
            Throw(leftItem);
    }

    private void RightThrow(InputAction.CallbackContext callbackContext)
    {
        if (rightItem)
            Throw(rightItem);
    }

    private void LeftEquip(InputAction.CallbackContext callbackContext)
    {
        if (!leftItem)
            GrabCheck(controller.left);
    }

    private void RightEquip(InputAction.CallbackContext callbackContext)
    {
        if (!rightItem)
            GrabCheck(controller.right);
    }

    void GrabCheck(Transform hand)
    {
        Ray ray = Camera.main!.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (!Physics.Raycast(ray, out RaycastHit hit, 30f))
            return;
        if (hit.collider.TryGetComponent(out Item item))
        {
            Grab(item, hand);
        }
    }

    void Grab(Item item, Transform hand)
    {
        if (hand == controller.left)
            leftItem = item;
        else
            rightItem = item;
        item.transform.SetParent(hand);
        item.Grabbed(hand);

        item.transform.localPosition = item.itemData.pos;
        item.transform.localRotation = Quaternion.Euler(item.itemData.rot);
    }

    private void Throw(Item item)
    {
        item.Thrown();
        item.rb.AddForce(Camera.main!.transform.forward * throwForce, ForceMode.Impulse);
        SoundManager.Instance.Play("Throw");

        if (item == leftItem)
            leftItem = null;
        else
            rightItem = null;
    }
}
