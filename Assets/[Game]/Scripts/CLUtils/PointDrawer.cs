#region Header
//Created by Mustafa Akgul (CL)
//mustafaakgul248@gmail.com
#endregion

using UnityEngine;

namespace CLUtils
{
    public class PointDrawer : MonoBehaviour
    {
        [Header("General Variables")]
        [SerializeField] Color gizmosColor = Color.blue;
        [SerializeField] float range = 0.5f;
        [SerializeField] bool isWired = false;

        void OnDrawGizmos()
        {
            Gizmos.color = gizmosColor;

            if (isWired)
            {
                Gizmos.DrawWireSphere(transform.position, range);
            }
            else
            {
                Gizmos.DrawSphere(transform.position, range);
            }
        }

    } // class
} // namespace

