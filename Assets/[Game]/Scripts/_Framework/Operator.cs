#region Header
//Created by Mustafa Akgul (CL)
//mustafaakgul248@gmail.com
#endregion

using UnityEngine;
using System;

namespace CLUtils
{
    public class Operator : MonoBehaviour
    {
        protected void Anounce(Action<object[]> _eventName, params object[] _parameters)
        {
            _eventName?.Invoke(_parameters);
        }

    } // class
} // namespace

