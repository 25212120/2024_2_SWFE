using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    // 각 마법 유형의 스킬 트리 잠금 이미지
    public Image lockImage_Wood1;
    public Image lockImage_Wood2;

    public Image lockImage_Fire1;
    public Image lockImage_Fire2;

    public Image lockImage_Ice1;
    public Image lockImage_Ice2;

    public Image lockImage_Sand1;
    public Image lockImage_Sand2;


    // 스킬 트리 잠금 해제 함수
    public void UnlockSkillTree(PlayerMagicType magicType)
    {
        switch (magicType)
        {
            case PlayerMagicType.Wood:
                if (lockImage_Wood1 != null && lockImage_Wood2 != null)
                    lockImage_Wood1.gameObject.SetActive(false);
                    lockImage_Wood2.gameObject.SetActive(false); // 잠금 이미지 비활성화
                                                                 // 잠금 이미지 비활성화
                Debug.Log("Wood 스킬 트리 잠금 해제 UI 업데이트 완료.");
                break;
            case PlayerMagicType.Fire:
                if (lockImage_Fire1 != null && lockImage_Fire2 != null)
                    lockImage_Fire1.gameObject.SetActive(false);
                    lockImage_Fire2.gameObject.SetActive(false);

                Debug.Log("Fire 스킬 트리 잠금 해제 UI 업데이트 완료.");
                break;
            case PlayerMagicType.Ice:
                if (lockImage_Ice1 != null && lockImage_Ice2 != null)
                    lockImage_Ice1.gameObject.SetActive(false);
                    lockImage_Ice2.gameObject.SetActive(false);

                Debug.Log("Ice 스킬 트리 잠금 해제 UI 업데이트 완료.");
                break;
            case PlayerMagicType.Sand:
                if (lockImage_Sand1 != null && lockImage_Sand2 != null)
                    lockImage_Sand1.gameObject.SetActive(false);
                     lockImage_Sand2.gameObject.SetActive(false);

                Debug.Log("Sand 스킬 트리 잠금 해제 UI 업데이트 완료.");
                break;
            default:
                Debug.LogWarning("알 수 없는 마법 유형입니다.");
                break;
        }
    }
}
