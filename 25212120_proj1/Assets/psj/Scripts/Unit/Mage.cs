using UnityEngine;

public class Mage : Unit
{
    //[SerializeField] private GameObject spellPrefab;

    protected override void InitializeUnitParameters()
    {
        detectionRange = 18f;
        attackRange = 8f;
        attackSpeed = 0.6f;
    }

    protected override void PerformAttack()
    {
        animator.SetTrigger("attack");
        Debug.Log("Mage Attacked");
        /*if (targetEnemy != null)
        {
            if (spellPrefab != null)
            {
                GameObject spell = Instantiate(spellPrefab,
                    targetEnemy.position,
                    Quaternion.identity);
            }
        }
        */
    }
}