using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum BottleType
{
    Normal,
    Temp,
    SingleColor,
}

public class Bottle
{
    public BottleType bottleType = BottleType.Normal;
    public string BottleColor = null;
    private int segment = 3;
    private Stack<ColorType> colorStack = new Stack<ColorType>();
    public Stack<ColorType> ColorStack => colorStack;

    public ColorType[] colors => colorStack.ToArray();

    public int Segment
    {
        get => segment;
        set => segment = value;
    }
    public int ColorCount => colorStack.Count;
    public bool IsEmpty => colorStack.Count == 0;
    public bool IsFull => colorStack.Count == segment;

    public Bottle()
    {
    }

    public Bottle(BottleType type, int segment, ColorType[] colors = null)
    {
        bottleType = type;
        this.segment = segment;

        if (colors != null)
        {
            foreach (var color in colors)
            {
                AddColor(color);
            }
        }
    }

    public string GetAllColorString()
    {
        return string.Join(",", colorStack);
    }

    public bool AddColor(ColorType color)
    {
        if (IsFull) return false;

        colorStack.Push(color);
        return true;
    }

    /// <summary>
    /// 得到瓶子顶部颜色
    /// </summary>
    /// <returns></returns>
    public ColorType GetTopColor()
    {
        if (colorStack.Count == 0)
            return ColorType.None;

        return colorStack.Peek();
    }

    /// <summary>
    /// 计算单个瓶子的复杂度
    /// </summary>
    /// <returns></returns>
    public int CalculateComplexity()
    {
        if (colorStack == null || colorStack.Count == 0)
        {
            return 0; // 空瓶子
        }

        // 使用 HashSet 来计算唯一颜色的数量
        var uniqueColors = new HashSet<ColorType>();
        foreach (var color in colorStack)
        {
            uniqueColors.Add(color);
            if (uniqueColors.Count > segment)
            {
                break; // 提前终止计算
            }
        }

        // 复杂度 = 颜色段数 - 1
        return Mathf.Max(0, uniqueColors.Count - 1);
    }

    /// <summary>
    /// 倒出颜色
    /// </summary>
    /// <param name="bottle"></param>
    public bool PourOut(Bottle bottle)
    {
        if (!IsCanPourOut(bottle)) return false;

        var color = colorStack.Pop();
        bottle.PoruIn(color);

        // 递归倒入(相同颜色全部倒入)
        while (PourOut(bottle))
        {
        }

        return true;
    }

    public bool PourOut(Bottle bottle, ref int num)
    {
        if (!IsCanPourOut(bottle)) return false;

        var color = colorStack.Pop();
        bottle.PoruIn(color);

        num++;

        // 递归倒入(相同颜色全部倒入)
        while (PourOut(bottle, ref num))
        {
        }

        return true;
    }

    public bool IsCanPourOut(Bottle bottle)
    {
        if (IsEmpty) return false;
        if (bottle.IsFull) return false;
        if (!bottle.IsEmpty && !GetTopColor().Equals(bottle.GetTopColor())) return false;
        if (bottle.bottleType == BottleType.SingleColor && !GetTopColor().Equals(bottle.BottleColor)) return false;

        return true;
    }

    /// <summary>
    /// 倒入颜色
    /// </summary>
    /// <param name="color"></param>
    public void PoruIn(ColorType color)
    {
        colorStack.Push(color);
    }
}

public class BottleState
{
    public Bottle[] bottles = null;

    public bool PourOut(int from, int to)
    {
        var pourOut = bottles[from];
        var pourIn = bottles[to];

        // Debug.Log($"from: {from + 1}  to: {to + 1}  | pourOutColor: {pourOut.GetTopColor()} pourInColor: {pourIn.GetTopColor()}");

        return pourOut.PourOut(pourIn);
    }

    public bool PourOut(int from, int to, ref int num)
    {
        var pourOut = bottles[from];
        var pourIn = bottles[to];

        // Debug.Log($"from: {from + 1}  to: {to + 1}  | pourOutColor: {pourOut.GetTopColor()} pourInColor: {pourIn.GetTopColor()}");

        return pourOut.PourOut(pourIn, ref num);
    }

    /// <summary>
    /// 强制回退之前的倒出操作
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void ForceBackPourOut(int from, int to, int num)
    {
        var pourOut = bottles[to];
        var pourIn = bottles[from];

        for (int i = 0; i < num; i++)
        {
            pourOut.PourOut(pourIn);
        }
    }

