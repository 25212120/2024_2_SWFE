using UnityEngine;

public class Unit_AttackState : BaseState<UnitStateType>
{
    private Transform unitTransform;
    private UnitController controller;
    private MonoBehaviour monoBeahviour;
    private Unit unit;
    private Animator animator;
    private Rigidbody rb;

    public Unit_AttackState(UnitStateType key, StateManager<UnitStateType> stateManager, Transform unitTransform, UnitController controller, Animator animator, Rigidbody rb, MonoBehaviour monoBehaviour, Unit unit) : base(key, stateManager)
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
        unit.agent.isStopped = true;
        unit.canDetectEnemy = false;

        if (unit.targetEnemy != null && Vector3.Distance(unit.transform.position, unit.targetEnemy.position) <= unit.attackRange)
        {
             unit.Attack();
        }
    }

    public override void UpdateState()
    {
        if (unit.targetEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(unit.transform.position, unit.targetEnemy.position);

            if (distanceToEnemy > unit.attackRange)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsTag("Attack") && stateInfo.normalizedTime >= 0.8f)
                {
                    unit.agent.isStopped = false; // 이동 가능하도록 설정
                    stateManager.ChangeState(UnitStateType.Chase);
                }
            }
            else
            {
                Vector3 direction = (unit.targetEnemy.position - unit.transform.position).normalized;
                direction.y = 0;
                unit.transform.rotation = Quaternion.RotateTowards(unit.transform.rotation, Quaternion.LookRotation(direction), 1440f * Time.deltaTime);
                unit.Attack();
            }
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        animator.ResetTrigger("attack");
    }

    public override void CheckTransitions()
    {
        if (unit.targetEnemy == null)
        {
            stateManager.ChangeState(UnitStateType.Idle);
            return;
        }

        float distanceFromSavedPosition = Vector3.Distance(unit.transform.position, unit.savedPosition);
        if (distanceFromSavedPosition > unit.maxDistanceFromSavedPosition)
        {
            unit.agent.isStopped = false; // 이동 가능하도록 설정
            stateManager.ChangeState(UnitStateType.Return);
        }
    }

}