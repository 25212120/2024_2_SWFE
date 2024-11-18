using UnityEngine;

public class Unit_ReturnState : BaseState<UnitStateType>
{
    private Transform unitTransform;
    private UnitController controller;
    private MonoBehaviour monoBeahviour;
    private Unit unit;
    private Animator animator;
    private Rigidbody rb;

    public Unit_ReturnState(UnitStateType key, StateManager<UnitStateType> stateManager, Transform unitTransform, UnitController controller, Animator animator, Rigidbody rb, MonoBehaviour monoBehaviour, Unit unit) : base(key, stateManager)
    {
        this.controller = controller;
        this.monoBeahviour = monoBehaviour;
        this.unitTransform = unitTransform;
        this.animator = animator;
        this.unit = unit;
        this.rb = rb;
    }

    public override void EnterState()
    {
        unit.agent.isStopped = false;
        unit.canDetectEnemy = false;
        unit.targetEnemy = null;
        unit.agent.SetDestination(unit.savedPosition);
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
    }

    public override void CheckTransitions()
    {
        float distanceToSavedPosition = Vector3.Distance(unit.transform.position, unit.savedPosition);

        if (distanceToSavedPosition <= unit.agent.stoppingDistance)
        {
            stateManager.ChangeState(UnitStateType.Idle);
        }
        else if (distanceToSavedPosition <= 5f) 
        {
            unit.canDetectEnemy = true;
        }
    }

}