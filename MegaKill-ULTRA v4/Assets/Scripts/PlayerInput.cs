using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInput : MonoBehaviour
{
    public TMP_InputField inputField;
    private List<string> playerAnswer = new List<string>();

    private void Start()
    {
        if (inputField != null)
        {
            inputField.onSubmit.AddListener(delegate { inputField.ActivateInputField(); });
        }
        else
        {
            Debug.LogError("Input Field is not assigned in the inspector");
        }
    }
    public void StoreAnswer()
    {
        if (inputField != null && !string.IsNullOrEmpty(inputField.text))
        {
            playerAnswer.Add(inputField.text);
            Debug.Log("Answer recorded: " + playerAnswer[playerAnswer.Count - 1]);
            inputField.text = "";
        }
        else
        {
            Debug.LogWarning("Input Field is empty or not assigned.");
        }
    }

    public void OnSubmit()
    {
        StoreAnswer();
    }

}
