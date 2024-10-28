using System.Collections;
using UnityEngine;

public class PlayerCoolDownManager : MonoBehaviour
{
    private bool isDashOnCoolTime = false;
    private bool isSwordShieldWeaponSkillOnCoolTime = false;
    private bool iSingleTwoHandSwordWeaponSkillOnCoolTime = false;
    private bool isDoubleSwordsWeaponSkillOnCoolTime = false;
    private bool isBowWeaponSkillOnCoolTime = false;


    private float dashCoolTime = 0.7f;
    // 스킬 시전되는 동안의 시간 + 맞추고 싶은 쿨타임
    private float weaponSkillCoolTime_SwordShield = 6f;
    private float weaponSkillCoolTime_SingleTwoHandSword = 6f;
    private float weaponSkillCoolTime_DoubleSwords = 3f;
    private float weaponSkillCoolTime_Bow = 3f;

    public bool CanDash()
    {
        if (!isDashOnCoolTime)
        {
            StartCoroutine(StartDashCoolTime());
            return true;
        }
        return false;
    }

    public bool CanUseWeaponSkill(int weaponIndex)
    {
        switch (weaponIndex)
        {
            case 0:
                if (!isSwordShieldWeaponSkillOnCoolTime)
                {
                    StartCoroutine(StartWeaponSkillCoolTime(weaponIndex));
                    return true;
                }
                return false;
            case 1:
                if (!iSingleTwoHandSwordWeaponSkillOnCoolTime)
                {
                    StartCoroutine(StartWeaponSkillCoolTime(weaponIndex));
                    return true;
                }
                return false;
            case 2:
                if (!isDoubleSwordsWeaponSkillOnCoolTime)
                {
                    StartCoroutine(StartWeaponSkillCoolTime(weaponIndex));
                    return true;
                }
                return false;
            case 3:
                if (!isBowWeaponSkillOnCoolTime)
                {
                    StartCoroutine(StartWeaponSkillCoolTime(weaponIndex));
                    return true;
                }
                return false;
            default:
                return false;
        }
    }

    private IEnumerator StartDashCoolTime()
    {
        isDashOnCoolTime = true;
        yield return new WaitForSeconds(dashCoolTime);
        isDashOnCoolTime = false;
    }

    private IEnumerator StartWeaponSkillCoolTime(int WeaponIndex)
    {
        switch (WeaponIndex)
        {
            case 0:
                isSwordShieldWeaponSkillOnCoolTime = true;
                yield return new WaitForSeconds(weaponSkillCoolTime_SwordShield);
                isSwordShieldWeaponSkillOnCoolTime = false;
                break;
            case 1:
                iSingleTwoHandSwordWeaponSkillOnCoolTime = true;
                yield return new WaitForSeconds(weaponSkillCoolTime_SingleTwoHandSword);
                iSingleTwoHandSwordWeaponSkillOnCoolTime = false;
                break;
            case 2:
                isDoubleSwordsWeaponSkillOnCoolTime = true;
                yield return new WaitForSeconds(weaponSkillCoolTime_DoubleSwords);
                isDoubleSwordsWeaponSkillOnCoolTime = false;
                break;
            case 3:
                isBowWeaponSkillOnCoolTime = true;
                yield return new WaitForSeconds(weaponSkillCoolTime_Bow);
                isBowWeaponSkillOnCoolTime = false;
                break;
        }
    }
}
