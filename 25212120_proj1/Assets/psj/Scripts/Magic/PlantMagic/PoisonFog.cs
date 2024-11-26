using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonFog : MonoBehaviour
{
    public PlayerStat playerStat;

    private HashSet<GameObject> affectedObjects = new HashSet<GameObject>(); 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) 
        {
            if (!affectedObjects.Contains(other.gameObject))
            {
                affectedObjects.Add(other.gameObject);
                StartCoroutine(ApplyDamage(other.gameObject));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (affectedObjects.Contains(other.gameObject))
            {
                affectedObjects.Remove(other.gameObject);
            }
        }
    }

    private IEnumerator ApplyDamage(GameObject target)
    {
        while (affectedObjects.Contains(target))
        {
            if (target == null || !target) 
            {
                affectedObjects.Remove(target); 
                yield break; 
            }

            BaseMonster enemy = target.GetComponent<BaseMonster>();
            if (enemy == null) 
            {
                affectedObjects.Remove(target); 
                yield break;
            }

            playerStat.MagicAttack(enemy, 0);

            yield return new WaitForSeconds(1f); 
        }
    }
}
