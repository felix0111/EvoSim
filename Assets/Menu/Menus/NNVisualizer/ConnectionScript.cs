using NeuraSuite.NeatExpanded;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConnectionScript : MonoBehaviour, IPointerClickHandler {

    private TMP_Text _weightText;
    private Connection _connection;
    private Image _image;
    private RectTransform _rectTransform, _textTransform;
    private readonly float _lineWidth = 2f;

    public void UpdateConnection(Connection connection, Vector3 pos1, Vector3 pos2, Color color) {
        _connection = connection;

        MakeLine(pos1, pos2, null, color);
        
        if(_weightText == null) AddWeightText();
        _weightText.text = connection.Weight.ToString("F1");
    }

    private void MakeLine(Vector2 start, Vector2 end, Sprite sprite, Color color) {
        gameObject.name = "line from " + start.x + " to " + end.x;
        gameObject.layer = LayerMask.NameToLayer("UI");

        if(_image == null) _image = gameObject.GetComponent<Image>();
        _image.sprite = sprite;
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
        _rectTransform.sizeDelta = new Vector2(dif.magnitude, _lineWidth);
        _rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / (dif.x + 0.0001f)) / Mathf.PI));
    }

    private void AddWeightText() {
        //add weight text
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(transform, false);
        _weightText = textObject.AddComponent<TextMeshProUGUI>();
        _weightText.text = _connection.Weight.ToString("F1");
        _weightText.fontSize = 20f;
        _weightText.alignment = TextAlignmentOptions.Center;

        _textTransform = textObject.GetComponent<RectTransform>();
        _textTransform.sizeDelta = new Vector2(100, 25);
        _textTransform.anchoredPosition = new Vector3(0, 20, 0);
    }

    public void OnPointerClick(PointerEventData data) {
        SimulationScript.Instance.MenuManager.EntityMenu.NNVisualizerMenu.OnClickConnection(data, _connection);
    }

}
