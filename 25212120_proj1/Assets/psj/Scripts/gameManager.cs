using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    public void UpgradeAllUnitStat()
    {
        foreach (GameObject unit in units)
        {
            BaseUnit baseUnit = unit.GetComponent<BaseUnit>();
            if (baseUnit != null)
            {
                baseUnit.Upgrade();
            }
        }
    }

    public void UpgradeAllEnemyStat()
    {
        foreach (GameObject enemy in enemies)
        {
            BaseMonster baseEnemy = enemy.GetComponent<BaseMonster>();
            if (baseEnemy != null)
            {
                baseEnemy.PerformUpgrade();
            }
        }
    }
}
