using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingMaterial : MonoBehaviour
{
    public float scrollSpeed = 0.1f;

    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        Material material = lineRenderer.material;
        Vector2 offset = material.GetTextureOffset("_MainTex");
        offset.x += Time.deltaTime * scrollSpeed;
        material.SetTextureOffset("_MainTex", offset);
    }
}
