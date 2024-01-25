using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICustomQuadScript : Graphic, ICanvasRaycastFilter {

    public Vector2[] VertexPositions = new Vector2[4];

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) {

        //offset
        Vector3[] array = new Vector3[4];
        rectTransform.parent.GetComponent<RectTransform>().GetWorldCorners(array);
        sp -= (Vector2)array[0];
        
        if (PointInTriangle(VertexPositions[0], VertexPositions[1], VertexPositions[2], sp) || PointInTriangle(VertexPositions[2], VertexPositions[3], VertexPositions[0], sp)) {
            return true;
        }

        return false;
    }

    private bool PointInTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 p) {
        Vector2 d, e;
        float w1, w2;
        d = b - a;
        e = c - a;
        w1 = (e.x * (a.y - p.y) + e.y * (p.x - a.x)) / (d.x * e.y - d.y * e.x);
        w2 = (p.y - a.y - w1 * d.y) / e.y;
        return (w1 >= 0.0) && (w2 >= 0.0) && ((w1 + w2) <= 1.0);
    }

    protected override void OnPopulateMesh(VertexHelper vh) {
        base.OnPopulateMesh(vh);

        vh.Clear();

        for (int i = 0; i < VertexPositions.Length; i++) {
            vh.AddVert(VertexPositions[i], color, Vector4.one);
        }

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    protected override void OnDestroy() {
        base.OnDestroy();

        GetComponent<HoverScript>().OnPointerExit(new PointerEventData(EventSystem.current));
    }
}
