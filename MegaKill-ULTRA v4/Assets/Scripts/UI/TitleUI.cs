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
    public void ChaptersButton()
    {

    }
    public void OptionsButton()
    {

    }
    public void ExitButton()
    {

    }

    public void TangoFile()
    {
        tangoText.Play();
        titleScreen.SetActive(false);
    }
}
