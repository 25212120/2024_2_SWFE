using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class FireBall_MagicState : BaseState<PlayerStateType>
{
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Animator animator;
    private MonoBehaviour monoBehaviour;

    public FireBall_MagicState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Transform playerTransform, MonoBehaviour monoBehaviour, Animator animator)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.playerInputManager = inputManager;
        this.monoBehaviour = monoBehaviour;
        this.animator = animator;
    }

    private GameObject fireBall;
    private GameObject fireCircle;
    private GameObject flameThrower;

    private Vector3 spawnPosition;

    private bool finishedCasting = false;

    public override void EnterState()
    {
        playerInputManager.isPeformingAction = true;
        LoadFireCircle("Prefabs/Magic/Fire/MagicCircleSimpleYellow");
        LoadFireBall("Prefabs/Magic/Fire/FireBall/FireBallObject");
        LoadFlameThrower("Prefabs/Magic/Fire/FireBall/FlamethrowerSharpFire");


        Vector3 magicCirclePos = playerTransform.position + new Vector3(0, 0.2f, 0);
        Object.Instantiate(fireCircle, magicCirclePos, Quaternion.Euler(-90, 0, 0));

        spawnPosition = playerTransform.TransformPoint(playerInputManager.magicSpawnPoints[0]);

        monoBehaviour.StartCoroutine(MaigcCasting());
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
        playerInputManager.isPeformingAction = false;
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
    private void LoadFlameThrower(string prefabAddress)
    {
        flameThrower = Resources.Load<GameObject>(prefabAddress);
    }

    IEnumerator MaigcCasting()
    {
        GameObject instantiatedFireBall = Object.Instantiate(fireBall, spawnPosition, Quaternion.identity);
        Rigidbody fireBallRigidbody = instantiatedFireBall.GetComponent<Rigidbody>();
        yield return new WaitForSeconds(1f);

        Vector3 direction = (playerInputManager.magicPoint - spawnPosition).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        instantiatedFireBall.transform.rotation = rotation;

        Transform childTransform = instantiatedFireBall.transform.GetChild(0);
        childTransform.gameObject.SetActive(true);
        fireBallRigidbody.AddForce(direction * 50f, ForceMode.VelocityChange);

        finishedCasting = true;
    }
}