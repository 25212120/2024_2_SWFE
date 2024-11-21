using UnityEngine;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;

public class RockFall_MagicState : BaseState<PlayerStateType>
{
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Animator animator;
    private MonoBehaviour monoBehaviour;
    private PlayerStat playerStat;

    public RockFall_MagicState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Transform playerTransform, MonoBehaviour monoBehaviour, Animator animator, PlayerStat playerStat)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.playerInputManager = inputManager;
        this.monoBehaviour = monoBehaviour;
        this.animator = animator;
        this.playerStat = playerStat;
    }

    private GameObject earthCircle0;
    private GameObject earthCircle1;
    private GameObject rock;

    private Vector3 magicCirclePos;

    private bool finishedMagic = false;
    public override void EnterState()
    {
        LoadEarthCircle0("Prefabs/Magic/Earth/MagicCircleBrown");
        LoadEarthCircle1("Prefabs/Magic/Earth/MagicCircleBrown 1");
        LoadRock("Prefabs/Magic/Earth/RockFall/rock");

        magicCirclePos = playerTransform.position + new Vector3(0, 0.2f, 0);

        monoBehaviour.StartCoroutine(InstanatiateEarthCircles());
        InstantiateRocks();
    }
    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        finishedMagic = false;
    }

    public override void CheckTransitions()
    {
        if (finishedMagic == true) stateManager.PopState();
    }

    private void LoadEarthCircle0(string prefabAddress)
    {
        earthCircle0 = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadEarthCircle1(string prefabAddress)
    {
        earthCircle1 = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadRock(string prefabAddress)
    {
        rock = Resources.Load<GameObject>(prefabAddress);
    }

    private IEnumerator InstanatiateEarthCircles()
    {
        GameObject instantiatedEarthCircle0 = Object.Instantiate(earthCircle0, magicCirclePos, Quaternion.Euler(-90, 0, 0));
        yield return new WaitForSeconds(0.2f);
        GameObject instantiatedEarthCircle1 = Object.Instantiate(earthCircle1, magicCirclePos, Quaternion.Euler(-90, 0, 0));


        yield return new WaitForSeconds(1.5f);
        Object.Destroy(instantiatedEarthCircle0);
        Object.Destroy(instantiatedEarthCircle1);
    }

    private List<GameObject> enemies = new List<GameObject>();
    private void findEnemies(int radius)
    {
        enemies.Clear();

        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, radius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                enemies.Add(hitCollider.gameObject);
            }
        }
    }

    private IEnumerator InstantiateRock(Vector3 rockSpawnPos)
    {
        GameObject instantiatedRock = Object.Instantiate(rock, rockSpawnPos, Quaternion.Euler(-90f, 0, 0));
        yield return new WaitForSeconds(0.6f);
        Rigidbody rockRb = instantiatedRock.GetComponent<Rigidbody>();
        rockRb.AddForce(Vector3.down * 20f, ForceMode.VelocityChange);

        yield return new WaitForSeconds(2f);

        Object.Destroy(instantiatedRock);
    }

    private void InstantiateRocks()
    {
        findEnemies(10);

        foreach(GameObject enemy in  enemies)
        {
            Vector3 rockSpawnPos = enemy.transform.position + new Vector3(0, 8f, 0);
            monoBehaviour.StartCoroutine(InstantiateRock(rockSpawnPos));
        }

        finishedMagic = true;
    }
}