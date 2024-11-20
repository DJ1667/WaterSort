using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class LevelController : SingletonBase<LevelController>
{
    readonly string LevelPath = "Assets/AssetBundle/ScriptableObjects/LevelConfig";

    private GameObject LevelRoot = null;

    public void LoadLevel()
    {
        if (LevelRoot != null)
            GameObject.Destroy(LevelRoot);

        LevelRoot = new GameObject("LevelRoot");

        var levelConfig = AssetDatabase.LoadAssetAtPath<LevelConfig>(LevelPath + "/LevelConfig10.asset");
        var bottlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AssetBundle/Prefabs/Bottle.prefab");
        var waterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AssetBundle/Prefabs/Water.prefab");

        for (int i = 0; i < levelConfig.bottleDataList.Count; i++)
        {
            var bottleData = levelConfig.bottleDataList[i];

            var colorList = bottleData.Colors;
            var segment = bottleData.Segment;
            var bottleObj = GameObject.Instantiate(bottlePrefab);
            bottleObj.transform.SetParent(LevelRoot.transform);

            var yOffset = i / 6;
            var xOffset = i % 6;
            bottleObj.transform.position = new Vector3(-12 - 2 + xOffset * 5.5f, 5 + yOffset * (-23), 0);
            var bottleMono = bottleObj.GetComponent<BottleMono>();
            var bottle = new Bottle();
            bottle.Segment = segment;
            bottleMono.Init(bottle, i);

            for (int j = 0; j < colorList.Count; j++)
            {
                var color = colorList[j];
                var waterObj = GameObject.Instantiate(waterPrefab);
                var waterMono = waterObj.GetComponent<WaterMono>();
                waterMono.Init(color);
                bottleMono.AddWater(waterMono, true);
            }
        }
    }
}
