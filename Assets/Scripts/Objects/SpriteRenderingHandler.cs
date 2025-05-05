using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRenderingHandler : MonoBehaviour
{
    public Camera renderCamera;
    public MeshRenderer meshRenderer;

    void Awake() {
        renderCamera = GetComponentInChildren<Camera>();
        meshRenderer = GetComponent<MeshRenderer>();

        RenderTexture renderTexture = new RenderTexture(32,32,0,RenderTextureFormat.ARGB32);
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();
        renderCamera.targetTexture = renderTexture;
        meshRenderer.material.SetTexture("_MainTex", renderTexture);
    }
}
