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
                int unitsPerRow = Mathf.CeilToInt(Mathf.Sqrt(unitCount));
                float spacing = 1.5f;

                Vector3 moveDirection = (destination - Camera.main.transform.position).normalized;
                Quaternion rotation = Quaternion.LookRotation(moveDirection);

            for (int i = 0; i < unitCount; i++)
            {
                GameObject unitObject = SelectedUnits[i];
                Unit unit = unitObject.GetComponent<Unit>();

                int row = i / unitsPerRow;
                int column = i % unitsPerRow;

                Vector3 offset = rotation * new Vector3(column * spacing, 0, row * spacing);

                Vector3 unitDestination = destination + offset - rotation * new Vector3((unitsPerRow - 1) * spacing / 2, 0, (unitsPerRow - 1) * spacing / 2);

                NavMeshHit navHit;
                if (NavMesh.SamplePosition(unitDestination, out navHit, 1.0f, NavMesh.AllAreas))
                {
                    unit.MoveToPosition(navHit.position);
                }
                else
                {
                    Debug.Log($"유닛 {unit.gameObject.name}이(가) NavMesh 위에 있지 않습니다.");
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
