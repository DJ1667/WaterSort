using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreatorRuntime : MonoBehaviour
{
    public List<Color32> AllColors = new List<Color32>(); // 所有颜色
    public int BottleCount = 5; // 瓶子数量
    public int EmptyBottleCount = 2; // 空瓶子数量
    public int Segment = 5; // 每个瓶子的段数
    public int TempBottleCount = 2; // 临时瓶子数量
    public int TempSegment = 5; // 临时瓶子的段数
    public int SingleColorBottleCount = 2; // 单色瓶子数量
    public int SingleColorSegment = 5; // 单色瓶子的段数
    public float DegreeOfDifficulty = 0.5f; // 难度

    public List<BottleData> CreateLevelConfig()
    {
        //选出BottleCount数量的不重复颜色
        var colors = DUtils.GetRandomElement(AllColors, BottleCount);

        List<BottleData> bottleDataList = new List<BottleData>();
        //创建普通瓶子
        for (int i = 0; i < BottleCount; i++)
        {
            var bottleData = new BottleData
            {
                bottleType = BottleType.Normal,
                Segment = Segment,
            };
            bottleDataList.Add(bottleData);
        }

        //填充颜色
        int totalColorNum = BottleCount * Segment; // 总液体数量
        List<Color32> colorPool = new List<Color32>();
        List<Color32> backupsColorPool = new List<Color32>(); //备份颜色
        for (int i = 0; i < totalColorNum; i++)
        {
            for (int j = 0; j < colors.Count; j++)
            {
                colorPool.Add(colors[j]);
                backupsColorPool.Add(colors[j]);
            }
        }

        //打乱颜色
        DUtils.ShuffleWithIntensity(colorPool, DegreeOfDifficulty);

        //将颜色填充到瓶子中
        for (int i = 0; i < BottleCount; i++)
        {
            for (int j = 0; j < Segment; j++)
            {
                bottleDataList[i].Colors.Add(colorPool[i * Segment + j]);
            }
        }

        //创建空瓶子
        for (int i = 0; i < EmptyBottleCount; i++)
        {
            var bottleData = new BottleData
            {
                bottleType = BottleType.Normal,
                Segment = Segment,
            };
            bottleDataList.Add(bottleData);
        }

        //创建临时瓶子
        for (int i = 0; i < TempBottleCount; i++)
        {
            var bottleData = new BottleData
            {
                bottleType = BottleType.Temp,
                Segment = TempSegment,
            };
            bottleDataList.Add(bottleData);
        }

        //创建单色瓶子
        for (int i = 0; i < SingleColorBottleCount; i++)
        {
            var bottleData = new BottleData
            {
                bottleType = BottleType.SingleColor,
                Segment = SingleColorSegment,
            };
            bottleDataList.Add(bottleData);
        }

        int tryCount = 1;
        while (!TestData(bottleDataList))
        {
            tryCount++;
            colorPool.Clear();
            colorPool.AddRange(backupsColorPool);
            DUtils.ShuffleWithIntensity(colorPool, DegreeOfDifficulty);

            for (int i = 0; i < BottleCount; i++)
            {
                for (int j = 0; j < Segment; j++)
                {
                    bottleDataList[i].Colors[j] = colorPool[i * Segment + j];
                }
            }
        }

        Debug.Log("关卡创建成功，尝试了" + tryCount + "次");

        return bottleDataList;
    }

    private bool TestData(List<BottleData> bottleDataList)
    {
        BottleState bottleState = new BottleState();
        bottleState.bottles = new Bottle[bottleDataList.Count];

        for (int i = 0; i < bottleDataList.Count; i++)
        {
            var bottleData = bottleDataList[i];
            var bottle = new Bottle();
            bottle.bottleType = bottleData.bottleType;
            bottle.Segment = bottleData.Segment;
            foreach (var color in bottleData.Colors)
            {
                bottle.AddColor(color);
            }
            bottleState.bottles[i] = bottle;
        }

        var result = WaterSort.Solve(bottleState);
        return result != null;
    }
}

[System.Serializable]
public class BottleData
{
    public BottleType bottleType = BottleType.Normal;
    public int Segment = 4;
    public List<Color32> Colors = new List<Color32>();
}
