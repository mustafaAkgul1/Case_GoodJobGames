#region Header
//Created by Mustafa Akgul (CL)
//mustafaakgul248@gmail.com
#endregion

using UnityEngine;

namespace CLUtils
{
    public abstract class BaseRagdollHandler : MonoBehaviour
    {
        public abstract void OpenRagdoll(bool _withForce = true);

        public abstract void CloseRagdoll();

    } // class
} // namespace

