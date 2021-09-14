using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootRotator : MonoBehaviour
{
    public float amplitude = 0.05f;
    public float verticalSpeed = 2f;
    public float rotationSpeed = 90;
    private float originalY;
    private void Start()
    {
        originalY = transform.position.y;
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, rotationSpeed, 0) * Time.deltaTime);

        Vector3 pos = transform.position;
        float newY = (Mathf.Sin(Time.time * verticalSpeed)) * amplitude;
        //Debug.Log(newY);
        transform.position = new Vector3(pos.x, newY + originalY, pos.z);
    }
}
