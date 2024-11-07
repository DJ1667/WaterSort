#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class LevelConfigCreateByExcelData
{
    readonly string DataPath = "Assets/Resources/Data/AssetConfig/LevelConfig_ExcelData.asset";

    [LabelText("开始ID")]
    public int StartId = 0; // 开始ID
    [LabelText("结束ID")]
    public int EndId = 0; // 结束ID

    [Button("生成关卡配置")]
    public void CreateLevelConfig()
    {
        if (StartId < 1) StartId = 1;
        var levelConfig_ExcelData = AssetDatabase.LoadAssetAtPath<LevelConfig_ExcelData>(DataPath);
        levelConfig_ExcelData.Init();

        if (EndId > levelConfig_ExcelData.Sheet1ConfigList.Count) EndId = levelConfig_ExcelData.Sheet1ConfigList.Count;
        if (EndId < StartId) return;

        for (int i = StartId; i <= EndId; i++)
        {
            var config = levelConfig_ExcelData.GetSheet1_Config(i);
            LevelConfigCreateData levelData = new LevelConfigCreateData();
            levelData.ID = i;
            levelData.BottleCount = config.BottleCount;
            levelData.EmptyBottleCount = config.EmptyBottleCount;
            levelData.Segment = config.Segment;
            levelData.TempBottleCount = config.TempBottleCount;
            levelData.TempSegment = config.TempSegment;
            levelData.SingleColorBottleCount = config.SingleColorBottleCount;
            levelData.SingleColorSegment = config.SingleColorSegment;
            levelData.DegreeOfDifficulty = config.DegreeOfDifficulty;

            var bottleDataList = LevelCreatorRuntime.CreateLevelConfig(levelData);
            var obj = ScriptableObject.CreateInstance(typeof(LevelConfig)) as LevelConfig;
            obj.bottleDataList = bottleDataList;
            var dest = $"Assets/AssetBundle/ScriptableObjects/LevelConfig/LevelConfig{i}.asset";
            AssetDatabase.DeleteAsset(dest);
            AssetDatabase.CreateAsset(obj, dest);
        }
        AssetDatabase.Refresh();
    }
}
#endif