using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class LevelCreator : MonoBehaviour
{
    public List<BottleData> BottleDataList = new List<BottleData>(); // 最终状态的瓶子数量
    private List<BottleData> _bottleDataList = new List<BottleData>(); // 备份

    public int NeedStep = 30; // 需要的步数

    [ContextMenu("创建关卡数据")]
    public void CreateData()
    {
        _bottleDataList.Clear();
        foreach (var bottleData in BottleDataList)
        {
            _bottleDataList.Add(new BottleData
            {
                bottleType = bottleData.bottleType,
                Segment = bottleData.Segment,
                Colors = new List<Color32>(bottleData.Colors)
            });
        }

        List<BottleData> emptyBottleDataList = new List<BottleData>();
        List<BottleData> notEmptyBottleDataList = new List<BottleData>();
        foreach (var bottleData in BottleDataList)
        {
            if (bottleData.Colors.Count == 0)
                emptyBottleDataList.Add(bottleData);
            else
                notEmptyBottleDataList.Add(bottleData);
        }

        if (emptyBottleDataList.Count == 0)
        {
            Debug.LogError("请先设置空瓶子数量");
            return;
        }

        //打乱瓶子中的颜色
        for (int step = 0; step < NeedStep; step++)
        {
            int i = Random.Range(0, BottleDataList.Count);
            while (BottleDataList[i].Colors.Count == 0)
            {
                i = Random.Range(0, BottleDataList.Count);
            }

            int j = Random.Range(0, BottleDataList.Count);
            while (i == j || BottleDataList[j].Colors.Count == BottleDataList[j].Segment)
            {
                j = Random.Range(0, BottleDataList.Count);
            }

            var pourOut = BottleDataList[i];
            var pourIn = BottleDataList[j];

            var color = pourOut.Colors[pourOut.Colors.Count - 1];
            pourOut.Colors.RemoveAt(pourOut.Colors.Count - 1);
            pourIn.Colors.Add(color);
        }

        //处理空瓶子
        for (int i = 0; i < emptyBottleDataList.Count; i++)
        {
            var emptyBottleData = emptyBottleDataList[i];
            while (emptyBottleData.Colors.Count != 0)
            {
                int j = Random.Range(0, notEmptyBottleDataList.Count);
                var notEmptyBottleData = notEmptyBottleDataList[j];
                while (notEmptyBottleData.Colors.Count == notEmptyBottleData.Segment)
                {
                    j = Random.Range(0, notEmptyBottleDataList.Count);
                    notEmptyBottleData = notEmptyBottleDataList[j];
                }

                var color = emptyBottleData.Colors[emptyBottleData.Colors.Count - 1];
                emptyBottleData.Colors.RemoveAt(emptyBottleData.Colors.Count - 1);
                notEmptyBottleData.Colors.Add(color);
            }
        }

        Debug.Log("生成数据成功");
    }

    [ContextMenu("还原数据")]
    public void Restore()
    {
        BottleDataList.Clear();
        foreach (var bottleData in _bottleDataList)
        {
            BottleDataList.Add(new BottleData
            {
                bottleType = bottleData.bottleType,
                Segment = bottleData.Segment,
                Colors = new List<Color32>(bottleData.Colors)
            });
        }
    }
}