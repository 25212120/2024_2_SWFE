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
        animator.SetBool("move", true);

        unit.agent.isStopped = false;
        unit.canDetectEnemy = false;
    }

    public override void UpdateState()
    {
        unit.DetectEnemy();

        if (unit.targetEnemy == null)
        {
            stateManager.ChangeState(UnitStateType.Idle);
            return;
        }
        else 
        {
            unit.agent.SetDestination(unit.targetEnemy.position);
        }

        Vector3 velocity = unit.agent.desiredVelocity;
        velocity.y = 0;

        if (velocity.sqrMagnitude > 0.01f)
        {
            float rotationSpeed = 1000f;
            unit.transform.rotation = Quaternion.RotateTowards(
                unit.transform.rotation,
                Quaternion.LookRotation(velocity),
                rotationSpeed * Time.deltaTime
            );
            unit.transform.position += velocity * Time.deltaTime;
            unit.agent.nextPosition = unit.transform.position;
        }


    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        animator.SetBool("move", false);
    }

    public override void CheckTransitions()
    {

        if (unit.agent.remainingDistance <= unit.agent.stoppingDistance)
        {
            // 목표 지점에 도달한 경우 처리
            unit.agent.isStopped = true;
            stateManager.ChangeState(UnitStateType.Idle); // Idle 상태로 전환
        }


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
