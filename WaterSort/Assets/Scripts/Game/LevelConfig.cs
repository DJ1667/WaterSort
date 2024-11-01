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
    [SerializeField]
    int Segment = 3;
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

    [TableMatrix(DrawElementMethod = nameof(DrawElement), HorizontalTitle = "瓶子配置", IsReadOnly = true)]
    [OnValueChanged(nameof(UpdateBottleList))]
    public Color32[,] SquareCelledMatrix;

    [OnInspectorInit]
    private void CreateData()
    {
        SquareCelledMatrix = new Color32[BottleList.Count, Segment];
        for (int i = 0; i < BottleList.Count; i++)
        {
            var colorList = BottleList[i];
            for (int j = 0; j < Segment; j++)
            {
                var color = Color.white;
                if (colorList != null && j < colorList.Count)
                    color = colorList[j];

                SquareCelledMatrix[i, Segment - 1 - j] = color;
            }
        }
    }

    [Button("增加瓶子")]
    private void AddBottle()
    {
        BottleNum++;
        BottleList.Add(new List<Color32> { });
        CreateData();
    }

    [Button("减少瓶子")]
    private void RemoveBottle()
    {
        if (BottleNum > 2)
        {
            BottleNum--;
            BottleList.RemoveAt(BottleList.Count - 1);
            CreateData();
        }
    }

    [Button("清空所有瓶子")]
    private void ClearAllBottle()
    {
        foreach (var bottle in BottleList)
        {
            bottle.Clear();
        }
        CreateData();
    }

    private Color32 DrawElement(Rect rect, Color32 value)
    {
        if (Event.current.type == EventType.MouseDown
            && rect.Contains(Event.current.mousePosition))
        {
            value = CurColor;
            GUI.changed = true;
            Event.current.Use();
        }

        EditorGUI.DrawRect(
            rect.Padding(1),
            value
        );

        return value;
    }

    private void UpdateBottleList()
    {
        for (int i = 0; i < SquareCelledMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < SquareCelledMatrix.GetLength(1); j++)
            {
                var color = SquareCelledMatrix[i, j];
                if (color.Equals((Color32)Color.white)) continue;
                var colorIndex = (Segment - 1) - j;
                if (colorIndex < BottleList[i].Count)
                    BottleList[i][colorIndex] = color;
                else
                    BottleList[i].Add(color);
            }
        }
    }
}