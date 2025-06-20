using System;
using KBCore.Refs;
using NaughtyAttributes;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // All refrences in playerController are shared between multiple compartments in order to avoid extra refrences between compartments.
    // All other refrences that are specific to a compartment are stored within that compartment

    [ResetOnPlay]
    public static PlayerController Instance { get; private set; }

    [Header("External")]
    [Child]
    public Camera cam;

    [KBCore.Refs.Scene]
    public SoundManager sound;

    [Header("Hands")]
    [Anywhere]
    public Transform left;

    [Anywhere]
    public Transform right;

    [Header("Compartments")]
    [Self]
    public PlayerCombat combat;

    [Self]
    public PlayerMovement movement;

    [Self]
    public PlayerInteract interact;

    [Self]
    public PlayerItems items;

    [Self]
    public PlayerHealth health;

    [Foldout("UI")]
    [Child]
    public Command commandUI;

    [Foldout("UI")]
    [Child]
    public Dialogue tutorialUI;

    [Foldout("UI")]
    [Child]
    public Dialogue dialogueUI;

    [Foldout("UI")]
    [Child]
    public Dialogue infoUI;

    [Foldout("UI")]
    [Child]
    public PopUp popUpUI;

    [Foldout("UI")]
    [Child]
    public UEye uEye;

    public void Awake()
    {
        Instance = this;
    }

    public void OnValidate() => this.ValidateRefs();
}
