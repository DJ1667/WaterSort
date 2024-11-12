using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class LevelCreatorRuntime
{
    public static List<BottleData> CreateLevelConfig(LevelConfigCreateData levelData)
    {
        List<ColorType> AllColors = new List<ColorType>(); // 所有颜色
        //加入除了None以外的所有颜色
        foreach (ColorType color in System.Enum.GetValues(typeof(ColorType)))
        {
            if (color != ColorType.None && color != ColorType.Disable)
            {
                AllColors.Add(color);
            }
        }

        //选出BottleCount数量的不重复颜色
        var colors = DUtils.GetRandomElement(AllColors, levelData.BottleCount);

        List<BottleData> bottleDataList = new List<BottleData>();
        //创建普通瓶子
        for (int i = 0; i < levelData.BottleCount; i++)
        {
            var bottleData = new BottleData
            {
                bottleType = BottleType.Normal,
                Segment = levelData.Segment,
            };
            bottleDataList.Add(bottleData);
        }

        //填充颜色
        int totalColorNum = levelData.BottleCount * levelData.Segment; // 总液体数量
        List<ColorType> colorPool = new List<ColorType>();
        List<ColorType> backupsColorPool = new List<ColorType>(); //备份颜色
        for (int i = 0; i < levelData.BottleCount; i++)
        {
            var color = colors[i];
            for (int j = 0; j < levelData.Segment; j++)
            {
                colorPool.Add(color);
                backupsColorPool.Add(color);
            }
        }

        //打乱颜色
        DUtils.ShuffleWithIntensity1(colorPool, levelData.DegreeOfDifficulty);

        //将颜色填充到瓶子中
        for (int i = 0; i < levelData.BottleCount; i++)
        {
            for (int j = 0; j < levelData.Segment; j++)
            {
                bottleDataList[i].Colors.Add(colorPool[i * levelData.Segment + j]);
            }
        }

        //创建空瓶子
        for (int i = 0; i < levelData.EmptyBottleCount; i++)
        {
            var bottleData = new BottleData
            {
                bottleType = BottleType.Normal,
                Segment = levelData.Segment,
            };
            bottleDataList.Add(bottleData);
        }

        //创建临时瓶子
        for (int i = 0; i < levelData.TempBottleCount; i++)
        {
            var bottleData = new BottleData
            {
                bottleType = BottleType.Temp,
                Segment = levelData.TempSegment,
            };
            bottleDataList.Add(bottleData);
        }

        //创建单色瓶子
        for (int i = 0; i < levelData.SingleColorBottleCount; i++)
        {
            var bottleData = new BottleData
            {
                bottleType = BottleType.SingleColor,
                Segment = levelData.SingleColorSegment,
            };
            bottleDataList.Add(bottleData);
        }


        int tryCount = 1;
        while (!TestData(bottleDataList))
        {
            if (tryCount > 500)
            {
                Debug.LogError($"<color=red>关卡{levelData.ID} 创建失败, 尝试超过500次</color>");
                return null;
            }

            tryCount++;
            colorPool.Clear();
            colorPool.AddRange(backupsColorPool);
            DUtils.ShuffleWithIntensity1(colorPool, levelData.DegreeOfDifficulty);

            for (int i = 0; i < levelData.BottleCount; i++)
            {
                for (int j = 0; j < levelData.Segment; j++)
                {
                    bottleDataList[i].Colors[j] = colorPool[i * levelData.Segment + j];
                }
            }
        }

        Debug.Log($"<color=yellow>关卡{levelData.ID} 创建成功，尝试了{tryCount}次</color>");

        return bottleDataList;
    }

    private static bool TestData(List<BottleData> bottleDataList)
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
                if (color == ColorType.None || color == ColorType.Disable) continue;
                bottle.AddColor(color);
            }
            bottleState.bottles[i] = bottle;
        }

        var result = WaterSort.Solve(bottleState);
        return result != null;
    }
}

public class LevelConfigCreateData
{
    public int ID = 0;
    public int BottleCount = 5; // 瓶子数量
    public int EmptyBottleCount = 2; // 空瓶子数量
    public int Segment = 5; // 每个瓶子的段数
    public int TempBottleCount = 2; // 临时瓶子数量
    public int TempSegment = 5; // 临时瓶子的段数
    public int SingleColorBottleCount = 2; // 单色瓶子数量
    public int SingleColorSegment = 5; // 单色瓶子的段数
    public float DegreeOfDifficulty = 0.5f; // 难度
}