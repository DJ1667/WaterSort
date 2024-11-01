

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public class Bottle
{
    public Stack<string> colors = null;

    // 计算单个瓶子的复杂度
    public int CalculateComplexity()
    {
        if (colors == null || colors.Count == 0)
        {
            return 0; // 空瓶子
        }

        // 使用 HashSet 来计算唯一颜色的数量
        var uniqueColors = new HashSet<string>(colors);
        int uniqueColorCount = uniqueColors.Count;

        // 复杂度 = 颜色段数 - 1
        return Math.Max(0, uniqueColorCount - 1);
    }

    public void PourOut(Bottle bottle)
    {
        if (colors.Count < 1) return;

        var color = colors.Pop();
        bottle.PoruIn(color);
    }

    public void PoruIn(string color)
    {
        colors.Push(color);
    }
}

public class BottleState
{
    public int segment = 3;
    public Bottle[] bottles = null;

    public bool PourOut(int from, int to)
    {
        var pourOut = bottles[from];
        var pourIn = bottles[to];

        if (pourOut.colors.Count < 1) return false;
        if (pourIn.colors.Count == segment) return false;

        pourOut.PourOut(pourIn);
        return true;
    }

    public string CalculateHashString()
    {
        if (bottles == null || bottles.Length == 0)
        {
            return string.Empty;
        }

        // 按照颜色长度从大到小排序，长度相同则按照颜色字符串排序
        var sortedBottles = bottles
            .OrderByDescending(b => b.colors?.Count ?? 0)
            .ThenBy(b => string.Join(",", b.colors ?? new Stack<string>()))
            .ToArray();

        // 生成哈希字符串
        var hashString = string.Join("|", sortedBottles.Select(b => string.Join(",", b.colors ?? new Stack<string>())));

        return hashString;
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
            doneState = this.doneState,
            stateHash = this.stateHash,
            step = this.step,
            complexity = this.complexity,
        };
    }
}