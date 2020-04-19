using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : MonoBehaviour
{
    [SerializeField] private BaseBrush brush;
    [SerializeField] private DrawSurface drawSurface;
    [SerializeField] private float minBrushSize;
    [SerializeField] private float maxBrushSize;
    [SerializeField] private float growSpeed;
    private float dynamicProgress;
    private Vector3 oldPos;

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Ray drawRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(drawSurface.TryDraw(brush, drawRay, out DrawData drawData))
            {
                
                float dist = (drawData.worldPoint - oldPos).magnitude;

                oldPos = drawData.worldPoint;
                if(dist > 0.001f)
                {
                    brush.size = Mathf.Lerp(brush.size, minBrushSize, growSpeed * Time.deltaTime * 3);
                } else
                {
                    brush.size = Mathf.Lerp(brush.size, maxBrushSize, growSpeed * Time.deltaTime);
                }

            }
        } else
        {
            brush.size = minBrushSize;
        }
    }
}
