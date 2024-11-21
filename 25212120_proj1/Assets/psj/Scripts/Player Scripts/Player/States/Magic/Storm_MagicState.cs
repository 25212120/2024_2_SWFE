using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Storm_MagicState : BaseState<PlayerStateType>
{
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Animator animator;
    private MonoBehaviour monoBehaviour;
    private PlayerStat playerStat;

    public Storm_MagicState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Transform playerTransform, MonoBehaviour monoBehaviour, Animator animator, PlayerStat playerStat)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.playerInputManager = inputManager;
        this.monoBehaviour = monoBehaviour;
        this.animator = animator;
        this.playerStat = playerStat;
    }

    private GameObject waterCircle0;
    private GameObject waterCircle1;
    private GameObject waterCircle2;

    private GameObject lightning;
    private GameObject lightningNova;
    private GameObject instantiatedRainEffect;
    private GameObject rainstorm;
    private GameObject overlayObject;

    private bool stormfinished = false;


    private Vector3 magicCirclePos;

    public override void EnterState()
    {
        LoadWaterCircle0("Prefabs/Magic/Water/MagicCircleBlue");
        LoadWaterCircle1("Prefabs/Magic/Water/MagicCircleBlue 1");
        LoadWaterCircle2("Prefabs/Magic/Water/MagicCircleBlue 2");
        LoadLightning("Prefabs/Magic/Water/Storm/LightningStrikeSharpTallBlue");
        LoadLightningNova("Prefabs/Magic/Water/Storm/NovaLightningBlue");
        LoadRainStorm("Prefabs/Magic/Water/Storm/RainStorm");
        overlayObject = playerInputManager.dim;
        magicCirclePos = playerTransform.position + new Vector3(0, 0.2f, 0);

        playerInputManager.isPerformingAction = true;

        InstantiateRainEffect();
        monoBehaviour.StartCoroutine(InstantiateWaterCircles());
        overlayObject.SetActive(true);
        Debug.Log(overlayObject.activeSelf);
        UseStormSkill();
    }

    public override void UpdateState()
    {

    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        stormfinished = false;
        monoBehaviour.StartCoroutine(DestroyRainEffect());
        playerInputManager.isPerformingAction = false;
    }

    public override void CheckTransitions()
    {
        if (stormfinished == true) stateManager.PopState();
    }

    private void InstantiateRainEffect()
    {
        Vector3 spawnPoint = playerTransform.position + new Vector3(0, 20f, 0);
        instantiatedRainEffect = Object.Instantiate(rainstorm, spawnPoint, Quaternion.Euler(-90, 0, 0));
    }

    private IEnumerator DestroyRainEffect()
    {
        Object.Destroy(instantiatedRainEffect);
        yield return new WaitForSeconds(0.2f);
        overlayObject.SetActive(false);
    }

    private IEnumerator InstantiateWaterCircles()
    {
        GameObject instantiatedWaterCircle0 = Object.Instantiate(waterCircle0, magicCirclePos, Quaternion.Euler(-90, 0, 0));
        yield return new WaitForSeconds(0.2f);
        GameObject instantiatedWaterCircle1 = Object.Instantiate(waterCircle1, magicCirclePos, Quaternion.Euler(-90, 0, 0));
        yield return new WaitForSeconds(0.2f);
        GameObject instantiatedWaterCircle2 = Object.Instantiate(waterCircle2, magicCirclePos, Quaternion.Euler(-90, 0, 0));


        yield return new WaitForSeconds(1.5f);
        Object.Destroy(instantiatedWaterCircle0);
        Object.Destroy(instantiatedWaterCircle1);
        Object.Destroy(instantiatedWaterCircle2);
    }

    private void LoadWaterCircle0(string prefabAddress)
    {
        waterCircle0 = Resources.Load<GameObject>(prefabAddress);
    }

    private void LoadWaterCircle1(string prefabAddress)
    {
        waterCircle1 = Resources.Load<GameObject>(prefabAddress);
    }

    private void LoadWaterCircle2(string prefabAddress)
    {
        waterCircle2 = Resources.Load<GameObject>(prefabAddress);
    }

    private void LoadLightning(string prefabAddress)
    {
        lightning = Resources.Load<GameObject>(prefabAddress);
    }

    private void LoadLightningNova(string prefabAddress)
    {
        lightningNova = Resources.Load<GameObject>(prefabAddress);
    }
    private void LoadRainStorm(string prefabAddress)
    {
        rainstorm = Resources.Load<GameObject>(prefabAddress);
    }

    private float skillRadius = 15f;     // ½ºÅ³ ¹üÀ§ ¹Ý°æ
    private int lightningCount = 12;      // ³«·Ú °³¼ö
    private float lightningHeight = 7f; // ³«·Ú°¡ ¶³¾îÁö´Â ³ôÀÌ
    private float minDistance = 7f;      // ³«·Úµé »çÀÌÀÇ ÃÖ¼Ò °Å¸®

    private void UseStormSkill()
    {
        monoBehaviour.StartCoroutine(UseStormSkillCoroutine());
    }

    private IEnumerator UseStormSkillCoroutine()
    {
        List<Vector2> points = GeneratePoissonPoints(skillRadius, minDistance, lightningCount);

        foreach (Vector2 point in points)
        {
            Vector3 position = new Vector3(point.x, lightningHeight, point.y);
            position += playerTransform.position;

            monoBehaviour.StartCoroutine(InstantiateLightning(position));

            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(1.0f);
        stormfinished = true;
    }


    private IEnumerator InstantiateLightning(Vector3 position)
    {
        GameObject instantiatedLightning = Object.Instantiate(lightning, position, Quaternion.Euler(-90, 0, 0));
        yield return new WaitForSeconds(0.05f);
        GameObject instantiatedLightningNova = Object.Instantiate(lightningNova, position, Quaternion.Euler(-90, 0, 0));

        yield return new WaitForSeconds(0.3f);
        Object.Destroy(instantiatedLightning);
        Object.Destroy(instantiatedLightningNova);
    }

    // Æ÷¾Æ¼Û µð½ºÅ© »ùÇÃ¸µÀ¸·Î »ê¹ßÀû ³«·Ú ±¸Çö
    private List<Vector2> GeneratePoissonPoints(float radius, float minDist, int sampleCount)
    {
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();

        spawnPoints.Add(Vector2.zero);

        while (points.Count < sampleCount && spawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            bool accepted = false;

            for (int i = 0; i < 30; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2);
                float distance = Random.Range(minDist, minDist * 2);
                Vector2 candidate = spawnCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

                if (candidate.magnitude <= radius && IsValid(candidate, points, minDist))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    accepted = true;
                    break;
                }
            }

            if (!accepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return points;
    }


    // 
    private bool IsValid(Vector2 candidate, List<Vector2> points, float minDist)
    {
        foreach (Vector2 point in points)
        {
            if (Vector2.Distance(candidate, point) < minDist)
            {
                return false;
            }
        }
        return true;
    }
}