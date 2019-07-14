﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSubimageOrderAction : LegacyEditorAction
{
    private AnimationDefinition animationToModify;
    private int index;
    private int moveAmount;

    public void init(int index, int moveAmount)
    {
        this.index = index;
        this.moveAmount = moveAmount;
    }

    public override void execute()
    {
        animationToModify = LegacyEditorData.instance.currentAnimation;
        animationToModify.moveSubimageIndex(index, moveAmount);
        LegacyEditorData.ChangedAnimation();
    }

    public override void undo()
    {
        animationToModify.moveSubimageIndex(index+moveAmount, -1*moveAmount);
        LegacyEditorData.ChangedAnimation();
    }
}
