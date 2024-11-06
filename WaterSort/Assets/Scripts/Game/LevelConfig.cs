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
    [LabelText("瓶子最大容量")]
    public int SegmentMax = 3;
    int BottleNum = 3;

    [ColorPalette("My Palette")]
    public Color CurColor;
    [HideInInspector]
    public List<List<Color32>> BottleList = new List<List<Color32>>()
    {
       new List<Color32> {},
       new List<Color32> {},
       new List<Color32> {},
    };

    public List<int> BottleSegmentList = new List<int> { 3, 3, 3 };

    [TableMatrix(DrawElementMethod = nameof(DrawElement), HorizontalTitle = "瓶子配置", IsReadOnly = true)]
    [OnValueChanged(nameof(UpdateBottleList))]
    public Color32[,] SquareCelledMatrix;

    [OnInspectorInit]
    private void CreateData()
    {
        SquareCelledMatrix = new Color32[BottleList.Count, SegmentMax];
        for (int i = 0; i < BottleList.Count; i++)
        {
            var colorList = BottleList[i];
            var segment = BottleSegmentList[i];
            for (int j = 0; j < SegmentMax; j++)
            {
                var color = Color.white;
                if (j < segment)
                {
                    if (colorList != null && j < colorList.Count)
                        color = colorList[j];
                }
                else
                {
                    color = Color.black;
                }

                SquareCelledMatrix[i, SegmentMax - 1 - j] = color;
            }
        }
    }

    [SerializeField, LabelText("需要清除的瓶子索引")]
    int CleanBottleIndex = 0;

    [Button("增加瓶子")]
    [ButtonGroup("操作瓶子")]
    private void AddBottle()
    {
        BottleNum++;
        BottleList.Add(new List<Color32> { });
        BottleSegmentList.Add(SegmentMax);
        CreateData();
        Checked = false;
    }

    [Button("减少瓶子")]
    [ButtonGroup("操作瓶子")]
    private void RemoveBottle()
    {
        if (BottleNum > 2)
        {
            BottleNum--;
            BottleList.RemoveAt(BottleList.Count - 1);
            BottleSegmentList.RemoveAt(BottleSegmentList.Count - 1);
            CreateData();

            Checked = false;
        }
    }

    [Button("清空指定瓶子")]
    [ButtonGroup("操作瓶子")]
    private void ClearSelectedBottle()
    {
        if (CleanBottleIndex < BottleList.Count)
        {
            BottleList[CleanBottleIndex].Clear();
            CreateData();
            Checked = false;
        }
    }

    [Button("清空所有瓶子")]
    [ButtonGroup("操作瓶子")]
    private void ClearAllBottle()
    {
        foreach (var bottle in BottleList)
        {
            bottle.Clear();
        }
        CreateData();

        Checked = false;
    }

    private Color32 DrawElement(Rect rect, Color32 value)
    {
        if (Event.current.type == EventType.MouseDown
            && rect.Contains(Event.current.mousePosition)
            && Event.current.button == 0)
        {
            if (!value.Equals((Color32)Color.black))
            {
                value = CurColor;
                GUI.changed = true;
                Event.current.Use();
            }
        }

        // if (Event.current.type == EventType.MouseDown
        //    && rect.Contains(Event.current.mousePosition)
        //    && Event.current.button == 1)
        // {
        //     Debug.LogError("右键" + value);
        //     if (!value.Equals((Color32)Color.black) && !value.Equals((Color32)Color.white))
        //     {
        //         value = Color.white;
        //         GUI.changed = true;
        //         Event.current.Use();
        //     }
        // }

        EditorGUI.DrawRect(
            rect.Padding(1),
            value
        );

        return value;
    }

    private void UpdateBottleList()
    {
        bool isChange = false;
        for (int i = 0; i < SquareCelledMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < SquareCelledMatrix.GetLength(1); j++)
            {
                var color = SquareCelledMatrix[i, j];
                if (color.Equals((Color32)Color.black)) continue;
                if (color.Equals((Color32)Color.white)) continue;

                var bottle = BottleList[i];
                var segment = BottleSegmentList[i];

                var colorIndex = (segment - 1) - j;
                // Debug.Log($"瓶子 {i} 颜色 {j} 为 {color}    colorIndex {colorIndex}");
                if (colorIndex < 0) continue;

                if (colorIndex < bottle.Count)
                {
                    if (!bottle[colorIndex].Equals(color))
                        isChange = true;
                    else
                        continue;

                    // Debug.LogError($"修改瓶子 {i} 颜色 {colorIndex} 为 {color}");

                    bottle[colorIndex] = color;
                }
                else
                {
                    bottle.Add(color);
                    isChange = true;
                }
            }
        }

        if (isChange)
            Checked = false;
    }

    private bool Checked = false;
    private bool Error = false;
    private bool ShowError => Checked && Error;
    private bool ShowNotError => Checked && !Error;
#if UNITY_EDITOR

    [ShowInInspector, ReadOnly, HideIf(nameof(Checked)), HideLabel]
    [InfoBox("请先验证关卡", InfoMessageType.Warning)]
    private string Info1 = "";
    [ShowInInspector, ReadOnly, ShowIf(nameof(ShowError)), HideLabel]
    [InfoBox("验证失败，无解!", InfoMessageType.Error)]
    private string Info2 = "";

    [ShowInInspector, ReadOnly, ShowIf(nameof(ShowNotError)), HideLabel]
    [InfoBox("验证成功，有解!", InfoMessageType.Info)]
    private string Info3 = "验证成功，有解!";

    [Button("验证关卡", ButtonSizes.Large), GUIColor(0, 1, 1)]
    public void CheckLevel()
    {
        BottleState bottleState = new BottleState();
        bottleState.bottles = new Bottle[BottleList.Count];
        for (int i = 0; i < BottleList.Count; i++)
        {
            var bottle = new Bottle();
            bottle.Segment = BottleSegmentList[i];
            foreach (var color in BottleList[i])
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
            Error = true;
        }
        else
        {
            Error = false;
        }

        Checked = true;
    }
#endif
}