    public bool IsCanPourOut(int from, int to)
    {
        var pourOut = bottles[from];
        var pourIn = bottles[to];

        return pourOut.IsCanPourOut(pourIn);
    }

    public string CalculateHashString()
    {
        if (bottles == null || bottles.Length == 0)
        {
            return string.Empty;
        }

        // 预先计算所有颜色字符串，避免重复计算
        var bottleWithColorStrings = bottles.Select(b => new
            BottleComparerData
        {
            ColorCount = b.ColorCount,
            ColorString = b.GetAllColorString()
        }).ToList();

        // 创建自定义比较器
        var bottleComparer = new BottleComparer();

        // 使用自定义比较器进行排序
        var sortedBottles = bottleWithColorStrings
            .OrderBy(b => b, bottleComparer)
            .Select(b => b.ColorString);

        // 生成哈希字符串
        return string.Join("|", sortedBottles);
    }

    public int CalculateComplexity()
    {
        return CalculateComplexity2();
    }

    public int CalculateComplexity1()
    {
        if (bottles == null || bottles.Length == 0)
        {
            return 0; // 没有瓶子，复杂度为0
        }

        int totalComplexity = 0;

        foreach (var bottle in bottles)
        {
            totalComplexity += bottle.CalculateComplexity();
        }

        return totalComplexity;
    }

    public int CalculateComplexity2()
    {
        int totalComplexity = 0;

        foreach (var bottle in bottles)
        {
            if (bottle.ColorCount == 0) continue;

            int complexity = 1;
            var preColor = bottle.GetTopColor();
            bool isAllSameColor = true;

            foreach (var color in bottle.ColorStack)
            {
                if (!color.Equals(preColor))
                {
                    complexity++;
                    isAllSameColor = false;
                    preColor = color;
                }
            }

            if (isAllSameColor && bottle.IsFull)
                complexity -= 1;

            totalComplexity += complexity;
        }

        return totalComplexity;
    }

    public BottleState Clone()
    {
        var newBottles = new Bottle[bottles.Length];
        for (int i = 0; i < bottles.Length; i++)
        {
            var bottle = bottles[i];
            newBottles[i] = new Bottle();
            newBottles[i].Segment = bottles[i].Segment;

            var colors = bottle.colors;
            for (int i1 = colors.Length - 1; i1 >= 0; i1--)
            {
                var color = colors[i1];
                newBottles[i].AddColor(color);
            }
        }

        return new BottleState
        {
            bottles = newBottles,
        };
    }
}


public class BottleTransition : IComparable<BottleTransition>
{
    public BottleTransition parent = null;
    public int fromBottleIndex = -1;
    public int toBottleIndex = -1;
    public BottleState doneState = null;
    public string stateHash = null; // 状态hash
    public int step = 0;   // 从最初状态到当前的步数
    public int complexity = 0;  // 复杂度
    public int f => step + complexity; //f = step + complexity

    public void UpdateTransition()
    {
        stateHash = doneState.CalculateHashString();
        complexity = doneState.CalculateComplexity();
    }

    public void UpdateTransition(string stateHash, int complexity)
    {
        this.stateHash = stateHash;
        this.complexity = complexity;
    }

    public int CompareTo(BottleTransition other)
    {
        return f.CompareTo(other.f);
    }

    public BottleTransition Clone()
    {
        return new BottleTransition
        {
            parent = this.parent,
            fromBottleIndex = this.fromBottleIndex,
            toBottleIndex = this.toBottleIndex,
            doneState = this.doneState.Clone(),
            stateHash = this.stateHash,
            step = this.step,
            complexity = this.complexity,
        };
    }
}

public class BottleComparerData
{
    public int ColorCount;
    public string ColorString;
}

public class BottleComparer : IComparer<BottleComparerData>
{
    public int Compare(BottleComparerData x, BottleComparerData y)
    {
        // 比较 ColorCount，降序排列
        int colorCountComparison = y.ColorCount.CompareTo(x.ColorCount);

        // 如果 ColorCount 相同，则比较颜色字符串，升序排列
        if (colorCountComparison == 0)
        {
            return string.Compare(x.ColorString, y.ColorString);
        }

        return colorCountComparison;
    }
}