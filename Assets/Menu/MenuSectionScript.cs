using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuSectionScript : MonoBehaviour {

    public TMP_Text Headline;
    public RectTransform ContentRect;

    public void UpdateContentSize() {

        float sizeY = ContentRect.GetComponent<GridLayoutGroup>().padding.top;

        foreach (RectTransform rectTransform in ContentRect) {
            sizeY += rectTransform.rect.height + ContentRect.GetComponent<GridLayoutGroup>().spacing.y;
        }

        ContentRect.sizeDelta = new Vector2(ContentRect.sizeDelta.x, sizeY);

    }

}
