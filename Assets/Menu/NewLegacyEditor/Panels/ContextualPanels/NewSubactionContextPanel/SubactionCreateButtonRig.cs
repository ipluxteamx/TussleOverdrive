﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubactionCreateButtonRig : MonoBehaviour {
    public GameObject subactionButtonPrefab;

    private Dictionary<SubactionType, List<SubactionData>> subactionsByCategory = new Dictionary<SubactionType, List<SubactionData>>();
    private List<GameObject> children = new List<GameObject>();
    private UIGrid grid;
    public UIDraggablePanel dragPanel;

    void Awake()
    {
        grid = GetComponent<UIGrid>();
    }

    // Use this for initialization
    void Start () {

        SubactionData[] data = Resources.LoadAll<SubactionData>("SubactionData");
        foreach (SubactionData sub in data)
        {
            if (!subactionsByCategory.ContainsKey(sub.subType))
                subactionsByCategory[sub.subType] = new List<SubactionData>();

            subactionsByCategory[sub.subType].Add(sub);
        }
        Debug.Log(subactionsByCategory);

        //Find a way to get this width
        //grid.maxPerLine = (int)(GetComponent<UIWidget>().width / cellWidth);
	}

    void OnContextualPanelChanged()
    {
        //Only execute if it's the right kind of contextual panel
        if (ContextualPanelData.isOfType(typeof(NewSubactionContextPanel)))
        {
            NewSubactionContextPanel panel = (NewSubactionContextPanel)LegacyEditorData.contextualPanel;
            if (panel.selectedTypeDirty)
            {
                //Clear away all the old buttons
                foreach(GameObject child in children)
                {
                    NGUITools.Destroy(child);
                }
                children.Clear();
                
                foreach(SubactionData subData in subactionsByCategory[panel.selectedType])
                {
                    instantiateSubactionButton(subData);
                }

                //Realign the grid
                grid.Reposition();
                dragPanel.ResetPosition();
            }
        }
    }

    void instantiateSubactionButton(SubactionData subData)
    {
        GameObject go = NGUITools.AddChild(gameObject, subactionButtonPrefab);
        SubactionCreateButton button = go.GetComponent<SubactionCreateButton>();
        button.SetAction(subData);
        children.Add(go);
    }
}