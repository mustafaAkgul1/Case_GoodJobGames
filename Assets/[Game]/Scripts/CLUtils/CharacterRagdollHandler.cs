#region Header
//Created by Mustafa Akgul (CL)
//mustafaakgul248@gmail.com
#endregion

using System.Collections.Generic;
using UnityEngine;

namespace CLUtils
{
    public class CharacterRagdollHandler : BaseRagdollHandler
    {
        [Header("General Variables")]
        [SerializeField] LayerMask activeRigPartMask;
        [SerializeField] LayerMask passiveRigPartMask;
        [SerializeField] string activeRigPartTag = "Untagged";
        [SerializeField] string passiveRigPartTag = "Untagged";
        [SerializeField] float pelvisForceValue = 15f;
        [SerializeField] float headForceValue = 50f;
        [SerializeField] float pelvisUpForceValue = 50f;

        [Header("References")]
        [SerializeField] Transform pelvisTransform;
        [SerializeField] Animator animator;
        Rigidbody pelvisRb;
        Rigidbody headRb;
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

            pelvisRb = pelvisTransform.GetComponent<Rigidbody>();
            headRb = pelvisTransform.GetComponentInChildren<SphereCollider>().GetComponent<Rigidbody>();

            foreach (Rigidbody _rigRb in pelvisTransform.GetComponentsInChildren<Rigidbody>())
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

            animator.enabled = false;

            for (int i = 0; i < rigRbs.Count; i++)
            {
                rigRbs[i].isKinematic = false;
                rigColliders[i].isTrigger = false;

                rigRbs[i].gameObject.layer = (int)Mathf.Log(passiveRigPartMask, 2);
                rigRbs[i].gameObject.tag = passiveRigPartTag;
            }

            if (!_withForce)
            {
                return;
            }

            Vector3 _pelvisForce =
                (transform.forward * -Random.Range(pelvisForceValue * 0.3f, pelvisForceValue))
                + (Vector3.up * Random.Range(pelvisUpForceValue * 0.5f, pelvisUpForceValue));
            pelvisRb.AddForce(_pelvisForce, ForceMode.Impulse);

            Vector3 _headForce = transform.forward * (-Random.Range(headForceValue * 0.5f, headForceValue));
            headRb.AddForce(_headForce, ForceMode.Impulse);
        }

        public override void CloseRagdoll()
        {
            animator.enabled = true;

            for (int i = 0; i < rigRbs.Count; i++)
            {
                rigRbs[i].isKinematic = true;
                rigColliders[i].isTrigger = true;

                rigRbs[i].gameObject.layer = (int)Mathf.Log(activeRigPartMask, 2);
                rigRbs[i].gameObject.tag = activeRigPartTag;
            }
        }

    } // class
} // namespace

