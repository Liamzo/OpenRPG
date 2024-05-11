using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStatusHandler : MonoBehaviour
{
    public bool hasMovement {get; set;} = true;
    int canRegainStamina = 0;
    public int hasMovementControls {get; set;} = 0;
    public int hasControls {get; set;} = 0;

    public bool isDodging  {get; set;} = false;

    public bool isBlocking {get; set;} = false;
    public float blockAngle = 0f;



    public bool HasMovement() {
        if (GameManager.instance.gamePaused || GameManager.instance.waitingHitStop || !hasMovement)
            return false;
        
        return true;
    }


    public bool CanRegainStamina () {
        if (GameManager.instance.gamePaused || GameManager.instance.waitingHitStop || canRegainStamina > 0) 
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
        if (GameManager.instance.gamePaused || GameManager.instance.waitingHitStop || hasMovementControls > 0)
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
        if (GameManager.instance.gamePaused || GameManager.instance.waitingHitStop || hasControls > 0)
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


    public void Block(float angle) {
        isBlocking = true;
        blockAngle = angle;
    }

    public void StopBlock() {
        isBlocking = false;
        blockAngle = 0f;
    }
}
