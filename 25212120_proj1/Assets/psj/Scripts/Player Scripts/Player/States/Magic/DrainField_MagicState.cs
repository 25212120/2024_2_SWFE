using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DrainField_MagicState : BaseState<PlayerStateType>
{
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Animator animator;
    private MonoBehaviour monoBehaviour;

    public DrainField_MagicState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Transform playerTransform, MonoBehaviour monoBehaviour, Animator animator)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.playerInputManager = inputManager;
        this.monoBehaviour = monoBehaviour;
        this.animator = animator;
    }

    private GameObject plantCircle0;
    private GameObject plantCircle1;
    private GameObject plantCircle2;
    private GameObject drainField;

    private bool magicFinished = false;

    private Vector3 magicCirclePos;

    public override void EnterState()
    {
        LoadPlantCircle0("Prefabs/Magic/Plant/MagicCircleGreen");
        LoadPlantCircle1("Prefabs/Magic/Plant/MagicCircleGreen 1");
        LoadPlantCircle2("Prefabs/Magic/Plant/MagicCircleGreen 2");
        LoadGreenAura("Prefabs/Magic/Plant/DrainField/AuraSoftGreen");

        magicCirclePos = playerTransform.position + new Vector3(0, 0.2f, 0);

        playerInputManager.isPerformingAction = true;

        monoBehaviour.StartCoroutine(InstantiatePlantCircle());
        monoBehaviour.StartCoroutine(InstantiateDrainField());


    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        magicFinished = false;
        playerInputManager.isPerformingAction = false;
    }

    public override void CheckTransitions()
    {
        if (magicFinished == true) stateManager.PopState();
    }

    private void LoadPlantCircle0(string prefabAddress)
    {
        plantCircle0 = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadPlantCircle1(string prefabAddress)
    {
        plantCircle1 = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadPlantCircle2(string prefabAddress)
    {
        plantCircle2 = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadGreenAura(string prefabAddress)
    {
        drainField = Resources.Load<GameObject>(prefabAddress);
    }

    private IEnumerator InstantiatePlantCircle()
    {
        GameObject instantiatedPlantCircle0 = Object.Instantiate(plantCircle0, magicCirclePos, Quaternion.Euler(-90, 0, 0));
        yield return new WaitForSeconds(0.2f);
        GameObject instantiatedPlantCircle1 = Object.Instantiate(plantCircle1, magicCirclePos, Quaternion.Euler(-90, 0, 0));
        yield return new WaitForSeconds(0.2f);
        GameObject instantiatedPlantCircle2 = Object.Instantiate(plantCircle2, magicCirclePos, Quaternion.Euler(-90, 0, 0));


        yield return new WaitForSeconds(1.5f);
        Object.Destroy(instantiatedPlantCircle0);
        Object.Destroy(instantiatedPlantCircle1);
        Object.Destroy(instantiatedPlantCircle2);
    }

    private IEnumerator InstantiateDrainField()
    {   
        float elapsedTime = 0f;

        GameObject instantiatedDrainField = Object.Instantiate(drainField, magicCirclePos, Quaternion.Euler(-90, 0, 0));

        magicFinished = true;

        while (instantiatedDrainField != null && elapsedTime < 11f)
        {
            instantiatedDrainField.transform.position = playerTransform.position + new Vector3(0, 0.2f, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Object.Destroy(instantiatedDrainField);
    }

}