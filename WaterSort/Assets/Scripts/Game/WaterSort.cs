using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSort
{
    private static List<BottleTransition> Solve(BottleState initState)
    {
        // 按照 f 排序，用小顶堆
        MinHeap<BottleTransition> openList = new MinHeap<BottleTransition>();
        List<BottleTransition> clostList = new List<BottleTransition>();

        BottleTransition startTransition = new BottleTransition();
        startTransition.doneState = initState;
        startTransition.UpdateTransition();

        openList.Insert(startTransition);

        while (openList.Count > 0)
        {
            var min = openList.Pop();

            // 最终状态
            if (min.complexity == 0)
            {
                List<BottleTransition> resultList = new List<BottleTransition>();
                var p = min;
                while (p != null)
                {
                    resultList.Add(p);
                    p = p.parent;
                }

                resultList.Reverse();
                return resultList;
            }

            List<BottleTransition> opts = ListOptions(min); // 输出可能的每一步
            opts.ForEach(opt =>
            {
                if (clostList.FindIndex(v => v.stateHash == opt.stateHash) < 0)
                {
                    // 该状态不在close列表中，未被探索过
                    openList.Insert(opt);
                }
            });

            clostList.Add(min); // 将min标记为已探索
        }

        // 无解
        return null;
    }

    private static List<BottleTransition> ListOptions(BottleTransition currentTransition)
    {
        List<BottleTransition> options = new List<BottleTransition>();

        var bottleNum = currentTransition.doneState.bottles.Length;

        HashSet<string> optionsHash = new HashSet<string>();
        for (int i = 0; i < bottleNum; i++)
        {
            for (int j = 0; j < bottleNum; j++)
            {
                if (j == i) continue;

                var newTransition = currentTransition.Clone();
                newTransition.parent = currentTransition;
                if (newTransition.doneState.PourOut(i, j))
                {
                    newTransition.UpdateTransition();
                    if (optionsHash.Contains(newTransition.stateHash))
                    {
                        optionsHash.Add(newTransition.stateHash);
                        options.Add(newTransition);
                    }
                }
            }
        }

        return options;
    }
}
