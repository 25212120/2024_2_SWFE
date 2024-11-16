using UnityEngine;

public class Unit_ChaseState : BaseState<UnitStateType>
{
    private Transform unitTransform;
    private UnitController controller;
    private MonoBehaviour monoBeahviour;
    private Unit unit;
    private Animator animator;
    private Rigidbody rb;

    public Unit_ChaseState(UnitStateType key, StateManager<UnitStateType> stateManager, Transform unitTransform, UnitController controller, Animator animator, Rigidbody rb, MonoBehaviour monoBehaviour, Unit unit) : base(key, stateManager)
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
        if (unit.targetEnemy == null)
        {
            stateManager.ChangeState(UnitStateType.Idle);
            return;
        }

        unit.agent.SetDestination(unit.targetEnemy.position);

        float distanceToEnemy = Vector3.Distance(unit.transform.position, unit.targetEnemy.position);
        if (distanceToEnemy <= unit.attackRange)
        {
            stateManager.ChangeState(UnitStateType.Attack);
        }

        float distanceFromSavedPosition = Vector3.Distance(unit.transform.position, unit.savedPosition);
        if (distanceFromSavedPosition > unit.maxDistanceFromSavedPosition)
        {
            stateManager.ChangeState(UnitStateType.Return);
        }
    }

}