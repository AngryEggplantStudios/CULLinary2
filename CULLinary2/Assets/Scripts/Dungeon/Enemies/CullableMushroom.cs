using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullableMushroom : MonoBehaviour
{
    void Start()
    {
        ObjectCuller.Instance.AddMushroom(this.gameObject.GetComponent<MonsterScript>());
    }

    void OnDestroy()
    {
        ObjectCuller.Instance.RemoveMushroom(this.gameObject.GetComponent<MonsterScript>());
    }
}
