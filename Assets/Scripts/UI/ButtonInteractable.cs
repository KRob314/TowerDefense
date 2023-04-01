using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInteractable : Button
{
    public event Action<bool> InteractableChanged;

    protected SelectionState prevState;
    public string disabledTextToDisplay;
    public Text disabledText;
    public Text buttonText;

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        // Debug.Log("DoStateTransition");

        base.DoStateTransition(state, instant);
        if (state != prevState)
        {
            InteractableChanged?.Invoke(interactable);
            prevState = state;
        }

        SetButtonText();
    }

    private void SetButtonText()
    {
        if (this.interactable)
        {
            // Debug.Log("is active");
            disabledText.text = "";
            disabledText.gameObject.SetActive(false);
            buttonText.gameObject.SetActive(true);
        }
        else
        {
            //Debug.Log("is not active");
            disabledText.text = disabledTextToDisplay;
            disabledText.gameObject.SetActive(true);
            buttonText.gameObject.SetActive(false);
            //buttonText.color = new Color(.1f, .1f, .1f); //Color.gray;
        }
    }

    public void Update() { }
}
