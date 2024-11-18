using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitController : MonoBehaviour
{
    private List<GameObject> SelectedUnits = new List<GameObject>();
    private Vector2 startPos;
    private Vector2 endPos;
    private const float dragThreshold = 25f;

    private bool isSelecting = false;
    public bool Move_Command_given = false;

    private PlayerMovement playerInput;

    public RectTransform selectionBox;
    private Canvas canvas;

    private InputAction select;

    private void Awake()
    {
        playerInput = new PlayerMovement();
        canvas = selectionBox.GetComponentInParent<Canvas>();
    }

    private void OnEnable()
    {
        playerInput.Enable();

        playerInput.UnitControl.Select.performed += OnSelectPerformed;
        playerInput.UnitControl.Select.canceled += OnSelectCanceled;
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

        foreach (GameObject unit in SelectedUnits)
        {
            Transform greenCircle = unit.transform.Find("GreenCircle");
            if (greenCircle != null)
            {
                greenCircle.gameObject.SetActive(false);
            }
        }
        SelectedUnits.Clear();



        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("unit"))
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (screenPos.z >= 0 && selectionRect.Contains(screenPos))
            {
                SelectedUnits.Add(unit);
                Transform greenCircle = unit.transform.Find("GreenCircle");
                if (greenCircle != null)
                {
                    greenCircle.gameObject.SetActive(true);
                }
            }
        }
    }
}
