#region Header
//Created by Mustafa Akgul (CL)
//mustafaakgul248@gmail.com
#endregion

using UnityEngine;

namespace CLUtils
{
    public class GroundChecker : MonoBehaviour
    {
        [Header("General Variables")]
        [SerializeField] float rayDistance = 0.3f;
        [SerializeField] float rayStartOffset = 0.2f;
        [SerializeField] LayerMask groundMask;
        [SerializeField] bool drawDebug = true;

        [Space(10)]
        [Header("! Debug !")]
        [SerializeField] bool isGrounded = true;

        void Update()
        {
            if (drawDebug)
            {
                Debug.DrawRay(transform.position + (Vector3.up * rayStartOffset), Vector3.down, Color.black);
            }

            isGrounded = Physics.Raycast(transform.position + (Vector3.up * rayStartOffset), Vector3.down, rayDistance, groundMask);
        }

        public bool IsGrounded()
        {
            return isGrounded;
        }

    } // class
} // namespace


