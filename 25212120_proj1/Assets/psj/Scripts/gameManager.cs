using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public GameObject[] nexuses;
    public GameObject core;

    public delegate void ProximityCheck(GameObject nexus);
    public static event ProximityCheck OnPlayerProximityToCore;

    public delegate void EnemySpawnRequest(GameObject spawner);
    public static event EnemySpawnRequest OnEnemySpawnRequested;
    private PhotonView pv;

    private float spawnTimer = 0f;
    public float globalSpawnInterval = 20f;


    private void BroadcastEnemySpawnRequest()
    {
        foreach (var spawner in FindObjectsOfType<EnemySpawner>())
        {
            OnEnemySpawnRequested?.Invoke(spawner.gameObject);
        }
    }


    public static GameManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        StartCoroutine(InitializePlayer());

    }

    private IEnumerator InitializePlayer()
    {
        // �÷��̾� �̸� ����
        if (GameSettings.IsMultiplayer)
        {
            playerName = PhotonNetwork.IsMasterClient ? "Player 1(Clone)" : "Player 2(Clone)";
        }
        else
        {
            playerName = "Player 1(Clone)";
        }

        while (GameObject.Find(playerName) == null)
        {
            yield return null; // ���� �����ӱ��� ���
        }

        player = GameObject.Find(playerName);

        pv = GetComponent<PhotonView>();
    }

    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> units = new List<GameObject>();

    string playerName;
    public GameObject player;

    private void Start()
    {
        pv = GetComponent<PhotonView>();


        StartCoroutine(CountDays());
    }

    private float dayDuration = 100f;
    private int dayCount = 1;
    public Light directionalLight;
    //public TextMeshProUGUI dayText;

    IEnumerator CountDays()
    {
        float time = 0f;
        float spawnTimer = 0f;
        bool isDaytime = true; // ��/�� ���¸� ����

        // �ڷ�ƾ ���� Ȯ��
        Debug.Log("CountDays coroutine started.");

        while (true)
        {
            // �� �Ǵ� �� ���¸� ����
            while (time < dayDuration)
            {
                time += Time.deltaTime;
                Debug.Log($"Time: {time}, DayDuration: {dayDuration}, IsDayTime: {isDaytime}");

                if (isDaytime)
                {
                    //dayText.text = "Day: " + dayCount.ToString() + " - Daytime";
                    directionalLight.intensity = 1f; // ������ �ִ� ��� ����
                }
                else
                {
                    //dayText.text = "Day: " + dayCount.ToString() + " - Nighttime";
                    directionalLight.intensity = 0.5f; // �㿡�� �ּ� ��� ����

                    // �㿡�� �� ����
                    spawnTimer += Time.deltaTime;

                    if (spawnTimer > globalSpawnInterval)
                    {
                        spawnTimer = 0f;
                        BroadcastEnemySpawnRequest();
                    }
                }

                yield return null;
            }

            Debug.Log("Time >= dayDuration, starting transition");

            // ��ȯ �ð� ���� �� �α� ���
            float transitionTime = 10f;
            float elapsedTime = 0f;
            float startIntensity = isDaytime ? 1f : 0.5f; // ���� ���
            float endIntensity = isDaytime ? 0.5f : 1f;   // ��ǥ ���

            // ��ȯ ����
            while (elapsedTime < transitionTime)
            {
                elapsedTime += Time.deltaTime;
                Debug.Log($"Transitioning... ElapsedTime: {elapsedTime}/{transitionTime}");

                directionalLight.intensity = Mathf.Lerp(startIntensity, endIntensity, elapsedTime / transitionTime);
                Debug.Log($"Current Intensity: {directionalLight.intensity}");

                yield return null; // �� ������ ���
            }

            Debug.Log("Transition complete.");

            isDaytime = !isDaytime; // �� <-> �� ��ȯ
            time = 0f;

            if (isDaytime)
            {
                dayCount++;
                UpgradeAllEnemyStat();
            }

            yield return null; // ���� ���������� �̵�
        }
    }



    public void AddEnemy(GameObject enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }
    public void RemoveEnemy(GameObject enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }
    public void AddUnit(GameObject unit)
    {
        if (!units.Contains(unit))
        {
            units.Add(unit);
        }
    }
    public void RemoveUnit(GameObject unit)
    {
        if (units.Contains(unit))
        {
            units.Remove(unit);
        }
    }

    public float initialHP_Unit_Warrior = 100f;
    public float initialHP_Unit_Mage = 50f;
    public float initialAtk_Unit_Warrior = 10f;
    public float initialAtk_Unit_Mage = 20f;
    public float initialDef_Unit_Warrior = 5f;
    public float initialDef_Unit_Mage = 2f;

    public void UpgradeAllUnitStat()
    {
        foreach (GameObject unit in units)
        {
            BaseUnit baseUnit = unit.GetComponent<BaseUnit>();
            if (baseUnit != null)
            {
                baseUnit.Upgrade();
                initialHP_Unit_Warrior += 50f;
                initialHP_Unit_Mage += 30f;
                initialAtk_Unit_Warrior += 5f;
                initialAtk_Unit_Mage += 7f;
                initialDef_Unit_Warrior += 1f;
                initialDef_Unit_Mage += 1f;
            }
        }
    }

    public float initialHP_Enemy_Warrior = 100f;
    public float initialHP_Enemy_Mage = 50f;
    public float initialAtk_Enemy_Warrior = 10f;
    public float initialAtk_Enemy_Mage = 20f;
    public float initialDef_Enemy_Warrior = 5f;
    public float initialDef_Enemy_Mage = 2f;

    public void UpgradeAllEnemyStat()
    {
        initialHP_Enemy_Warrior += 50f;
        initialHP_Enemy_Mage += 30f;
        initialAtk_Enemy_Warrior += 5f;
        initialAtk_Enemy_Mage += 7f;
        initialDef_Enemy_Warrior += 1f;
        initialDef_Enemy_Mage += 1f;
    }
}
