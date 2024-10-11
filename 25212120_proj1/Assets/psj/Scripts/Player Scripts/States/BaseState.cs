using System;

public abstract class BaseState<EState> where EState : Enum
{
    protected StateManager<EState> stateManager;
    public BaseState(EState key, StateManager<EState> stateManager)
    {
        StateKey = key;
        this.stateManager = stateManager;
    }

    public EState StateKey { get; private set; }

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void CheckTransitions();
}