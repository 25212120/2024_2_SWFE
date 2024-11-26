using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;

public class EarthQuake_MagicState : BaseState<PlayerStateType>
{
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Animator animator;
    private Rigidbody rb;
    private MonoBehaviour monoBehaviour;
    private PlayerStat playerStat;

    public EarthQuake_MagicState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Transform playerTransform, MonoBehaviour monoBehaviour, Animator animator, Rigidbody rb, PlayerStat playerStat)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.playerInputManager = inputManager;
        this.rb = rb;
        this.monoBehaviour = monoBehaviour;
        this.animator = animator;
        this.playerStat = playerStat;
    }

    private GameObject earthCircle0;
    private GameObject earthCircle1;
    private GameObject earthCircle2;
    private GameObject novaBrown0;
    private GameObject novaBrown1;
    private GameObject novaBrown2;

    private Vector3 magicCirclePos;

    private bool magicfinished;

    public override void EnterState()
    {
        LoadEarthCircle0("Prefabs/Magic/Earth/MagicCircleBrown");
        LoadEarthCircle1("Prefabs/Magic/Earth/MagicCircleBrown 1");
        LoadEarthCircle2("Prefabs/Magic/Earth/MagicCircleBrown 2");
        LoadNovaBrown0("Prefabs/Magic/Earth/EarthQuake/NovaBrown");
        LoadNovaBrown1("Prefabs/Magic/Earth/EarthQuake/NovaBrown 1");
        LoadNovaBrown2("Prefabs/Magic/Earth/EarthQuake/NovaBrown 2");

        magicCirclePos = playerTransform.position + new Vector3(0, 0.2f, 0);

        playerInputManager.isPerformingAction = true;

        monoBehaviour.StartCoroutine(InstantiateEarthCircles());
        monoBehaviour.StartCoroutine(Jump());
        monoBehaviour.StartCoroutine(InstantiateNova());


        //밀어내는거 구현
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        findEnemies(100);
        foreach(GameObject enemy in enemies)
        {
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            agent.enabled = true;
        }
        magicfinished = false;
        playerInputManager.isPerformingAction = false;
    }

    public override void CheckTransitions()
    {
        if (magicfinished == true)  stateManager.PopState();
    }

    private void LoadEarthCircle0(string prefabAddress)
    {
        // R = 4
        earthCircle0 = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadEarthCircle1(string prefabAddress)
    {
        // R = 7
        earthCircle1 = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadEarthCircle2(string prefabAddress)
    {
        // R =  10
        earthCircle2 = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadNovaBrown0(string prefabAddress)
    {
        novaBrown0 = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadNovaBrown1(string prefabAddress)
    {
        novaBrown1 = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadNovaBrown2(string prefabAddress)
    {
        novaBrown2 =  Resources.Load<GameObject>(prefabAddress);
    }
    private IEnumerator InstantiateEarthCircles()
    {
        GameObject instantiatedEarthCircle0 = Object.Instantiate(earthCircle0, magicCirclePos, Quaternion.Euler(-90, 0, 0));
        yield return new WaitForSeconds(0.2f);
        GameObject instantiatedEarthCircle1 = Object.Instantiate(earthCircle1, magicCirclePos, Quaternion.Euler(-90, 0, 0));
        yield return new WaitForSeconds(0.2f);
        GameObject instantiatedEarthCircle2 = Object.Instantiate(earthCircle2, magicCirclePos, Quaternion.Euler(-90, 0, 0));


        yield return new WaitForSeconds(1.5f);
        Object.Destroy(instantiatedEarthCircle0);
        Object.Destroy(instantiatedEarthCircle1);
        Object.Destroy(instantiatedEarthCircle2);
    }
    private void JumpStart()
    {
        rb.useGravity = false;
        rb.AddForce(playerTransform.up * 40f, ForceMode.Impulse);
    }
    private void JumpEnd()
    {
        rb.useGravity = true;
        rb.AddForce(playerTransform.up * -50f, ForceMode.Impulse);
    }
    private IEnumerator Jump()
    {
        yield return new WaitForSeconds(0.6f);
        JumpStart();
        yield return new WaitForSeconds(0.3f);
        JumpEnd();
        yield return new WaitForSeconds(0.1f);
        aviate(10);
    }
    private IEnumerator InstantiateNova()
    {
        yield return new WaitForSeconds(1.3f);
        GameObject instantiatedNovaBrown0 = Object.Instantiate(novaBrown0, magicCirclePos, Quaternion.Euler(-90, 0, 0));
        // 함수 호출
        knockBack(8);
        yield return new WaitForSeconds(0.5f);
        GameObject instantiatedNovaBrown1 = Object.Instantiate(novaBrown1, magicCirclePos, Quaternion.Euler(-90, 0, 0));
        // 함수 호출
        knockBack(14);
        yield return new WaitForSeconds(0.5f);
        GameObject instantiatedNovaBrown2 = Object.Instantiate(novaBrown2, magicCirclePos, Quaternion.Euler(-90, 0, 0));
        // 함수 호출
        knockBack(20);

        magicfinished = true;

        yield return new WaitForSeconds(1.5f);
        Object.Destroy(instantiatedNovaBrown0);
        Object.Destroy(instantiatedNovaBrown1);
        Object.Destroy(instantiatedNovaBrown2);
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
                NavMeshAgent agent = hitCollider.GetComponent<NavMeshAgent>();
                agent.enabled = false;
                enemies.Add(hitCollider.gameObject);
            }
        }
    }
    private IEnumerator aviateEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            Transform transform = enemy.GetComponent<Transform>();
            Rigidbody rb = enemy.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.AddForce(transform.up * 10f, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(1.0f);


        foreach (GameObject enemy in enemies)
        {
            Transform transform = enemy.GetComponent<Transform>();
            Rigidbody rb = enemy.GetComponent<Rigidbody>();
            rb.useGravity = true;
        }
    }
    private void knockBackEnemies(int novaRadius)
    {
        foreach (GameObject enemy in enemies)
        {
            Transform enemyTransform = enemy.GetComponent<Transform>();
            BaseMonster enemyStat = enemy.GetComponent<BaseMonster>();
            Rigidbody rb = enemy.GetComponent <Rigidbody>();

            float distance = Vector3.Distance(playerTransform.position, enemyTransform.position);
            if (distance <= novaRadius)
            {
                playerStat.MagicAttack(enemyStat, 3);
                Vector3 knockbackDirection = (enemyTransform.position - playerTransform.position).normalized;
                rb.AddForce(knockbackDirection * 20f, ForceMode.Impulse);
            }
        }
    }


    private void aviate(int radius)
    {

        findEnemies(radius);
        monoBehaviour.StartCoroutine(aviateEnemies());
    }


    private void knockBack(int radius) 
    {
        findEnemies(radius);
        knockBackEnemies(radius);
    }

}