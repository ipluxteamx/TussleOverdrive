﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSelectionButton : MonoBehaviour
{
    public UILabel label;
    private UIButton button;
    public AnimationDefinition animationDefinition;

    // Use this for initialization
    void Awake()
    {
        label = GetComponentInChildren<UILabel>();
        button = GetComponent<UIButton>();
    }

    void ChangedAnimation(AnimationDefinition anim)
    {
        //AnimationDefinition anim = LegacyEditorData.instance.currentAnimation;
        
        if (anim != null && anim == animationDefinition)
        {
            button.defaultColor = new Color(1, 1, 1, 1);
        }
        else
        {
            button.defaultColor = new Color(1, 1, 1, 0.5f);
        }
    }

    public void SetAnimation(AnimationDefinition anim)
    {
        animationDefinition = anim;
        label.text = anim.animationName;
        ChangeCurrentAnimationAction legacyAction = ScriptableObject.CreateInstance<ChangeCurrentAnimationAction>();
        legacyAction.init(anim);
        GetComponent<OnClickSendAction>().action = legacyAction;
    }

    private void OnEnable()
    {
        LegacyEditorData.instance.CurrentAnimationChangedEvent += ChangedAnimation;
    }

    private void OnDisable()
    {
        LegacyEditorData.instance.CurrentAnimationChangedEvent -= ChangedAnimation;
    }
}