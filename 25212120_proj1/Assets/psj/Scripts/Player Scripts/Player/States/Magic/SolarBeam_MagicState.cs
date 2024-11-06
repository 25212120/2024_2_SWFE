using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SolarBeam_MagicState : BaseState<PlayerStateType>
{
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Animator animator;
    private MonoBehaviour monoBehaviour;

    public SolarBeam_MagicState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Transform playerTransform, MonoBehaviour monoBehaviour, Animator animator)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.playerInputManager = inputManager;
        this.monoBehaviour = monoBehaviour;
        this.animator = animator;
    }

    private GameObject laserPoint;

    private GameObject instantiatedLaser0;
    private GameObject instantiatedLaser1;
    private GameObject instantiatedLaser2;
    private GameObject instantiatedLaser3;

    private Vector3 spawnPosition0;
    private Vector3 spawnPosition1;
    private Vector3 spawnPosition2;
    private Vector3 spawnPosition3;

    private Vector3 forwardPosition0;
    private Vector3 forwardPosition1;
    private Vector3 forwardPosition2;
    private Vector3 forwardPosition3;


    private bool shootLaser = true;
    private bool lasersMovedForward = false;
    public override void EnterState()
    {
        LoadLaserPoint("Prefabs/Magic/Plant/SolarBeam/LaserPoint");
        monoBehaviour.StartCoroutine(laserEnable());

        InstantiateLasers();
    }

    public override void UpdateState()
    {
        if (shootLaser == true)
        {
            ToggleLaserPositions();
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        DestroyLasers();
        shootLaser = true;
    }

    public override void CheckTransitions()
    {
        if (shootLaser == false)
        {
            stateManager.PopState();
        }
    }

    private void LoadLaserPoint(string prefabAddress)
    {
        laserPoint = Resources.Load<GameObject>(prefabAddress);
    }

    private IEnumerator laserEnable()
    {
        yield return new WaitForSeconds(2f);
        shootLaser = false;
    }

    private void InstantiateLasers()
    {
        spawnPosition0 = playerTransform.TransformPoint(playerInputManager.magicSpawnPoints[1]);
        spawnPosition1 = playerTransform.TransformPoint(playerInputManager.magicSpawnPoints[2]);
        spawnPosition2 = playerTransform.TransformPoint(playerInputManager.magicSpawnPoints[3]);
        spawnPosition3 = playerTransform.TransformPoint(playerInputManager.magicSpawnPoints[4]);

        instantiatedLaser0 = Object.Instantiate(laserPoint, spawnPosition0, playerTransform.rotation);
        instantiatedLaser1 = Object.Instantiate(laserPoint, spawnPosition1, playerTransform.rotation);
        instantiatedLaser2 = Object.Instantiate(laserPoint, spawnPosition2, playerTransform.rotation);
        instantiatedLaser3 = Object.Instantiate(laserPoint, spawnPosition3, playerTransform.rotation);

        float forwardDistance = 50f;
        forwardPosition0 = spawnPosition0 + playerTransform.forward * forwardDistance;
        forwardPosition1 = spawnPosition1 + playerTransform.forward * forwardDistance;
        forwardPosition2 = spawnPosition2 + playerTransform.forward * forwardDistance;
        forwardPosition3 = spawnPosition3 + playerTransform.forward * forwardDistance;
    }

    private void ToggleLaserPositions()
    {
        if (!lasersMovedForward)
        {
            instantiatedLaser0.transform.position = forwardPosition0;
            instantiatedLaser1.transform.position = forwardPosition1;
            instantiatedLaser2.transform.position = forwardPosition2;
            instantiatedLaser3.transform.position = forwardPosition3;
            lasersMovedForward = true;
        }
        else
        {
            instantiatedLaser0.transform.position = spawnPosition0;
            instantiatedLaser1.transform.position = spawnPosition1;
            instantiatedLaser2.transform.position = spawnPosition2;
            instantiatedLaser3.transform.position = spawnPosition3;
            lasersMovedForward = false;
        }
    }

    private void DestroyLasers()
    {
        if (instantiatedLaser0 != null) Object.Destroy(instantiatedLaser0);
        if (instantiatedLaser1 != null) Object.Destroy(instantiatedLaser1);
        if (instantiatedLaser2 != null) Object.Destroy(instantiatedLaser2);
        if (instantiatedLaser3 != null) Object.Destroy(instantiatedLaser3);
    }
}