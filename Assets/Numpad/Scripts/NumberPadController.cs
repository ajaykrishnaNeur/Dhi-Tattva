using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NumberPadController: MonoBehaviour
{
    public TMP_InputField inputText;

    private void Start()
    {
        if (inputText == null)
        {
            Debug.LogError("Input Text component not assigned!");
            return;
        }
    }

    public void OnButtonClick(string value)
    {
        inputText.text += value;
    }

    public void OnBackspaceClick()
    {
        if (inputText.text.Length > 0)
        {
            inputText.text = inputText.text.Substring(0, inputText.text.Length - 1);
        }
    }

    public void OnSubmitClick()
    {
        Debug.Log("Entered Number: " + inputText.text);
    }
}

