using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// This script acts as a single point for all other scripts to get
// the current input from. It uses Unity's new Input System and
// functions should be mapped to their corresponding controls
// using a PlayerInput component with Unity Events.

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    private Vector2 moveDirection = Vector2.zero;
    private bool leftMousePressed = false;
    private bool rightMousePressed = false;
    private bool inventoryPressed = false;
    private bool interactPressed = false;
    private bool submitPressed = false;
    private bool menuPressed = false;
    private bool tradePressed = false;
    private bool offerTradePressed = false;
    private bool dashPressed = false;
    private bool tabPressed = false;
    private bool journalPressed = false;
    private bool sprintPressed = false;
    private bool mapPressed = false;
    private bool holsterPressed = false;
    private bool toggleRangedPressed = false;

    private static InputManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Input Manager in the scene.");
        }
        instance = this;
    }

    public static InputManager GetInstance() 
    {
        return instance;
    }

    public void MovePresed(InputAction.CallbackContext context) {
        if (context.performed)
        {
            moveDirection = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            moveDirection = context.ReadValue<Vector2>();
        } 
    }

    public void LeftMousePressed(InputAction.CallbackContext context) {
        if (context.performed)
        {
            leftMousePressed = true;
        }
        else if (context.canceled)
        {
            leftMousePressed = false;
        }
    }

    public void RightMousePressed(InputAction.CallbackContext context) {
        if (context.performed)
        {
            rightMousePressed = true;
        }
        else if (context.canceled)
        {
            rightMousePressed = false;
        }
    }

    public void InventoryButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            inventoryPressed = true;
        }
        else if (context.canceled)
        {
            inventoryPressed = false;
        } 
    }
    
    public void InteractButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactPressed = true;
        }
        else if (context.canceled)
        {
            interactPressed = false;
        } 
    }

    public void SubmitPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            submitPressed = true;
        }
        else if (context.canceled)
        {
            submitPressed = false;
        } 
    }

    public void MenuPressed(InputAction.CallbackContext context) {
        if (context.performed)
        {
            menuPressed = true;
        }
        else if (context.canceled)
        {
            menuPressed = false;
        } 
    }

    public void TradePressed(InputAction.CallbackContext context) {
        if (context.performed)
        {
            tradePressed = true;
        }
        else if (context.canceled)
        {
            tradePressed = false;
        } 
    }

    public void OfferTradePressed(InputAction.CallbackContext context) {
        if (context.performed)
        {
            offerTradePressed = true;
        }
        else if (context.canceled)
        {
            offerTradePressed = false;
        } 
    }

    public void DashPressed(InputAction.CallbackContext context) {
        if (context.performed)
        {
            dashPressed = true;
        }
        else if (context.canceled)
        {
            dashPressed = false;
        } 
    }

    public void TabPressed(InputAction.CallbackContext context) {
        if (context.performed)
        {
            tabPressed = true;
        }
        else if (context.canceled)
        {
            tabPressed = false;
        } 
    }

    public void JournalPressed(InputAction.CallbackContext context) {
        if (context.performed)
        {
            journalPressed = true;
        }
        else if (context.canceled)
        {
            journalPressed = false;
        } 
    }

    public void SprintPressed(InputAction.CallbackContext context) {
        if (context.performed)
        {
            sprintPressed = true;
        }
        else if (context.canceled)
        {
            sprintPressed = false;
        } 
    }

    public void MapPressed(InputAction.CallbackContext context) {
        if (context.performed)
        {
            mapPressed = true;
        }
        else if (context.canceled)
        {
            mapPressed = false;
        } 
    }

    public void HolsterPressed(InputAction.CallbackContext context) {
        if (context.performed)
        {
            holsterPressed = true;
        }
        else if (context.canceled)
        {
            holsterPressed = false;
        } 
    }
    public void ToggleRangedPressed(InputAction.CallbackContext context) {
        if (context.performed)
        {
            toggleRangedPressed = true;
        }
        else if (context.canceled)
        {
            toggleRangedPressed = false;
        } 
    }


    public Vector2 GetMoveDirection() 
    {
        return moveDirection;
    }

    public bool GetLeftMousePressed() {
        return leftMousePressed;
    }
    public void UseLeftMousePressed() {
        leftMousePressed = false;
    }

    public bool GetRightMousePressed() {
        return rightMousePressed;
    }

    public bool GetDashPressed() 
    {
        return dashPressed;
    }

    public bool GetSprintPressed() 
    {
        return sprintPressed;
    }
    public bool GetToggleRangedPressed() 
    {
        return toggleRangedPressed;
    }

    // for any of the below 'Get' methods, if we're getting it then we're also using it,
    // which means we should set it to false so that it can't be used again until actually
    // pressed again.

    public bool GetInteractPressed() 
    {
        bool result = interactPressed;
        interactPressed = false;
        return result;
    }

    public bool GetInventoryPressed() 
    {
        bool result = inventoryPressed;
        inventoryPressed = false;
        return result;
    }
    public bool GetSubmitPressed() 
    {
        bool result = submitPressed;
        submitPressed = false;
        return result;
    }

    public bool GetMenuPressed() 
    {
        bool result = menuPressed;
        menuPressed = false;
        return result;
    }

    public bool GetTradePressed() 
    {
        bool result = tradePressed;
        tradePressed = false;
        return result;
    }

    public bool GetOfferTradePressed() 
    {
        bool result = offerTradePressed;
        offerTradePressed = false;
        return result;
    }

    public bool GetTabPressed() 
    {
        bool result = tabPressed;
        tabPressed = false;
        return result;
    }

    public bool GetJournalPressed() 
    {
        bool result = journalPressed;
        journalPressed = false;
        return result;
    }

    public bool GetMapPressed() 
    {
        bool result = mapPressed;
        mapPressed = false;
        return result;
    }

    public bool GetHolsterPressed() 
    {
        bool result = holsterPressed;
        holsterPressed = false;
        return result;
    }

    public void RegisterSubmitPressed() 
    {
        submitPressed = false;
    }

}