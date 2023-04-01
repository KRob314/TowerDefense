using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using System;

[CustomEditor(typeof(ButtonInteractable))]
public class ButtonInteractableEditor : Editor
{
    private ButtonInteractable Target
    {
        get { return (ButtonInteractable)target; }
    }

    /*
    public override void OnInspectorGUI()
    {
        ButtonInteractable targetMyButton = (ButtonInteractable)target;

        targetMyButton.Overlay = (GameObject)
            EditorGUILayout.ObjectField(
                "Overlay:",
                targetMyButton.Overlay,
                typeof(GameObject),
                true
            );

        base.OnInspectorGUI();
    }
    */
}
