﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HitboxLoader : MonoBehaviour {
    public static HitboxLoader loader;

    public Hitbox hitbox_prefab;
    
    private void Start()
    {
        loader = this;
    }

    public static Hitbox CreateHitbox(GameObject owner, Dictionary<string, string> dict)
    {
        GameObject hboxObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hboxObj.name = "Hitbox";
        Hitbox hbox = hboxObj.AddComponent<Hitbox>();
        hbox.transform.SetParent(owner.transform);

        Renderer rend = hbox.GetComponent<Renderer>();
        Material mat = rend.material;
        mat.SetColor("_Color", new Color(1.0f, 0, 0, 0.5f));
        StandardShaderUtils.ChangeRenderMode(mat, StandardShaderUtils.BlendMode.Transparent);
        rend.enabled = Settings.current_settings.display_hitboxes;

        dict.Add("Width", "100");
        dict.Add("Height", "100");
        dict.Add("CenterX", "0");
        dict.Add("CenterY", "20");
        hbox.LoadValuesFromDict(dict);
        hbox.SizeToOwner(owner.GetComponent<BattleObject>());

        return hbox;
    }

    public Hitbox LoadHitbox(AbstractFighter owner, GameAction action, Dictionary<string, string> dict)
    {
        Hitbox hbox = Instantiate(hitbox_prefab);
        hbox.owner = owner.getBattleObject();
        hbox.transform.parent = owner.transform;
        hbox.LoadValuesFromDict(dict);

        //Flip it if the fighter is flipped
        if (owner.GetIntVar(TussleConstants.FighterVariableNames.FACING_DIRECTION) == -1)
            hbox.trajectory = 180 - hbox.trajectory;

        //Set up the hitbox lock, if applicable
        if (hbox.lock_name != "") //If it has a name, we need to check if it's got a lock already
        {
            if (action.hitbox_locks.ContainsKey(hbox.lock_name)) //If it's already there, just assign it to the hitbox
            {
                hbox.hitbox_lock = action.hitbox_locks[hbox.lock_name];
            }
            else //If it has a name, but isn't in the list, we need to add it
            {
                HitboxLock new_lock = new HitboxLock(hbox.lock_name);
                hbox.hitbox_lock = new_lock;
                action.hitbox_locks.Add(hbox.lock_name, new_lock);
            }
         }
        else //If it's unnamed, we just need to create a new lock for this hitbox
        {
            HitboxLock new_lock = new HitboxLock("GenericLockName"+action.hitbox_locks.Count.ToString());
            hbox.hitbox_lock = new_lock;
            action.hitbox_locks.Add(new_lock.name, new_lock);
        }

        return hbox;
    }
    
}
