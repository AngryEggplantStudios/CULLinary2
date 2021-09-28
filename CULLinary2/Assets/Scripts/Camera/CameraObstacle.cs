using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObstacle : MonoBehaviour
{
    public GameObject solid;
    public GameObject transparent;

    void Start()
    {
        ShowSolid();
    }

    public void ShowTransparent()
    {
        solid.SetActive(false);
        transparent.SetActive(true);
    }
    
    public void ShowSolid()
    {
        solid.SetActive(true);
        transparent.SetActive(false);
    }
}
