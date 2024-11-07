using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

public class LevelConfig : SerializedScriptableObject
{
    public List<BottleData> bottleDataList = new List<BottleData>();
}