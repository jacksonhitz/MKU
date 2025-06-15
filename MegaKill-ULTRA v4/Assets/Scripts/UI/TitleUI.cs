using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    Title title;

    [SerializeField]
    Dialogue introText;

    [SerializeField]
    Dialogue tangoText;

    [SerializeField]
    GameObject titleScreen;

    void Awake()
    {
        title = FindObjectOfType<Title>();
    }

    public void StartButton()
    {
        // TODO: Check to see if this needs to be awaited
        _ = introText.Play();
        titleScreen.SetActive(false);
    }

    //Not sure if this will be used as it's simple to connect to them through the Unity UI
    public void ChaptersButton() { }

    // Todo: Change these to use one method with a parameter
    public void InstructionButton()
    {
        _ = StateManager.LoadLevel(StateManager.GameState.TUTORIAL, 2f, destroyCancellationToken);
    }

    public void RehearsalButton()
    {
        _ = StateManager.LoadLevel(StateManager.GameState.REHEARSAL, 2f, destroyCancellationToken);
    }

    public void TangoButton()
    {
        _ = StateManager.LoadLevel(StateManager.GameState.TANGO, 2f, destroyCancellationToken);
    }

    public void SableButton()
    {
        _ = StateManager.LoadLevel(StateManager.GameState.SABLE, 2f, destroyCancellationToken);
    }

    public void SpearheadButton()
    {
        _ = StateManager.LoadLevel(StateManager.GameState.SPEARHEAD, 2f, destroyCancellationToken);
    }

    public void OptionsButton()
    {
        SettingsManager.Instance.Pause();
    }

    public void ExitButton()
    {
        Debug.Log("Quitting/Exiting Application");
        Application.Quit();
    }

    public void TangoFile()
    {
        // TODO: Check to see if this needs to be awaited
        _ = tangoText.Play();
        titleScreen.SetActive(false);
    }
}
