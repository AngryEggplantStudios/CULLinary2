using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootSolver : Monster
{
    [SerializeField] Transform body = default;
    [SerializeField] IKFootSolver otherFoot = default;
    [SerializeField] float speed = 5;
    [SerializeField] float stepDistance = 20;
    [SerializeField] float stepLength = 20;
    [SerializeField] float stepHeight = 5;
    [SerializeField] Vector3 footOffset = default;
    [SerializeField] GameObject collision;

    // Audio
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip meleeSound;
    [SerializeField] private AudioClip stepSound;

    float footSpacing;
    public Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp;
    private bool isAttacking;
    private BossMeleeAttack meleeAttackScript;
    private ClownController parentController;

    private void Start()
    {
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = -transform.up;
        lerp = 1;
        isAttacking = false;
        meleeAttackScript = gameObject.transform.parent.gameObject.transform.GetComponentInChildren<BossMeleeAttack>();
        parentController = gameObject.GetComponentInParent<ClownController>();
    }

    void Update()
    {
        transform.position = currentPosition;
        transform.up = -currentNormal;

        Ray ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit info, 100, ~(1 << 5)))
        {
            if (Vector3.Distance(newPosition, info.point) > stepDistance && !otherFoot.IsMoving() && !IsMoving())
            {
                int direction = body.InverseTransformPoint(info.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                SetTarget(info.point + (body.forward * stepLength * direction) + footOffset,
                        info.normal);
            }
        }
        else
        {
            Debug.Log("No suitable stepping spot for " + transform.parent.name);
        }

        if (IsMoving())
        {
            if (lerp > 0.5 && isAttacking)
            {
                if (lerp > 0.9)
                {
                    meleeAttackScript.enableCollider(true);
                }
                meleeAttackScript.enableSprite(true);
            }

            //Collission boxx isn't attached to IKFoot need to follow
            collision.transform.position = new Vector3(newPosition.x, newPosition.y, newPosition.z);
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = tempPosition;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;

            if (!IsMoving())
            {
                meleeAttackScript.enableCollider(false);
                meleeAttackScript.enableSprite(false);
                if (isAttacking)
                {
                    audioSource.clip = meleeSound;
                    audioSource.Play();
                } else {
                    audioSource.clip = stepSound;
                    audioSource.Play();
                }
                isAttacking = false;
            }
        }
    }

    public void SetTarget(Vector3 pos, Vector3 normal)
    {
        lerp = 0;
        oldPosition = currentPosition;
        oldNormal = currentNormal;
        newPosition = pos;
        newNormal = normal;
    }
    public void meleeAttackStart()
    {
        isAttacking = true;
    }

    public void meleeAttackEnd()
    {
        isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(body.position + (body.right * footSpacing), Vector3.down * 100, Color.red);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.1f);
    }

    public bool IsMoving()
    {
        return lerp < 1;
    }
    public override void HandleHit(float damage)
    {
        parentController.HandleHit(damage);
    }
}
