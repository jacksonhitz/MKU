using IngameDebugConsole;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtils;

public class PlayerItems : ValidatedMonoBehaviour
{
    public Item leftItem;
    public Item rightItem;
    public float throwForce;
    private bool usedLeft;
    private bool usedRight;

    [SerializeField, Self]
    PlayerController controller;

    private void OnEnable()
    {
        InputManager.PlayerActionMap.EquipLeft.performed += LeftEquip;
        InputManager.PlayerActionMap.EquipRight.performed += RightEquip;
        InputManager.PlayerActionMap.ThrowLeft.performed += LeftThrow;
        InputManager.PlayerActionMap.ThrowRight.performed += RightThrow;
        InputManager.PlayerActionMap.UseRight.performed += UseRight;
        InputManager.PlayerActionMap.UseLeft.performed += UseLeft;
        leftItem?.SetActive();
        rightItem?.SetActive();
    }

    private void OnDisable()
    {
        InputManager.PlayerActionMap.EquipLeft.performed -= LeftEquip;
        InputManager.PlayerActionMap.EquipRight.performed -= RightEquip;
        InputManager.PlayerActionMap.ThrowLeft.performed -= LeftThrow;
        InputManager.PlayerActionMap.ThrowRight.performed -= RightThrow;
        InputManager.PlayerActionMap.UseRight.performed -= UseRight;
        InputManager.PlayerActionMap.UseLeft.performed -= UseLeft;
        leftItem?.SetInactive();
        rightItem?.SetInactive();
    }

    private void Update()
    {
        usedLeft = false;
        usedRight = false;
    }

    private void UseLeft(InputAction.CallbackContext callbackContext)
    {
        if (!leftItem || usedLeft)
            return;
        usedLeft = true;
        leftItem.UseCheck();
    }

    private void UseRight(InputAction.CallbackContext callbackContext)
    {
        if (!rightItem || usedRight)
            return;
        usedRight = true;
        rightItem.UseCheck();
    }

    private void LeftThrow(InputAction.CallbackContext callbackContext)
    {
        if (!leftItem || usedLeft)
            return;
        usedLeft = true;
        Throw(leftItem);
    }

    private void RightThrow(InputAction.CallbackContext callbackContext)
    {
        if (!rightItem || usedRight)
            return;
        usedRight = true;
        Throw(rightItem);
    }

    private void LeftEquip(InputAction.CallbackContext callbackContext)
    {
        if (leftItem || usedLeft)
            return;
        usedLeft = true;
        GrabCheck(controller.left);
    }

    private void RightEquip(InputAction.CallbackContext callbackContext)
    {
        if (rightItem || usedRight)
            return;
        usedRight = true;
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
        SoundManager.Instance.CreateSoundBuilder().Play("Throw");

        if (item == leftItem)
            leftItem = null;
        else
            rightItem = null;
    }

    public enum ItemType
    {
        Revolver,
        Shotgun,
        MG,
        Beer,
        Meth,
    }

    [ConsoleMethod("GiveItem", "Give an item to the player")]
    public static void GiveItem(ItemType item)
    {
        var pi = PlayerController.Instance.items;
        bool isLeft = !pi.leftItem || (pi.leftItem && pi.rightItem);
        var hand = isLeft ? pi.controller.left : pi.controller.right;
        if (isLeft && pi.leftItem)
        {
            pi.leftItem.Dropped();
        }
        else if (!isLeft && pi.rightItem)
        {
            pi.rightItem.Dropped();
        }

        switch (item)
        {
            case ItemType.Revolver:
                var rev = Resources.Load<ItemData>("Items/Rev").prefab;
                pi.Grab(Instantiate(rev).GetComponent<Revolver>(), hand);
                break;
            case ItemType.MG:
                var mg = Resources.Load<ItemData>("Items/MG").prefab;
                pi.Grab(Instantiate(mg).GetComponent<MG>(), hand);
                break;
            case ItemType.Shotgun:
                var sg = Resources.Load<ItemData>("Items/SG").prefab;
                pi.Grab(Instantiate(sg).GetComponent<Shotgun>(), hand);
                break;
            case ItemType.Beer:
                var beer = Resources.Load<ItemData>("Items/Beer").prefab;
                pi.Grab(Instantiate(beer).GetComponent<Item>(), hand);
                break;
            case ItemType.Meth:
                var meth = Resources.Load<ItemData>("Items/Meth").prefab;
                pi.Grab(Instantiate(meth).GetComponent<Item>(), hand);
                break;
        }
    }
}
