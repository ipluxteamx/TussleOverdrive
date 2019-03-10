﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LegacyEditorWidget : MonoBehaviour
{
    protected LegacyEditorData editor;

    private bool registeredListeners = false;

    public void SetEditor(LegacyEditorData editor)
    {
        this.editor = editor;
    }

    private void Start()
    {
        SetEditor(LegacyEditorData.instance);
        RegisterListeners();
        registeredListeners = true;
    }

    private void OnDisable()
    {
        if (registeredListeners)
        {
            UnregisterListeners();
        }
    }
    public abstract void RegisterListeners();
    public abstract void UnregisterListeners();
}