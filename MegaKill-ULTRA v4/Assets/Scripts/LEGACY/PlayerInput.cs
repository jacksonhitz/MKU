using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerInput : MonoBehaviour
{
    public TMP_InputField inputField;
    List<string> playerAnswer = new List<string>();

    public void InputBox()
    {
        inputField.ActivateInputField();
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
    }
    public void Clear()
    {
        inputField.text = "";
    }
    public void StoreAnswer()
    {
        if (inputField != null && !string.IsNullOrEmpty(inputField.text))
        {
            playerAnswer.Add(inputField.text);
            Debug.Log("Answer: " + playerAnswer[playerAnswer.Count - 1]);
            inputField.text = "";
        }
    }

    public void OnSubmit()
    {
        StoreAnswer();
    }

}
