﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStun : GameAction {
    public HitStun()
    {
        SetVar("angle", 0);
        SetVar("slow_getup", false);
        SetVar("feet_planted", false);
        SetVar("tech_cooldown", 0);
        SetVar("hitstun_length", 0);
    }

    public override void SetUp(BattleObject obj)
    {
        base.SetUp(obj);
        //These classes will be phased out as time goes on. Until then, we need to just exit early if we're in the builder since these don't actually use Subactions
        if (isInBuilder) return;
        SetVar("tech_cooldown", 0);
    }

    public override void stateTransitions()
    {
        base.stateTransitions();
        //These classes will be phased out as time goes on. Until then, we need to just exit early if we're in the builder since these don't actually use Subactions
        if (isInBuilder) return;
        int direction = (int)MotionHandler.GetDirectionMagnitude(actor).x;

        if (actor.GetIntVar("tech_window") > 0)
        {
            //_actor.ground_elasticity = 0
            //if _actor.grounded and not self.feet_planted:
            //    _actor.doTech()
        }
        else
        {
            if (last_frame > 15 && current_frame > 2) //If the hitstun is long enough and we're past the start of it
            {
                if (actor.GetFloatVar(TussleConstants.MotionVariableNames.YSPEED) >= actor.GetFloatVar("max_fall_speed"))
                {
                    actor.SetVar("ground_elasticity", actor.GetVarData(TussleConstants.FighterAttributes.HITSTUN_ELASTICITY));
                }
                else if (Mathf.Abs(actor.GetFloatVar(TussleConstants.MotionVariableNames.XSPEED)) > actor.GetFloatVar(TussleConstants.FighterAttributes.RUN_SPEED)) //skid trip
                {
                    actor.SetVar("ground_elasticity", 0);
                    if (actor.GetBoolVar(TussleConstants.FighterVariableNames.IS_GROUNDED) && !GetBoolVar("feet_planted"))
                    {
                        actor.SendMessage("DoAction", "Prone");
                    }
                }
                else if (actor.GetFloatVar(TussleConstants.MotionVariableNames.YSPEED) < actor.GetFloatVar("max_fall_speed"))
                {
                    actor.SetVar("ground_elasticity", 0);
                    if (actor.GetBoolVar(TussleConstants.FighterVariableNames.IS_GROUNDED) && !GetBoolVar("feet_planted"))
                    {
                        actor.SendMessage("DoAction", "Prone");
                    }
                }
                else
                    actor.SetVar("ground_elasticity", actor.GetFloatVar(TussleConstants.FighterAttributes.HITSTUN_ELASTICITY) / 2);
            }
            else if (last_frame <= 15) //If the hitstun is short, we don
                actor.SetVar("ground_elasticity", 0);
            else
                actor.SetVar("ground_elasticity", actor.GetVarData(TussleConstants.FighterAttributes.HITSTUN_ELASTICITY));
        }

        if (current_frame == last_frame)
        {
            if (last_frame > 15) //if it was long enough, tumble
            {
                if (actor.GetBoolVar(TussleConstants.FighterVariableNames.IS_GROUNDED))
                    actor.SendMessage("DoAction", "NeutralAction");
                else
                    actor.SendMessage("DoAction", "Fall"); //"Tumble");
            }
            else
            {
                if (actor.GetBoolVar(TussleConstants.FighterVariableNames.IS_GROUNDED))
                {
                    if (GetBoolVar("slow_getup"))
                        actor.SendMessage("DoAction", "SlowGetup"); //Jab Reset
                    else
                        actor.SendMessage("DoAction", "NeutralGetup");
                }
                else
                {
                    //actor.SetVar("landing_lag", actor.GetIntVar("heavy_land_lag"));
                    actor.SendMessage("DoAction", "Fall");
                }
            }
        }
    }

    public override void TearDown(GameAction new_action)
    {
        base.TearDown(new_action);
        //These classes will be phased out as time goes on. Until then, we need to just exit early if we're in the builder since these don't actually use Subactions
        if (isInBuilder) return;
        actor.SetVar("elasticity", 0.0f);
        actor.SetVar("ground_elasticity", 0.0f);
        actor.SetVar("tech_window", 0);
        actor.SendMessage("UnRotate");
    }

    public override void Update()
    {
        base.Update();
        //These classes will be phased out as time goes on. Until then, we need to just exit early if we're in the builder since these don't actually use Subactions
        if (isInBuilder) return;
        AbstractFighter fighter = actor.GetAbstractFighter();
        if (current_frame > 15 && actor.GetAbstractFighter().KeyBuffered("Shield", 5) && GetIntVar("tech_cooldown") == 0 && !actor.GetBoolVar(TussleConstants.FighterVariableNames.IS_GROUNDED))
        {
            actor.SetVar("tech_window", 12);
            //anti_grab = statusEffect.TemporaryHitFilter(_actor, hurtbox.GrabImmunity(_actor), 10)
            //anti_grab.activate()
            SetVar("tech_cooldown", 40);
        }
        if (actor.GetIntVar("tech_window") > 0)
            actor.SetVar("elasticity", 0.0f);
        else
            actor.SetVar("elasticity", actor.GetVarData(TussleConstants.FighterAttributes.HITSTUN_ELASTICITY));
        SetVar("feet_planted",actor.GetBoolVar(TussleConstants.FighterVariableNames.IS_GROUNDED));
        if (GetIntVar("tech_cooldown") > 0) SetVar("tech_cooldown",GetIntVar("tech_cooldown")-1);
        if (current_frame == 0)
        {
            //anti_grab = statusEffect.TemporaryHitFilter(_actor, hurtbox.GrabImmunity(_actor), 10)
            //anti_grab.activate()
            Vector2 directMagn = MotionHandler.GetDirectionMagnitude(actor.GetFloatVar(TussleConstants.MotionVariableNames.XSPEED), actor.GetFloatVar(TussleConstants.MotionVariableNames.XSPEED));
            if (directMagn.x != 0 && directMagn.x != 180)
            {
                actor.SetVar(TussleConstants.FighterVariableNames.IS_GROUNDED, false);
                if (directMagn.y > 10)
                {
                    actor.SendMessage("UnRotate");
                    actor.SendMessage("RotateSprite", (directMagn.x-90)*actor.GetIntVar(TussleConstants.FighterVariableNames.FACING_DIRECTION));
                }
                    
            }
            if (last_frame > 15) //If the hitstun is long enough
            {
                GameObject particles = ObjectPooler.current_pooler.GetPooledObject("LaunchTrail", actor.transform);
                particles.SetActive(true);
                if (fighter.hitTagged != null)
                {
                    particles.SendMessage("ChangeColor", Settings.current_settings.player_colors[fighter.hitTagged.GetIntVar(TussleConstants.FighterVariableNames.PLAYER_NUM)]);
                }
                particles.SendMessage("Play", last_frame);
            }
        }
    }
}
