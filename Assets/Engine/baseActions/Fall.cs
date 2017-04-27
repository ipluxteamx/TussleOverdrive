﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall : GameAction
{
    
    public override void stateTransitions()
    {
        base.stateTransitions();
        StateTransitions.AirState(actor.GetAbstractFighter());
        StateTransitions.CheckLedges(actor.GetAbstractFighter());
    }
}