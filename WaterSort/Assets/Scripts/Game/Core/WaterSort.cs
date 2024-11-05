using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSort
{
    public static List<BottleTransition> Solve(BottleState initState)
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        // 按照 f 排序，用小顶堆
        MinHeap<BottleTransition> openList = new MinHeap<BottleTransition>();
        HashSet<string> clostHash = new HashSet<string>();

        BottleTransition startTransition = new BottleTransition();
        startTransition.doneState = initState;
        startTransition.UpdateTransition();

        openList.Insert(startTransition);

        int count = 0;

        while (openList.Count > 0)
        {
            var min = openList.Pop();
            count++;

            //------------------Test----------------
            // string path = "";
            // List<BottleTransition> tempResultList = new List<BottleTransition>();
            // var tempP = min;
            // while (tempP != null)
            // {
            //     tempResultList.Add(tempP);
            //     tempP = tempP.parent;
            // }
            // tempResultList.Reverse();
            // for (int i = 0; i < tempResultList.Count; i++)
            // {
            //     var transition = tempResultList[i];
            //     path += $" {transition.fromBottleIndex + 1} -> {transition.toBottleIndex + 1}\n";
            // }
            // Debug.LogError("当前最小状态: " + " f: " + min.complexity + " path: " + path);
            //--------------------------------------

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

                Debug.Log($"------------------计算了{count}次---------------------");
                for (int i = 1; i < resultList.Count; i++)
                {
                    var transition = resultList[i];
                    Debug.Log($"第 {i} 步：从 {transition.fromBottleIndex + 1} 倒入 {transition.toBottleIndex + 1}");
                }
                stopwatch.Stop();
                Debug.Log("总共耗时: " + stopwatch.ElapsedMilliseconds + "ms");

                return resultList;
            }

            List<BottleTransition> opts = ListOptions(min); // 输出可能的每一步
            foreach (var opt in opts)
            {
                if (!clostHash.Contains(opt.stateHash))
                {
                    // 该状态不在close列表中，未被探索过
                    openList.Insert(opt);
                    // Debug.Log("加入第 " + opt.step + " 步的所有可能: " + "from " + (opt.fromBottleIndex + 1) + " to " + (opt.toBottleIndex + 1));
                }
            }

            clostHash.Add(min.stateHash);// 将min标记为已探索
        }

        stopwatch.Stop();
        Debug.Log("总共耗时: " + stopwatch.ElapsedMilliseconds + "ms");
        // 无解
        return null;
    }

    private static List<BottleTransition> ListOptions(BottleTransition currentTransition)
    {
        List<BottleTransition> options = new List<BottleTransition>();

        var bottleNum = currentTransition.doneState.bottles.Length;
        HashSet<string> optionsHash = new HashSet<string>();

        var tempBottleState = currentTransition.doneState.Clone();
        int tempPortoutNum = 0;
        for (int i = 0; i < bottleNum; i++)
        {
            for (int j = 0; j < bottleNum; j++)
            {
                if (j == i) continue;

                if (currentTransition.doneState.IsCanPourOut(i, j))
                {
                    // 计算倒出后的状态是否已经存在
                    tempPortoutNum = 0;
                    tempBottleState.PourOut(i, j, ref tempPortoutNum);
                    var tempHash = tempBottleState.CalculateHashString();
                    var tempComplexity = tempBottleState.CalculateComplexity();

                    if (!optionsHash.Contains(tempHash))
                    {
                        var newTransition = currentTransition.Clone();
                        newTransition.parent = currentTransition;
                        newTransition.doneState.PourOut(i, j);
                        newTransition.fromBottleIndex = i;
                        newTransition.toBottleIndex = j;
                        newTransition.step++;
                        newTransition.UpdateTransition(tempHash, tempComplexity);

                        optionsHash.Add(newTransition.stateHash);
                        options.Add(newTransition);
                    }

                    //还原临时状态
                    tempBottleState.ForceBackPourOut(i, j, tempPortoutNum);
                }
            }
        }

        return options;
    }
}
