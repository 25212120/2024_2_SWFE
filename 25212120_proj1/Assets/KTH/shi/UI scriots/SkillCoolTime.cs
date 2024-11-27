using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolTime : MonoBehaviour
{
    [SerializeField] private PlayerCoolDownManager coolDownManager;
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private Image dashCoolTimeImage;
    [SerializeField] private Image weaponSkillCoolTimeImage;
    [SerializeField] private Image magicSkill1CoolTimeImage;
    [SerializeField] private Image magicSkill2CoolTimeImage;

    private Coroutine dashCoroutine = null;
    private Coroutine weaponCoroutine = null;
    private Coroutine magic1Coroutine = null;
    private Coroutine magic2Coroutine = null;

    private void Start()
    {
       
        // 처음 모든 UI 비활성화
        dashCoolTimeImage.enabled = false;
        weaponSkillCoolTimeImage.enabled = false;
        magicSkill1CoolTimeImage.enabled = false;
        magicSkill2CoolTimeImage.enabled = false;

        
    }

    private void Update()
    {
        

        // 대쉬 쿨타임 업데이트
        if (coolDownManager.isDashOnCoolTime && dashCoroutine == null)
        {
            dashCoolTimeImage.enabled = true; // 이미지 활성화
            dashCoroutine = StartCoroutine(UpdateCoolDownUI(
                dashCoolTimeImage,
                coolDownManager.dashCoolTime_SwordShield,
                () => dashCoroutine = null // Coroutine 초기화
            ));
        }

        // 무기 스킬 쿨타임 업데이트
        if (IsWeaponSkillOnCoolTime() && weaponCoroutine == null)
        {
            weaponSkillCoolTimeImage.enabled = true; // 이미지 활성화
            weaponCoroutine = StartCoroutine(UpdateCoolDownUI(
                weaponSkillCoolTimeImage,
                GetWeaponSkillCoolTime(),
                () => weaponCoroutine = null // Coroutine 초기화
            ));
        }

        // 매직 스킬 1 쿨타임 업데이트
        if (IsMagic1OnCoolTime(playerInputManager.magic1) && magic1Coroutine == null)
        {
            magicSkill1CoolTimeImage.enabled = true; // 이미지 활성화
            magic1Coroutine = StartCoroutine(UpdateCoolDownUI(
                magicSkill1CoolTimeImage,
                GetMagic1CoolTime(playerInputManager.magic1),
                () => magic1Coroutine = null // Coroutine 초기화
            ));
        }

        // 매직 스킬 2 쿨타임 업데이트
        if (IsMagic2OnCoolTime(playerInputManager.magic2) && magic2Coroutine == null)
        {
            magicSkill2CoolTimeImage.enabled = true; // 이미지 활성화
            magic2Coroutine = StartCoroutine(UpdateCoolDownUI(
                magicSkill2CoolTimeImage,
                GetMagic2CoolTime(playerInputManager.magic2),
                () => magic2Coroutine = null // Coroutine 초기화
            ));
        }
    }

    private IEnumerator UpdateCoolDownUI(Image coolDownImage, float coolTime, System.Action onComplete)
    {
        Debug.Log($"Starting Cooldown UI for: {coolDownImage.name}");
        float elapsedTime = 0f;
        coolDownImage.fillAmount = 1f;

        while (elapsedTime < coolTime)
        {
            elapsedTime += Time.deltaTime;
            coolDownImage.fillAmount = Mathf.Clamp01(1f - elapsedTime / coolTime);
            yield return null;
        }

        coolDownImage.fillAmount = 0f;
        coolDownImage.enabled = false;
        Debug.Log($"Cooldown finished for: {coolDownImage.name}");
        onComplete?.Invoke();
    }


    private bool IsWeaponSkillOnCoolTime()
    {
        return coolDownManager.isSwordShieldWeaponSkillOnCoolTime ||
               coolDownManager.isSingleTwoHandSwordWeaponSkillOnCoolTime ||
               coolDownManager.isDoubleSwordsWeaponSkillOnCoolTime ||
               coolDownManager.isBowWeaponSkillOnCoolTime;
    }

    private float GetWeaponSkillCoolTime()
    {
        int weaponIndex = playerInputManager.currentRightHandIndex;

        return weaponIndex switch
        {
            0 => coolDownManager.weaponSkillCoolTime_SwordShield,
            1 => coolDownManager.weaponSkillCoolTime_SingleTwoHandSword,
            2 => coolDownManager.weaponSkillCoolTime_DoubleSwords,
            3 => coolDownManager.weaponSkillCoolTime_Bow,
            _ => 0f
        };
    }

    private bool IsMagic1OnCoolTime(PlayerStateType magic)
    {
        return magic switch
        {
            PlayerStateType.FireBall_MagicState => coolDownManager.isFireBallOnCoolTime,
            PlayerStateType.Meteor_MagicState => coolDownManager.isMeteorOnCoolTime,
            PlayerStateType.PoisonFog_MagicState => coolDownManager.isPoisonFogOnCoolTime,
            PlayerStateType.DrainField_MagicState => coolDownManager.isDrainFieldOnCoolTime,
            PlayerStateType.IceSpear_MagicState => coolDownManager.isIceSpearOnCoolTime,
            PlayerStateType.Storm_MagicState => coolDownManager.isStormOnCoolTime,
            PlayerStateType.RockFall_MagicState => coolDownManager.isRockFallOnCoolTime,
            PlayerStateType.EarthQuake_MagicState => coolDownManager.isEarthQuakeOnCoolTime,
            _ => false
        };
    }

    private bool IsMagic2OnCoolTime(PlayerStateType magic)
    {
        return magic switch
        {
            PlayerStateType.FireBall_MagicState => coolDownManager.isFireBallOnCoolTime,
            PlayerStateType.Meteor_MagicState => coolDownManager.isMeteorOnCoolTime,
            PlayerStateType.PoisonFog_MagicState => coolDownManager.isPoisonFogOnCoolTime,
            PlayerStateType.DrainField_MagicState => coolDownManager.isDrainFieldOnCoolTime,
            PlayerStateType.IceSpear_MagicState => coolDownManager.isIceSpearOnCoolTime,
            PlayerStateType.Storm_MagicState => coolDownManager.isStormOnCoolTime,
            PlayerStateType.RockFall_MagicState => coolDownManager.isRockFallOnCoolTime,
            PlayerStateType.EarthQuake_MagicState => coolDownManager.isEarthQuakeOnCoolTime,
            _ => false
        };
    }

    private float GetMagic1CoolTime(PlayerStateType magic)
    {
        return magic switch
        {
            PlayerStateType.FireBall_MagicState => coolDownManager.magicCoolTime_FireBall,
            PlayerStateType.Meteor_MagicState => coolDownManager.magicCoolTime_Meteor,
            PlayerStateType.PoisonFog_MagicState => coolDownManager.magicCoolTime_PoisonFog,
            PlayerStateType.DrainField_MagicState => coolDownManager.magicCoolTime_DrainField,
            PlayerStateType.IceSpear_MagicState => coolDownManager.magicCoolTime_IceSpear,
            PlayerStateType.Storm_MagicState => coolDownManager.magicCoolTime_Storm,
            PlayerStateType.RockFall_MagicState => coolDownManager.magicCoolTime_RockFall,
            PlayerStateType.EarthQuake_MagicState => coolDownManager.magicCoolTime_EarthQuake,
            _ => 0f
        };
    }

    private float GetMagic2CoolTime(PlayerStateType magic)
    {
        return magic switch
        {
            PlayerStateType.FireBall_MagicState => coolDownManager.magicCoolTime_FireBall,
            PlayerStateType.Meteor_MagicState => coolDownManager.magicCoolTime_Meteor,
            PlayerStateType.PoisonFog_MagicState => coolDownManager.magicCoolTime_PoisonFog,
            PlayerStateType.DrainField_MagicState => coolDownManager.magicCoolTime_DrainField,
            PlayerStateType.IceSpear_MagicState => coolDownManager.magicCoolTime_IceSpear,
            PlayerStateType.Storm_MagicState => coolDownManager.magicCoolTime_Storm,
            PlayerStateType.RockFall_MagicState => coolDownManager.magicCoolTime_RockFall,
            PlayerStateType.EarthQuake_MagicState => coolDownManager.magicCoolTime_EarthQuake,
            _ => 0f
        };
    }
}
