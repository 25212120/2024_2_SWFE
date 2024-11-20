using System.Collections;
using UnityEngine;

public class Enemy_Mage : Enemy
{
    private GameObject spellPrefab;


    protected override void Start()
    {
        LoadSpellPrefab("Prefabs/Enemy/MagicNovaGreen");
        base.Start();
    }

    protected override void InitializeEnemyParameters()
    {
        detectionRange = 18f;
        attackRange = 8f;
        attackSpeed = 0.6f;
    }

    protected override void PerformAttack()
    {
        if (Vector3.Distance(transform.position, target.transform.position) <= attackRange)
        {
            animator.SetTrigger("attack");
            StartCoroutine(InstantiateSpell());
        }
    }

    private void LoadSpellPrefab(string prefabAddress)
    {
        spellPrefab = Resources.Load<GameObject>(prefabAddress);
    }

    private IEnumerator InstantiateSpell()
    {
        yield return new WaitForSeconds(0.5f);

        if (target != null)
        {
            Vector3 novaPoint = target.position;
            novaPoint.y = 0.2f;
            GameObject instantiatedSpell = Instantiate(spellPrefab, novaPoint, Quaternion.Euler(-90, 0, 0));

            yield return new WaitForSeconds(2f);

            Destroy(instantiatedSpell);
        }
    }

}
