using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UnitStateMachine : StateManager<UnitStateType>
{
    private Unit unit;
    private Transform unitTransform;
    private UnitController controller;
    private Animator animator;
    private Rigidbody rb;
    private MonoBehaviour monoBehaviour;

    private void Awake()
    {
        unitTransform = GetComponent<Transform>();
        controller = FindAnyObjectByType<UnitController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        monoBehaviour = GetComponent<MonoBehaviour>();
        unit = GetComponent<Unit>();


        States.Add(UnitStateType.Idle, new Unit_IdleState(UnitStateType.Idle, this, unitTransform, controller, animator, rb, monoBehaviour, unit));
        States.Add(UnitStateType.Move, new Unit_MoveState(UnitStateType.Move, this, unitTransform, controller, animator, rb, monoBehaviour, unit));
        States.Add(UnitStateType.Attack, new Unit_AttackState(UnitStateType.Attack, this, unitTransform, controller, animator, rb, monoBehaviour, unit));
        States.Add(UnitStateType.Return, new Unit_ReturnState(UnitStateType.Return, this, unitTransform, controller, animator, rb, monoBehaviour, unit));
        States.Add(UnitStateType.Chase, new Unit_ChaseState(UnitStateType.Chase, this, unitTransform, controller, animator, rb, monoBehaviour, unit));

    }



}
