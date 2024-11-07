using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ExcelData
{
    //表数据（包含一个excel文件中的所有表）
    public DataSet resultSet;

    //Excel中的所有数据（解析过的 第一层：表名-表数据 第二层：每一行的数据 第三层：列属性名-列中的数据） 
    public Dictionary<string, List<Dictionary<string, object>>> _excelDataDict =
        new Dictionary<string, List<Dictionary<string, object>>>();

    //枚举类型的对应关系  （防止在遍历表数据的时候频繁反射）
    public Dictionary<string, Type> _enumTypeDict = new Dictionary<string, Type>();

    //要生成的脚本名称
    public string ClassName = "";

    //要生成的资源文件名称
    public string AssetName = "";

    //Excel名称 (真实名字->未分割)
    public string ExcelName = "";

    //Excel名称 (分割后名字) -> 为了表数据做AB测试
    public string ExcelNameAfterSplit = "";

    //ExcelPath
    public string ExcelPath = "";
}