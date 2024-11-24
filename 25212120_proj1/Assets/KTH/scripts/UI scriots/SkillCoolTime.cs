using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolTime : MonoBehaviour
{
    [SerializeField] public PlayerCoolDownManager coolDownManager;
    [SerializeField] private Image dashCoolTimeImage;

    // 선택된 무기 및 마법 스킬 이미지
    [SerializeField] private Image weaponSkillCoolTimeImage;
    [SerializeField] private Image magicSkill1CoolTimeImage;
    [SerializeField] private Image magicSkill2CoolTimeImage;

    private int selectedWeaponIndex = 0;
    private PlayerStateType selectedMagicSkill1 = PlayerStateType.None;
    private PlayerStateType selectedMagicSkill2 = PlayerStateType.None;

    private PlayerInputManager playerInputManager;

    // 쿨타임 데이터를 배열로 저장
    private Dictionary<PlayerStateType, float> magicCoolDownTimes = new Dictionary<PlayerStateType, float>();
    private float[] weaponSkillCoolDownTimes = new float[] { 10.0f, 12.0f, 8.0f, 15.0f }; // 예시로 각 무기 쿨타임 설정
    private float dashCoolDownTime = 5.0f;

    void Start()
    {
        // PlayerInputManager 컴포넌트를 찾아서 참조합니다.
        playerInputManager = GetComponent<PlayerInputManager>();

        // 초기 선택된 무기 및 마법 스킬 설정
        selectedWeaponIndex = playerInputManager.currentRightHandIndex;
        selectedMagicSkill1 = playerInputManager.magic1;
        selectedMagicSkill2 = playerInputManager.magic2;

        // 마법 쿨타임 초기화
        magicCoolDownTimes[PlayerStateType.FireBall_MagicState] = 8.0f;
        magicCoolDownTimes[PlayerStateType.Meteor_MagicState] = 10.0f;
        magicCoolDownTimes[PlayerStateType.PoisonFog_MagicState] = 7.0f;
        magicCoolDownTimes[PlayerStateType.DrainField_MagicState] = 9.0f;
        magicCoolDownTimes[PlayerStateType.IceSpear_MagicState] = 6.0f;
        magicCoolDownTimes[PlayerStateType.Storm_MagicState] = 11.0f;
        magicCoolDownTimes[PlayerStateType.RockFall_MagicState] = 10.0f;
        magicCoolDownTimes[PlayerStateType.EarthQuake_MagicState] = 12.0f;
    }

    void Update()
    {
        UpdateDashCoolDown();
        UpdateWeaponSkillCoolDown();
        UpdateMagicCoolDown();
    }

    private void UpdateDashCoolDown()
    {
        if (!coolDownManager.CanDash(selectedWeaponIndex))
        {
            StartCoroutine(UpdateCoolDownUI(dashCoolTimeImage, dashCoolDownTime));
        }
    }

    private void UpdateWeaponSkillCoolDown()
    {
        if (!coolDownManager.CanUseWeaponSkill(selectedWeaponIndex))
        {
            float coolDownTime = weaponSkillCoolDownTimes[selectedWeaponIndex];
            StartCoroutine(UpdateCoolDownUI(weaponSkillCoolTimeImage, coolDownTime));
        }
    }

    private void UpdateMagicCoolDown()
    {
        // 첫 번째 마법 스킬 쿨타임 업데이트
        if (!coolDownManager.CanUseMagic(selectedMagicSkill1))
        {
            float coolDownTime = magicCoolDownTimes[selectedMagicSkill1];
            StartCoroutine(UpdateCoolDownUI(magicSkill1CoolTimeImage, coolDownTime));
        }

        // 두 번째 마법 스킬 쿨타임 업데이트
        if (!coolDownManager.CanUseMagic(selectedMagicSkill2))
        {
            float coolDownTime = magicCoolDownTimes[selectedMagicSkill2];
            StartCoroutine(UpdateCoolDownUI(magicSkill2CoolTimeImage, coolDownTime));
        }
    }

    private IEnumerator UpdateCoolDownUI(Image coolDownImage, float coolDownTime)
    {
        float timer = 0f;
        coolDownImage.fillAmount = 1f;
        coolDownImage.fillClockwise = true;
        while (timer < coolDownTime)
        {
            timer += Time.deltaTime;
            coolDownImage.fillAmount = 1f - (timer / coolDownTime);
            yield return null;
        }
        coolDownImage.fillAmount = 0f;
    }

    public void SwapWeapon(int newWeaponIndex)
    {
        selectedWeaponIndex = newWeaponIndex;
        weaponSkillCoolTimeImage.sprite = playerInputManager.rightHand_Weapons[selectedWeaponIndex].GetComponent<SpriteRenderer>().sprite;
    }

    public void SwapMagicSkill(PlayerStateType magicSkill1, PlayerStateType magicSkill2)
    {
        selectedMagicSkill1 = magicSkill1;
        selectedMagicSkill2 = magicSkill2;

        magicSkill1CoolTimeImage.sprite = playerInputManager.magicRangeSprites[(int)magicSkill1].GetComponent<SpriteRenderer>().sprite;
        magicSkill2CoolTimeImage.sprite = playerInputManager.magicRangeSprites[(int)magicSkill2].GetComponent<SpriteRenderer>().sprite;
    }
}

