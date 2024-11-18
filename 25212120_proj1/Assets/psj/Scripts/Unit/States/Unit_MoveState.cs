using JetBrains.Annotations;
using UnityEngine;

public class Unit_MoveState : BaseState<UnitStateType>
{
    private Transform unitTransform;
    private UnitController controller;
    private MonoBehaviour monoBeahviour;
    private Unit unit;
    private Animator animator;
    private Rigidbody rb;

    public Unit_MoveState(UnitStateType key, StateManager<UnitStateType> stateManager, Transform unitTransform, UnitController controller, Animator animator, Rigidbody rb, MonoBehaviour monoBehaviour, Unit unit) : base(key, stateManager)
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

        unit.isMove = true;
        unit.agent.isStopped = false;
        unit.canDetectEnemy = false;
        unit.targetEnemy = null;
    }

    public override void UpdateState()
    {
        Vector3 velocity = unit.agent.desiredVelocity;
        velocity.y = 0; // Y�� ����

        // ��ǥ ������ ���� ������ ȸ��
        if (velocity.sqrMagnitude > 0.01f) // �̵� ���� ��쿡�� ȸ��
        {
            float rotationSpeed = 1000f; // ���� ȸ�� �ӵ�
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
        unit.isMove = false;
    }

    public override void CheckTransitions()
    {
        if (!unit.agent.pathPending && unit.agent.remainingDistance <= unit.agent.stoppingDistance)
        {
            unit.agent.isStopped = true;
            unit.savedPosition = unit.transform.position;
            stateManager.ChangeState(UnitStateType.Idle);
        }
    }

}