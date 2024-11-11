using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BottleMono : MonoBehaviour
{
    private Bottle _bottle = null;
    private int _id = 0;
    public int Id => _id;
    private List<WaterMono> _waterMonoList = new List<WaterMono>();

    bool _isSelected = false;
    public bool IsSelected => _isSelected;
    bool _isPourIning = false;

    Sequence _sequenceShake = null;
    Sequence _sequenceUp = null;
    Sequence _sequenceDown = null;

    public void Init(Bottle bottle, int id)
    {
        this._bottle = bottle;
        this._id = id;
    }

    #region 数据变化

    public void AddWater(WaterMono waterMono, bool isInit = false)
    {
        if (_waterMonoList.Count >= _bottle.Segment) return;

        waterMono.transform.SetParent(transform);
        if (isInit)
            waterMono.transform.localPosition = new Vector3(0, WaterMono.OriginY + WaterMono.Height * _waterMonoList.Count, 0);
        _waterMonoList.Add(waterMono);
        waterMono.ChangeOrder(_waterMonoList.Count);
        AddColor(waterMono.Color);
    }

    public void AddColor(ColorType color)
    {
        if (_bottle.IsFull) return;
        _bottle.AddColor(color);
    }

    public void RemoveWater(WaterMono waterMono)
    {
        if (_waterMonoList.Count <= 0) return;

        for (int i = _waterMonoList.Count - 1; i >= 0; i--)
        {
            if (_waterMonoList[i] == waterMono)
            {
                _waterMonoList.RemoveAt(i);
                RemoveColor(waterMono.Color);
                break;
            }
        }
    }

    public void RemoveColor(ColorType color)
    {
        if (_bottle.IsEmpty) return;
        if (!_bottle.RemoveColor(color))
        {
            Debug.LogError("RemoveWater Error");
        }
    }

    #endregion

    #region 元素的动作

    private void WaterUp()
    {
        //还未完成倒入操作则不允许上升
        if (_isPourIning) return;

        var selectList = GetTopWater();
        if (selectList == null) return;

        _isSelected = true;

        // Debug.Log($"{_id} 号瓶子上升");

        //上升动画
        if (_sequenceDown != null)
            _sequenceDown.Kill();
        _sequenceUp = DOTween.Sequence();
        for (int i = 0; i < selectList.Count; i++)
        {
            var water = selectList[i];
            var endPosY = WaterMono.OriginY + WaterMono.Height * (_waterMonoList.Count + 0.5f - i); //每个上升1.5f
            // water.transform.DOLocalMoveY(endPosY, 0.25f);
            _sequenceUp.Join(water.transform.DOLocalMoveY(endPosY, 0.25f));
        }
    }

    private void WaterDown()
    {
        var selectList = GetTopWater();
        if (selectList == null) return;

        _isSelected = false;

        // Debug.Log($"{_id} 号瓶子下降");

        //下降动画
        if (_sequenceUp != null)
            _sequenceUp.Kill();
        _sequenceDown = DOTween.Sequence();
        for (int i = 0; i < selectList.Count; i++)
        {
            var water = selectList[i];
            int index = i;
            var endPosY = WaterMono.OriginY + WaterMono.Height * (_waterMonoList.Count - 1 - i);
            // water.transform.DOLocalMoveY(endPosY, 0.25f).OnComplete(() =>
            // {
            //     if (index == 0)
            //         LastWaterShake(water);
            // });
            _sequenceDown.Join(water.transform.DOLocalMoveY(endPosY, 0.25f));
        }

        _sequenceDown.OnComplete(() =>
        {
            LastWaterShake(selectList[0]);
        });
    }

    private void WaterToTarget(BottleMono bottleMono)
    {
        _isSelected = false;
        var waterList = GetTopWater();
        var posList = bottleMono.GetEmptyPosList();

        if (waterList.Count > 0)
        {
            bottleMono._isPourIning = true;

            //清除还未完成的抖动动画
            if (bottleMono._sequenceShake != null)
                bottleMono._sequenceShake.Kill();

            //清除还未完成的上升动画
            if (_sequenceUp != null)
                _sequenceUp.Kill();

            // Debug.Log($"{_id} 号瓶子倒入 {bottleMono.Id} 号瓶子");
        }

        for (int i = 0; i < waterList.Count; i++)
        {
            var water = waterList[i];
            var endPos = posList[i];
            var mousePos = bottleMono.GetBottleMouthPos();
            var startPos = GetBottleMouthPos();
            startPos.y = Mathf.Max(startPos.y, mousePos.y);
            mousePos.y = Mathf.Max(startPos.y, mousePos.y);

            Vector3[] path = new Vector3[3];
            path[0] = startPos;
            path[1] = mousePos;
            path[2] = endPos;

            var index = i;

            //数据先行
            RemoveWater(water);
            bottleMono.AddWater(water);

            water.transform.DOPath(path, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (index == waterList.Count - 1)
                    LastWaterShake(water, () => { bottleMono._isPourIning = false; });
            });
        }
    }

    #endregion

    #region 通用动画

    private void LastWaterShake(WaterMono waterMono, Action oncomplete = null)
    {
        _sequenceShake = waterMono.transform.DoShakeY(0.25f, 0.15f, 2).OnComplete(() => { oncomplete?.Invoke(); });
    }

    #endregion

    #region 获得一些数据

    public List<Vector3> GetEmptyPosList()
    {
        List<Vector3> posList = new List<Vector3>();
        for (int i = 0; i < _bottle.Segment - _waterMonoList.Count; i++)
        {
            posList.Add(transform.position + new Vector3(0, WaterMono.OriginY + WaterMono.Height * (_waterMonoList.Count + i), 0));
        }

        return posList;
    }

    private List<WaterMono> GetTopWater()
    {
        if (_bottle.IsEmpty) return null;

        List<WaterMono> selectList = new List<WaterMono>();
        var waterMono = _waterMonoList[^1];
        selectList.Add(waterMono);

        for (int i = _waterMonoList.Count - 2; i >= 0; i--)
        {
            if (_waterMonoList[i].Color == waterMono.Color)
            {
                selectList.Add(_waterMonoList[i]);
            }
            else
            {
                break;
            }
        }

        return selectList;
    }

    public Vector3 GetBottleMouthPos()
    {
        return transform.position + new Vector3(0, WaterMono.OriginY + WaterMono.Height * (_bottle.Segment + 1), 0);
    }

    #endregion

    public bool IsCanPourOut(BottleMono bottleMono)
    {
        return _bottle.IsCanPourOut(bottleMono._bottle) && GetTopWater().Count <= bottleMono.GetEmptyPosList().Count;
    }

    public void OnBottleClick(BottleMono lastBottleMono)
    {
        //是否是自己
        if (lastBottleMono == null || _id == lastBottleMono.Id)
        {
            if (_waterMonoList.Count > 0)
            {
                if (_isSelected)
                {
                    WaterDown();
                }
                else
                {
                    WaterUp();
                }
            }
        }
        else
        {
            if (lastBottleMono.IsSelected)
            {
                if (lastBottleMono.IsCanPourOut(this))
                {
                    lastBottleMono.WaterToTarget(this);
                }
                else
                {
                    lastBottleMono.WaterDown();

                    if (_waterMonoList.Count > 0)
                        WaterUp();
                }
            }
            else
            {
                if (_waterMonoList.Count > 0)
                    WaterUp();
            }
        }
    }
}
