using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleUI : MonoBehaviour
{
    Title title;
    [SerializeField] Dialogue introText;
    [SerializeField] Dialogue tangoText;
    [SerializeField] GameObject titleScreen;

    [SerializeField] GameObject settingsScreen;

    void Awake()
    {
        title = FindObjectOfType<Title>();
    }
    public void StartButton()
    {
        StateManager.LoadState(StateManager.GameState.INTRO);
        introText.Play();
        titleScreen.SetActive(false);
    }

    //Not sure if this will be used as it's simple to connect to them through the Unity UI
    public void ChaptersButton()
    {

    }

    public void InstructionButton()
    {
        StateManager.LoadState(StateManager.GameState.TUTORIAL);
    }
    public void RehearsalButton()
    {
        StateManager.LoadState(StateManager.GameState.REHEARSAL);
    }
    public void TangoButton()
    {
        StateManager.LoadState(StateManager.GameState.TANGO);
    }

    public void SableButton()
    {
        StateManager.LoadState(StateManager.GameState.SABLE);
    }
    public void SpearheadButton()
    {
        StateManager.LoadState(StateManager.GameState.SPEARHEAD);
    }
    public void OptionsButton()
    {
        settingsScreen.SetActive(true);
    }
    public void ExitButton()
    {
        Debug.Log("Quitting/Exiting Application");
        Application.Quit();
    }

    public void TangoFile()
    {
        tangoText.Play();
        titleScreen.SetActive(false);
    }
}
