using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineScript : MonoBehaviour {

    public float LineWidth;

    private Image _image;
    private RectTransform _rectTransform;

    //called once directly after Instantiating
    void Awake() {

    }

    public void InitLine(Vector2 start, Vector2 end, Color color) {
        gameObject.name = "line from " + start.x + " to " + end.x;
        gameObject.layer = LayerMask.NameToLayer("UI");

        if (_image == null) _image = gameObject.GetComponent<Image>();
        //_image.sprite = sprite;
        _image.color = color;
        _image.raycastPadding = new Vector4(0, 5, 5, 0);

        _rectTransform = gameObject.GetComponent<RectTransform>();
        _rectTransform.localScale = Vector3.one;

        // set them to start bottom-left
        _rectTransform.anchorMin = Vector2.zero;
        _rectTransform.anchorMax = Vector2.zero;

        Vector3 a = new Vector3(start.x * 1, start.y * 1, 0);
        Vector3 b = new Vector3(end.x * 1, end.y * 1, 0);

        _rectTransform.position = (a + b) / 2;
        Vector3 dif = a - b;
        _rectTransform.sizeDelta = new Vector2(dif.magnitude, LineWidth);
        _rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / (dif.x + 0.0001f)) / Mathf.PI));
    }

}
