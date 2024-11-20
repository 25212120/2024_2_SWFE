using UnityEngine;

public class Warrior : Unit
{
    protected override void InitializeUnitParameters()
    {
        detectionRange = 10f;
        attackRange = 1.5f;
        attackSpeed = 1.2f;
    }

    protected override void PerformAttack()
    {
        if (targetEnemy != null)
        {
            animator.SetTrigger("attack");
        }
    }
}