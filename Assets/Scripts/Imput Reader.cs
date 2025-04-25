using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ImputReader : MonoBehaviour
{
    public event Action<Vector2> OnMovementInput;
    public event Action<Vector2> OnLookInput;
    public event Action<bool> OnRunningInput;
    public bool canHandleInput = true;

    public void HandleMovement(InputAction.CallbackContext context)
    {
        if (!canHandleInput) return;

        if (context.performed)
        {
            Vector2 movementInput = context.ReadValue<Vector2>();
            OnMovementInput?.Invoke(movementInput);
        }
        else if (context.canceled)
        {
            OnMovementInput?.Invoke(Vector2.zero);
        }
    }

    public void HandleLook(InputAction.CallbackContext context)
    {
        if (!canHandleInput) return;

        if (context.performed || context.canceled)
        {
            Vector2 lookInput = context.ReadValue<Vector2>();
            OnLookInput?.Invoke(lookInput);
        }
    }
    public void HandleRunning(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnRunningInput?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnRunningInput?.Invoke(false);
        }
    }
}
