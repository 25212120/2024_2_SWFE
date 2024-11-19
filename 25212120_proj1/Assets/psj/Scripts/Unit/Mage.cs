using System.Collections;
using UnityEngine;

public class Mage : Unit
{
    private GameObject spellPrefab;

    protected override void Start()
    {
        LoadSpellPrefab("Prefabs/Unit/MagicNovaBlue");
        base.Start();
    }

    protected override void InitializeUnitParameters()
    {
        detectionRange = 18f;
        attackRange = 8f;
        attackSpeed = 0.6f;
    }

    protected override void PerformAttack()
    {
        animator.SetTrigger("attack");
        if (targetEnemy != null)
        {
            if (spellPrefab != null)
            {
                StartCoroutine(InstantiateSpell());
            }
        }
    }
    
    private void LoadSpellPrefab(string prefabAddress)
    {
        spellPrefab = Resources.Load<GameObject>(prefabAddress);
    }

    private IEnumerator InstantiateSpell()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject instantiatedSpell = Instantiate(spellPrefab, targetEnemy.position, Quaternion.Euler(-90, 0, 0));

        yield return new WaitForSeconds(2f);

        Destroy(instantiatedSpell);
    }
}
