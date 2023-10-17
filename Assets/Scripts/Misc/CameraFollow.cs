using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0,0,-10);

    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null) {
            return;
        }
        Vector3 targetPos = target.GetComponent<Rigidbody2D>().position;
        targetPos += offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        //Vector3 movePos = Vector3.Lerp(transform.position, targetPos, smoothSpeed);

        //transform.position = movePos;
    }
}
