﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// This class is here to derive the SerializableDictionaryPropertyDrawer class
/// for various other serializable dictionaries
/// </summary>

[CustomPropertyDrawer(typeof(SubVarDict))]
public class TussleSerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}