using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] Canvas screenSpace;
    [SerializeField] Canvas worldSpace;

    [SerializeField] Dialogue tutorial;
    [SerializeField] Dialogue intro;

    [SerializeField] GameObject crosshair;


    [SerializeField] PopUp popUp; 
    [SerializeField] UEye uEye; 


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        StateManager.OnStateChanged += StateChange;
    }

    void OnDisable()
    {
        StateManager.OnStateChanged -= StateChange;
    }

    void StateChange(StateManager.GameState state)
    {
        switch (state)
        {
            case StateManager.GameState.Title:
                Title();
                break;
            case StateManager.GameState.Intro:
                Intro();
                break;
            case StateManager.GameState.Tutorial:
                Tutorial();
                break;
            case StateManager.GameState.Lvl:
                break;
            case StateManager.GameState.Outro:
                break;
        }
    }

    void Title() 
    {

    }

    void Intro()
    {
        UIOn();
        intro.CallDialogue();
    }

    void Tutorial()
    {
        tutorial.CallDialogue();
    }

    public void TutorialOff()
    {
        tutorial.Off();
    }

    public void IntroOff()
    {
        intro.Off();
    }

    public void UIOn()
    {
        screenSpace.gameObject.SetActive(true);
        crosshair.gameObject.SetActive(true);
        worldSpace.gameObject.SetActive(true);
    }

    public void UIOff()
    {
        screenSpace.gameObject.SetActive(false);
        crosshair.gameObject.SetActive(false);
    }

    public void PopUp(string text)
    {
        popUp.UpdatePopUp(text); 
    }

    public void UpdateHealth(float newHealth)
    {
        uEye.UpdateHealth(newHealth); 
    }
}
