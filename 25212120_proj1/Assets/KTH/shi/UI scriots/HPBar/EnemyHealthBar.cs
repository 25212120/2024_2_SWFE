using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSliderPrefab;
    [SerializeField] private float visibleDuration = 2.0f; // HP �ٰ� ���̴� �ð�
    [SerializeField] private Vector3 offset = new Vector3(0, 2.0f, 0); // ü�¹ٰ� ���� ���� �ö󰡰� �ϱ� ���� ������
    [SerializeField] private Canvas healthBarCanvas; // ü�¹ٰ� ������ ĵ���� ����
    private Monster_1 enemy; // ���� ��ũ��Ʈ ����
    private BaseEntity _entity;
    private Coroutine hideCoroutine;
    [SerializeField] private Camera mainCamera;
    private Slider healthSlider;

    void Start()
    {
        _entity = GetComponentInParent<BaseEntity>(); // �θ� ��ü���� BaseEntity ������Ʈ ��������
        enemy = _entity.GetComponent<Monster_1>(); // BaseEntity���� Monster_1 ������Ʈ ��������
        mainCamera = Camera.main;

        if (_entity != null && enemy != null && healthBarCanvas != null)
        {
            // �� ���͸��� ���� ü�¹� �ν��Ͻ� ���� (ĵ������ ����)
            healthSlider = Instantiate(healthSliderPrefab, healthBarCanvas.transform);
            healthSlider.maxValue = enemy.GetMaxHP();
            healthSlider.value = enemy.GetCurrentHP();
            healthSlider.gameObject.SetActive(false); // ó���� ��Ȱ��ȭ
            _entity.TakeDamageEvent += HandleTakeDamage; // ���Ͱ� ���ظ� ���� �� �̺�Ʈ ����
        }
    }

    private void OnDestroy()
    {
        if (_entity != null)
        {
            _entity.TakeDamageEvent -= HandleTakeDamage; // �̺�Ʈ ����
        }

        if (healthSlider != null)
        {
            Destroy(healthSlider.gameObject); // ü�¹� �ν��Ͻ� ����
        }
    }

    private void LateUpdate()
    {
        if (healthSlider != null && healthSlider.gameObject.activeSelf)
        {
            Vector3 worldPosition = _entity.transform.position + offset; // ���� ���� ��ġ
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            healthSlider.transform.position = screenPosition; // ü�¹ٸ� ȭ�� ��ǥ�� ����
        }
    }

    private void HandleTakeDamage(float damage)
    {
        ShowHealthBar();
        healthSlider.value = enemy.GetCurrentHP(); // ü�� ������Ʈ
    }

    public void ShowHealthBar()
    {
        if (healthSlider == null) return;

        healthSlider.gameObject.SetActive(true);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(visibleDuration);
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(false);
        }
    }
}
