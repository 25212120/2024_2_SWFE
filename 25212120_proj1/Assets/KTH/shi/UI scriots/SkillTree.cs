using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    // �� ���� ������ ��ų Ʈ�� ��� �̹���
    public Image lockImage_Wood1;
    public Image lockImage_Wood2;

    public Image lockImage_Fire1;
    public Image lockImage_Fire2;

    public Image lockImage_Ice1;
    public Image lockImage_Ice2;

    public Image lockImage_Sand1;
    public Image lockImage_Sand2;


    // ��ų Ʈ�� ��� ���� �Լ�
    public void UnlockSkillTree(PlayerMagicType magicType)
    {
        switch (magicType)
        {
            case PlayerMagicType.Wood:
                if (lockImage_Wood1 != null && lockImage_Wood2 != null)
                    lockImage_Wood1.gameObject.SetActive(false);
                    lockImage_Wood2.gameObject.SetActive(false); // ��� �̹��� ��Ȱ��ȭ
                                                                 // ��� �̹��� ��Ȱ��ȭ
                Debug.Log("Wood ��ų Ʈ�� ��� ���� UI ������Ʈ �Ϸ�.");
                break;
            case PlayerMagicType.Fire:
                if (lockImage_Fire1 != null && lockImage_Fire2 != null)
                    lockImage_Fire1.gameObject.SetActive(false);
                    lockImage_Fire2.gameObject.SetActive(false);

                Debug.Log("Fire ��ų Ʈ�� ��� ���� UI ������Ʈ �Ϸ�.");
                break;
            case PlayerMagicType.Ice:
                if (lockImage_Ice1 != null && lockImage_Ice2 != null)
                    lockImage_Ice1.gameObject.SetActive(false);
                    lockImage_Ice2.gameObject.SetActive(false);

                Debug.Log("Ice ��ų Ʈ�� ��� ���� UI ������Ʈ �Ϸ�.");
                break;
            case PlayerMagicType.Sand:
                if (lockImage_Sand1 != null && lockImage_Sand2 != null)
                    lockImage_Sand1.gameObject.SetActive(false);
                     lockImage_Sand2.gameObject.SetActive(false);

                Debug.Log("Sand ��ų Ʈ�� ��� ���� UI ������Ʈ �Ϸ�.");
                break;
            default:
                Debug.LogWarning("�� �� ���� ���� �����Դϴ�.");
                break;
        }
    }
}
