using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class DrawSurface : MonoBehaviour
{
    [SerializeField] private RtResolutions texSize = RtResolutions._256;
    [Header("Dynamic Settings")] [SerializeField]
    private RenderTexture targetTexture;
    private RenderTexture auxTexture;
    private new Renderer renderer;
    private Material material;
   
    private static readonly int DisplaceTexId = Shader.PropertyToID("_DisplaceTex");
    private static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    private static readonly int SourceTexCoordsId = Shader.PropertyToID("_SourceTexCoords");
    private static readonly int SurfaceTexId = Shader.PropertyToID("_SurfaceTex");

    private enum RtResolutions
    {
        _32 = 32,
        _64 = 64,
        _128 = 128,
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048
    }

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        material = renderer.material;
        targetTexture = new RenderTexture((int) texSize, (int) texSize, 0, RenderTextureFormat.RFloat);
        auxTexture = new RenderTexture((int) texSize, (int) texSize, 0, RenderTextureFormat.RFloat);
        material.SetTexture(DisplaceTexId, targetTexture);
    }

    private void OnDisable()
    {
        targetTexture.Release();
        auxTexture.Release();
    }

    public bool TryDraw(BaseBrush baseBrush, Ray brushRay, out DrawData drawData)
    {
        drawData = new DrawData();
        if(Physics.Raycast(brushRay, out var hit))
        {
            if(hit.collider.gameObject == gameObject)
            {
                float x = hit.textureCoord.x;
                float y = hit.textureCoord.y;
                DrawAt(x * targetTexture.width, y * targetTexture.height, baseBrush);
                drawData.worldPoint = hit.point;
                return true;
            }
        }
        return false;
    }

    private void DrawAt(float x, float y, BaseBrush brush)
    {
        Graphics.Blit(targetTexture, auxTexture);
        RenderTexture.active = targetTexture;

        GL.PushMatrix();
        GL.LoadPixelMatrix(0, targetTexture.width, targetTexture.height, 0);

        x = Mathf.Round(x);
        y = Mathf.Round(y);

        // Calculate draw rect
        Vector2 bw = new Vector2((float) targetTexture.width / brush.texture.width, (float) targetTexture.height / brush.texture.height) * brush.size;
        Rect screenRect = new Rect
        {
            x = x - bw.x * 0.5f,
            y = (targetTexture.height - y) - bw.y * 0.5f,
            width = bw.x,
            height = bw.y
        };

        // put the center of the stamp at the actual draw position
        var tempVec = new Vector4
        {
            x = screenRect.x / targetTexture.width,
            y = 1 - screenRect.y / targetTexture.height,
            z = screenRect.width / targetTexture.width,
            w = screenRect.height / targetTexture.height
        };

        tempVec.y -= tempVec.w;

        brush.material.SetTexture(MainTexId, brush.texture);
        brush.material.SetVector(SourceTexCoordsId, tempVec);
        brush.material.SetTexture(SurfaceTexId, auxTexture);

        // Draw the texture
        Graphics.DrawTexture(screenRect, brush.texture,  brush.material);

        GL.PopMatrix();
        RenderTexture.active = null;
    }
}