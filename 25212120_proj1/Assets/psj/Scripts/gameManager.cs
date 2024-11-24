using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public GameObject[] nexuses;
    public GameObject core;

    public delegate void ProximityCheck(GameObject nexus);
    public static event ProximityCheck OnPlayerProximityToCore;

    public delegate void EnemySpawnRequest(GameObject spawner);
    public static event EnemySpawnRequest OnEnemySpawnRequested;

    private float spawnTimer = 0f;
    public float globalSpawnInterval = 10f;

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer > globalSpawnInterval )
        {
            spawnTimer = 0f;
            BroadcastEnemySpawnRequest();
        }

        foreach (var nexus in nexuses)
        {
            if (nexus != null)
            {
                OnPlayerProximityToCore?.Invoke(player1);
                OnPlayerProximityToCore?.Invoke(player2);
            }
        }
    }

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
    }

    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> units = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(CountDays());
    }

    private float dayDuration = 240f;
    private int dayCount = 0;
    //public Light directionalLight;
    //public TextMeshProUGUI dayText;

    IEnumerator CountDays()
    {
        float time = 0f;

        while (true)
        {
            while (time < dayDuration)
            {
                time += Time.deltaTime;

                float timeNormalized = (time % dayDuration) / dayDuration;

                //directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timeNormalized * 360f) - 90f, 170f, 0));

                yield return null; 
            }

            time = 0f;
            dayCount++;
            UpgradeAllEnemyStat();
            Debug.Log("Day: " + dayCount);
            //dayText.text = "Day: " + dayCount.ToString();

            yield return null; 
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
