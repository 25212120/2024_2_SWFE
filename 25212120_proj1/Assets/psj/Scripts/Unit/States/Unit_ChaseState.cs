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

    private float sqrVelocity;

    public override void EnterState()
    {
        if (unit.targetEnemy == null)
        {
            unit.DetectEnemy();
        }

        animator.SetBool("move", true);

        unit.agent.isStopped = false;
        unit.canDetectEnemy = false;
    }

    public override void UpdateState()
    {

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
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (unit.agent.remainingDistance <= unit.agent.stoppingDistance)
        {
            stateManager.ChangeState(UnitStateType.Idle);
        }

        if (unit.targetEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(unit.transform.position, unit.targetEnemy.position);
            if (distanceToEnemy <= unit.attackRange)
            {
                stateManager.ChangeState(UnitStateType.Attack);
            }

            //너무 멀리 떨어진 경우
            float distanceFromSavedPosition = Vector3.Distance(unit.transform.position, unit.savedPosition);
            if (distanceFromSavedPosition > unit.maxDistanceFromSavedPosition)
            {
                stateManager.ChangeState(UnitStateType.Return);
            }
        }
        else
        {
            unit.DetectEnemy();
            //enemy가 없는 경우
            if (unit.targetEnemy == null) stateManager.ChangeState(UnitStateType.Return);
        }
    }

}
