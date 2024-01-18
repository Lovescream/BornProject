using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachin : MonoBehaviour
{
public class State<T> where T : Enum
{

    public T Current
    {
        get => _current;
        set
        {
            if (_current.Equals(value)) return;
            _onExited[_current]?.Invoke();
            _current = value;
            _onEntered[_current]?.Invoke();
            _nextState = _current;
        }
    }

    private T _current;
    private T _nextState;
    private float _transitionTime;
    
    private readonly Dictionary<T, Action> _onEntered = new();
    private readonly Dictionary<T, Action> _onStay = new();
    private readonly Dictionary<T, Action> _onExited = new();

    public State()
    {
        foreach (T state in Enum.GetValues(typeof(T)))
        {
            _onEntered[state] = null;
            _onStay[state] = null;
            _onExited[state] = null;
        }
    }

    public void AddOnEntered(T state, Action cb)
    {
        if (!_onEntered.ContainsKey(state)) _onEntered[state] = default;
        _onEntered[state] += cb;
    }
    public void AddOnStay(T state, Action cb)
    {
        if (!_onStay.ContainsKey(state)) _onStay[state] = default;
        _onStay[state] += cb;
    }
    public void AddOnExited(T state, Action cb)
    {
        if (!_onExited.ContainsKey(state)) _onExited[state] = default;
        _onExited[state] += cb;
    }

    public void Clear()
    {
        _onEntered.Clear();
        _onStay.Clear();
        _onExited.Clear();
        _transitionTime = 0;
        _current = default;
        _nextState = default;
    }

    public void OnStay()
    {
        if (!_onStay.ContainsKey(Current)) _onStay[Current] = default;
        _onStay[Current]?.Invoke();

        if (_nextState.Equals(Current)) return;
        _transitionTime -= Time.deltaTime;
        if (_transitionTime <= 0)
        {
            _transitionTime = 0;
            Current = _nextState;
        }
    }
    public void SetStateAfterTime(T nextState, float time)
    {
        _nextState = nextState;
        _transitionTime = time;
    }
}
}
