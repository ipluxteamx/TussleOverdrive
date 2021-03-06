﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ANIMATION SUBACTION
/// Resets the sprite's rotation, back to default position
/// No arguments required.
/// </summary>
public class SubactionUnrotate : Subaction {

    public override void Execute(BattleObject actor, GameAction action)
    {
        base.Execute(actor, action);
        actor.SendMessage("UnRotate");
    }

    public override SubactionType getSubactionType()
    {
        return SubactionType.ANIMATION;
    }
}
