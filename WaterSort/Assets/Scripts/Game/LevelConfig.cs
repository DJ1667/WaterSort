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
    [TableList(DrawScrollView = true)]
    public List<BottleData> bottleDataList = new List<BottleData>();

#if UNITY_EDITOR

    [TableMatrix(HorizontalTitle = "瓶子配置", IsReadOnly = true)]
    [OnValueChanged(nameof(UpdateBottleList))]
    [ShowInInspector]
    ColorType[,] PreviewColors = new ColorType[3, 3];

    [OnInspectorInit]
    private void CreateData()
    {
        var SegmentMax = 0;
        for (int i = 0; i < bottleDataList.Count; i++)
        {
            var bottleData = bottleDataList[i];
            if (bottleData.Segment > SegmentMax)
                SegmentMax = bottleData.Segment;
        }

        PreviewColors = new ColorType[bottleDataList.Count, SegmentMax];
        for (int i = 0; i < bottleDataList.Count; i++)
        {
            var bottleData = bottleDataList[i];
            for (int j = 0; j < SegmentMax; j++)
            {
                var color = ColorType.None;

                if (j < bottleData.Segment)
                {
                    if (bottleData.Colors != null && j < bottleData.Colors.Count)
                        color = bottleData.Colors[j];

                    if (bottleData.bottleType != BottleType.Normal)
                    {
                        color = ColorType.None;
                    }
                }
                else
                {
                    color = ColorType.Disable;
                }

                PreviewColors[i, SegmentMax - 1 - j] = color;
            }
        }
    }

    private void UpdateBottleList()
    {
        for (int i = 0; i < PreviewColors.GetLength(0); i++)
        {
            var bottle = bottleDataList[i];
            bottle.Colors.Clear();
            if (bottle.bottleType != BottleType.Normal) continue;

            for (int j = PreviewColors.GetLength(1) - 1; j >= 0; j--)
            {
                var color = PreviewColors[i, j];
                if (color == ColorType.Disable) continue;
                if (color == ColorType.None) continue;
                bottle.Colors.Add(color);
            }
        }

        UpdatePreviewColors();
    }

    private void UpdatePreviewColors()
    {
        var SegmentMax = PreviewColors.GetLength(1);
        for (int i = 0; i < bottleDataList.Count; i++)
        {
            var bottleData = bottleDataList[i];
            for (int j = 0; j < SegmentMax; j++)
            {
                var color = ColorType.None;
                if (j < bottleData.Segment)
                {
                    if (bottleData.Colors != null && j < bottleData.Colors.Count)
                        color = bottleData.Colors[j];

                    if (bottleData.bottleType != BottleType.Normal)
                    {
                        color = ColorType.None;
                    }
                }
                else
                {
                    color = ColorType.Disable;
                }

                PreviewColors[i, SegmentMax - 1 - j] = color;
            }
        }
    }

    [VerticalGroup("Buttons", PaddingTop = 20)]
    [Button("测试关卡数据", buttonSize: ButtonSizes.Large), GUIColor(1f, 0.78f, 0f)]
    private void TestLevelData()
    {
        BottleState bottleState = new BottleState();
        bottleState.bottles = new Bottle[bottleDataList.Count];

        for (int i = 0; i < bottleDataList.Count; i++)
        {
            var bottleData = bottleDataList[i];
            var bottle = new Bottle();
            bottle.bottleType = bottleData.bottleType;
            bottle.Segment = bottleData.Segment;
            for (int i1 = 0; i1 < bottleData.Colors.Count; i1++)
            {
                ColorType color = bottleData.Colors[i1];
                if (color == ColorType.None || color == ColorType.Disable) continue;
                bottle.AddColor(color);
            }
            bottleState.bottles[i] = bottle;
        }

        WaterSort.Solve(bottleState, showTestLog);
    }

    [VerticalGroup("Buttons")]
    [SerializeField, LabelText("显示步数日志")]
    bool showTestLog = false;
#endif
}

[System.Serializable]
public class BottleData
{
    public BottleType bottleType = BottleType.Normal;
    [ShowIf("bottleType", BottleType.SingleColor)]
    public ColorType BottleColor = ColorType.None;
    public int Segment = 4;
    [HideInInspector]
    public List<ColorType> Colors = new List<ColorType>();
}

public enum ColorType
{
    None,
    Disable,
    // Red,
    // Green,
    // Blue,
    // Yellow,
    // Purple,
    // Cyan,
    // White,
    // Black,
    绿,
    红,
    黄,
    深绿,
    白,
    蓝,
    深蓝,
    橙色,
    紫色,
    粉色,

}