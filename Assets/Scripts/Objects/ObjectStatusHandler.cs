using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStatusHandler : MonoBehaviour
{
    GameManager gameManager;

    private void Start() {
        gameManager = GameManager.instance;
    }

    public bool hasMovement {get; set;} = true;
    public int hasMovementControls {get; set;} = 0;

    public int hasControls {get; set;} = 0;


    public bool isDodging  {get; set;} = false;

    int canRegainStamina = 0;
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
    public void UnblockMovementControls() {
        hasMovementControls -= 1;
    }

    public bool HasMovement() {
        if (gameManager.gamePaused || gameManager.waitingHitStop || !hasMovement)
            return false;
        
        return true;
    }

    public bool HasControls() {
        if (gameManager.gamePaused || gameManager.waitingHitStop || hasControls > 0)
            return false;
        
        return true;
    }
    public void BlockControls() {
        hasControls += 1;
    }
    public void UnblockControls() {
        hasControls -= 1;
    }
}
