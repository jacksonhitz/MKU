using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Tutorial : ScenesManager
{
    //Main instruction text
    [SerializeField]
    private TMP_Text instruction;
    //Four text boxes mapping control name -> TMP_Text in counterclockwise order.
    //0 -> top, 1 -> left, 2 -> bottom, 3 -> right.
    [SerializeField]
    private List<TMP_Text> controls = new List<TMP_Text>();

    //Bools of whether actions are actually done. Same correspondance as controls
    private List<bool> completion = new List<bool> { false, false, false, false };

    //Reusable values
    [SerializeField]
    private Color32 todo;
    [SerializeField]
    private Color32 done;
    [SerializeField]
    private Color32 header;

    //Hands
    [SerializeField]
    private Transform leftHand;
    [SerializeField]
    private Transform rightHand;

    //For Pickup/Throw
    public bool itemHeldL = false;
    public bool itemHeldR = false;


    protected override void Awake()
    {
        base.Awake();
        StateManager.lvl = StateManager.GameState.TUTORIAL;
        if (StateManager.State != StateManager.GameState.FILE)
        {
            StateManager.StartLvl();
        }
    }

    protected override void Update()
    {
        base.Update();
        if (StateManager.IsActive())
        {
            EnumLogic();
        }
    }

    void EnumLogic()
    {
        if (!instruction.gameObject.activeSelf)
        {
            instruction.gameObject.SetActive(true);
        }

        switch (TutorialStateManager.State)
        {
            case TutorialStateManager.TutorialState.WASD:
                WASD();
                break;
            case TutorialStateManager.TutorialState.Jump:
                Jump();
                break;
            case TutorialStateManager.TutorialState.Punch:
                Punch();
                break;
            // case TutorialStateManager.TutorialState.Items:
            //     Items();
            //     break;
            case TutorialStateManager.TutorialState.Pickup:
                itemHeldL = leftHand.childCount > 0;
                itemHeldR = rightHand.childCount > 0;
                Pickup();
                break;
            case TutorialStateManager.TutorialState.Use:
                Use();
                break;
            case TutorialStateManager.TutorialState.Throw:
                Throw();
                itemHeldL = leftHand.childCount > 0;
                itemHeldR = rightHand.childCount > 0;
                break;
            case TutorialStateManager.TutorialState.Interact:
                Interact();
                break;
        }
    }

    void WASD()
    {
        //wasd
        //Input checks
        if (Input.GetKeyDown(KeyCode.W) && !completion[0])
        {
            controls[0].color = done;
            completion[0] = true;
        }
        if (Input.GetKeyDown(KeyCode.A) && !completion[1])
        {
            controls[1].color = done;
            completion[1] = true;
        }
        if (Input.GetKeyDown(KeyCode.S) && !completion[2])
        {
            controls[2].color = done;
            completion[2] = true;
        }
        if (Input.GetKeyDown(KeyCode.D) && !completion[3])
        {
            controls[3].color = done;
            completion[3] = true;
        }

        //Check if all are done
        if (completion[0] && completion[1] && completion[2] && completion[3])
        {
            TutorialStateManager.State = TutorialStateManager.TutorialState.Jump;
            completion[0] = completion[1] = completion[2] = completion[3] = false;
            //Change on-screen text/color/visibility
            instruction.text = "JUMP";
            controls[2].text = "SPACE";
            controls[2].color = todo;
            controls[0].gameObject.SetActive(false);
            controls[1].gameObject.SetActive(false);
            controls[3].gameObject.SetActive(false);

            Debug.Log("Changed state to Jump");
        }

    }
    void Jump()
    {
        //space
        //Input check - switch straight to next one, no resets of completion necessary.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TutorialStateManager.State = TutorialStateManager.TutorialState.Punch;
            //Set on-screen text/color/visibility
            instruction.text = "PUNCH";
            controls[0].gameObject.SetActive(true);
            controls[0].text = "CLICK";
            controls[0].color = header;
            controls[1].gameObject.SetActive(true);
            controls[1].text = "LEFT";
            controls[1].color = todo;
            controls[2].gameObject.SetActive(false);
            controls[3].gameObject.SetActive(true);
            controls[3].text = "RIGHT";
            controls[3].color = todo;

            Debug.Log("Changed state to Punch");
        }
    }
    void Punch()
    {
        // left/right click to punch
        if (Input.GetKeyDown(KeyCode.Mouse0) && !completion[1])
        {
            controls[1].color = done;
            completion[1] = true;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && !completion[3])
        {
            controls[3].color = done;
            completion[3] = true;
        }

        //Check if both are done
        if (completion[1] && completion[3])
        {
            TutorialStateManager.State = TutorialStateManager.TutorialState.Pickup;
            completion[1] = completion[3] = false;
            //Set on-screen text/color/visibility, as well as world space canvas?
            instruction.text = "PICK UP";
            controls[0].color = todo;
            controls[0].gameObject.SetActive(false);
            controls[1].color = todo;
            controls[1].text = "Q";
            controls[3].color = todo;
            controls[3].text = "E";

            Debug.Log("Changed state to Pickup");
        }
    }
    void Pickup()
    {
        // q/e to pickup to left/right hand
        if (Input.GetKeyDown(KeyCode.Q) && !completion[1] && itemHeldL)
        {
            controls[1].color = done;
            completion[1] = true;
        }
        if (Input.GetKeyDown(KeyCode.E) && !completion[3] && itemHeldR)
        {
            controls[3].color = done;
            completion[3] = true;
        }

        //Check if both are done
        if (completion[1] && completion[3])
        {
            completion[1] = completion[3] = false;
            TutorialStateManager.State = TutorialStateManager.TutorialState.Use;
            instruction.text = "USE";
            controls[0].gameObject.SetActive(true);
            controls[0].color = header;
            controls[0].text = "CLICK";
            controls[1].gameObject.SetActive(true);
            controls[1].color = todo;
            controls[1].text = "LEFT";
            controls[2].gameObject.SetActive(false);
            controls[3].gameObject.SetActive(true);
            controls[3].color = todo;
            controls[3].text = "RIGHT";

            Debug.Log("Changed state to Use");
        }
    }
    // void Items()
    // {
    //     // tab to see items
    //     if (Input.GetKeyDown(KeyCode.Tab))
    //     {
    //         TutorialStateManager.State = TutorialStateManager.TutorialState.Use;
    //         instruction.text = "USE";
    //         controls[0].gameObject.SetActive(true);
    //         controls[0].color = header;
    //         controls[0].text = "CLICK";
    //         controls[1].gameObject.SetActive(true);
    //         controls[1].color = todo;
    //         controls[1].text = "LEFT";
    //         controls[2].gameObject.SetActive(false);
    //         controls[3].gameObject.SetActive(true);
    //         controls[3].color = todo;
    //         controls[3].text = "RIGHT";

    //         Debug.Log("Changed state to Use");
    //     }
    // }
    void Use()
    {
        // left/right click to use left/right item
        if (Input.GetKeyDown(KeyCode.Mouse0) && !completion[1])
        {
            controls[1].color = done;
            completion[1] = true;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && !completion[3])
        {
            controls[3].color = done;
            completion[3] = true;
        }

        //Check if both are done
        if (completion[1] && completion[3])
        {
            TutorialStateManager.State = TutorialStateManager.TutorialState.Throw;
            completion[1] = completion[3] = false;
            //Set on-screen text/color/visibility
            instruction.text = "THROW";
            controls[0].color = todo;
            controls[0].gameObject.SetActive(false);
            controls[1].color = todo;
            controls[1].text = "Q";
            controls[3].color = todo;
            controls[3].text = "E";

            Debug.Log("Changed state to Throw");
        }
    }
    void Throw()
    {
        // q/e to throw left/right
        if (Input.GetKeyDown(KeyCode.Q) && !completion[1] && itemHeldL)
        {
            controls[1].color = done;
            completion[1] = true;
        }
        if (Input.GetKeyDown(KeyCode.E) && !completion[3] && itemHeldR)
        {
            controls[3].color = done;
            completion[3] = true;
        }

        //Check if both are done - end tutorial if so
        if (completion[1] && completion[3])
        {
            // StateManager.NextState(this);
            TutorialStateManager.State = TutorialStateManager.TutorialState.Interact;
            completion[1] = completion[3] = false;
            //Set on-screen text/color/visibility
            instruction.text = "INTERACT";
            controls[0].color = todo;
            controls[0].gameObject.SetActive(false);
            controls[1].gameObject.SetActive(false);
            controls[2].gameObject.SetActive(true);
            controls[2].color = todo;
            controls[2].text = "F";
            controls[3].gameObject.SetActive(false);

            Debug.Log("Changed state to Interact");

        }
    }

    public override void Interact()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TutorialStateManager.State = TutorialStateManager.TutorialState.Fight;
            instruction.text = "";
            controls[2].gameObject.SetActive(false);

            StateManager.LoadNext();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            StateManager.LoadNext();
        }
    }
}


//Tutorial-specific state machine
public static class TutorialStateManager
{
    public enum TutorialState
    {
        WASD,
        Jump,
        Punch,
        // Items,
        Pickup,
        Use,
        Throw,
        Interact,
        Fight,
        Done
    }

    private static TutorialState state;
    private static TutorialState previous;

    public static event Action<TutorialState> OnStateChanged;
    public static TutorialState Previous => previous;

    private static readonly HashSet<TutorialState> Stages = new()
    {
        TutorialState.WASD,
        TutorialState.Jump,
        TutorialState.Punch,
        // TutorialState.Items,
        TutorialState.Pickup,
        TutorialState.Use,
        TutorialState.Throw,
        TutorialState.Interact,
        TutorialState.Fight,
        TutorialState.Done,
    };

    public static TutorialState State
    {
        get => state;
        set
        {
            previous = state;
            state = value;

            OnStateChanged?.Invoke(state);
            Debug.Log($"Tutorial state changed to {state}");
        }
    }

    public static bool GroupCheck(HashSet<TutorialState> group)
    {
        return group.Contains(state);
    }
}
