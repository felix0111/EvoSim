using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class FoodSpawnerMenu : MonoBehaviour {

    public GameObject SliderPrefab, CheckboxPrefab;
    public RectTransform ContentRect;

    private FoodSpawnAreaScript _foodSpawner;

    public void OpenMenu(FoodSpawnAreaScript foodSpawner) {
        _foodSpawner = foodSpawner;

        gameObject.SetActive(true);
        
        InitMenu();
    }

    private void InitMenu() {
        foreach (RectTransform rectTransform in ContentRect) {
            Destroy(rectTransform.gameObject);
        }

        AddSlider("Radius", _foodSpawner.Radius, true, 200, 1500, o => _foodSpawner.Radius = o);
        AddSlider("Spawn Frequency", _foodSpawner.SpawnFrequency, false, 0.01f, 10f, o => _foodSpawner.SpawnFrequency = o);
        AddSlider("Max Food Amount", _foodSpawner.MaxFoodAmount, true, 200, 6000, o => _foodSpawner.MaxFoodAmount = (int)o);
        AddCheckbox("Spawn Meat", _foodSpawner.SpawnMeat, o => _foodSpawner.SpawnMeat = o);
        AddCheckbox("Hold Max Amount", _foodSpawner.HoldMaxAmount, o => _foodSpawner.HoldMaxAmount = o);
    }

    private void AddSlider(string sliderDescription, float defaultValue, bool useInt, float minValue, float maxValue, UnityAction<float> call) {
        GameObject slider = Instantiate(SliderPrefab, ContentRect);
        slider.GetComponentInChildren<TMP_Text>().text = sliderDescription;

        Slider s = slider.GetComponentInChildren<Slider>();
        s.minValue = minValue;
        s.maxValue = maxValue;
        s.wholeNumbers = useInt;
        s.onValueChanged.AddListener(call);
        s.onValueChanged.AddListener(s.GetComponent<SliderUpdateScript>().OnValueChanged);
        s.value = defaultValue;
    }

    /*
    private void AddDoubleSlider(string sliderDescription, float defaultValueMin, float defaultValueMax, bool useInt, float minValue, float maxValue, UnityAction<float, float> call) {
        GameObject slider = Instantiate(DoubleSliderPrefab, ContentRect);
        slider.GetComponentInChildren<TMP_Text>().text = sliderDescription;

        RangeSlider s = slider.GetComponentInChildren<RangeSlider>();
        s.MinValue = minValue;
        s.MaxValue = maxValue;
        s.WholeNumbers = useInt;
        s.OnValueChanged.AddListener(call);
        s.OnValueChanged.AddListener(s.GetComponent<SliderUpdateScript>().OnValueChanged);
        s.LowValue = defaultValueMin;
        s.HighValue = defaultValueMax;
    }*/

    private void AddCheckbox(string checkboxDescription, bool defaultValue, UnityAction<bool> call) {
        GameObject checkbox = Instantiate(CheckboxPrefab, ContentRect);
        checkbox.GetComponentInChildren<TMP_Text>().text = checkboxDescription;

        Toggle t = checkbox.GetComponentInChildren<Toggle>();
        t.onValueChanged.AddListener(call);
        t.isOn = defaultValue;

    }


}
