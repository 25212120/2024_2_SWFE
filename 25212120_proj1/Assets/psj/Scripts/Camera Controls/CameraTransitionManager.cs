using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using System.Collections;
using Photon.Pun;

public class CameraTransitionManager : MonoBehaviour
{
    private PlayerMovement playerInput;

    public CinemachineVirtualCamera isoCamera;
    public CinemachineVirtualCamera topViewCamera;
    public Camera mainCamera;

    private bool isOrthographic = false;
    private bool isTransitioning = false;
    private float transitionDuration = 1f; // 전환 시간 (초)
    private float transitionProgress = 0f;

    public HighlightArea highlightArea;
    public SpawnPoint_Select spawnPoint_Select;

    public bool isActive_ha = false;
    public bool isActive_sp = false;

    private PhotonView pv;

    private void Awake()
    {
        isoCamera.Priority = 10;
        topViewCamera.Priority = 9;
        mainCamera.orthographic = false;
        playerInput = new PlayerMovement();
        pv = GetComponent<PhotonView>();
    }

    

    private void OnEnable()
    {
        if (pv.IsMine == false) return;

        playerInput.Enable();
        playerInput.CameraControl.Transition.performed += OnScrollPerformed;
        playerInput.CameraControl.ToggleBuild.performed += OnToggleHighlightArea;
        playerInput.CameraControl.SpawnPoint_Select.performed += OnToggleSpawnPoint;
    }

    private void OnDisable()
    {

        if (pv.IsMine == false) return;
        playerInput.CameraControl.Transition.performed -= OnScrollPerformed;
        playerInput.CameraControl.ToggleBuild.performed -= OnToggleHighlightArea;
        playerInput.CameraControl.SpawnPoint_Select.performed -= OnToggleSpawnPoint;
        playerInput.Disable();
    }

    private void OnScrollPerformed(InputAction.CallbackContext context)
    {
        Vector2 scrollValue = context.ReadValue<Vector2>();

        if (scrollValue.y > 0 && !isOrthographic)
        {
            StartTransition(true); // Orthographic으로 전환
        }
        else if (scrollValue.y < 0 && isOrthographic)
        {
            StartTransition(false); // Perspective로 전환
        }
    }


    void OnToggleHighlightArea(InputAction.CallbackContext ctx)
    {
        if (highlightArea != null)
        {
            if (isActive_ha)
            {
                GetComponent<PlayerInputManager>().isPerformingAction = false;
                highlightArea.isActive = false;
                isActive_ha = false;
            }

            else
            {
                GetComponent<PlayerInputManager>().isPerformingAction = true;
                highlightArea.ActivateHighlightArea();
                highlightArea.isActive = true;
                isActive_ha = true;
            }

        }
    }
    void OnToggleSpawnPoint(InputAction.CallbackContext ctx)
    {
        if (spawnPoint_Select != null)
        {
            if (isActive_sp)
            {
                GetComponent<PlayerInputManager>().isPerformingAction = false;
                spawnPoint_Select.isActive = false;

                isActive_sp = false;
            }
            else
            {
                GetComponent<PlayerInputManager>().isPerformingAction = true;

                spawnPoint_Select.ActivateHighlightObject();
                spawnPoint_Select.isActive = true;
                isActive_sp = true;
            }
        }
    }

    private void StartTransition(bool toOrthographic)
    {
        if (isTransitioning)
            return;

        isTransitioning = true;
        transitionProgress = 0f;
        isOrthographic = toOrthographic;

        if (isOrthographic)
        {
            isoCamera.Priority = 9;
            topViewCamera.Priority = 10;
            // Projection 타입은 전환 완료 후 변경
        }
        else
        {
            isoCamera.Priority = 10;
            topViewCamera.Priority = 9;
            // Projection 타입은 전환 완료 후 변경
        }
    }

    private void Update()
    {
        if (isTransitioning)
        {
            transitionProgress += Time.deltaTime / transitionDuration;

            if (transitionProgress >= 1f)
            {
                transitionProgress = 1f;
                isTransitioning = false;

                // 전환 완료 후 Projection 타입 설정
                mainCamera.orthographic = isOrthographic;
                mainCamera.ResetProjectionMatrix();
            }

            float t = transitionProgress;
            if (!isOrthographic)
                t = 1f - t;

            // Projection Matrix 보간
            mainCamera.projectionMatrix = MatrixLerp(
                Matrix4x4.Perspective(mainCamera.fieldOfView, mainCamera.aspect, mainCamera.nearClipPlane, mainCamera.farClipPlane),
                Matrix4x4.Ortho(-mainCamera.orthographicSize * mainCamera.aspect, mainCamera.orthographicSize * mainCamera.aspect,
                                -mainCamera.orthographicSize, mainCamera.orthographicSize,
                                mainCamera.nearClipPlane, mainCamera.farClipPlane),
                t);
        }
    }

    private Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float t)
    {
        Matrix4x4 result = new Matrix4x4();

        for (int i = 0; i < 16; i++)
        {
            result[i] = Mathf.Lerp(from[i], to[i], t);
        }

        return result;
    }

}
