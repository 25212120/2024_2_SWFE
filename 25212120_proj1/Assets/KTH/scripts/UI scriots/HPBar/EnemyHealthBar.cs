using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSliderPrefab;
    [SerializeField] private float visibleDuration = 2.0f; // HP 바가 보이는 시간
    [SerializeField] private Vector3 offset = new Vector3(0, 2.0f, 0); // 체력바가 몬스터 위로 올라가게 하기 위한 오프셋
    [SerializeField] private Canvas healthBarCanvas; // 체력바가 생성될 캔버스 참조
    private Monster_1 enemy; // 몬스터 스크립트 참조
    private BaseEntity _entity;
    private Coroutine hideCoroutine;
    [SerializeField] private Camera mainCamera;
    private Slider healthSlider;

    void Start()
    {
        _entity = GetComponentInParent<BaseEntity>(); // 부모 객체에서 BaseEntity 컴포넌트 가져오기
        enemy = _entity.GetComponent<Monster_1>(); // BaseEntity에서 Monster_1 컴포넌트 가져오기
        mainCamera = Camera.main;

        if (_entity != null && enemy != null && healthBarCanvas != null)
        {
            // 각 몬스터마다 개별 체력바 인스턴스 생성 (캔버스에 생성)
            healthSlider = Instantiate(healthSliderPrefab, healthBarCanvas.transform);
            healthSlider.maxValue = enemy.GetMaxHP();
            healthSlider.value = enemy.GetCurrentHP();
            healthSlider.gameObject.SetActive(false); // 처음엔 비활성화
            _entity.TakeDamageEvent += HandleTakeDamage; // 몬스터가 피해를 받을 때 이벤트 연결
        }
    }

    private void OnDestroy()
    {
        if (_entity != null)
        {
            _entity.TakeDamageEvent -= HandleTakeDamage; // 이벤트 해제
        }

        if (healthSlider != null)
        {
            Destroy(healthSlider.gameObject); // 체력바 인스턴스 제거
        }
    }

    private void LateUpdate()
    {
        if (healthSlider != null && healthSlider.gameObject.activeSelf)
        {
            Vector3 worldPosition = _entity.transform.position + offset; // 몬스터 위의 위치
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            healthSlider.transform.position = screenPosition; // 체력바를 화면 좌표로 설정
        }
    }

    private void HandleTakeDamage(float damage)
    {
        ShowHealthBar();
        healthSlider.value = enemy.GetCurrentHP(); // 체력 업데이트
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
