using System.Collections;
using UnityEngine;

public class Enemy_DieState : BaseState<EnemyStateType>
{
    private Transform enemyTransform;
    private MonoBehaviour monoBeahviour;
    private Animator animator;
    private Monster_1 enemyStat;
    private Rigidbody rb;

    public Enemy_DieState(EnemyStateType key, StateManager<EnemyStateType> stateManager, Animator animator, MonoBehaviour monoBehaviour, Monster_1 enemyStat) : base(key, stateManager)
    {
        this.enemyStat = enemyStat;
        this.monoBeahviour = monoBehaviour;
        this.animator = animator;
    }

    public override void EnterState()
    {
        animator.SetTrigger("dead");
        enemyStat.gameObject.layer = LayerMask.NameToLayer("Dead");
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
        enemyStat.gameObject.GetComponent<Collider>().enabled = false;
        enemyStat.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        yield return new WaitForSeconds(2.5f);
        UnityEngine.Object.Destroy(enemyStat.gameObject);
    }
}