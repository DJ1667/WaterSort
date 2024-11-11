using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

public class EventManager
{
    private static Dictionary<string, List<Events>> FuncDict = new Dictionary<string, List<Events>>();

    public static void Add<T1>(string key, Action<T1> func)
    {
        Add(key, func.Target, func.Method, 1);
    }

    public static void Add(string key, Action func)
    {
        Add(key, func.Target, func.Method, 0);
    }

    public static void Add<T1, T2>(string key, Action<T1, T2> func)
    {
        Add(key, func.Target, func.Method, 2);
    }

    public static void Add<T1, T2, T3>(string key, Action<T1, T2, T3> func)
    {
        Add(key, func.Target, func.Method, 3);
    }

    public static void Add<T1, T2, T3, T4>(string key, Action<T1, T2, T3, T4> func)
    {
        Add(key, func.Target, func.Method, 4);
    }

    private static void Add(string key, object target, MethodInfo metod, int paramNum)
    {
        List<Events> list = null;
        Events events;
        if (!FuncDict.TryGetValue(key, out list))
        {
            list = new List<Events>();
            FuncDict[key] = list;
        }
        events.info = metod;
        events.Target = target;
        events.ParamCount = paramNum;
        FuncDict[key].Add(events);
    }

    public static void Remove<T1>(string key, Action<T1> func)
    {
        Remove(key, func.Target, func.Method, 1);
    }

    public static void Remove(string key, Action func)
    {
        Remove(key, func.Target, func.Method, 0);
    }

    public static void Remove<T1, T2>(string key, Action<T1, T2> func)
    {
        Remove(key, func.Target, func.Method, 2);
    }

    public static void Remove<T1, T2, T3>(string key, Action<T1, T2, T3> func)
    {
        Remove(key, func.Target, func.Method, 3);
    }

    public static void Remove<T1, T2, T3, T4>(string key, Action<T1, T2, T3, T4> func)
    {
        Remove(key, func.Target, func.Method, 4);
    }

    private static void Remove(string key, object target, MethodInfo method, int paramNum)
    {
        List<Events> list = null;
        List<Events> removeList = new List<Events>();
        if (FuncDict.TryGetValue(key, out list))
        {
            foreach (Events events in list)
            {
                if (((events.Target == target) && (events.info == method)) && (paramNum == events.ParamCount))
                {
                    removeList.Add(events);
                }
            }
            if (removeList.Count > 0)
            {
                foreach (Events events in removeList)
                {
                    list.Remove(events);
                }
                removeList.Clear();
            }
        }
    }

    public static void Clear()
    {
        FuncDict.Clear();
    }

    public static void Trigger(string key)
    {
        Trigger(key, new object[0]);
    }

    public static void Trigger<T1>(string key, T1 t1)
    {
        object[] objlist = new object[] { t1 };
        Trigger(key, objlist);
    }

    public static void Trigger<T1, T2>(string key, T1 t1, T2 t2)
    {
        object[] objlist = new object[] { t1, t2 };
        Trigger(key, objlist);
    }

    public static void Trigger<T1, T2, T3>(string key, T1 t1, T2 t2, T3 t3)
    {
        object[] objlist = new object[] { t1, t2, t3 };
        Trigger(key, objlist);
    }

    public static void Trigger<T1, T2, T3, T4>(string key, T1 t1, T2 t2, T3 t3, T4 t4)
    {
        object[] objlist = new object[] { t1, t2, t3, t4 };
        Trigger(key, objlist);
    }

    private static void Trigger(string key, params object[] objlist)
    {
        List<Events> list = null;
        if (FuncDict.TryGetValue(key, out list))
        {
            int length = objlist.Length;
            foreach (Events events in list)
            {
                if (events.ParamCount == length)
                {
                    events.Trigger(objlist);
                }
            }
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct Events
{
    public MethodInfo info;
    public object Target;
    public int ParamCount;
    public void Trigger(params object[] objList)
    {
        this.info.Invoke(this.Target, objList);
    }
}