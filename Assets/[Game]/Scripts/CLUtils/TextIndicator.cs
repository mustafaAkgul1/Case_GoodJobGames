#region Header
//Created by Mustafa Akgul (CL)
//mustafaakgul248@gmail.com
#endregion

using UnityEngine;
using TMPro;
using DG.Tweening;

namespace CLUtils
{
    public class TextIndicator : MonoBehaviour, IPoolable
    {
        [Header("General Variables")]
        [SerializeField] float upwardSpeed;
        [SerializeField][Range(0.1f, 5f)] float fadeDuration = 0.75f;
        [SerializeField][Range(0f, 2f)] float fadeDelay = 0.5f;
        [SerializeField] Color[] colors;

        [Header("References")]
        [SerializeField] TextMeshProUGUI valueText;

        [Space(10)]
        [Header("! Debug !")]
        [SerializeField] ActivationStates activationState;
        Transform _transform;
        Transform camTransform;
        Transform poolHolder;

        enum ActivationStates
        {
            Passive,
            Active
        }

        void Awake()
        {
            _transform = transform;
            camTransform = Camera.main.transform;
        }

        void Update()
        {
            HandleFloating();
        }

        void HandleFloating()
        {
            if (activationState == ActivationStates.Passive)
            {
                return;
            }

            _transform.localPosition += upwardSpeed * Time.deltaTime * Vector3.up;
        }

        public void DisableFromPool(Transform _poolHolder)
        {
            activationState = ActivationStates.Passive;
            gameObject.SetActive(false);
            _transform.DOKill();

            poolHolder = _poolHolder;

            _transform.SetParent(_poolHolder);

            valueText.text = "99";
        }

        public void SpawnFromPool(params object[] _args)
        {
            string _value = (string)_args[0];
            Transform _parent = (Transform)_args[1];
            Vector3 _localPosition = (Vector3)_args[2];
            TextIndicatorTypes _textIndicatorType = (TextIndicatorTypes)_args[3];
            bool _useRandomizedPosition = (bool)_args[4];
            bool _lookAtCamera = (bool)_args[5];

            gameObject.SetActive(true);
            activationState = ActivationStates.Active;

            _transform.SetParent(_parent);
            _transform.localPosition = _useRandomizedPosition ? _localPosition + (Random.insideUnitSphere * Random.Range(-1f, 1f)) : _localPosition;
            _transform.forward = _lookAtCamera ? camTransform.forward : Vector3.forward;
            _transform.SetParent(poolHolder);

            switch (_textIndicatorType)
            {
                case TextIndicatorTypes.Normal:
                case TextIndicatorTypes.Alert:
                    valueText.text = _value;
                    break;
                case TextIndicatorTypes.Positive:
                    valueText.text = "+" + _value;
                    break;
                case TextIndicatorTypes.Negative:
                    valueText.text = "-" + _value;
                    break;
                default:
                    break;
            }

            valueText.color = colors[(int)_textIndicatorType];

            valueText.DOFade(0f, fadeDuration)
                .SetDelay(fadeDelay)
                .SetLink(gameObject)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    TextIndicatorPoolManager.Instance.AddToPool(this);
                });
        }

    } // class
} // namespace
