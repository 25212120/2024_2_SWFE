using System.Collections;
using UnityEngine;

public class PlayerCoolDownManager : MonoBehaviour
{
    private bool isDashOnCoolTime = false;
    private bool isSwordShieldWeaponSkillOnCoolTime = false;
    private bool isSingleTwoHandSwordWeaponSkillOnCoolTime = false;
    private bool isDoubleSwordsWeaponSkillOnCoolTime = false;
    private bool isBowWeaponSkillOnCoolTime = false;

    private bool isFireBallOnCoolTime = false;
    private bool isMeteorOnCoolTime = false;
    private bool isPoisonFogOnCoolTime = false;
    private bool isDrainFieldOnCoolTime = false;
    private bool isIceSpearOnCoolTime = false;
    private bool isStormOnCoolTime = false;
    private bool isRockFallOnCoolTime = false;
    private bool isEarthQuakeOnCoolTime = false;

    private float dashCoolTime_SwordShield = 1f;
    private float dashCoolTime_SingleTwoHandSword = 1.3f;
    private float dashCoolTime_DoubleSwords = 0.7f;
    private float dashCoolTime_Bow = 1.8f;

    // 스킬 시전되는 동안의 시간 + 맞추고 싶은 쿨타임
    private float weaponSkillCoolTime_SwordShield = 6f;
    private float weaponSkillCoolTime_SingleTwoHandSword = 6f;
    private float weaponSkillCoolTime_DoubleSwords = 3f;
    private float weaponSkillCoolTime_Bow = 12f;

    private float magicCoolTime_FireBall = 5f;
    private float magicCoolTime_Meteor = 10f;
    private float magicCoolTime_PoisonFog = 5f;
    private float magicCoolTime_DrainField = 10f;
    private float magicCoolTime_IceSpear = 5f;
    private float magicCoolTime_Storm = 10f;
    private float magicCoolTime_RockFall = 5f;
    private float magicCoolTime_EarthQuake = 10f;

    public bool CanDash(int weaponIndex)
    {
        if (!isDashOnCoolTime)
        {
            StartCoroutine(StartDashCoolTime(weaponIndex));
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
                if (!isSingleTwoHandSwordWeaponSkillOnCoolTime)
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

    public bool CanUseMagic(PlayerStateType magic)
    {
        switch (magic)
        {
            case PlayerStateType.FireBall_MagicState:
                if (!isFireBallOnCoolTime)
                {
                    StartCoroutine(StartMagicCoolTime(magic));
                    return true;
                }
                return false;
            case PlayerStateType.Meteor_MagicState:
                if (!isMeteorOnCoolTime)
                {
                    StartCoroutine(StartMagicCoolTime(magic));
                    return true;
                }
                return false;
            case PlayerStateType.PoisonFog_MagicState:
                if (!isPoisonFogOnCoolTime)
                {
                    StartCoroutine(StartMagicCoolTime(magic));
                    return true;
                }
                return false;
            case PlayerStateType.DrainField_MagicState:
                if (!isDrainFieldOnCoolTime)
                {
                    StartCoroutine(StartMagicCoolTime(magic));
                    return true;
                }
                return false;
            case PlayerStateType.IceSpear_MagicState:
                if (!isIceSpearOnCoolTime)
                {
                    StartCoroutine(StartMagicCoolTime(magic));
                    return true;
                }
                return false;
            case PlayerStateType.Storm_MagicState:
                if (!isStormOnCoolTime)
                {
                    StartCoroutine(StartMagicCoolTime(magic));
                    return true;
                }
                return false;
            case PlayerStateType.RockFall_MagicState:
                if (!isRockFallOnCoolTime)
                {
                    StartCoroutine(StartMagicCoolTime(magic));
                    return true;
                }
                return false;
            case PlayerStateType.EarthQuake_MagicState:
                if (!isEarthQuakeOnCoolTime)
                {
                    StartCoroutine(StartMagicCoolTime(magic));
                    return true;
                }
                return false;
            default:
                return false;
        }
    }

    private IEnumerator StartDashCoolTime(int weaponIndex)
    {
        isDashOnCoolTime = true;

        float selectedDashCoolTime = 0f;

        switch (weaponIndex)
        {
            case 0:
                selectedDashCoolTime = dashCoolTime_SwordShield;
                break;
            case 1:
                selectedDashCoolTime = dashCoolTime_SingleTwoHandSword;
                break;
            case 2:
                selectedDashCoolTime = dashCoolTime_DoubleSwords;
                break;
            case 3:
                selectedDashCoolTime = dashCoolTime_Bow;
                break;
        }

        yield return new WaitForSeconds(selectedDashCoolTime);
        isDashOnCoolTime = false;
    }

    private IEnumerator StartWeaponSkillCoolTime(int weaponIndex)
    {
        switch (weaponIndex)
        {
            case 0:
                isSwordShieldWeaponSkillOnCoolTime = true;
                yield return new WaitForSeconds(weaponSkillCoolTime_SwordShield);
                isSwordShieldWeaponSkillOnCoolTime = false;
                break;
            case 1:
                isSingleTwoHandSwordWeaponSkillOnCoolTime = true;
                yield return new WaitForSeconds(weaponSkillCoolTime_SingleTwoHandSword);
                isSingleTwoHandSwordWeaponSkillOnCoolTime = false;
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

    private IEnumerator StartMagicCoolTime(PlayerStateType magicType)
    {
        switch (magicType)
        {
            case PlayerStateType.FireBall_MagicState:
                isFireBallOnCoolTime = true;
                yield return new WaitForSeconds(magicCoolTime_FireBall);
                isFireBallOnCoolTime = false;
                break;
            case PlayerStateType.Meteor_MagicState:
                isMeteorOnCoolTime = true;
                yield return new WaitForSeconds(magicCoolTime_Meteor);
                isMeteorOnCoolTime = false;
                break;
            case PlayerStateType.PoisonFog_MagicState:
                isPoisonFogOnCoolTime = true;
                yield return new WaitForSeconds(magicCoolTime_PoisonFog);
                isPoisonFogOnCoolTime = false;
                break;
            case PlayerStateType.DrainField_MagicState:
                isDrainFieldOnCoolTime = true;
                yield return new WaitForSeconds(magicCoolTime_DrainField);
                isDrainFieldOnCoolTime = false;
                break;
            case PlayerStateType.IceSpear_MagicState:
                isIceSpearOnCoolTime = true;
                yield return new WaitForSeconds(magicCoolTime_IceSpear);
                isIceSpearOnCoolTime = false;
                break;
            case PlayerStateType.Storm_MagicState:
                isStormOnCoolTime = true;
                yield return new WaitForSeconds(magicCoolTime_Storm);
                isStormOnCoolTime = false;
                break;
            case PlayerStateType.RockFall_MagicState:
                isRockFallOnCoolTime = true;
                yield return new WaitForSeconds(magicCoolTime_RockFall);
                isRockFallOnCoolTime = false;
                break;
            case PlayerStateType.EarthQuake_MagicState:
                isEarthQuakeOnCoolTime = true;
                yield return new WaitForSeconds(magicCoolTime_EarthQuake);
                isEarthQuakeOnCoolTime = false;
                break;
        }
    }
}
