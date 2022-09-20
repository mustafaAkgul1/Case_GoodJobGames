#region Header
//Created by Mustafa Akgul
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CLUtils
{
    public class Utils
    {
        public static IEnumerator DelayerCor(Action _action)
        {
            yield return null;

            _action?.Invoke();
        }

        public static IEnumerator DelayerCor(float _delayDuration, Action _action)
        {
            yield return new WaitForSeconds(_delayDuration);

            _action?.Invoke();
        }

        public static float GetPercentageValue(float _value, float _minValue, float _maxValue, bool _hasNegativeBounds = false)
        {
            float _percentage =
                _hasNegativeBounds ? (_maxValue - _value) / (_maxValue - _minValue) :
                1 - ((_maxValue - _value) / (_maxValue - _minValue));

            return _percentage;
        }

        public bool IsClickedOnUIObject()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        public bool CheckSegmentIntersection(Vector3 _pointStart, Vector3 _pointEnd, Vector3 _targetStart, Vector3 _targetEnd, out float r, out float s)
        {
            float denom = (_pointEnd.x - _pointStart.x) * (_targetEnd.z - _targetStart.z) - (_pointEnd.z - _pointStart.z) * (_targetEnd.x - _targetStart.x);
            float rNum = (_pointStart.z - _targetStart.z) * (_targetEnd.x - _targetStart.x) - (_pointStart.x - _targetStart.x) * (_targetEnd.z - _targetStart.z);
            float sNum = (_pointStart.z - _targetStart.z) * (_pointEnd.x - _pointStart.x) - (_pointStart.x - _targetStart.x) * (_pointEnd.z - _pointStart.z);

            if (Mathf.Approximately(rNum, 0) || Mathf.Approximately(denom, 0))
            { r = -1; s = -1; return false; }

            r = rNum / denom;
            s = sNum / denom;

            return (r >= 0 && r <= 1 && s >= 0 && s <= 1);
        }

    } // class
} // namespace
