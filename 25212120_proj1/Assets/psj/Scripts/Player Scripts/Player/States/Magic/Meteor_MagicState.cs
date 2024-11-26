using System.Collections;
using UnityEngine;

public class Meteor_MagicState : BaseState<PlayerStateType>
{
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Animator animator;
    private MonoBehaviour monoBehaviour;
    private PlayerStat playerStat;

    public Meteor_MagicState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Transform playerTransform, MonoBehaviour monoBehaviour, Animator animator, PlayerStat playerStat)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.playerInputManager = inputManager;
        this.monoBehaviour = monoBehaviour;
        this.animator = animator;
        this.playerStat = playerStat;
    }

    private GameObject fireCircle0;
    private GameObject fireCircle1;
    private GameObject fireCircle2;
    private GameObject meteor;

    private Vector3 spawnPosition;
    private Vector3 magicCirclePos;

    private bool finishedCasting = false;

    public override void EnterState()
    {
        LoadFireCircle0("Prefabs/Magic/Fire/MagicCircleSimpleYellow");
        LoadFireCircle1("Prefabs/Magic/Fire/MagicCircleSimpleYellow 1");
        LoadFireCircle2("Prefabs/Magic/Fire/MagicCircleSimpleYellow 2");
        LoadMeteor("Prefabs/Magic/Fire/Meteor/Meteor");
        meteor.GetComponent<MeteorHandler>().playerStat = playerStat;

        magicCirclePos = playerTransform.position + new Vector3(0, 0.2f, 0);

        playerInputManager.isPerformingAction = true;
        monoBehaviour.StartCoroutine(InstantiateFireCircles());
        spawnPosition = playerInputManager.magicPoint + new Vector3(0, 30f, 0);
        monoBehaviour.StartCoroutine(MagicCasting());
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
        playerInputManager.isPerformingAction = false;
    }

    public override void CheckTransitions()
    {
        if (finishedCasting == true)
        {
            stateManager.PopState();
        }
    }


    private void LoadFireCircle0(string prefabAddress)
    {
        fireCircle0 = Resources.Load<GameObject>(prefabAddress);
    }

    private void LoadFireCircle1(string prefabAddress)
    {
        fireCircle1 = Resources.Load<GameObject>(prefabAddress);
    }

    private void LoadFireCircle2(string prefabAddress)
    {
        fireCircle2 = Resources.Load<GameObject>(prefabAddress);
    }

    private void LoadMeteor(string prefabAddress)
    {
        meteor = Resources.Load<GameObject>(prefabAddress); 
    }


    private IEnumerator InstantiateFireCircles()
    {
        GameObject instantiatedFireCircle0 = Object.Instantiate(fireCircle0, magicCirclePos, Quaternion.Euler(-90, 0, 0));
        yield return new WaitForSeconds(0.2f);
        GameObject instantiatedFireCircle1 = Object.Instantiate(fireCircle1, magicCirclePos, Quaternion.Euler(-90, 0, 0));
        yield return new WaitForSeconds(0.2f);
        GameObject instantiatedFireCircle2 = Object.Instantiate(fireCircle2, magicCirclePos, Quaternion.Euler(-90, 0, 0));


        yield return new WaitForSeconds(1.5f);
        Object.Destroy(instantiatedFireCircle0);
        Object.Destroy(instantiatedFireCircle1);
        Object.Destroy(instantiatedFireCircle2);
    }

    private IEnumerator MagicCasting()
    {
        GameObject instantiatedFireBall = Object.Instantiate(meteor, spawnPosition, Quaternion.identity);
        Rigidbody fireBallRigidbody = instantiatedFireBall.GetComponent<Rigidbody>();

        fireBallRigidbody.AddForce(Vector3.down * 10f, ForceMode.VelocityChange);

        finishedCasting = true;
        yield return null;
    }
}