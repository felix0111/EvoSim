using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class EntityMenu : MonoBehaviour {

    public NNVisualizerMenu NNVisualizerMenu;
    public TMP_Text InfoText;
    public Material LineRendererMaterial;

    private EntityScript _entity;
    private bool _isLocked;
    private LineRenderer _lineRenderer;

    //ui
    private Slider _energyBar, _healthBar, _meatBar, _plantBar;
    private TMP_Text _energyBarText, _healthBarText, _meatBarText, _plantBarText;

    void Awake() {
        _energyBar = transform.Find("EnergyBar").GetComponent<Slider>();
        _healthBar = transform.Find("HealthBar").GetComponent<Slider>();
        _meatBar = transform.Find("MeatBar").GetComponent<Slider>();
        _plantBar = transform.Find("PlantBar").GetComponent<Slider>();

        _energyBarText = _energyBar.GetComponentInChildren<TMP_Text>();
        _healthBarText = _healthBar.GetComponentInChildren<TMP_Text>();
        _meatBarText = _meatBar.GetComponentInChildren<TMP_Text>();
        _plantBarText = _plantBar.GetComponentInChildren<TMP_Text>();
    }

    public void OpenMenu(EntityScript entity) {
        gameObject.SetActive(true);
        _entity = entity;
        _isLocked = false;
    }

    public void UpdateEntity(EntityScript es) {
        _entity = es;
        if(NNVisualizerMenu.isActiveAndEnabled) NNVisualizerMenu.UpdateEntity(es);
    }

    void FixedUpdate() {
        if(!_entity.isActiveAndEnabled) SimulationScript.Instance.MenuManager.FocusedEntityIndex++;

        if (_isLocked) Camera.main.transform.position = new Vector3(_entity.transform.position.x, _entity.transform.position.y, -20);
    }

    void Update() {
        UpdateInfoText();
        UpdateVisionVisualizer();
        UpdateVisualBars();
    }

    private void UpdateVisionVisualizer() {
        if (_lineRenderer == null) _lineRenderer = Utility.MakeVisualizerLine(transform, Color.magenta, LineRendererMaterial);

        //from entity position in main vision direction
        Vector2 startPos = _entity.transform.position;
        Vector2 endPos = startPos + _entity.VisionVector * (_entity.Gene.ViewDistance + _entity.Radius);
        _lineRenderer.SetPosition(0, startPos);
        _lineRenderer.SetPosition(1, endPos);
    }

    private void UpdateInfoText() {
        InfoText.text = _entity.gameObject.name + ": <br>";
        InfoText.text += "Age: " + _entity.Age + "<br>";
        InfoText.text += "Health: " + _entity.Health.ToString("F1") + "<br>";
        InfoText.text += "Generation: " + _entity.Gene.Generation + "<br>";
        InfoText.text += "Diet: " + (_entity.Gene.Diet == EntityDiet.Herbivore ? "Herbivore" : "Carnivore") + "<br>";
        InfoText.text += "Stomach Energy: " + _entity.EnergyHandler.AmountInStomach.ToString("F1") + "<br>";
        InfoText.text += "Active Energy: " + _entity.EnergyHandler.ActiveEnergy.ToString("F1") + "<br>";
        InfoText.text += "Is Reproducing: " + _entity.EnergyHandler.IsReproducing + "<br>";
        InfoText.text += "Reproduction Energy: " + _entity.EnergyHandler.ReproductionEnergy.ToString("F1") + "<br>";
        InfoText.text += "Energy Consumption: " + _entity.EnergyHandler.EnergyConsumption.ToString("F1") + "<br>";
        InfoText.text += "Oscillator Frequency: " + _entity.Gene.OscillatorFrequency.ToString("F1") + "<br>";
        InfoText.text += "Entity Size: " + _entity.Gene.EntitySize.ToString("F1") + "<br>";
        InfoText.text += "View Distance: " + _entity.Gene.ViewDistance.ToString("F1") + "<br>";
        InfoText.text += "Velocity: " + _entity.Rigidbody.velocity.ToString("F1") + "<br>";
        InfoText.text += "Target: " + (_entity.VisionTarget != null ? _entity.VisionTarget.name : "/") + "<br>";
        InfoText.text += "Target Distance: " + _entity.TargetDistance + "<br>";
        InfoText.text += "Species: " + _entity.Network.SpeciesID + "<br>";
        InfoText.text += "Fitness: " + _entity.Network.Fitness + "<br>";
        InfoText.text += "Is mutated: " + _entity.IsMutated + "<br>";
    }

    private void UpdateVisualBars() {
        _healthBar.value = Mathf.InverseLerp(0f, _entity.ScaledMaxHealth, _entity.Health);
        _energyBar.value = Mathf.InverseLerp(0f, _entity.ScaledMaxEnergy, _entity.EnergyHandler.ActiveEnergy);
        _meatBar.value = Mathf.InverseLerp(0f, _entity.ScaledStomachContent, _entity.EnergyHandler.MeatContent);
        _plantBar.value = Mathf.InverseLerp(0f, _entity.ScaledStomachContent, _entity.EnergyHandler.PlantContent);

        _healthBarText.text = _entity.Health.ToString("F1") + "/" + _entity.ScaledMaxHealth.ToString("F1");
        _energyBarText.text = _entity.EnergyHandler.ActiveEnergy.ToString("F1") + "/" + _entity.ScaledMaxEnergy.ToString("F1");
        _meatBarText.text = _entity.EnergyHandler.MeatContent.ToString("F1") + "/" + _entity.ScaledStomachContent.ToString("F1");
        _plantBarText.text = _entity.EnergyHandler.PlantContent.ToString("F1") + "/" + _entity.ScaledStomachContent.ToString("F1");
    }

    public void OnNNVisualizerButton() {
        if (NNVisualizerMenu.isActiveAndEnabled) {
            NNVisualizerMenu.gameObject.SetActive(false);
        } else {
            NNVisualizerMenu.gameObject.SetActive(true);
            NNVisualizerMenu.OpenMenu(_entity);
        }
    }

    public void OnLockButton() {
        _isLocked = !_isLocked;
    }

    public void OnReproduceButton() {
        _entity.Reproduce();
    }

    public void OnKillButton() {
        _entity.Kill(true);
    }

    public void OnDisable() {
        if(NNVisualizerMenu.isActiveAndEnabled) NNVisualizerMenu.gameObject.SetActive(false);
        if(_lineRenderer != null) Destroy(_lineRenderer.gameObject);
    }
    
}
