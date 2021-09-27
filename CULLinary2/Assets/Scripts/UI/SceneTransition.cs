using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Animator sceneTransition;

    void OnEnable()
    {
        Debug.Log("scene transition enabled");
        sceneTransition.StartPlayback();
        // gameObject.SetActive(false);
    }
}
