using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

public class DemoStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // LevelController.Instance.LoadLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Profiler.BeginSample("WaterSort");
            var levelConfig = AssetDatabase.LoadAssetAtPath<LevelConfig>("Assets/AssetBundle/ScriptableObjects/LevelConfig/LevelConfig1.asset");
            BottleState bottleState = new BottleState();
            bottleState.segmentMax = levelConfig.SegmentMax;
            bottleState.bottles = new Bottle[levelConfig.BottleList.Count];
            for (int i = 0; i < levelConfig.BottleList.Count; i++)
            {
                var bottle = new Bottle();
                bottle.Segment = levelConfig.BottleSegmentList[i];
                foreach (var color in levelConfig.BottleList[i])
                {
                    bottle.AddColor(color);
                }
                bottleState.bottles[i] = bottle;
                // Debug.Log($"第 {i} 个瓶子有 {bottle.ColorCount} 种颜色");
            }

            var resultTransition = WaterSort.Solve(bottleState);
            if (resultTransition == null)
            {
                Debug.LogError("无解");
            }
            // Profiler.EndSample();
        }
    }
}