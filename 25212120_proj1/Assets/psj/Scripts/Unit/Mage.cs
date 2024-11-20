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
        Vector3 novaPoint = targetEnemy.position;
        novaPoint.y = 0.3f;
        yield return new WaitForSeconds(0.5f);
        GameObject instantiatedSpell = Instantiate(spellPrefab, novaPoint, Quaternion.Euler(-90, 0, 0));

        yield return new WaitForSeconds(2f);

        Destroy(instantiatedSpell);
    }
}
