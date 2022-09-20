#region Header
//Created by Mustafa Akgul (CL)
//mustafaakgul248@gmail.com
#endregion

using UnityEngine;

namespace CLUtils
{
    public class RotationLockerModule : MonoBehaviour
    {
        [Header("General Variables")]
        [SerializeField] Vector3 targetRot;

        void Update()
        {
            transform.eulerAngles = targetRot;
        }

    } // class
} // namespace

