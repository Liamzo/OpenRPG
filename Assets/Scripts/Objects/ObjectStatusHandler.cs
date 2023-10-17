using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStatusHandler : MonoBehaviour
{
    GameManager gameManager;

    private void Start() {
        gameManager = GameManager.instance;
    }

    public bool hasMovementControls {get; set;} = true;
    public bool hasMovement {get; set;} = true;

    public bool hasControls {get; set;} = true;


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
        if (gameManager.gamePaused || gameManager.waitingHitStop || !hasMovementControls)
            return false;
        
        return true;
    }

    public bool HasMovement() {
        if (gameManager.gamePaused || gameManager.waitingHitStop || !hasMovement)
            return false;
        
        return true;
    }

    public bool HasControls() {
        if (gameManager.gamePaused || gameManager.waitingHitStop || !hasControls)
            return false;
        
        return true;
    }
}
