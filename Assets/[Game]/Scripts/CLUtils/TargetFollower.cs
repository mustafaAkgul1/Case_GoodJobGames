#region Header
//Created by Mustafa Akgul (CL)
//mustafaakgul248@gmail.com
#endregion

using UnityEngine;

namespace CLUtils
{
    public class TargetFollower : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Transform targetTransform;

        void LateUpdate()
        {
            if (!targetTransform)
            {
                return;
            }

            transform.position = targetTransform.position;
        }

    } // class
} // namespace

