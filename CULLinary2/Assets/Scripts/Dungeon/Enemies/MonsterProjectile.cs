using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    private Vector3 projDir;

    [SerializeField] private float moveSpeed;
    [SerializeField] private int damage;
    [SerializeField] private float heightFromGround;

    private float spinSpeed = 500;

    //Remember to setup Environment
    public void Setup(Vector3 sourcePosition, Vector3 targetPosition)
    {
        this.projDir = (targetPosition - sourcePosition).normalized;
        transform.position += new Vector3(0, heightFromGround, 0);
        transform.eulerAngles = new Vector3(0, CalculateAngle(projDir), 90);
        Destroy(gameObject, 5);
    }

    private float CalculateAngle(Vector3 v)
    {
        v = v.normalized;
        float n = Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg + 90;
        if (n < 0)
        {
            n += 360;
        }
        return n;
    }

    private float CalculateDuration(Vector3 sourcePosition, Vector3 targetPosition)
    {
        return Vector3.Distance(sourcePosition, targetPosition) / moveSpeed;
    }

    private void Update()
    {
        transform.position += projDir * moveSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, heightFromGround, transform.position.z); 
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }


    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth target = other.GetComponent<PlayerHealth>();
        if (target != null)
        {
            target.HandleHit(damage);
            Destroy(gameObject);
        }
        //To Handle 
        if (other.gameObject.tag == "Environment")
        {
            Destroy(gameObject);
        }
    }

}
