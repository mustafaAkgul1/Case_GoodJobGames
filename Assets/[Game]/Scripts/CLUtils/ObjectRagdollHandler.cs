#region Header
//Created by Mustafa Akgul (CL)
//mustafaakgul248@gmail.com
#endregion

using System.Collections.Generic;
using UnityEngine;

namespace CLUtils
{
    public class ObjectRagdollHandler : BaseRagdollHandler
    {
        [Header("General Variables")]
        [SerializeField] LayerMask activeRigPartMask;
        [SerializeField] LayerMask passiveRigPartMask;
        [SerializeField] float explosionForce = 10f;
        [SerializeField] float explosionUpModifier = 0.25f;

        [Header("References")]
        [SerializeField] Transform rootTransform;
        [SerializeField] Animator animator;
        List<Rigidbody> rigRbs;
        List<Collider> rigColliders;

        [Space(10)]
        [Header("! Debug !")]
        [SerializeField] RagdollStates ragdollState;

        enum RagdollStates
        {
            Closed,
            Opened
        }

        void Awake()
        {
            InitVariables();
        }

        void InitVariables()
        {
            rigRbs = new List<Rigidbody>();
            rigColliders = new List<Collider>();

            foreach (Rigidbody _rigRb in rootTransform.GetComponentsInChildren<Rigidbody>())
            {
                rigRbs.Add(_rigRb);

                Collider _rigCollider = _rigRb.GetComponent<Collider>();
                rigColliders.Add(_rigCollider);
            }

            CloseRagdoll();
        }

        public override void OpenRagdoll(bool _withForce = true)
        {
            if (ragdollState == RagdollStates.Opened)
            {
                return;
            }

            ragdollState = RagdollStates.Opened;

            if (animator)
            {
                animator.enabled = false;
            }

            for (int i = 0; i < rigRbs.Count; i++)
            {
                rigRbs[i].isKinematic = false;
                rigColliders[i].isTrigger = false;

                rigRbs[i].gameObject.layer = (int)Mathf.Log(passiveRigPartMask, 2);
            }

            if (!_withForce)
            {
                return;
            }

            for (int i = 0; i < rigRbs.Count; i++)
            {
                rigRbs[i].AddExplosionForce(explosionForce, rootTransform.position, 10f, explosionUpModifier, ForceMode.Impulse);
            }
        }

        public override void CloseRagdoll()
        {
            if (animator)
            {
                animator.enabled = true;
            }

            for (int i = 0; i < rigRbs.Count; i++)
            {
                rigRbs[i].isKinematic = true;
                rigColliders[i].isTrigger = true;

                rigRbs[i].gameObject.layer = (int)Mathf.Log(activeRigPartMask, 2);
            }
        }

    } // class
} // namespace

