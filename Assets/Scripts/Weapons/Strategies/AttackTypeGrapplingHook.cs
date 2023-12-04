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
    public Rigidbody2D m_rigidbody;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)] [SerializeField] private float rotationSpeed = 4;

    [Header("Launching:")]
    [SerializeField] private bool launchToPoint = true;
    [SerializeField] private float launchSpeed = 25;

    [HideInInspector] public Vector3 grapplePoint;
    [HideInInspector] public GameObject grappleHitObject;
    [HideInInspector] public Vector2 grappleDistanceVector;

    private bool canGrapple = true;

    private void Start()
    {
        grappleRope.enabled = false;

        weapon.OnTrigger += DoAttack;

        weapon.OnTriggerRelease += ReleaseGrapple;
        weapon.OnHolseter += ReleaseGrapple;
        weapon.item.OnUnequip += ReleaseGrapple;

        m_rigidbody = weapon.item.owner.GetComponent<Rigidbody2D>();
    }

    public void DoAttack(float charge)
    {
        if (!canGrapple) { return; }

        if (grappleRope.enabled == false) {
            SetGrapplePoint();
        } else {
            // RotateGun(grapplePoint, false);
        
            if (launchToPoint && grappleRope.isGrappling) {

                Vector3 firePointDistnace = weapon.attackPoint.position - weapon.item.owner.transform.position;
                Vector3 targetPos = grapplePoint - firePointDistnace;

                if (Vector3.Distance(weapon.item.owner.transform.position, targetPos) < 1f) {
                    grappleRope.enabled = false;
                    canGrapple = false;
                    return;
                }

                Vector3 dir = (targetPos - weapon.item.owner.transform.position).normalized;
                weapon.item.owner.movement += dir * launchSpeed * Time.deltaTime;
                
            }
        }
    }

    void ReleaseGrapple() {
        grappleRope.enabled = false;
        canGrapple = true;
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
            grappleHitObject = hit.transform.gameObject;

            if (Vector2.Distance(hit.point, weapon.attackPoint.position) <= weapon.GetStatValue(WeaponStatNames.Range)) {
                grapplePoint = hit.point;
                grappleDistanceVector = grapplePoint - weapon.item.owner.transform.position;
                grappleRope.enabled = true;
            }
        }
    }

    public void Grapple()
    {
        m_rigidbody.velocity = Vector2.zero;

        if (grappleHitObject.GetComponent<ObjectHandler>()) {
            weapon.CallOnAttack();
        }
    }
}
