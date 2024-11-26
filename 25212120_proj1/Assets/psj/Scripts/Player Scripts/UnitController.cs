using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;

public class UnitController : MonoBehaviour
{
    private PhotonView pv;
    private List<GameObject> SelectedUnits = new List<GameObject>();
    private Vector2 startPos;
    private Vector2 endPos;
    private const float dragThreshold = 25f;

    private bool isSelecting = false;
    public bool Move_Command_given = false;

    private PlayerMovement playerInput;

    public RectTransform selectionBox;
    private Canvas canvas;

    public bool SpecifyWarrior = false;
    public bool SpecifyMage = false;

    private InputAction select;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerInput = new PlayerMovement();
    }

    private void Start()
    {
        GameObject selectBox = GameObject.Find("Select");
        selectionBox = selectBox.GetComponentInChildren<RectTransform>();
    }

    private void OnEnable()
    {

        if (pv.IsMine == false) return;
        
        playerInput.Enable();

        playerInput.UnitControl.Select.performed += OnSelectPerformed;
        playerInput.UnitControl.Select.canceled += OnSelectCanceled;
        playerInput.UnitControl.Specify_Warrior.performed += OnSpecifyWarriorPerformed;
        playerInput.UnitControl.Specify_Warrior.canceled += OnSpecifyWarriorCanceled;
        playerInput.UnitControl.Specify_Mage.performed += OnSpecifyMagePerformed;
        playerInput.UnitControl.Specify_Mage.canceled += OnSpecifyMageCanceled;
    }

    private void OnDisable()
    {
        if (pv.IsMine == false) return;

        playerInput.Disable();

        playerInput.UnitControl.Select.performed -= OnSelectPerformed;
        playerInput.UnitControl.Select.canceled -= OnSelectCanceled;
    }

    void OnSelectPerformed(InputAction.CallbackContext ctx)
    {
        if (isSelecting == false)
        {
            isSelecting = true;
            startPos = Mouse.current.position.ReadValue();

            selectionBox.gameObject.SetActive(true);
            selectionBox.sizeDelta = Vector2.zero;
        }
    }
    void OnSelectCanceled(InputAction.CallbackContext ctx)
    {
        isSelecting = false;
        selectionBox.gameObject.SetActive(false);

        float dragDistance = Vector2.Distance(startPos, endPos);

        if (dragDistance < dragThreshold)
        {
            MoveUnitsToPosition();
        }
        else
        {
            selectionBox.gameObject.SetActive(false);
            SelectUnitsInRecTangle();
        }
    }

    void OnSpecifyWarriorPerformed(InputAction.CallbackContext ctx)
    {
        SpecifyWarrior = true;
    }
    void OnSpecifyWarriorCanceled(InputAction.CallbackContext ctx)
    {
        SpecifyWarrior = false;
    }
    void OnSpecifyMagePerformed(InputAction.CallbackContext ctx)
    {
        SpecifyMage = true;
    }
    void OnSpecifyMageCanceled(InputAction.CallbackContext ctx)
    {
        SpecifyMage = false;
    }

    void MoveUnitsToPosition()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 destination = hit.point;

            int unitCount = SelectedUnits.Count;
            int gridSize = Mathf.CeilToInt(Mathf.Sqrt(unitCount));
            float spacing = 1.5f; // 유닛 간 간격

            // 격자의 중앙을 기준으로 오프셋을 계산
            float offsetX = (gridSize - 1) * spacing / 2;
            float offsetZ = (gridSize - 1) * spacing / 2;

            Quaternion rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);

            int index = 0;

            for (int x = 0; x < gridSize; x++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    if (index >= unitCount)
                        break;

                    Vector3 unitPosition = new Vector3(x * spacing - offsetX, 0, z * spacing - offsetZ);
                    Vector3 rotatedPosition = rotation * unitPosition;
                    Vector3 unitDestination = destination + rotatedPosition;

                    GameObject unitObject = SelectedUnits[index];
                    Unit unit = unitObject.GetComponent<Unit>();

                    NavMeshHit navHit;
                    if (NavMesh.SamplePosition(unitDestination, out navHit, 1.0f, NavMesh.AllAreas))
                    {
                        unit.MoveToPosition(navHit.position);
                    }
                    else
                    {
                        Debug.Log($"유닛 {unit.gameObject.name}이(가) NavMesh 위에 있지 않습니다.");
                    }

                    index++;
                }
            }
        }
    }

    private void Update()
    {
        if (isSelecting == true)
        {
            endPos = Mouse.current.position.ReadValue();
            float dragDistance = Vector2.Distance(startPos, endPos);

            if (dragDistance >= dragThreshold)
            {
                selectionBox.gameObject.SetActive(true);
                UpdateSelectionBox();
            }
            else 
            {
                selectionBox.gameObject.SetActive(false);
            }   
        }
    }

    void UpdateSelectionBox()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        Vector2 startPosCanvas;
        Vector2 endPosCanvas;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, startPos, canvas.worldCamera, out startPosCanvas);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePos, canvas.worldCamera, out endPosCanvas);

        Vector2 center = (startPosCanvas + endPosCanvas) / 2;

        selectionBox.localPosition = center;

        float sizeX = Mathf.Abs(startPosCanvas.x - endPosCanvas.x);
        float sizeY = Mathf.Abs(startPosCanvas.y - endPosCanvas.y);

        selectionBox.sizeDelta = new Vector2(sizeX, sizeY);
    }
    void SelectUnitsInRecTangle()
    {
        Vector2 min = Vector2.Min(startPos, endPos);
        Vector2 max = Vector2.Max(startPos, endPos);

        Rect selectionRect = new Rect(min, max - min);

        // 이전 선택된 유닛 초기화
        for (int i = SelectedUnits.Count - 1; i >= 0; i--)
        {
            if (SelectedUnits[i] == null)
            {
                SelectedUnits.RemoveAt(i);
                continue;
            }

            Transform greenCircle = SelectedUnits[i].transform.Find("GreenCircle");
            if (greenCircle != null)
            {
                greenCircle.gameObject.SetActive(false);
            }
        }
        SelectedUnits.Clear();

        // 모든 유닛을 탐색
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("unit"))
        {
            if (unit == null) continue;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (screenPos.z >= 0 && selectionRect.Contains(screenPos))
            {
                // Warrior만 선택
                if (SpecifyWarrior && !SpecifyMage)
                {
                    if (unit.GetComponent<Warrior>() != null)
                    {
                        AddUnitToSelection(unit);
                    }
                }
                // Mage만 선택
                else if (SpecifyMage && !SpecifyWarrior)
                {
                    if (unit.GetComponent<Mage>() != null)
                    {
                        AddUnitToSelection(unit);
                    }
                }
                // 아무 조건도 없을 때 모든 유닛 선택
                else if (!SpecifyWarrior && !SpecifyMage)
                {
                    AddUnitToSelection(unit);
                }
            }
        }
    }

    // 선택된 유닛에 추가하고 하이라이트 활성화
    void AddUnitToSelection(GameObject unit)
    {
        SelectedUnits.Add(unit);

        Transform greenCircle = unit.transform.Find("GreenCircle");
        if (greenCircle != null)
        {
            greenCircle.gameObject.SetActive(true);
        }
    }
}
