using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public static class DUtils
{
    #region 列表

    /// <summary>
    /// 随机获取列表中的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="num">获取元素的数量</param>
    /// <param name="isRepeat">是否允许重复</param>
    /// <returns></returns>
    public static List<T> GetRandomElement<T>(List<T> list, int num, bool isRepeat = false)
    {
        List<T> result = new List<T>();
        List<T> tempList = new List<T>(list);
        for (int i = 0; i < num; i++)
        {
            int index = Random.Range(0, tempList.Count);
            result.Add(tempList[index]);
            if (!isRepeat)
            {
                tempList.RemoveAt(index);
            }
        }
        return result;
    }

    /// <summary>
    /// Fisher-Yates 打乱算法，随机打乱列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void FisherYatesShuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// 控制打乱程度的算法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="intensity"></param>
    public static void ShuffleWithIntensity<T>(List<T> list, float intensity)
    {
        int n = list.Count;
        int maxSwaps = (int)(n * intensity);  // 根据打乱程度调整交换次数

        for (int i = 0; i < maxSwaps; i++)
        {
            int index1 = Random.Range(0, n);
            int index2 = Random.Range(0, n);
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }
    }

    #endregion

    #region 组件

    public static T AddOrGetComponent<T>(this GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }

    public static T AddOrGetComponent<T>(this Transform trans) where T : Component
    {
        T component = trans.GetComponent<T>();
        if (component == null)
            component = trans.gameObject.AddComponent<T>();

        return component;
    }

    public static void SetActiveEx(this GameObject go, bool active)
    {
        if (!go)
        {
            Debug.LogError("GameObject is null");
            return;
        }

        if (go.activeSelf != active)
            go.SetActive(active);
    }

    public static void SetActiveEx(this Transform trans, bool active)
    {
        if (!trans)
        {
            Debug.LogError("Transform is null");
            return;
        }

        if (trans.gameObject.activeSelf != active)
            trans.gameObject.SetActive(active);
    }

    public static void SetAnchor(this RectTransform source, AnchorPresets allign, int offsetX = 0, int offsetY = 0)
    {
        source.anchoredPosition = new Vector3(offsetX, offsetY, 0);

        switch (allign)
        {
            case (AnchorPresets.TopLeft):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.TopCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 1);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.TopRight):
                {
                    source.anchorMin = new Vector2(1, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.MiddleLeft):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(0, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0.5f);
                    source.anchorMax = new Vector2(0.5f, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleRight):
                {
                    source.anchorMin = new Vector2(1, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }

            case (AnchorPresets.BottomLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 0);
                    break;
                }
            case (AnchorPresets.BottomCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 0);
                    break;
                }
            case (AnchorPresets.BottomRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

            case (AnchorPresets.HorStretchTop):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
            case (AnchorPresets.HorStretchMiddle):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }
            case (AnchorPresets.HorStretchBottom):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

            case (AnchorPresets.VertStretchLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.VertStretchCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.VertStretchRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.StretchAll):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
        }
    }

    public static void SetPivot(this RectTransform source, PivotPresets preset)
    {
        switch (preset)
        {
            case (PivotPresets.TopLeft):
                {
                    source.pivot = new Vector2(0, 1);
                    break;
                }
            case (PivotPresets.TopCenter):
                {
                    source.pivot = new Vector2(0.5f, 1);
                    break;
                }
            case (PivotPresets.TopRight):
                {
                    source.pivot = new Vector2(1, 1);
                    break;
                }

            case (PivotPresets.MiddleLeft):
                {
                    source.pivot = new Vector2(0, 0.5f);
                    break;
                }
            case (PivotPresets.MiddleCenter):
                {
                    source.pivot = new Vector2(0.5f, 0.5f);
                    break;
                }
            case (PivotPresets.MiddleRight):
                {
                    source.pivot = new Vector2(1, 0.5f);
                    break;
                }

            case (PivotPresets.BottomLeft):
                {
                    source.pivot = new Vector2(0, 0);
                    break;
                }
            case (PivotPresets.BottomCenter):
                {
                    source.pivot = new Vector2(0.5f, 0);
                    break;
                }
            case (PivotPresets.BottomRight):
                {
                    source.pivot = new Vector2(1, 0);
                    break;
                }
        }
    }

    /// <summary>
    /// 更改所有子物体的Layer
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="layer"></param>
    public static void SetLayersRecursively(this GameObject gameObject, int layer, bool isIncludeSelf = true)
    {
        if (isIncludeSelf)
        {
            gameObject.layer = layer;
        }

        foreach (Transform transform in gameObject.transform)
        {
            transform.gameObject.SetLayersRecursively(layer, true);
        }
    }

    public static Vector2 GetAnchoredPos(this Component component)
    {
        var anchoredPos = Vector2.zero;
        RectTransform rectTransform = component.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            anchoredPos = rectTransform.anchoredPosition;
        }
        return anchoredPos;
    }

    #endregion

    #region 自定义log

    public static bool IsDebug = false;

    public static void Log(string str)
    {
        if (!IsDebug) return;
        Debug.Log(str);
    }

    public static void LogWarning(string str)
    {
        if (!IsDebug) return;
        Debug.LogWarning(str);
    }

    public static void LogError(string str)
    {
        if (!IsDebug) return;
        Debug.LogError(str);
    }

    #endregion

    #region 时间

    /// <summary>
    /// 获取今日日期
    /// </summary>
    /// <returns>year-month-day</returns>
    public static string GetDateStr_Today()
    {
        return GetDateStr_YMD(DateTime.Now);
    }

    /// <summary>
    /// 获取本地当前时间字符串
    /// </summary>
    /// <param name="dt"></param>
    /// <returns>年-月-日</returns>
    public static string GetDateStr_YMD(DateTime dt)
    {
        string dt_str = dt.Year + "-" + dt.Month.ToString("00") + "-" + dt.Day.ToString("00");
        return dt_str;
    }

    /// <summary>
    /// 获取本地当前时间字符串
    /// </summary>
    /// <param name="dt"></param>
    /// <returns>年-月-日-时-分-秒</returns>
    public static string GetDateStr_YMD_HMS(DateTime dt)
    {
        string dt_str = dt.Year + "-" + dt.Month.ToString("00") + "-" + dt.Day.ToString("00") + "-" + dt.Hour.ToString("00") + "-" + dt.Minute.ToString("00") + "-" + dt.Second.ToString("00");
        return dt_str;
    }

    /// <summary>
    /// 本地时间字符串转DateTime
    /// </summary>
    /// <param name="dataStr"></param>
    /// <returns>返回DateTime类型</returns>
    public static DateTime DateStrToDateTime(string dataStr)
    {
        DateTime dt = default;
        string[] timeDatas = dataStr.Split('-');
        if (timeDatas.Length < 6)
        {
            Debug.LogError("时间字符串格式错误");
            return dt;
        }
        string tempTimeString = timeDatas[0] + "-" + timeDatas[1] + "-" + timeDatas[2] + "-" + timeDatas[3] + ":" + timeDatas[4] + ":" + timeDatas[5];
        //将固定格式的字符串格式化，并转为 DateTime 类型 时间是本地时间
        if (DateTime.TryParseExact(tempTimeString, "yyyy-MM-dd-HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture,
        System.Globalization.DateTimeStyles.AdjustToUniversal, out dt))
        {
            return dt;
        }

        Debug.LogError("时间字符串转换失败");
        return dt;
    }

    /// <summary>
    /// 获取时间间隔
    /// </summary>
    /// <param name="dataStr"></param>
    /// <returns></returns>
    public static TimeSpan GetTimeIntervalFromNow(string dataStr)
    {
        DateTime passTime = DateStrToDateTime(dataStr);
        DateTime nowTime = DateTime.Now;
        TimeSpan timeInterval = nowTime - passTime;
        return timeInterval;
    }

    /// <summary>
    /// 获取时间间隔
    /// </summary>
    /// <param name="lastTime"></param>
    /// <returns></returns>
    public static TimeSpan GetTimeIntervalFromNow(DateTime lastTime)
    {
        DateTime nowTime = DateTime.Now;
        TimeSpan timeInterval = nowTime - lastTime;
        return timeInterval;
    }

    /// <summary>
    /// 获取时间间隔
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    public static TimeSpan GetTimeIntervalFromNow(long timeStamp)
    {
        DateTime lastTime = TimeStampToDateTime(timeStamp);
        DateTime nowTime = DateTime.Now;
        TimeSpan timeInterval = nowTime - lastTime;
        return timeInterval;
    }

    /// <summary>
    /// 时间戳转DateTime
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    public static DateTime TimeStampToDateTime(long timeStamp)
    {
        DateTime dtStart = new DateTime(1970, 1, 1, 8, 0, 0);
        var targetTime = dtStart.AddSeconds(timeStamp);
        return targetTime;
    }

    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <returns></returns>
    public static long GetTimeStamp()
    {
        TimeSpan st = DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0);
        return Convert.ToInt64(st.TotalSeconds);
    }

    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static long GetTimeStamp(DateTime dateTime)
    {
        TimeSpan st = dateTime - new DateTime(1970, 1, 1, 8, 0, 0);
        return Convert.ToInt64(st.TotalSeconds);
    }

    /// <summary>
    /// 与今天是否是同一天
    /// </summary>
    /// <param name="lastTime"></param>
    /// <returns></returns>
    public static bool IsSameDay(long lastTime)
    {
        DateTime lastDateTime = TimeStampToDateTime(lastTime);
        return IsSameDay(lastDateTime);
    }

    /// <summary>
    /// 与今天是否是同一天
    /// </summary>
    /// <param name="lastDateTime"></param>
    /// <returns></returns>
    public static bool IsSameDay(DateTime lastDateTime)
    {
        DateTime nowDateTime = DateTime.Now;
        return lastDateTime.Year == nowDateTime.Year && lastDateTime.Month == nowDateTime.Month && lastDateTime.Day == nowDateTime.Day;
    }

    /// <summary>
    /// 与指定天是否是同一天
    /// </summary>
    /// <param name="lastDateTime"></param>
    /// <param name="nowDateTime"></param>
    /// <returns></returns>
    public static bool IsSameDay(DateTime lastDateTime, DateTime nowDateTime)
    {
        return lastDateTime.Year == nowDateTime.Year && lastDateTime.Month == nowDateTime.Month && lastDateTime.Day == nowDateTime.Day;
    }

    /// <summary>
    /// 与指定天是否是同一天
    /// </summary>
    /// <param name="lastTime"></param>
    /// <param name="nowTime"></param>
    /// <returns></returns>
    public static bool IsSameDay(long lastTime, long nowTime)
    {
        DateTime lastDateTime = TimeStampToDateTime(lastTime);
        DateTime nowDateTime = TimeStampToDateTime(nowTime);
        return IsSameDay(lastDateTime, nowDateTime);
    }

    #endregion

    #region Vector

    /// <summary>
    /// 三维转二维
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static Vector2 ToV2(this Vector3 input) => new Vector2(input.x, input.y);

    /// <summary>
    /// y方向归零
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static Vector3 Flat(this Vector3 input) => new Vector3(input.x, 0, input.z);

    /// <summary>
    /// 转成int类型数值
    /// </summary>
    /// <param name="vec3"></param>
    /// <returns></returns>
    public static Vector3Int ToVector3Int(this Vector3 vec3) =>
        new Vector3Int((int)vec3.x, (int)vec3.y, (int)vec3.z);

    /// <summary>
    /// 转成int类型数值
    /// </summary>
    /// <param name="vec2"></param>
    /// <returns></returns>
    public static Vector2Int ToVector3Int(this Vector2Int vec2) =>
    new Vector2Int((int)vec2.x, (int)vec2.y);

    /// <summary>
    /// 将一个Vector2旋转degrees度   返回旋转后的结果
    /// </summary>
    /// <param name="v"></param>
    /// <param name="degrees"></param>
    /// <returns></returns>
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        //得到二维旋转矩阵
        // cos -sin
        // sin  cos

        //矩阵乘以向量得到结果
        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }

    #endregion

    #region 文件操作

    public static string RootPath
    {
        get
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            {
                string tempPath = Application.persistentDataPath;
                string dataPath = "";

                if (!string.IsNullOrEmpty(tempPath))
                {

                    dataPath = PlayerPrefs.GetString("DataPath", "");
                    if (string.IsNullOrEmpty(dataPath))
                    {
                        PlayerPrefs.SetString("DataPath", tempPath);
                    }

                    return tempPath + "/";
                }
                else
                {
                    Debug.Log("Application.persistentDataPath Is Null.");

                    dataPath = PlayerPrefs.GetString("DataPath", "");

                    return dataPath + "/";
                }
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                ///*如果是电脑的编辑模式，先放在项目外面*/
                return Application.dataPath.Replace("Assets", "");
            }
            else
            {
                return Application.dataPath + "/";
            }
        }
    }

    /// <summary>
    /// 文件是否存在
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsFileExists(string path)
    {
        return File.Exists(path);
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="path"></param>
    public static void DeleteFile(string path)
    {
        File.Delete(path);
    }

    /// <summary>
    /// 判断文件夹是否存在
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsFolderExists(string path)
    {
        return Directory.Exists(path);
    }

    /// <summary>
    /// 创建文件夹
    /// </summary>
    /// <param name="path"></param>
    public static void CreateFolder(string path)
    {
        Directory.CreateDirectory(path);
    }

    /// <summary>
    /// 拷贝文件夹
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public static void CopyFolder(string from, string to)
    {
        if (!Directory.Exists(to))
            Directory.CreateDirectory(to);

        foreach (var dir in Directory.GetDirectories(from))
        {
            CopyFolder(dir, to + Path.GetFileName(dir) + "/");
        }

        foreach (var file in Directory.GetFiles(from))
        {
            File.Copy(file, to + Path.GetFileName(file), true);
        }
    }

    /// <summary>
    /// 拷贝文件
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="overWrite"></param>
    public static void CopyFile(string from, string to, bool overWrite)
    {
        File.Copy(from, to, overWrite);
    }

    /// <summary>
    /// 创建或写入(覆盖)文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="info"></param>
    public static void CreateOrWriteFile(string path, string info)
    {

        //判断这个路径是否存在
        var tempPath = GetFilePathWithOutFileName(path);
        if (!Directory.Exists(tempPath))
        {
            Directory.CreateDirectory(tempPath);
        }

        //判断这个文件是否存在
        //写入文件
        if (File.Exists(path))
        {
            Debug.Log(path + "    文件已存在，将被替换");
        }
        else
        {
            Debug.Log(path + "    创建文件");
        }
        //补充 using(){} ()中的对象必须继承IDispose接口,在{}结束后会自动释放资源,也就是相当于帮你调用了Dispos()去释放资源
        using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            using (TextWriter textWriter = new StreamWriter(fileStream))
            {
                textWriter.Write(info);
            }
        }

        //再unity中使用要刷新一下
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    /// <summary>
    /// 读取文件第一行
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReadFileFirstLine(string path)
    {
        if (!IsFileExists(path)) return "";

        string resultStr;
        using (StreamReader sr = File.OpenText(path))
        {
            resultStr = sr.ReadLine();
        }

        return resultStr;
    }

    /// <summary>
    /// 读取文件所有内容
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReadFile(string path)
    {
        if (!IsFileExists(path)) return "";

        string resultStr;
        using (StreamReader sr = File.OpenText(path))
        {
            resultStr = sr.ReadToEnd();
        }

        return resultStr;
    }

    /// <summary>
    /// 根据文件全路径获取 文件名
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFileName(string path)
    {
        var tempIndex = path.LastIndexOf("/");
        var fileNameWithSuffix = path.Substring(tempIndex + 1);
        var suffixIndex = path.LastIndexOf(".");
        var fileName = fileNameWithSuffix.Substring(0, suffixIndex);

        return fileName;
    }

    /// <summary>
    /// 根据文件全路径获取 路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFilePathWithOutFileName(string path)
    {
        var tempIndex = path.LastIndexOf("/");
        var result = path.Substring(0, tempIndex);

        return result;
    }


    public static void CreateFile(string filePath, byte[] bytes)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        FileInfo file = new FileInfo(filePath);
        using (Stream stream = file.Create())
        {
            stream.Write(bytes, 0, bytes.Length);
            // stream.Close();
            // stream.Dispose();
        }
    }

    /// <summary>
    /// 删除文件夹下所有文件
    /// </summary>
    /// <param name="fullPath"></param>
    public static void DeleteAllFile(string fullPath)
    {
        if (Directory.Exists(fullPath))
        {
            DirectoryInfo directory = new DirectoryInfo(fullPath);
            FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                //if (files[i].Name.EndsWith(".mate"))
                //{
                //    continue;
                //}
                File.Delete(files[i].FullName);
            }
        }
    }

    /// <summary>
    /// 写文件操作
    /// 指定路径文件不存在会被创建
    /// </summary>
    /// <param name="path">文件路径（包含Application.persistentDataPath）.</param>
    /// <param name="name">文件名.</param>
    /// <param name="info">写入内容.</param>
    public static string CreateORwriteFile(string path, string info)
    {
        //创建文件夹
        var dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        fs.Close();
        StreamWriter sw = new StreamWriter(path);
        sw.WriteLine(info);
        sw.Close();
        sw.Dispose();

        return path;
    }

    public static string CreateORwriteBytesFile(string path, byte[] info)
    {
        //创建文件夹
        var dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllBytes(path, info);

        return path;
    }

    public static byte[] ReadFileBytes(string path)
    {
        return File.ReadAllBytes(path);
    }

    /// <summary>
    /// 读取文件所有内容
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string ReadAllFile(string path)
    {
        string fileContent;
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(path);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
        fileContent = sr.ReadToEnd();
        sr.Close();
        sr.Dispose();
        return fileContent;

    }

    #endregion

    #region 动画Animation

    /// <summary>
    /// 动画正播
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="clipName"></param>
    public static void PlayForward(this Animation anim, string clipName)
    {
        var animationState = anim[clipName];
        if (animationState != null)
        {
            PlayAnim(anim, animationState, clipName, 0, 1);
        }
        else
        {
            Debug.LogError("没有这个动画  ====  " + clipName);
        }
    }

    /// <summary>
    /// 动画倒播
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="clipName"></param>
    public static void PlayBackward(this Animation anim, string clipName)
    {
        var animationState = anim[clipName];
        if (animationState != null)
        {
            PlayAnim(anim, animationState, clipName, animationState.clip.length, -1);
        }
        else
        {
            Debug.LogError("没有这个动画  ====  " + clipName);
        }
    }

    private static void PlayAnim(Animation anim, AnimationState animationState, string clipName, float animTime, float animSpeed)
    {
        if (anim.isPlaying)
        {
            anim.Stop();
        }

        animationState.time = animTime;
        anim.Sample();
        animationState.speed = animSpeed;
        anim.Play(clipName);
    }

    /// <summary>
    /// 动画是否正在正播  (一般用在动画事件中  根据正播还是倒播的状态控制动画事件的执行)
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="clipName">动画名称</param>
    /// <returns></returns>
    public static bool IsAnimForwardPlaying(this Animation anim, string clipName)
    {
        var animationState = anim[clipName];
        if (animationState != null)
        {
            if (animationState.speed > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.LogError("没有这个动画  ====  " + clipName);
            return true;
        }
    }

    /// <summary>
    /// 检查是否已存在该动画事件
    /// </summary>
    /// <param name="animationState"></param>
    /// <param name="funcName"></param>
    /// <returns></returns>
    private static bool CheckAnimEventIsLegal(AnimationState animationState, string funcName, float triggerTime)
    {
        if (triggerTime > animationState.clip.length || triggerTime < 0)
        {
            Debug.LogError("输入的时间超出动画时长 或 小于0");
            return true;
        }

        var events = animationState.clip.events;
        for (var i = 0; i < events.Length; i++)
        {
            var animEvent = events[i];

            if (animEvent.functionName.Equals(funcName))
            {
                //Debug.LogError("重复添加动画事件    " + funcName);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 添加动画事件
    /// </summary>
    /// <param name="animationState"></param>
    /// <param name="clipName"></param>
    /// <param name="triggerTime"></param>
    /// <param name="funcName"></param>
    /// <param name="arg"></param>
    private static void AddAnimEvent(AnimationState animationState, string clipName, float triggerTime, string funcName, object arg)
    {

        if (CheckAnimEventIsLegal(animationState, funcName, triggerTime)) return;

        AnimationEvent animationEvent = new AnimationEvent();
        animationEvent.functionName = funcName;
        animationEvent.time = triggerTime;

        if (arg != null)
        {
            if (arg is int)
                animationEvent.intParameter = (int)arg;
            else if (arg is float)
                animationEvent.floatParameter = (float)arg;
            else if (arg is string)
                animationEvent.stringParameter = (string)arg;
            else if (arg is UnityEngine.Object)
                animationEvent.objectReferenceParameter = (UnityEngine.Object)arg;
        }

        animationState.clip.AddEvent(animationEvent);

    }

    /// <summary>
    /// 添加动画结束事件
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="clipName">动画名称</param>
    /// <param name="funcName">函数名称</param>
    /// <param name="arg">传递的参数   如果是传递一个Object也就是一个类，那么这个类要继承ScriptableObject才能被传递</param>
    public static void AddAnimCallBackEnd(this Animation anim, string clipName, string funcName, object arg = null)
    {
        var animationState = anim[clipName];
        if (animationState != null)
        {
            AddAnimEvent(animationState, clipName, animationState.clip.length, funcName, arg);
        }
        else
        {
            Debug.LogError("没有这个动画  ====  " + clipName);
        }
    }

    /// <summary>
    /// 添加动画开始事件
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="clipName">动画名称</param>
    /// <param name="funcName">函数名称</param>
    /// <param name="arg">传递的参数   如果是传递一个Object也就是一个类，那么这个类要继承ScriptableObject才能被传递</param>
    public static void AddAnimCallBackStart(this Animation anim, string clipName, string funcName, object arg = null)
    {
        var animationState = anim[clipName];
        if (animationState != null)
        {
            AddAnimEvent(animationState, clipName, 0, funcName, arg);
        }
        else
        {
            Debug.LogError("没有这个动画  ====  " + clipName);
        }
    }

    /// <summary>
    /// 在动画的某个时间添加事件
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="clipName">动画名称</param>
    /// <param name="triggerTime">在动画播到哪个时间段调用</param>
    /// <param name="funcName">函数名称</param>
    /// <param name="arg">传递的参数   如果是传递一个Object也就是一个类，那么这个类要继承ScriptableObject才能被传递</param>
    public static void AddAnimCallBackInSomeTime(this Animation anim, string clipName, float triggerTime, string funcName, object arg = null)
    {
        var animationState = anim[clipName];
        if (animationState != null)
        {
            AddAnimEvent(animationState, clipName, triggerTime, funcName, arg);
        }
        else
        {
            Debug.LogError("没有这个动画  ====  " + clipName);
        }
    }

    #endregion

    #region 动画Animator

    public static void SetAnimEventByPercentage(this Animator animator, string clipName, float percentage, string EventName)
    {
        var clips = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            var clip = clips[i];
            if (clip.name == clipName)
            {
                AnimationEvent animationEvent = new AnimationEvent();
                animationEvent.functionName = EventName;
                animationEvent.time = percentage * clip.length;
                AddEvent(clip, animationEvent);
            }
        }
    }

    public static void SetAnimEventByTime(this Animator animator, string clipName, float time, string EventName)
    {
        var clips = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            var clip = clips[i];
            if (clip.name == clipName)
            {
                AnimationEvent animationEvent = new AnimationEvent();
                animationEvent.functionName = EventName;
                animationEvent.time = time;
                AddEvent(clip, animationEvent);
            }
        }
    }

    static void AddEvent(AnimationClip clip, AnimationEvent animationEvent)
    {
        for (int i = 0; i < clip.events.Length; i++)
        {
            if (clip.events[i].functionName == animationEvent.functionName)
                return;
        }

        clip.AddEvent(animationEvent);
    }

    #endregion
}

public enum AnchorPresets
{
    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    MiddleCenter,
    MiddleRight,

    BottomLeft,
    BottomCenter,
    BottomRight,

    VertStretchLeft,
    VertStretchRight,
    VertStretchCenter,

    HorStretchTop,
    HorStretchMiddle,
    HorStretchBottom,

    StretchAll
}

public enum PivotPresets
{
    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    MiddleCenter,
    MiddleRight,

    BottomLeft,
    BottomCenter,
    BottomRight,
}