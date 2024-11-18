using UnityEngine;

public class Warrior : Unit
{
    protected override void InitializeUnitParameters()
    {
        detectionRange = 10f;
        attackRange = 2f;
        attackSpeed = 1.2f;
    }

    protected override void PerformAttack()
    {
        if (targetEnemy != null)
        {
            animator.SetTrigger("attack");
            Debug.Log($"Warrior {gameObject.name} attacks {targetEnemy.name}!");
        }
    }
}