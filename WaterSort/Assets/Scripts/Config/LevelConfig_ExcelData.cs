/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class LevelConfig_ExcelData : ScriptableObject
{
	public void Init()
	{
			Sheet1ConfigDict.Clear();
		if(Sheet1ConfigList != null && Sheet1ConfigList.Count > 0)
		{
			for(int i = 0; i < Sheet1ConfigList.Count; i++)
			{
				Sheet1ConfigDict.Add(Sheet1ConfigList[i].Id, Sheet1ConfigList[i]);
			}
		}


	}

	public List<LevelConfig_Sheet1_Config> Sheet1ConfigList = new List<LevelConfig_Sheet1_Config>();

	public Dictionary<int,LevelConfig_Sheet1_Config> Sheet1ConfigDict = new Dictionary<int,LevelConfig_Sheet1_Config>();

	public LevelConfig_Sheet1_Config GetSheet1_Config(int id)
	{
		if(Sheet1ConfigDict.ContainsKey(id))
			return Sheet1ConfigDict[id];
		else
			return null;
	}
	#region --- Get Method ---
	public int Get_Sheet1_BottleCount(int id)
	{
		var config = GetSheet1_Config(id);
		if(config == null)
			return default;
		return config.BottleCount;
	}
	public int Get_Sheet1_EmptyBottleCount(int id)
	{
		var config = GetSheet1_Config(id);
		if(config == null)
			return default;
		return config.EmptyBottleCount;
	}
	public int Get_Sheet1_Segment(int id)
	{
		var config = GetSheet1_Config(id);
		if(config == null)
			return default;
		return config.Segment;
	}
	public int Get_Sheet1_TempBottleCount(int id)
	{
		var config = GetSheet1_Config(id);
		if(config == null)
			return default;
		return config.TempBottleCount;
	}
	public int Get_Sheet1_TempSegment(int id)
	{
		var config = GetSheet1_Config(id);
		if(config == null)
			return default;
		return config.TempSegment;
	}
	public int Get_Sheet1_SingleColorBottleCount(int id)
	{
		var config = GetSheet1_Config(id);
		if(config == null)
			return default;
		return config.SingleColorBottleCount;
	}
	public int Get_Sheet1_SingleColorSegment(int id)
	{
		var config = GetSheet1_Config(id);
		if(config == null)
			return default;
		return config.SingleColorSegment;
	}
	public int Get_Sheet1_DegreeOfDifficulty(int id)
	{
		var config = GetSheet1_Config(id);
		if(config == null)
			return default;
		return config.DegreeOfDifficulty;
	}
	#endregion

}
[Serializable]
public class LevelConfig_Sheet1_Config
{
	/// <summary>
	/// 关卡id
	/// </summary>>
	public int Id;
	/// <summary>
	/// 瓶子数量
	/// </summary>>
	public int BottleCount;
	/// <summary>
	/// 空瓶子数量
	/// </summary>>
	public int EmptyBottleCount;
	/// <summary>
	/// 每个瓶子的段数
	/// </summary>>
	public int Segment;
	/// <summary>
	/// 临时瓶子数量
	/// </summary>>
	public int TempBottleCount;
	/// <summary>
	/// 临时瓶子的段数
	/// </summary>>
	public int TempSegment;
	/// <summary>
	/// 单色瓶子数量
	/// </summary>>
	public int SingleColorBottleCount;
	/// <summary>
	/// 单色瓶子的段数
	/// </summary>>
	public int SingleColorSegment;
	/// <summary>
	/// 难度（1-10）
	/// </summary>>
	public int DegreeOfDifficulty;
}

