using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SliderUpdateScript : MonoBehaviour {

    void Start() {
        OnValueChanged();
    }

    public void OnValueChanged() {
        if (TryGetComponent(out Slider slider)) {
            slider.transform.parent.Find("SliderValue").GetComponent<TMP_Text>().text = slider.value.ToString(slider.wholeNumbers ? "0" : "F1");
        } else if (TryGetComponent(out RangeSlider rangeSlider)) {
            rangeSlider.transform.parent.Find("SliderValueLeft").GetComponent<TMP_Text>().text = rangeSlider.LowValue.ToString(rangeSlider.WholeNumbers ? "0" : "F1");
            rangeSlider.transform.parent.Find("SliderValueRight").GetComponent<TMP_Text>().text = rangeSlider.HighValue.ToString(rangeSlider.WholeNumbers ? "0" : "F1");
        } else {
            throw new Exception("There is no Slider/RangeSlider attached to this Gameobject.");
        }
    }

    public void OnValueChanged(float value) {
        Slider s = GetComponent<Slider>();
        s.transform.parent.Find("SliderValue").GetComponent<TMP_Text>().text = value.ToString(s.wholeNumbers ? "0" : "F1");
    }

    public void OnValueChanged(float valueMin, float valueMax) {
        RangeSlider rs = GetComponent<RangeSlider>();
        rs.transform.parent.Find("SliderValueLeft").GetComponent<TMP_Text>().text = valueMin.ToString(rs.WholeNumbers ? "0" : "F1");
        rs.transform.parent.Find("SliderValueRight").GetComponent<TMP_Text>().text = valueMax.ToString(rs.WholeNumbers ? "0" : "F1");
    }

}
