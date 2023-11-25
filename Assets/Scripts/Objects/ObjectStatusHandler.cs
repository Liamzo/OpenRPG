using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStatusHandler : MonoBehaviour
{
    GameManager gameManager;

    public bool hasMovement {get; set;} = true;
    int canRegainStamina = 0;
    public int hasMovementControls {get; set;} = 0;
    public int hasControls {get; set;} = 0;

    public bool isDodging  {get; set;} = false;


    private void Start() {
        gameManager = GameManager.instance;
    }


    public bool HasMovement() {
        if (gameManager.gamePaused || gameManager.waitingHitStop || !hasMovement)
            return false;
        
        return true;
    }


    public bool CanRegainStamina () {
        if (gameManager.gamePaused || gameManager.waitingHitStop || canRegainStamina > 0) 
            return false;
        else 
            return true;
    }
    public void BlockRegainStamina (float duration) {
        StartCoroutine("RegainStaminaBlocker", duration);
    }
    IEnumerator RegainStaminaBlocker(float duration) {
        canRegainStamina += 1;

        yield return new WaitForSeconds(duration);

        canRegainStamina -= 1;
    }


    public bool HasMovementControls() {
        if (gameManager.gamePaused || gameManager.waitingHitStop || hasMovementControls > 0)
            return false;
        
        return true;
    }
    public void BlockMovementControls() {
        hasMovementControls += 1;
    }
    public void BlockMovementControls(float duration) {
        StartCoroutine("MovementControlsBlocker", duration);
    }
    public void UnblockMovementControls() {
        hasMovementControls -= 1;
    }
    IEnumerator MovementControlsBlocker(float duration) {
        hasMovementControls += 1;

        yield return new WaitForSeconds(duration);

        hasMovementControls -= 1;
    }


    public bool HasControls() {
        if (gameManager.gamePaused || gameManager.waitingHitStop || hasControls > 0)
            return false;
        
        return true;
    }
    public void BlockControls() {
        hasControls += 1;
    }
    public void BlockControls(float duration) {
        StartCoroutine("ControlsBlocker", duration);
    }
    public void UnblockControls() {
        hasControls -= 1;
    }
    IEnumerator ControlsBlocker(float duration) {
        hasControls += 1;

        yield return new WaitForSeconds(duration);

        hasControls -= 1;
    }
}
