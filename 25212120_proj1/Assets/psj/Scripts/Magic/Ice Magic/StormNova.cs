using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormNova : MonoBehaviour
{
    public PlayerStat playerStat;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(DestroyGameObj());

        BaseMonster enemy = other.GetComponent<BaseMonster>();
        if (enemy != null)
        {
            playerStat.MagicAttack(enemy, 2);
        }

    }

    private IEnumerator DestroyGameObj()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }
}
