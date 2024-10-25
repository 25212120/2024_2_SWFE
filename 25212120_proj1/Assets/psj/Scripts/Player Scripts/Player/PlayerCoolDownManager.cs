using System.Collections;
using UnityEngine;

public class PlayerCoolDownManager : MonoBehaviour
{
    private bool isDashOnCoolTime = false;
    private float dashCoolTime = 0.7f;

    public bool CanDash()
    {
        if (!isDashOnCoolTime)
        {
            StartCoroutine(StartDashCoolTime());
            return true;
        }
        return false;
    }

    private IEnumerator StartDashCoolTime()
    {
        isDashOnCoolTime = true;
        yield return new WaitForSeconds(dashCoolTime);
        isDashOnCoolTime = false;
    }
}
