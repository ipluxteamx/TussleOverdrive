﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegacyEditorConstants {
    public static string[] LeftDropdownOptions = new string[] {
        "Fighter", "Actions", "Methods", "Sprites", "Animations"
        };

    public static string[] RightDropdownOptions = new string[] {
        "Properties"
        };

    public static Dictionary<string, string[]> RightDropdownOptionsDict = new Dictionary<string, string[]>
    {
        { "Fighter",    new string[] { "Attributes", "Variables", "Palettes"} },
        { "Actions",    new string[] { "Properties", "Subactions", "Hitboxes", "Hurtboxes" } },
        { "Methods",    new string[] { "" } },
        { "Sprites",    new string[] { "Subimages"} },
        { "Animations", new string[] { "Properties", "Subimages" } }
    };
}