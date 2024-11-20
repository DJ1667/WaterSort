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
        // DUtils.ShuffleWithIntensity1(colorPool, levelData.DegreeOfDifficulty);
        ShuffleList2(colorPool, levelData.DegreeOfDifficulty, levelData.Segment);

        //将颜色填充到瓶子中
        for (int i = 0; i < levelData.BottleCount; i++)
        {
            for (int j = 0; j < levelData.Segment; j++)
            {
                bottleDataList[i].Colors.Add(colorPool[i * levelData.Segment + j]);
            }
        }

        //打乱瓶子顺序
        DUtils.FisherYatesShuffle(bottleDataList);

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
        Debug.Log("解出需要步数: " + result.Count);
        return result != null;
    }

    private static void ShuffleList(List<ColorType> list, int complexity, int segment)
    {
        List<HashSet<ColorType>> colorSets = new List<HashSet<ColorType>>();
        var bottleCount = list.Count / segment;
        var complexCount = (int)(complexity / 10 * bottleCount);
        int index = 0;
        colorSets.Add(new HashSet<ColorType>());
        Debug.Log($"bottleCount: {bottleCount} complexity: {complexity}  complexCount: {complexCount}");
        for (int i = list.Count - 1; i > 0; i--)
        {
            if (colorSets[index].Count == segment)
            {
                index++;
                colorSets.Add(new HashSet<ColorType>());
            }

            int j = Random.Range(0, i + 1);
            if (index < complexCount)
            {
                while (colorSets[index].Contains(list[j]))
                {
                    j++;
                    if (j > i)
                    {
                        j = 0;
                    }
                    j = Random.Range(0, i + 1);
                }
            }

            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;

            colorSets[index].Add(list[i]);
        }
    }

    private static void ShuffleList2(List<ColorType> list, int complexity, int segment)
    {
        var bottleCount = list.Count / segment;

        //瓶底颜色最多重复次数
        var maxSameColorNum = segment - 1;
        // 有多少种颜色的最大沉底 保证并非所有瓶子都是三种颜色
        var maxSameColorCount = bottleCount % maxSameColorNum == 0 ? (bottleCount / maxSameColorNum - 1) : bottleCount / maxSameColorNum;
        //根据难度决定最大沉底颜色数量
        maxSameColorCount = (int)(maxSameColorCount * (complexity / 10f));

        if (maxSameColorCount < 7) maxSameColorCount = 0;

        List<List<ColorType>> colorSets = new List<List<ColorType>>();
        var complexCount = (int)(complexity / 10f * bottleCount);
        int index = 0;
        colorSets.Add(new List<ColorType>());

        int n = 0;
        bool haveSameColor = false;
        for (int i = list.Count - 1; i > 0; i--, n++)
        {
            if (colorSets[index].Count == segment)
            {
                index++;
                colorSets.Add(new List<ColorType>());
                haveSameColor = false;
            }
            if (index > 0 && index % maxSameColorNum == 0 && colorSets[index].Count == 0)
                maxSameColorCount--;

            int j = Random.Range(0, i + 1);

            if (maxSameColorCount > 0 && n % segment == 0)
            {
                var sameIndex = index / maxSameColorNum * maxSameColorNum;
                if (index != sameIndex)
                {
                    var tempN = sameIndex * segment;
                    var tempI = list.Count - 1 - tempN;
                    j = DUtils.GetSameElement(list, list[tempI], 0, i);

                    if (j == -1)
                    {
                        j = Random.Range(0, i + 1);
                    }

                    // Debug.Log($"maxSameColorCount:{maxSameColorCount}  瓶子{index} 重复瓶子{sameIndex} 颜色 {list[tempI]} - {list[j]}  第{n}个颜色-> {n % segment}");
                }
            }

            //在复杂度范围内
            if (index < complexCount)
            {
                int randomCount = 0;
                while (colorSets[index].Contains(list[j]))
                {

                    if (randomCount < 10)
                    {
                        j = Random.Range(0, i + 1);
                        randomCount++;
                    }
                    else
                    {
                        j++;
                        if (j > i)
                        {
                            j = 0;
                        }
                    }
                }
            }
            //否则简单点
            else
            {
                //排除瓶子底部
                var bti = colorSets[index].Count; //该色块在瓶子中的的位置
                if (bti != 0 && !haveSameColor)
                {
                    //使得非复杂瓶子一定有相同颜色
                    if (Random.Range(bti, segment) == (segment - 1))
                    {
                        var t = DUtils.GetSameElement(list, colorSets[index][Random.Range(0, bti)], 0, i);
                        if (t != -1)
                        {
                            j = t;
                            haveSameColor = true;
                        }
                    }
                }
            }

            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;

            colorSets[index].Add(list[i]);
        }

        list.Reverse();
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
    public int DegreeOfDifficulty = 5; // 难度
}