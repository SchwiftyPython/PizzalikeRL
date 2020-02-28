using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class BlastAOE : IAreaOfEffect
{
    private Vector2 _center;
    private Vector2 _origin;
    private int _radius;
    private Tile[,] _areaMap;
    private int _minDistance;
    private int _maxDistance;
    private AimLimit _limitType;

    public BlastAOE()
    {
        _center = Vector2.zero;
        _radius = 2;
        _limitType = AimLimit.Free;
    }

    public BlastAOE(Vector2 center, int radius)
    {
        _center = center;
        _radius = radius;
        _limitType = AimLimit.Free;
    }

    public BlastAOE(Vector2 center, int radius, int minRange, int maxRange)
    {
        _center = center;
        _radius = radius;
        _minDistance = minRange;
        _maxDistance = maxRange;
        _limitType = AimLimit.Free;
    }

    public Vector2 GetCenter()
    {
        return _center;
    }

    public void SetCenter(Vector2 center)
    {
        if (_areaMap != null && center.x >= 0 && center.x < _areaMap.GetLength(0) && center.y >= 0 &&
            center.y < _areaMap.GetLength(0))
        {
            _center = center;
        }
    }

    public int GetRadius()
    {
        return _radius;
    }

    public void SetRadius(int radius)
    {
        _radius = radius;
    }

    public void Shift(Vector2 point)
    {
        SetCenter(point);
    }

    public bool MayContainTarget(List<Vector2> targets)
    {
        foreach(var p in targets)
        {
            if (Radius(_center.x, _center.y, p.x, p.y) <= _radius)
            {
                return true;
            }
        }
        return false;
    }

    public float Radius(float startX, float startY, float endX, float endY)
    {
        double dx = startX - endX;
        double dy = startY - endY;

        dx = Math.Abs(dx);
        dy = Math.Abs(dy);

        //standard spherical radius
        return (float) Math.Sqrt(dx * dx + dy * dy);
    }

    public OrderedDictionary IdealLocations(List<Vector2> targets, List<Vector2> requiredExclusions)
    {
        throw new NotImplementedException();
    }

    public OrderedDictionary IdealLocations(List<Vector2> priorityTargets, List<Vector2> lesserTargets, List<Vector2> requiredExclusions)
    {
        throw new NotImplementedException();
    }

    public void SetAreaMap(Tile[,] areaMap)
    {
        _areaMap = areaMap;
    }

    public OrderedDictionary FindArea()
    {
        throw new NotImplementedException();
    }

    public Vector2 GetOrigin()
    {
        return _origin;
    }

    public void SetOrigin(Vector2 origin)
    {
        _origin = origin;
    }

    public AimLimit GetLimitType()
    {
        return _limitType;
    }

    public void SetLimitType(AimLimit limitType)
    {
        _limitType = limitType;
    }

    public int GetMinRange()
    {
        return _minDistance;
    }

    public int GetMaxRange()
    {
        return _maxDistance;
    }

    public void SetMinRange(int minRange)
    {
        _minDistance = minRange;
    }

    public void SetMaxRange(int maxRange)
    {
        _maxDistance = maxRange;
    }
}
