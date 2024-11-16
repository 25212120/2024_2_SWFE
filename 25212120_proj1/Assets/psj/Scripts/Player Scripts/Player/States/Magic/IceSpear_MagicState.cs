using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class IceSpear_MagicState : BaseState<PlayerStateType>
{
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Animator animator;
    private MonoBehaviour monoBehaviour;

    public IceSpear_MagicState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Transform playerTransform, MonoBehaviour monoBehaviour, Animator animator)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.playerInputManager = inputManager;
        this.monoBehaviour = monoBehaviour;
        this.animator = animator;
    }

    private GameObject waterCircle0;
    private GameObject waterCircle1;
    private GameObject iceSpear;
    private GameObject iceNova;

    private Vector3 magicCirclePos;
    private Vector3 spawnPosition;

    private bool magicfinished = false;

    public override void EnterState()
    {
        LoadWaterCircle0("Prefabs/Magic/Water/MagicCircleBlue");
        LoadWaterCircle1("Prefabs/Magic/Water/MagicCircleBlue 1");
        LoadIceSpear("Prefabs/Magic/Water/IceSpear/SharpMissileBlue");
        LoadIceNova("Prefabs/Magic/Water/IceSpear/NovaFrost");

        spawnPosition = playerTransform.TransformPoint(playerInputManager.magicSpawnPoints[0]);
        magicCirclePos = playerTransform.position + new Vector3(0, 0.2f, 0);

        playerInputManager.isPerformingAction = true;

        monoBehaviour.StartCoroutine(InstantiateWaterCircles());
        monoBehaviour.StartCoroutine(InstantiateIceSpears());

    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        magicfinished = false;
        playerInputManager.isPerformingAction = false;
    }

    public override void CheckTransitions()
    {
        if (magicfinished == true) stateManager.PopState();
    }


    private void LoadWaterCircle0(string prefabAddress)
    {
        waterCircle0 = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadWaterCircle1(string prefabAddress)
    {
        waterCircle1 = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadIceSpear(string prefabAddress)
    {
        iceSpear = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadIceNova(string prefabAddress)
    {
        iceNova = Resources.Load<GameObject>(prefabAddress);
    }

    private IEnumerator InstantiateWaterCircles()
    {
        GameObject instantiatedWaterCircle0 = Object.Instantiate(waterCircle0, magicCirclePos, Quaternion.Euler(-90, 0, 0));
        yield return new WaitForSeconds(0.2f);
        GameObject instantiatedWaterCircle1 = Object.Instantiate(waterCircle1, magicCirclePos, Quaternion.Euler(-90, 0, 0));


        yield return new WaitForSeconds(1.5f);
        Object.Destroy(instantiatedWaterCircle0);
        Object.Destroy(instantiatedWaterCircle1);
    }

    private IEnumerator InstantaiteIceSpear()
    {
        GameObject instantiatedIceSpear = Object.Instantiate(iceSpear, spawnPosition, Quaternion.Euler(-90, 0, 0));
        Rigidbody iceSpearRigidbody = instantiatedIceSpear.GetComponent<Rigidbody>();

        Vector3 direction = (playerInputManager.magicPoint - spawnPosition).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        instantiatedIceSpear.transform.rotation = rotation;

        iceSpearRigidbody.AddForce(direction * 50f, ForceMode.VelocityChange);

        yield return new WaitForSeconds(3f);

        Object.Destroy(instantiatedIceSpear);
    }

    private IEnumerator InstantiateIceSpears()
    {
        monoBehaviour.StartCoroutine(InstantaiteIceSpear());
        yield return new WaitForSeconds(0.2f);
        monoBehaviour.StartCoroutine(InstantaiteIceSpear());
        yield return new WaitForSeconds(0.2f);
        monoBehaviour.StartCoroutine(InstantaiteIceSpear());

        yield return new WaitForSeconds(0.2f);
        magicfinished = true;
    }

}