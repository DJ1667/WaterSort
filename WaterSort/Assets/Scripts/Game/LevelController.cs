using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class LevelController : SingletonBase<LevelController>
{
    readonly string LevelPath = "Assets/AssetBundle/ScriptableObjects/LevelConfig";

    public void LoadLevel()
    {
        var levelConfig = AssetDatabase.LoadAssetAtPath<LevelConfig>(LevelPath + "/LevelConfig1.asset");
        var bottlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AssetBundle/Prefabs/Bottle.prefab");
        var waterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AssetBundle/Prefabs/Water.prefab");

        for (int i = 0; i < levelConfig.BottleList.Count; i++)
        {
            var colorList = levelConfig.BottleList[i];
            var segment = levelConfig.BottleSegmentList[i];
            var bottleObj = GameObject.Instantiate(bottlePrefab);

            var yOffset = i / 5;
            var xOffset = i % 5;
            bottleObj.transform.position = new Vector3(-2 + xOffset, 1 + yOffset * (-4), 0);
            var bottleMono = bottleObj.GetComponent<BottleMono>();
            var bottle = new Bottle();
            bottle.Segment = segment;

            for (int j = 0; j < colorList.Count; j++)
            {
                var color = colorList[j];
                var waterObj = GameObject.Instantiate(waterPrefab);
                var waterMono = waterObj.GetComponent<WaterMono>();
                waterMono.Init(color);
                waterObj.transform.SetParent(bottleObj.transform);
                waterObj.transform.localPosition = new Vector3(0, 0.5f * j, 0);
                bottle.AddColor(color);
            }

            bottleMono.Init(bottle);
        }
    }
}
