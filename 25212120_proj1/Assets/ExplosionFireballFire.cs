using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionFireballFire : MonoBehaviour
{
    public PlayerStat playerStat;

    private void Start()
    {
        if (playerStat == null) Debug.Log("�÷��̾�� ���� ����");
    }

    private void OnTriggerEnter(Collider other)
    {
        BaseMonster enemy = other.GetComponent<BaseMonster>();
        if (enemy != null)
        {
            playerStat.MagicAttack(enemy, 1);
        }
    }
}
