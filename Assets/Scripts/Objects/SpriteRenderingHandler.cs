using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteRenderingHandler : MonoBehaviour
{
    Camera renderCamera;
    MeshRenderer meshRenderer;

    public int defaultLayer = 11;
    public int tempLayer = 13;
    List<SpriteRenderer> spriteRenderers = new ();

    void Awake() {
        renderCamera = GetComponentInChildren<Camera>(true);
        meshRenderer = GetComponent<MeshRenderer>();

        RenderTexture renderTexture = new RenderTexture(32,32,0,RenderTextureFormat.ARGB32);
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();
        renderCamera.targetTexture = renderTexture;
        meshRenderer.material.SetTexture("_MainTex", renderTexture);


        // defaultLayer = defaultMask;
        // tempLayer = tempMask;

        foreach (SpriteRenderer spriteRenderer in transform.parent.GetComponentsInChildren<SpriteRenderer>().Where(sprite => sprite.gameObject.layer == defaultLayer)) {
            spriteRenderers.Add(spriteRenderer);
        }
    }

    void LateUpdate() {
        SetLayer(tempLayer);
        renderCamera.Render();
        SetLayer(defaultLayer);
    }

    void SetLayer(int layer) {
        spriteRenderers.ForEach(spriteRenderer => spriteRenderer.gameObject.layer = layer);
    }
}
