using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // All refrences in playerController are shared between multiple compartments in order to avoid extra refrences between compartments.
    // All other refrences that are specific to a compartment are stored within that compartment

    public static PlayerController Instance { get; private set; }

    [Header("External")]
    public Camera cam;
    public SoundManager sound;

    [Header("Hands")]
    public Transform left;
    public Transform right;

    [Header("Compartments")]
    public PlayerCombat combat;
    public PlayerMovement movement;
    public PlayerInteract interact;
    public PlayerItems items;
    public PlayerHealth health;

    [Foldout("UI")]
    public Command commandUI;

    [Foldout("UI")]
    public Dialogue tutorialUI;

    [Foldout("UI")]
    public Dialogue dialogueUI;

    [Foldout("UI")]
    public Dialogue infoUI;

    [Foldout("UI")]
    public PopUp popUpUI;

    [Foldout("UI")]
    public UEye uEye;

    public void Awake()
    {
        Instance = this;

        combat = GetComponent<PlayerCombat>();
        movement = GetComponent<PlayerMovement>();
        interact = GetComponent<PlayerInteract>();
        items = GetComponent<PlayerItems>();
        health = GetComponent<PlayerHealth>();
    }

    public void OnApplicationQuit()
    {
        Instance = null;
    }
}
