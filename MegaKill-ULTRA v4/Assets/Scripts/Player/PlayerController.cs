using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // All refrences in playerController are shared between multiple compartments in order to avoid extra refrences between compartments.
    // All other refrences that are specific to a compartment are stored within that compartment

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


    void Awake()
    {
        cam = Camera.main;

        combat = GetComponent<PlayerCombat>();
        movement = GetComponent<PlayerMovement>();
        interact = GetComponent<PlayerInteract>();
        items = GetComponent<PlayerItems>();
        health = GetComponent<PlayerHealth>();
    }

    void Start()
    {
        sound = SoundManager.Instance;
    }
}
