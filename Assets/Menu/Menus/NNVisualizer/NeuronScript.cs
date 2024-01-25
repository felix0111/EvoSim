using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NeuronScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

    public int NeuronID;

    private TMP_Text _text;

    void Awake() {
        _text = GetComponentInChildren<TMP_Text>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        SimulationScript.Instance.MenuManager.EntityMenu.NNVisualizerMenu.OnClickNeuron(eventData, NeuronID);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        _text.fontSize = 13f * 3f;
    }

    public void OnPointerExit(PointerEventData eventData) {
        _text.fontSize = 13f;
    }
}
