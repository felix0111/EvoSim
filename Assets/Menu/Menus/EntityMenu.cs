using NeuraSuite.NeatExpanded;
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
    public GeneMenu GeneMenu;

    public TMP_Text InfoText;
    public Material LineRendererMaterial;

    private EntityScript _entity;
    private bool _isLocked;
    private LineRenderer _leftViewConeLine, _rightViewConeLine;

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
        if(GeneMenu.isActiveAndEnabled) GeneMenu.UpdateMenu(es);
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
        if (_leftViewConeLine == null) _leftViewConeLine = Utility.MakeVisualizerLine(transform, Color.magenta, LineRendererMaterial);
        if (_rightViewConeLine == null) _rightViewConeLine = Utility.MakeVisualizerLine(transform, Color.magenta, LineRendererMaterial);

        Vector2 dir = (Quaternion.Euler(0f, 0f, _entity.VisionAngle - _entity.Gene.FieldOfView) * _entity.transform.up).normalized;
        Vector2 startPos = _entity.transform.position;
        Vector2 endPos = startPos + dir * (_entity.Gene.ViewDistance + _entity.Radius);
        _leftViewConeLine.SetPosition(0, startPos);
        _leftViewConeLine.SetPosition(1, endPos);

        dir = (Quaternion.Euler(0f, 0f, _entity.VisionAngle + _entity.Gene.FieldOfView) * _entity.transform.up).normalized;
        endPos = startPos + dir * (_entity.Gene.ViewDistance + _entity.Radius);
        _rightViewConeLine.SetPosition(0, startPos);
        _rightViewConeLine.SetPosition(1, endPos);
    }

    private void UpdateInfoText() {
        InfoText.text = _entity.gameObject.name + ": <br>";
        InfoText.text += "----General----<br>";
        InfoText.text += "Age: " + _entity.Age + "<br>";
        InfoText.text += "Generation: " + _entity.Gene.Generation + "<br>";
        InfoText.text += "Diet: " + (_entity.Gene.Diet == EntityDiet.Herbivore ? "Herbivore" : "Carnivore") + "<br>";
        InfoText.text += "Is Pregnant: " + _entity.IsPregnant + "<br>";
        InfoText.text += "Pregnancy Progress: " + _entity.EnergyHandler.PregnancyEnergy / _entity.Gene.PregnancyEnergyInvest + "<br>";
        InfoText.text += "Energy Consumption: " + _entity.EnergyHandler.EnergyConsumption.ToString("F1") + "<br>";
        InfoText.text += "----Genes----<br>";
        InfoText.text += "Pregnancy Time: " + _entity.Gene.PregnancyTime.ToString("F1") + "<br>";
        InfoText.text += "Pregnancy Energy Invest: " + _entity.Gene.PregnancyEnergyInvest.ToString("F1") + "<br>";
        InfoText.text += "Oscillator Frequency: " + _entity.Gene.OscillatorFrequency.ToString("F1") + "<br>";
        InfoText.text += "Entity Size: " + _entity.Gene.EntitySize.ToString("F1") + "<br>";
        InfoText.text += "View Distance: " + _entity.Gene.ViewDistance.ToString("F1") + "<br>";
        InfoText.text += "Field of View: " + _entity.Gene.FieldOfView.ToString("F1") + "<br>";
        InfoText.text += "----Other----<br>";
        InfoText.text += "Current Vision Angle: " + _entity.VisionAngle.ToString("F1") + "<br>";
        InfoText.text += "Amount in View: " + _entity.VisionHandler.InVisionCone.Count + "<br>";
        InfoText.text += "Velocity: " + _entity.Rigidbody.linearVelocity.ToString("F1") + "<br>";
        InfoText.text += "Species: " + _entity.Network.SpeciesID + "<br>";
        InfoText.text += "Offspring Budget: " + (SimulationScript.Instance.OffspringBudget.TryGetValue(_entity.Species.SpeciesID, out var value) ? value : "undefined") + "<br>";
        InfoText.text += "Fitness: " + _entity.Fitness + "<br>";
        InfoText.text += "Adjusted Fitness: " + _entity.Fitness / _entity.Species.AllNetworks.Count + "<br>";
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
            NNVisualizerMenu.OpenMenu(_entity);
        }
    }

    public void OnGeneMenuButton() {
        if (GeneMenu.isActiveAndEnabled){
            GeneMenu.gameObject.SetActive(false);
        } else {
            GeneMenu.OpenMenu(_entity);
        }
    }

    public void OnLockButton() {
        _isLocked = !_isLocked;
    }

    public void OnReproduceButton() {
        _entity.SexualPartner = new SerializableEntity(_entity);
        _entity.CreateChild(_entity.Gene.PregnancyEnergyInvest);
    }

    public void OnKillButton() {
        _entity.Kill(true);
    }

    public void OnDisable() {
        if(NNVisualizerMenu.isActiveAndEnabled) NNVisualizerMenu.gameObject.SetActive(false);
        if(GeneMenu.isActiveAndEnabled) GeneMenu.gameObject.SetActive(false);
        if(_leftViewConeLine != null) Destroy(_leftViewConeLine.gameObject); //TODO ???
    }
    
}
