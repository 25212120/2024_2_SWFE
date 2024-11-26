using System.Collections;
using UnityEngine;

public class ExplosionFireballFire : MonoBehaviour
{
    public PlayerStat playerStat;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(DestroyGameObj());

        BaseMonster enemy = other.GetComponent<BaseMonster>();
        if (enemy != null)
        {
            playerStat.MagicAttack(enemy, 1);
        }

    }

    private IEnumerator DestroyGameObj()
    {
        yield return new WaitForSeconds(0.3f);
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }
}
