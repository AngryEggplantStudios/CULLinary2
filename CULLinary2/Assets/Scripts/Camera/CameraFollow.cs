using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform target;
    [SerializeField] private float fixedY;
    [SerializeField] private float translateSpeed;
    [SerializeField] private float rotationSpeed;

    private void FixedUpdate()
    {
        Vector3 targetPosition = target.TransformPoint(offset);
        Vector3 targetFixedY = new Vector3(targetPosition.x, fixedY, targetPosition.z);
        transform.position = Vector3.Lerp(transform.position, targetFixedY, translateSpeed * Time.deltaTime);

        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion rotationFixedXandZ = Quaternion.Euler(
            transform.rotation.eulerAngles.x, rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}
