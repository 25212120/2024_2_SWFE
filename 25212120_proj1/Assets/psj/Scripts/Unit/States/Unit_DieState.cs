using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Unit_DieState : BaseState<UnitStateType>
{
    private MonoBehaviour monoBeahviour;
    private Animator animator;
    private Unit unit;

    public Unit_DieState(UnitStateType key, StateManager<UnitStateType> stateManager, Animator animator, MonoBehaviour monoBehaviour, Unit unit) : base(key, stateManager)
    {
        this.unit = unit;
        this.monoBeahviour = monoBehaviour;
        this.animator = animator;
    }

    public override void EnterState()
    {
        animator.SetTrigger("dead");
        unit.gameObject.layer = LayerMask.NameToLayer("Dead");
        monoBeahviour.StartCoroutine(Die());
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
    }

    private IEnumerator Die()
    {
        GameManager.instance.RemoveUnit(unit.gameObject);
        unit.gameObject.GetComponent<Collider>().enabled = false;
        unit.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        yield return new WaitForSeconds(2.5f);
        UnityEngine.Object.Destroy(unit.gameObject);
    }
}