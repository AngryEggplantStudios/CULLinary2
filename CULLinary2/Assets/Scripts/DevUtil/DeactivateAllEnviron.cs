using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class DeactivateAllEnviron : MonoBehaviour
{
    private static List<GameObject> generatedRooms;

    // Start is called before the first frame update
    void Start()
    {
        generatedRooms = new List<GameObject>();
        foreach (Transform child in transform)
            child.transform.Find("Environment").Find("Deco").gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
