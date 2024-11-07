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

        }
    }

    [ContextMenu("获得解法步骤")]
    public void TestLevel()
    {
        BottleState bottleState = new BottleState();

        bottleState.bottles = new Bottle[12]{
            new Bottle(BottleType.Normal, 4, new ColorType[]{ColorType.绿, ColorType.红, ColorType.黄, ColorType.深绿}),
            new Bottle(BottleType.Normal, 4, new ColorType[]{ColorType.红, ColorType.白, ColorType.白, ColorType.深绿}),
            new Bottle(BottleType.Normal, 4, new ColorType[]{ColorType.绿, ColorType.蓝, ColorType.深蓝, ColorType.白}),
            new Bottle(BottleType.Normal, 4, new ColorType[]{ColorType.蓝, ColorType.深绿, ColorType.橙色, ColorType.蓝}),
            new Bottle(BottleType.Normal, 4, new ColorType[]{ColorType.深蓝, ColorType.紫色, ColorType.白, ColorType.深蓝}),
            new Bottle(BottleType.Normal, 4, new ColorType[]{ColorType.深蓝, ColorType.绿, ColorType.橙色, ColorType.黄}),
            new Bottle(BottleType.Normal, 4, new ColorType[]{ColorType.红, ColorType.紫色, ColorType.蓝, ColorType.橙色}),
            new Bottle(BottleType.Normal, 4, new ColorType[]{ColorType.橙色, ColorType.紫色, ColorType.紫色, ColorType.深绿}),
            new Bottle(BottleType.Normal, 4, new ColorType[]{ColorType.红, ColorType.黄, ColorType.黄, ColorType.绿}),
            new Bottle(BottleType.Normal, 4),
            new Bottle(BottleType.Normal, 4),
            new Bottle(BottleType.Normal, 2),
        };

        WaterSort.Solve(bottleState);
    }
}