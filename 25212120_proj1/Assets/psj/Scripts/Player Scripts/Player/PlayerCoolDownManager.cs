using System.Collections;
using UnityEngine;

public class PlayerCoolDownManager : MonoBehaviour
{
    public bool isDashOnCoolTime = false;
    public bool isSwordShieldWeaponSkillOnCoolTime = false;
    public bool isSingleTwoHandSwordWeaponSkillOnCoolTime = false;
    public bool isDoubleSwordsWeaponSkillOnCoolTime = false;
    public bool isBowWeaponSkillOnCoolTime = false;

    public bool isFireBallOnCoolTime = false;
    public bool isMeteorOnCoolTime = false;
    public bool isPoisonFogOnCoolTime = false;
    public bool isDrainFieldOnCoolTime = false;
    public bool isIceSpearOnCoolTime = false;
    public bool isStormOnCoolTime = false;
    public bool isRockFallOnCoolTime = false;
    public bool isEarthQuakeOnCoolTime = false;

    public float dashCoolTime_SwordShield = 1f;
    public float dashCoolTime_SingleTwoHandSword = 1.3f;
    public float dashCoolTime_DoubleSwords = 0.7f;
    public float dashCoolTime_Bow = 1.8f;

    // 스킬 시전되는 동안의 시간 + 맞추고 싶은 쿨타임
    public float weaponSkillCoolTime_SwordShield = 6f;
    public float weaponSkillCoolTime_SingleTwoHandSword = 6f;
    public float weaponSkillCoolTime_DoubleSwords = 3f;
    public float weaponSkillCoolTime_Bow = 12f;

    public float magicCoolTime_FireBall = 5f;
    public float magicCoolTime_Meteor = 10f;
    public float magicCoolTime_PoisonFog = 5f;
    public float magicCoolTime_DrainField = 10f;
    public float magicCoolTime_IceSpear = 5f;
    public float magicCoolTime_Storm = 10f;
    public float magicCoolTime_RockFall = 5f;
    public float magicCoolTime_EarthQuake = 10f;

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

    public float GetDashCoolTime(int weaponIndex)
    {
        return weaponIndex switch
        {
            0 => dashCoolTime_SwordShield,
            1 => dashCoolTime_SingleTwoHandSword,
            2 => dashCoolTime_DoubleSwords,
            3 => dashCoolTime_Bow,
            _ => 1f
        };
    }

    public float GetWeaponSkillCoolTime(int weaponIndex)
    {
        return weaponIndex switch
        {
            0 => weaponSkillCoolTime_SwordShield,
            1 => weaponSkillCoolTime_SingleTwoHandSword,
            2 => weaponSkillCoolTime_DoubleSwords,
            3 => weaponSkillCoolTime_Bow,
            _ => 1f
        };
    }

    public float GetMagicCoolTime(PlayerStateType magic)
    {
        return magic switch
        {
            PlayerStateType.FireBall_MagicState => magicCoolTime_FireBall,
            PlayerStateType.Meteor_MagicState => magicCoolTime_Meteor,
            PlayerStateType.PoisonFog_MagicState => magicCoolTime_PoisonFog,
            PlayerStateType.DrainField_MagicState => magicCoolTime_DrainField,
            PlayerStateType.IceSpear_MagicState => magicCoolTime_IceSpear,
            PlayerStateType.Storm_MagicState => magicCoolTime_Storm,
            PlayerStateType.RockFall_MagicState => magicCoolTime_RockFall,
            PlayerStateType.EarthQuake_MagicState => magicCoolTime_EarthQuake,
            _ => 1f
        };
    }


}
