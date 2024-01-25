using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverScript : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler {

    public Color TooltipBackground;
    public Sprite TooltipSprite;
    public string TooltipText;
    public Vector2 TooltipSize;
    public int FontSize;

    private GameObject _tooltip;

    public void OnPointerMove(PointerEventData eventData) {
        _tooltip.transform.GetChild(0).position = Input.mousePosition;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        _tooltip = new GameObject("Tooltip", typeof(Canvas));
        var can = _tooltip.GetComponent<Canvas>();
        can.renderMode = RenderMode.ScreenSpaceOverlay;
        can.worldCamera = Camera.main;
        can.sortingOrder = 10;

        var bg = new GameObject("TooltipBackground", typeof(Image)).GetComponent<Image>();
        var rectBg = bg.GetComponent<RectTransform>();
        var img = bg.GetComponent<Image>();
        bg.transform.SetParent(_tooltip.transform);
        rectBg.sizeDelta = TooltipSize;
        rectBg.pivot = Vector2.zero;
        img.raycastTarget = false;
        img.color = TooltipBackground;
        img.sprite = TooltipSprite;
        img.type = Image.Type.Sliced;

        var text = new GameObject("TooltipText", typeof(TextMeshProUGUI)).GetComponent<TMP_Text>();
        text.transform.SetParent(bg.transform);
        text.text = TooltipText;
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = FontSize;
        text.raycastTarget = false;
        text.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        _tooltip.transform.GetChild(0).position = Input.mousePosition;
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (_tooltip == null) return;
        Destroy(_tooltip.gameObject);
    }
}
