using KBCore.Refs;
using NaughtyAttributes;
using UnityEngine;
using UnityUtils;

public class PlayerController : Singleton<PlayerController>
{
    private bool active = true;
    public bool Active
    {
        get => active;
        set
        {
            bool wasActive = active;
            active = value;
            if (active == wasActive)
                return;
            if (active)
                Enable();
            else
                Disable();
        }
    }

    // All refrences in playerController are shared between multiple compartments in order to avoid extra refrences between compartments.
    // All other refrences that are specific to a compartment are stored within that compartment

    [Header("External")]
    [SerializeField, Child]
    public Camera cam;

    [SerializeField, KBCore.Refs.Scene]
    public SoundManager sound;

    [Header("Hands")]
    [SerializeField, Anywhere]
    public Transform left;

    [Anywhere]
    public Transform right;

    [Header("Compartments")]
    [SerializeField, Self]
    public PlayerCombat combat;

    [SerializeField, Self]
    public PlayerMovement movement;

    [SerializeField, Self]
    public PlayerInteract interact;

    [SerializeField, Self]
    public PlayerItems items;

    [SerializeField, Self]
    public PlayerHealth health;

    [Foldout("UI")]
    [SerializeField, Child]
    public Command commandUI;

    [Foldout("UI")]
    [SerializeField, Child]
    public Dialogue tutorialUI;

    [Foldout("UI")]
    [SerializeField, Child]
    public Dialogue dialogueUI;

    [Foldout("UI")]
    [SerializeField, Child]
    public Dialogue infoUI;

    [Foldout("UI")]
    [SerializeField, Child]
    public PopUp popUpUI;

    [Foldout("UI")]
    [SerializeField, Child]
    public UEye uEye;

    public void OnValidate() => this.ValidateRefs();

    private void Disable()
    {
        items.enabled = false;
        combat.enabled = false;
        movement.enabled = false;
        interact.enabled = false;
        health.enabled = false;
        commandUI.enabled = false;
    }

    private void Enable()
    {
        items.enabled = true;
        combat.enabled = true;
        movement.enabled = true;
        interact.enabled = true;
        health.enabled = true;
        commandUI.enabled = true;
    }
}
