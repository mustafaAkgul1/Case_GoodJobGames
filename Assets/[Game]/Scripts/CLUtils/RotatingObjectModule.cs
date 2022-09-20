#region Header
//Created by Mustafa Akgul (CL)
//mustafaakgul248@gmail.com
#endregion

using UnityEngine;

namespace CLUtils
{
    public class RotatingObjectModule : MonoBehaviour
    {
        [Header("General Variables")]
        [SerializeField] Vector3 rotationSpeed;

        void Update()
        {
            transform.Rotate(Time.deltaTime * rotationSpeed);
        }

    } // class
} // namespace

