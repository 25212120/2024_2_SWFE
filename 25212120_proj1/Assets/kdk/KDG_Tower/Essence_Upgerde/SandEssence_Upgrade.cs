using UnityEngine;

public class SandEssence_Upgrade : MonoBehaviour
{
    public float slowAmount = 0.5f;  

    private void OnTriggerEnter(Collider other)
    {
        BaseMonster targetMonster = other.GetComponent<BaseMonster>();

        if (targetMonster != null)
        {
            BuffController buffController = targetMonster.GetComponent<BuffController>();

            if (buffController != null)
            {
                // ���ο� ���� ���� (�̵��ӵ� ����)
                Buff slowBuff = new Buff(0f, -slowAmount, 0f); 
                buffController.ApplyBuff(slowBuff);

                StartCoroutine(RemoveBuffAfterDelay(buffController, slowBuff, 3f));  // 5�� �Ŀ� �ӵ� ����
            }
        }
    }

    private System.Collections.IEnumerator RemoveBuffAfterDelay(BuffController buffController, Buff slowBuff, float delay)
    {
        yield return new WaitForSeconds(delay);
        buffController.RemoveBuff(slowBuff);
    }
}
