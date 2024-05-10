using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    [SerializeField] private float _rayLength = 1f;
    [SerializeField, Range(0, 0.5f)] private float _raycastDelay;
    [SerializeField] private LayerMask _selectableLayer;

    private readonly Dictionary<GameObject, ISelectable> _selectableDictionary = new();
    private ISelectable _previousSelectable;
    private ISelectable _currentSelectable;
    private CancellationTokenSource _cts = new();

    private void OnDisable()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    private void Awake()
    {
        var selectables = FindObjectsOfType<SelectableObject>();

        foreach (var selectableComponent in selectables)
            if (selectableComponent is MonoBehaviour monoBehaviour)
                _selectableDictionary[monoBehaviour.gameObject] = selectableComponent;
    }

    private void Start() => Select().Forget();

    private async UniTaskVoid Select()
    {
        int delay = (int)(1000 * _raycastDelay);
        var token = _cts.Token;

        while (true)
        {
            Ray ray = new(transform.position, transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, _rayLength, _selectableLayer))
            {
                _currentSelectable = _selectableDictionary[hit.collider.gameObject];

                if (_currentSelectable != null)
                {
                    _currentSelectable.IsSelected = true;

                    if (_previousSelectable != null && _currentSelectable != _previousSelectable)
                        _previousSelectable.IsSelected = false;

                    _previousSelectable = _currentSelectable;
                }
                else if (_currentSelectable == null && _previousSelectable != null)
                {
                    _previousSelectable.IsSelected = false;
                }
            }
            else if (_previousSelectable != null)
            {
                _previousSelectable.IsSelected = false;
            }

            await UniTask.Delay(delay, cancellationToken: token);
        }
    }
}
