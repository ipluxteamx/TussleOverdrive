﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyActionDataCheckbox : LegacyEditorWidget {
    public enum ActionVarType
    {
        FIELD,
        VARIABLE
    }

    public ActionVarType varSource;
    public string varName;
    private UIToggle input;

    private void Start()
    {
        input = GetComponent<UIToggle>();
    }

    void OnActionChanged(DynamicAction action)
    {
        if (LegacyEditorData.instance.currentAction.name != "")
        {
            input.value = getActionVar();
        }
    }

    public void OnAction(bool inputData)
    {
        LegacyEditorAction action = null;
        if (varSource == ActionVarType.FIELD)
        {
            action = ScriptableObject.CreateInstance<ChangeActionField>();
            ((ChangeActionField)action).init(varName, inputData);
        }
        else if (varSource == ActionVarType.VARIABLE)
        {
            action = ScriptableObject.CreateInstance<ChangeActionField>();
            ((ChangeActionField)action).init(varName, inputData);
        }
        LegacyEditorData.instance.DoAction(action);
    }

    private bool getActionVar()
    {
        DynamicAction action = LegacyEditorData.instance.currentAction;
        if (action.name != "")
        {
            if (varSource == ActionVarType.FIELD)
            {
                return (bool)action.GetType().GetField(varName).GetValue(action);
            }
        }
        //else
        //{
        //return action.GetVar(varName);
        //}
        return false;
    }

    public override void RegisterListeners()
    {
        editor.CurrentActionChangedEvent += OnActionChanged;
    }

    public override void UnregisterListeners()
    {
        editor.CurrentActionChangedEvent -= OnActionChanged;
    }
}
