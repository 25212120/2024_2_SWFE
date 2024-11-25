using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class FireBall_MagicState : BaseState<PlayerStateType>
{
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Animator animator;
    private MonoBehaviour monoBehaviour;
    private PlayerStat playerStat;

    public FireBall_MagicState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Transform playerTransform, MonoBehaviour monoBehaviour, Animator animator, PlayerStat playerStat)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.playerInputManager = inputManager;
        this.monoBehaviour = monoBehaviour;
        this.animator = animator;
        this.playerStat = playerStat;
    }

    private GameObject fireBall;
    private GameObject fireCircle;
    private GameObject instantiatedFireCircle;

    private Vector3 spawnPosition;
    private Vector3 magicCirclePos;

    private bool finishedCasting = false;

    public override void EnterState()
    {
        playerInputManager.isPerformingAction = true;
        LoadFireCircle("Prefabs/Magic/Fire/MagicCircleSimpleYellow");
        LoadFireBall("Prefabs/Magic/Fire/FireBall/FireballMissileFire");


        magicCirclePos = playerTransform.position + new Vector3(0, 0.2f, 0);

        spawnPosition = playerTransform.TransformPoint(playerInputManager.magicSpawnPoints[0]);
        monoBehaviour.StartCoroutine(InstantiateMagicCircle());

        monoBehaviour.StartCoroutine(MaigcCasting());
        playerInputManager.isPerformingAction = false;
    }

    public override void UpdateState()
    {

    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        finishedCasting = false;
    }

    public override void CheckTransitions()
    {
        if (finishedCasting == true)
        {
            stateManager.PopState();
        }
    }


    private void LoadFireCircle(string prefabAddress)
    {
        fireCircle = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadFireBall(string prefabAddress)
    {
        fireBall = Resources.Load<GameObject>(prefabAddress);
    }

    IEnumerator InstantiateMagicCircle()
    {
        instantiatedFireCircle = Object.Instantiate(fireCircle, magicCirclePos, Quaternion.Euler(-90, 0, 0));

        yield return new WaitForSeconds(1f);

        Object.Destroy(instantiatedFireCircle);
    }

    IEnumerator MaigcCasting()
    {
        GameObject instantiatedFireBall = Object.Instantiate(fireBall, spawnPosition, Quaternion.identity);
        instantiatedFireBall.GetComponent<FireBallHandler>().playerStat = playerStat;
        Rigidbody fireBallRigidbody = instantiatedFireBall.GetComponent<Rigidbody>();
        yield return new WaitForSeconds(0.5f);

        Vector3 direction = (playerInputManager.magicPoint - spawnPosition).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        instantiatedFireBall.transform.rotation = rotation;

        fireBallRigidbody.AddForce(direction * 40f, ForceMode.VelocityChange);

        finishedCasting = true;
    }
}