﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FighterModuleLoader : MonoBehaviour {
    //public PortraitRig portraitRig;

    private float lastYPos = -0f;
    public List<FighterInfo> infos;

	// Use this for initialization
	void Start () {
        LoadFighterList();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadFighterList()
    {
        DirectoryInfo[] individualFighters = FileLoader.FighterDir.GetDirectories();
        foreach (DirectoryInfo fighterDir in individualFighters)
        {
            FighterInfo info = FighterInfo.LoadFighterInfoFile(fighterDir.Name);
            infos.Add(info);
            //portraitRig.AddPanel(info);
        }
    }
}
