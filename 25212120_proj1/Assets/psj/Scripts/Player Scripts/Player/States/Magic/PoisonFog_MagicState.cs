using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonFog_MagicState : BaseState<PlayerStateType>
{
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Animator animator;
    private Rigidbody rb;
    private MonoBehaviour monoBehaviour;
    private PlayerStat playerStat;

    public PoisonFog_MagicState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Transform playerTransform, MonoBehaviour monoBehaviour, Animator animator, Rigidbody rb, PlayerStat playerStat)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.playerInputManager = inputManager;
        this.rb = rb;
        this.monoBehaviour = monoBehaviour;
        this.animator = animator;
        this.playerStat = playerStat;
    }

    private GameObject plantCircle0;
    private GameObject plantCircle1;

    private GameObject poisonFog;

    private Vector3 fogSpawnPos;
    private Vector3 magicCirclePos;

    private bool magicfinished = false;
    public override void EnterState()
    {
        fogSpawnPos = playerInputManager.magicPoint + new Vector3(0, 1f, 0);
        magicCirclePos = playerTransform.position + new Vector3(0, 0.2f, 0);

        LoadPlantCircle0("Prefabs/Magic/Plant/MagicCircleGreen");
        LoadPlantCircle1("Prefabs/Magic/Plant/MagicCircleGreen 1");
        LoadPoisonFog("Prefabs/Magic/Plant/PoisonFog/PoisonFog");

        monoBehaviour.StartCoroutine(InstantiatePlantCircles());
        monoBehaviour.StartCoroutine(InstantaitePoisonFog());
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
    }
    public override void CheckTransitions()
    {
        if (magicfinished == true) stateManager.PopState();
    }


    private void LoadPlantCircle0(string prefabAddress)
    {
        plantCircle0 = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadPlantCircle1(string prefabAddress)
    {
        plantCircle1 = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadPoisonFog(string prefabAddress)
    {
        poisonFog = Resources.Load<GameObject>(prefabAddress);
    }

    private IEnumerator InstantiatePlantCircles()
    {
        GameObject instantiatedPlantCircle0 = Object.Instantiate(plantCircle0, magicCirclePos, Quaternion.Euler(-90, 0, 0));
        yield return new WaitForSeconds(0.2f);
        GameObject instantiatedPlantCircle1 = Object.Instantiate(plantCircle1, magicCirclePos, Quaternion.Euler(-90, 0, 0));


        yield return new WaitForSeconds(1.5f);
        Object.Destroy(instantiatedPlantCircle0);
        Object.Destroy(instantiatedPlantCircle1);
    }
    private IEnumerator InstantaitePoisonFog()
    {
        GameObject instantiatedPoisonFog = Object.Instantiate(poisonFog, fogSpawnPos, Quaternion.Euler(-90, 0, 0));
        instantiatedPoisonFog.GetComponentInChildren<PoisonFog>().playerStat = playerStat;

        magicfinished = true;

        yield return new WaitForSeconds(5f);
        Object.Destroy(instantiatedPoisonFog);
    }

}
