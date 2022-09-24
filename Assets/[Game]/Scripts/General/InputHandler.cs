using UnityEngine;
using CLUtils;

public class InputHandler : Operator
{
    [Header("General Variables")]
    [SerializeField] LayerMask interactableMask;

    [Space(10)]
    [Header("! Debug !")]
    Camera cam;

    void Start()
    {
        InitVariables();
    }

    void Update()
    {
        HandleInput();
    }

    void InitVariables()
    {
        cam = Camera.main;
    }

    void HandleInput()
    {
        Ray _ray = new Ray();

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
	    {
            _ray = cam.ScreenPointToRay(Input.mousePosition);
	    }
#endif

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount <= 0)
        {
            return;
        }

        Touch _touch = Input.GetTouch(0);
        if (_touch.phase == TouchPhase.Began)
        {
            _ray = cam.ScreenPointToRay(_touch.position);
        }
#endif

        if (!Physics.Raycast(_ray, out RaycastHit _rHit, 100f, interactableMask))
        {
            return;
        }

        if (!_rHit.collider.TryGetComponent(out IClickable _iClickable))
        {
            return;
        }

        _iClickable.Clicked();
    }


} // class
