using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NoFog : MonoBehaviour {
    bool doWeHaveFogInScene;

    void Start()
    {
        doWeHaveFogInScene = RenderSettings.fog;
    }

    void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += RenderPipelineManager_beginCameraRendering;
        RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= RenderPipelineManager_beginCameraRendering;
        RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
    }
    
    private void RenderPipelineManager_beginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        OnPreRender();
    }

    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        OnPostRender();
    }

    void OnPreRender()
    {
        RenderSettings.fog = false;
    }
    
    void OnPostRender()
    {
        RenderSettings.fog = doWeHaveFogInScene;
    }
}
