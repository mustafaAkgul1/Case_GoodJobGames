#region Header
//Created by Mustafa Akgul (CL)
//mustafaakgul248@gmail.com
#endregion

using UnityEngine;
using System;

namespace CLUtils
{
    public class SingletonOperator<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance { get { return _instance; } }


        public virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
            }
            else
            {
                _instance = this as T;
                //DontDestroyOnLoad(gameObject);
            }
        }

        protected void Anounce(Action<object[]> _eventName, params object[] _parameters)
        {
            _eventName?.Invoke(_parameters);
        }

    } // class
} // namespace
