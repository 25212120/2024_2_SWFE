using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{

    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();

    protected Stack<BaseState<EState>> stateStack = new Stack<BaseState<EState>>();
    protected BaseState<EState> CurrentState
    {
        get
        {
            if (stateStack.Count > 0)
            {
                return stateStack.Peek();
            }
            return null;
        }
    }

    protected virtual void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.UpdateState(); 
            CurrentState.CheckTransitions();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (CurrentState != null)   CurrentState.FixedUpdateState();

    }

    public void PushState(EState stateKey)
    {
        if (CurrentState == States[stateKey]) return;
        if (CurrentState != null)   CurrentState.ExitState();

        if (States.ContainsKey(stateKey))
        {
            stateStack.Push(States[stateKey]);
            CurrentState.EnterState();
        }
    }

    public void PopState()
    {
        if (CurrentState != null)
        {
            CurrentState.ExitState();
            stateStack.Pop();

            if (CurrentState != null)   CurrentState.EnterState();
        }
    }

    public void ChangeState(EState stateKey)
    {
        PopState();
        PushState(stateKey);
    }
}
