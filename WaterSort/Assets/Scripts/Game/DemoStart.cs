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

        BottleState bottleState = new BottleState()
        {
            segmentMax = 5,
            bottles = new Bottle[]
            {
                new Bottle()
                {
                    Segment = 5,
                       colorStack = new Stack<string>(new string[] { "bbb", Random.Range(10000,99999).ToString(),})
                },
                new Bottle()
                {
                    Segment = 5,
                    colorStack = new Stack<string>(new string[] { "aaa", "123", Random.Range(10000,99999).ToString() })
                },
                new Bottle()
                {
                    Segment = 5,
                      colorStack = new Stack<string>(new string[] { "cccc", "das", Random.Range(10000,99999).ToString() })
                },
                new Bottle()
                {
                    Segment = 5,
                       colorStack = new Stack<string>(new string[] { "bbbb", "blue", Random.Range(10000,99999).ToString() })
                },
                new Bottle()
                {
                    Segment = 5,
                       colorStack = new Stack<string>(new string[] { "bbbb", "blue", Random.Range(10000,99999).ToString() })
                },
                new Bottle()
                {
                    Segment = 5,
                       colorStack = new Stack<string>(new string[] { "sad","bbbb", "blue",Random.Range(10000,99999).ToString() })
                },
                new Bottle()
                {
                    Segment = 5,
                       colorStack = new Stack<string>(new string[] { "bbbb", "blue", Random.Range(10000,99999).ToString() })
                },
                new Bottle()
                {
                    Segment = 5,
                       colorStack = new Stack<string>(new string[] { "bbbb", "blue", Random.Range(10000,99999).ToString() })
                },
                new Bottle()
                {
                    Segment = 5,
                       colorStack = new Stack<string>(new string[] { "dasdaf","dsad","bbbb", "blue",Random.Range(10000,99999).ToString() })
                }
            }
        };

        Debug.Log(bottleState.CalculateHashString());
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
