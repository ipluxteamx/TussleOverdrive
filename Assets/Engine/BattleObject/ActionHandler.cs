﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandler : BattleComponent {
    private GameAction _current_action;
    public GameAction CurrentAction { get { return _current_action; } }

    public ActionFile actions_file_json = new ActionFile();
    
    // Use this for initialization
    void Start () {
        _current_action = new NeutralAction();
        _current_action.SetDynamicAction(actions_file_json.Get("NeutralAction"));
        _current_action.SetUp(battleObject);
    }

    // Update is called once per frame
    void Update () {
        _current_action.stateTransitions();
        _current_action.Update();
        _current_action.LateUpdate();
    }

    public void DoAction(string _actionName)
    {
        //Debug.Log("GameAction: "+_actionName);
        GameAction old_action = _current_action;
        _current_action = LoadAction(_actionName);
        _current_action.SetDynamicAction(actions_file_json.Get(_actionName));
        old_action.TearDown(_current_action);
        _current_action.SetUp(battleObject);
    }

    public static GameAction LoadAction(string _name)
    {
        switch (_name)
        {
            case "NeutralAction": return new NeutralAction();
            case "Fall": return new Fall();
            case "Jump": return new Jump();
            case "AirJump": return new AirJump();
            case "Crouch": return new Crouch();
            case "CrouchGetup": return new CrouchGetup();
            case "Move": return new Move();
            case "Stop": return new Stop();
            case "Land": return new Land();
            case "Dash": return new Dash();

            //Attacks
            case "NeutralAttack": return new BaseAttack();
            case "ForwardAttack": return new BaseAttack();
            case "ForwardSmash": return new BaseAttack();
            case "UpAttack": return new BaseAttack();
            case "UpSmash": return new BaseAttack();
            case "DownAttack": return new BaseAttack();
            case "DownSmash": return new BaseAttack();
            case "NeutralAir": return new AirAttack();
            case "ForwardAir": return new AirAttack();
            case "BackAir": return new AirAttack();
            case "UpAir": return new AirAttack();
            case "DownAir": return new AirAttack();
            default: return new GameAction();
        }
    }
}
