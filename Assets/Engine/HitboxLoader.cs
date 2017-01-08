﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxLoader : MonoBehaviour {

    public Hitbox hitbox_prefab;

    public Hitbox LoadHitbox(AbstractFighter owner, float centerx = 0.0f, float centery = 0.0f, float width = 1, float height = 1)
    {
        Hitbox hbox = Instantiate(hitbox_prefab);
        hbox.transform.parent = owner.transform; //The Fighter Rig keeps the fighter and hitboxes scales from affecting eachother
        float scale = owner.GetComponent<SpriteLoader>().pixelsPerUnit;
        hbox.transform.localPosition = new Vector3(centerx/scale, centery/scale, 0.0f);
        hbox.transform.localScale = new Vector3(width/scale, height/scale, 1.0f);
        //Debug.Log(hbox.transform.position);
        return hbox;
    }
}
