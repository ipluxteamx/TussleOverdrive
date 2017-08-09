﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleController : MonoBehaviour {
    public int current_game_frame = 0;

    public static BattleController current_battle;

    private List<BattleObject> objects = new List<BattleObject>();
    private Dictionary<int, AbstractFighter> fighterDict = new Dictionary<int, AbstractFighter>();
    private List<AbstractFighter> fighters = new List<AbstractFighter>();
    private List<Hitbox> hitboxes = new List<Hitbox>();
    public bool UpdateOnFrame;
    /// <summary>
    /// Singleton code. Will destroy any superfluous battle controllers that are in the scenes it loads into.
    /// When the battle processing is done, this object should be destroyed to make room for a new battle.
    /// </summary>
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (current_battle == null) //if we don't have a settings object
        {
            current_battle = this;
        }
        else //if it's already set
        {
            Destroy(gameObject); //Destroy the new one
        }
    }

    // Use this for initialization
    void Start()
    {
        //Fighters shouldn't collide with fighters
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Fighters"), LayerMask.NameToLayer("Fighters"), true);
        BattleLoader.current_loader.LoadBattle();
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (UpdateOnFrame)
        {
            foreach(BattleObject obj in objects)
                obj.StepFrame();
            foreach (Hitbox hbox in hitboxes)
                hbox.StepFrame();
            current_game_frame++;
        }

        if (Input.GetKeyDown(KeyCode.Slash))
            UpdateOnFrame = !UpdateOnFrame;
        if (Input.GetKeyDown(KeyCode.Period))
        {
            foreach (BattleObject obj in objects)
                obj.StepFrame();
            foreach (Hitbox hbox in hitboxes)
                hbox.StepFrame();
            current_game_frame++;
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
    
    /// <summary>
    /// Gets the fighter with the given player number from the list.
    /// </summary>
    /// <param name="playerNum">The number of the player to to find</param>
    /// <returns>A fighter with the given player number, or null if none is found</returns>
    public AbstractFighter GetFighter(int playerNum)
    {
        return fighterDict[playerNum];
    }

    public List<AbstractFighter> GetFighters()
    {
        return fighters;
    }

    /// <summary>
    /// Adds an object to the list of active battle objects
    /// </summary>
    /// <param name="obj"></param>
    public void RegisterObject(BattleObject obj)
    {
        objects.Add(obj);
        AbstractFighter fighter = obj.GetAbstractFighter();
        if (fighter != null)
        {
            fighters.Add(fighter);
            fighterDict.Add(fighter.player_num, fighter);
            SendMessage("LoadFighterIcons"); //Reload icons when a new fighter is added
        }
    }

    /// <summary>
    /// Removes an object from the list of active battle objects
    /// </summary>
    /// <param name="obj"></param>
    public void UnregisterObject(BattleObject obj)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hit"></param>
    public void RegisterHitbox(Hitbox hit)
    {
        hitboxes.Add(hit);
    }

    public void UnregisterHitbox(Hitbox hit)
    {

    }
}