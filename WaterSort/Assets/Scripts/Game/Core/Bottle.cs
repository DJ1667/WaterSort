using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Bottle
{
    private int segment = 3;
    public Stack<string> colorStack = new Stack<string>();

    public string[] colors => colorStack.ToArray();

    public int Segment
    {
        get => segment;
        set => segment = value;
    }
    public int ColorCount => colorStack.Count;
    public bool IsEmpty => colorStack.Count == 0;
    public bool IsFull => colorStack.Count == segment;

    public string GetAllColorString()
    {
        return string.Join(",", colorStack);
    }

    public bool AddColor(Color32 color)
    {
        return AddColor(color.ToString());
    }

    public bool AddColor(string color)
    {
        if (IsFull) return false;

        colorStack.Push(color);
        return true;
    }

    /// <summary>
    /// 得到瓶子顶部颜色
    /// </summary>
    /// <returns></returns>
    public string GetTopColor()
    {
        if (colorStack.Count == 0)
            return string.Empty;

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
        var uniqueColors = new HashSet<string>();
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
    public void PourOut(Bottle bottle)
    {
        if (colorStack.Count == 0)
        {
            Debug.LogError("倒出颜色失败，当前瓶子为空");
            return;
        }

        var color = colorStack.Pop();
        bottle.PoruIn(color);
    }

    /// <summary>
    /// 倒入颜色
    /// </summary>
    /// <param name="color"></param>
    public void PoruIn(string color)
    {
        colorStack.Push(color);
    }
}

public class BottleState
{
    public int segmentMax = 3;
    public Bottle[] bottles = null;

    public bool PourOut(int from, int to)
    {
        var pourOut = bottles[from];
        var pourIn = bottles[to];

        if (pourOut.IsEmpty) return false;
        if (pourIn.IsFull) return false;
        if (!pourIn.IsEmpty && !pourOut.GetTopColor().Equals(pourIn.GetTopColor())) return false;

        // Debug.Log($"from: {from + 1}  to: {to + 1}  | pourOutColor: {pourOut.GetTopColor()} pourInColor: {pourIn.GetTopColor()}");

        pourOut.PourOut(pourIn);

        // 递归倒入(相同颜色全部倒入)
        while (PourOut(from, to))
        {
        }

        return true;
    }

    public bool PourOut(int from, int to, ref int num)
    {
        var pourOut = bottles[from];
        var pourIn = bottles[to];

        if (pourOut.IsEmpty) return false;
        if (pourIn.IsFull) return false;
        if (!pourIn.IsEmpty && !pourOut.GetTopColor().Equals(pourIn.GetTopColor())) return false;

        // Debug.Log($"from: {from + 1}  to: {to + 1}  | pourOutColor: {pourOut.GetTopColor()} pourInColor: {pourIn.GetTopColor()}");

        pourOut.PourOut(pourIn);
        num++;

        // 递归倒入(相同颜色全部倒入)
        while (PourOut(from, to, ref num))
        {
        }

        return true;
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

        if (pourOut.IsEmpty) return false;
        if (pourIn.IsFull) return false;
        if (!pourIn.IsEmpty && !pourOut.GetTopColor().Equals(pourIn.GetTopColor())) return false;

        return true;
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
                string color = colors[i1];
                newBottles[i].AddColor(color);
            }
        }

        return new BottleState
        {
            segmentMax = segmentMax,
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