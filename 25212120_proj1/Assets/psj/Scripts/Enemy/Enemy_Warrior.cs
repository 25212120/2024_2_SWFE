using UnityEngine;

public class Enemy_Warrior : Enemy
{
    protected override void InitializeEnemyParameters()
    {
        detectionRange = 10f;
        attackRange = 1.5f;
        attackSpeed = 1.2f;
    }

    protected override void PerformAttack()
    {
        if (target != null)
        {
            animator.SetTrigger("attack");
        }
    }

}