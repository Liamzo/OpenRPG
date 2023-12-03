using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTypeGrapplingHook : BaseStrategy, IAttackType
{
    [Header("Scripts Ref:")]
    public GrappleRope grappleRope;

    [Header("Layers Settings:")]
    [SerializeField] private bool grappleToAll = false;
    [SerializeField] private int grappableLayerNumber = 9;

    [Header("Physics Ref:")]
    // public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)] [SerializeField] private float rotationSpeed = 4;

    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch
    }

    [Header("Launching:")]
    [SerializeField] private bool launchToPoint = true;
    [SerializeField] private LaunchType launchType = LaunchType.Transform_Launch;
    [SerializeField] private float launchSpeed = 1;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    private void Start()
    {
        grappleRope.enabled = false;
        //m_springJoint2D.enabled = false;

        weapon.OnTrigger += DoAttack;
        weapon.OnTriggerRelease += ReleaseGrapple;

        m_rigidbody = weapon.item.owner.GetComponent<Rigidbody2D>();
    }

    public void DoAttack(float charge)
    {
        if (grappleRope.enabled == false) {
            SetGrapplePoint();
        } else {
            // RotateGun(grapplePoint, false);
        
            if (launchToPoint && grappleRope.isGrappling)
            {
                if (launchType == LaunchType.Transform_Launch)
                {
                    Vector2 firePointDistnace = weapon.attackPoint.position - weapon.item.owner.transform.position;
                    Vector2 targetPos = grapplePoint - firePointDistnace;
                    Vector3 move = Vector3.Lerp(weapon.item.owner.transform.position, targetPos, Time.deltaTime * launchSpeed) - weapon.item.owner.transform.position;
                    weapon.item.owner.movement += move;
                }
            }
        }
    }

    void ReleaseGrapple() {
        grappleRope.enabled = false;
        //m_springJoint2D.enabled = false;
    }

    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        Vector3 distanceVector = lookPoint - weapon.item.owner.transform.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
        {
            weapon.item.owner.transform.rotation = Quaternion.Lerp(weapon.item.owner.transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
        {
            weapon.item.owner.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void SetGrapplePoint()
    {
        Vector2 distanceVector = Camera.main.ScreenToWorldPoint(Input.mousePosition) - weapon.item.owner.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(weapon.attackPoint.position, distanceVector.normalized);
        if (!hit) {return;}
        
        if (hit.transform.gameObject.layer == grappableLayerNumber || grappleToAll)
        {
            if (Vector2.Distance(hit.point, weapon.attackPoint.position) <= weapon.GetStatValue(WeaponStatNames.Range)) {
                grapplePoint = hit.point;
                grappleDistanceVector = grapplePoint - (Vector2)weapon.item.owner.transform.position;
                grappleRope.enabled = true;
            }
        }
    }

    public void Grapple()
    {
        // m_springJoint2D.autoConfigureDistance = false;
        // if (!launchToPoint && !autoConfigureDistance)
        // {
        //     m_springJoint2D.distance = targetDistance;
        //     m_springJoint2D.frequency = targetFrequncy;
        // }
        // if (!launchToPoint)
        // {
        //     if (autoConfigureDistance)
        //     {
        //         m_springJoint2D.autoConfigureDistance = true;
        //         m_springJoint2D.frequency = 0;
        //     }

        //     m_springJoint2D.connectedAnchor = grapplePoint;
        //     m_springJoint2D.enabled = true;
        // }
        // else
        // {
        //     switch (launchType)
        //     {
        //         case LaunchType.Physics_Launch:
        //             m_springJoint2D.connectedAnchor = grapplePoint;

        //             Vector2 distanceVector = weapon.attackPoint.position - weapon.item.owner.transform.position;

        //             m_springJoint2D.distance = distanceVector.magnitude;
        //             m_springJoint2D.frequency = launchSpeed;
        //             m_springJoint2D.enabled = true;
        //             break;
        //         case LaunchType.Transform_Launch:
        //             m_rigidbody.velocity = Vector2.zero;
        //             break;
        //     }
        // }

        m_rigidbody.velocity = Vector2.zero;
    }
}
