﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightDropdown : LegacyEditorWidget {
    private UIPopupList list;
    private Collider coll;

    private string[] rightDropdownOptions;

    // Use this for initialization
    void Awake()
    {
        list = GetComponent<UIPopupList>();
        coll = GetComponent<BoxCollider>();
    }

    void Start()
    {
        rightDropdownOptions = LegacyEditorConstants.RightDropdownOptionsDict[LegacyEditorData.instance.leftDropdown];
        UpdateListItems();
    }

    void UpdateListItems()
    {
        list.items.Clear();
        foreach (string opt in rightDropdownOptions)
        {
            list.items.Add(opt);
        }
        //If the list is only one (or zero I guess) items long, we don't need to actually be able to change dropdowns.
        if (list.items.Count < 2)
        {
            coll.enabled = false;
        } else
        {
            coll.enabled = true;
        }
        UpdateOptionWithoutEvent();
        //EventDelegate.Set(list.onChange, OnChangeDropdown); ^^
    }

    void OnLeftDropdownChanged(string s)
    {
        rightDropdownOptions = LegacyEditorConstants.RightDropdownOptionsDict[LegacyEditorData.instance.leftDropdown];
        UpdateListItems();
    }

    void OnRightDropdownChanged(string s)
    {
        UpdateOptionWithoutEvent();
    }

    //This is hacky as fuck, isn't it? I'm unsetting the event receiver so I can change this data without firing another change, preventing a double-fire and blowing up the redoList
    public void UpdateOptionWithoutEvent()
    {
        /* ^^
        EventDelegate.Remove(list.onChange, OnChangeDropdown);
        //list.eventReceiver = null; ^^
        list.value = LegacyEditorData.instance.rightDropdown;
        //list.eventReceiver = gameObject; ^^
        EventDelegate.Set(list.onChange, OnChangeDropdown);
        */
    }

    public void OnChangeDropdown()
    {
        string selected = UIPopupList.current.value;

        //Create a message object to have the model execute
        ModifyLegacyEditorDataAction act = ScriptableObject.CreateInstance<ModifyLegacyEditorDataAction>();
        act.init("rightDropdown", selected);

        LegacyEditorData.instance.DoAction(act);
    }

    public override void RegisterListeners()
    {
        editor.LeftDropdownChangedEvent += OnLeftDropdownChanged;
        editor.RightDropdownChangedEvent += OnRightDropdownChanged;
    }

    public override void UnregisterListeners()
    {
        editor.LeftDropdownChangedEvent -= OnLeftDropdownChanged;
        editor.RightDropdownChangedEvent -= OnRightDropdownChanged;
    }
}
