using Unity.VisualScripting;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerMovement playerInput;

    public Vector2 moveInput;
    public bool leftShiftKey_Pressed = false;

    private void Awake()
    {
        playerInput = new PlayerMovement();
    }

    private void OnEnable()
    {
        playerInput.Enable();

        playerInput.PlayerMove.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.PlayerMove.Move.canceled += ctx => moveInput = Vector2.zero;

        playerInput.PlayerMove.Sprint.performed += ctx => leftShiftKey_Pressed = true;
        playerInput.PlayerMove.Sprint.canceled += ctx => leftShiftKey_Pressed = false;
    }
}
