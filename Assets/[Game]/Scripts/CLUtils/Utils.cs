#region Header
//Created by Mustafa Akgul
#endregion

using System;
using System.Collections;
using UnityEngine;

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

    } // class
} // namespace
