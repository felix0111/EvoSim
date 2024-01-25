using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SettingsMenu : MonoBehaviour {

    //ui
    public GameObject MenuSectionPrefab, SliderPrefab, DoubleSliderPrefab;
    public RectTransform MenuSectionContent;
    private List<GameObject> _menuSections = new();

    void Start() {
        AddSection("Simulation");
        AddSection("Entity");
        AddSection("Eating & Digestion");
        AddSection("Energy Consumption");
        AddSection("Reproduction");
        AddSection("Fighting");
        AddSection("Food");

        AddSliderToSection("Simulation", "Check for Improvement Rate", SimulationScript.Instance.CoSh.CheckImprovementRate, false, 60f, 60f * 60f, o => SimulationScript.Instance.CoSh.CheckImprovementRate = o);
        AddSliderToSection("Simulation", "Species Logging Rate", SimulationScript.Instance.CoSh.SpeciesLoggingRate, false, 60f, 60f * 60f, o => SimulationScript.Instance.CoSh.SpeciesLoggingRate = o);
        AddSliderToSection("Simulation", "Vision Step", SimulationScript.Instance.CoSh.CheckVisionStep, true, 1, 8, o => SimulationScript.Instance.CoSh.CheckVisionStep = (int)o);

        AddDoubleSliderToSection("Entity", "Size", SimulationScript.Instance.CoSh.MinEntitySize, SimulationScript.Instance.CoSh.MaxEntitySize, false, 0.5f, 15f, (min, max) => { SimulationScript.Instance.CoSh.MinEntitySize = min; SimulationScript.Instance.CoSh.MaxEntitySize = max; });
        AddSliderToSection("Entity", "Max. Age", SimulationScript.Instance.CoSh.MaxAge, true, 10, 1000, o => SimulationScript.Instance.CoSh.MaxAge = (int)o);
        AddSliderToSection("Entity", "Ageing Frequency", SimulationScript.Instance.CoSh.AgeingFrequency, false, 1f, 10f, o => SimulationScript.Instance.CoSh.AgeingFrequency = o);
        AddDoubleSliderToSection("Entity", "Health", SimulationScript.Instance.CoSh.MinHealth, SimulationScript.Instance.CoSh.MaxHealth, false, 5f, 1000f, (min, max) => { SimulationScript.Instance.CoSh.MinHealth = min; SimulationScript.Instance.CoSh.MaxHealth = max; });
        AddDoubleSliderToSection("Entity", "Active Energy", SimulationScript.Instance.CoSh.MinActiveEnergy, SimulationScript.Instance.CoSh.MaxActiveEnergy, false, 5f, 1000f, (min, max) => { SimulationScript.Instance.CoSh.MinActiveEnergy = min; SimulationScript.Instance.CoSh.MaxActiveEnergy = max; });
        AddDoubleSliderToSection("Entity", "Stomach Size", SimulationScript.Instance.CoSh.MinStomachSize, SimulationScript.Instance.CoSh.MaxStomachSize, false, 5f, 1000f, (min, max) => { SimulationScript.Instance.CoSh.MinStomachSize = min; SimulationScript.Instance.CoSh.MaxStomachSize = max; });
        AddSliderToSection("Entity", "Healing Rate", SimulationScript.Instance.CoSh.HealingRate, false, 0f, 100f, o => SimulationScript.Instance.CoSh.HealingRate = o);
        AddSliderToSection("Entity", "Min. Energy to Heal", SimulationScript.Instance.CoSh.MinEnergyToHeal, false, 0f, 100f, o => SimulationScript.Instance.CoSh.MinEnergyToHeal = o);
        AddDoubleSliderToSection("Entity", "View Distance", SimulationScript.Instance.CoSh.MinViewDistance, SimulationScript.Instance.CoSh.MaxViewDistance, false, 1f, 40f, (min, max) => { SimulationScript.Instance.CoSh.MinViewDistance = min; SimulationScript.Instance.CoSh.MaxViewDistance = max; });
        AddSliderToSection("Entity", "Field Of View", SimulationScript.Instance.CoSh.FieldOfView, false, 1f, 179f, o => SimulationScript.Instance.CoSh.FieldOfView = o);
        AddDoubleSliderToSection("Entity", "Movement Speed", SimulationScript.Instance.CoSh.MinMovementSpeed, SimulationScript.Instance.CoSh.MaxMovementSpeed, false, 1f, 200f, (min, max) => { SimulationScript.Instance.CoSh.MinMovementSpeed = min; SimulationScript.Instance.CoSh.MaxMovementSpeed = max; });
        AddSliderToSection("Entity", "Rotation Speed", SimulationScript.Instance.CoSh.MaxRotationSpeed, false, 1f, 8f, o => SimulationScript.Instance.CoSh.MaxRotationSpeed = o);

        AddSliderToSection("Eating & Digestion", "Eating Distance", SimulationScript.Instance.CoSh.MaxEatDistance, false, 0.2f, 5f, o => SimulationScript.Instance.CoSh.MaxEatDistance = o);
        AddSliderToSection("Eating & Digestion", "Eating Cooldown", SimulationScript.Instance.CoSh.EatCooldown, false, 0.1f, 5f, o => SimulationScript.Instance.CoSh.EatCooldown = o);
        AddDoubleSliderToSection("Eating & Digestion", "Nutrition Intake", SimulationScript.Instance.CoSh.MinNutrientIntake, SimulationScript.Instance.CoSh.MaxNutrientIntake, false, 1f, 250f, (min, max) => { SimulationScript.Instance.CoSh.MinNutrientIntake = min; SimulationScript.Instance.CoSh.MaxNutrientIntake = max; });
        AddDoubleSliderToSection("Eating & Digestion", "Digestion Rate", SimulationScript.Instance.CoSh.MinDigestionRate, SimulationScript.Instance.CoSh.MaxDigestionRate, false, 0.2f, 10f, (min, max) => { SimulationScript.Instance.CoSh.MinDigestionRate = min; SimulationScript.Instance.CoSh.MaxDigestionRate = max; });
        AddSliderToSection("Eating & Digestion", "Plant To Energy Factor", SimulationScript.Instance.CoSh.PlantToEnergyFactor, false, 0.1f, 5f, o => SimulationScript.Instance.CoSh.PlantToEnergyFactor = o);
        AddSliderToSection("Eating & Digestion", "Meat To Energy Factor", SimulationScript.Instance.CoSh.MeatToEnergyFactor, false, 0.1f, 5f, o => SimulationScript.Instance.CoSh.MeatToEnergyFactor = o);


        AddDoubleSliderToSection("Energy Consumption", "Base Energy Consumption", SimulationScript.Instance.CoSh.MinBaseEnergyConsumption, SimulationScript.Instance.CoSh.MaxBaseEnergyConsumption, false, 0f, 5f, (min, max) => { SimulationScript.Instance.CoSh.MinBaseEnergyConsumption = min; SimulationScript.Instance.CoSh.MaxBaseEnergyConsumption = max; });
        AddSliderToSection("Energy Consumption", "Moving Energy Consumption", SimulationScript.Instance.CoSh.MoveEnergyConsumption, false, 0f, 5f, o => SimulationScript.Instance.CoSh.MoveEnergyConsumption = o);
        AddSliderToSection("Energy Consumption", "Attacking Energy Consumption", SimulationScript.Instance.CoSh.AttackEnergyConsumption, false, 0f, 5f, o => SimulationScript.Instance.CoSh.AttackEnergyConsumption = o);
        AddSliderToSection("Energy Consumption", "Pheromone Energy Consumption", SimulationScript.Instance.CoSh.PheromoneEnergyConsumption, false, 0f, 5f, o => SimulationScript.Instance.CoSh.PheromoneEnergyConsumption = o);
        AddSliderToSection("Energy Consumption", "Energy Consumption Multiplier", SimulationScript.Instance.CoSh.EnergyConsumptionMultiplier, false, 0f, 5f, o => SimulationScript.Instance.CoSh.EnergyConsumptionMultiplier = o);

        AddSliderToSection("Reproduction", "Max. Child Mutations", SimulationScript.Instance.CoSh.MaxChildMutations, true, 0, 20, o => SimulationScript.Instance.CoSh.MaxChildMutations = (int)o);
        AddSliderToSection("Reproduction", "Child Mutation Chance", SimulationScript.Instance.CoSh.ChildMutationChance, false, 0f, 1f, o => SimulationScript.Instance.CoSh.ChildMutationChance = o);
        AddDoubleSliderToSection("Reproduction", "Energy To Reproduce", SimulationScript.Instance.CoSh.MinEnergyToReproduce, SimulationScript.Instance.CoSh.MaxEnergyToReproduce, false, 10f, 1000f, (min, max) => { SimulationScript.Instance.CoSh.MinEnergyToReproduce = min; SimulationScript.Instance.CoSh.MaxEnergyToReproduce = max; });
        AddSliderToSection("Reproduction", "Time To Reproduce", SimulationScript.Instance.CoSh.TimeToReproduce, false, 60f, 60f * 10f, o => SimulationScript.Instance.CoSh.TimeToReproduce = o);

        AddSliderToSection("Fighting", "Attack Distance", SimulationScript.Instance.CoSh.MaxAttackDistance, false, 0.2f, 5f, o => SimulationScript.Instance.CoSh.MaxAttackDistance = o);
        AddSliderToSection("Fighting", "Attack Cooldown", SimulationScript.Instance.CoSh.AttackCooldown, false, 0.1f, 5f, o => SimulationScript.Instance.CoSh.AttackCooldown = o);
        AddDoubleSliderToSection("Fighting", "Attack Damage", SimulationScript.Instance.CoSh.MinAttackDamage, SimulationScript.Instance.CoSh.MaxAttackDamage, false, 1f, 100f, (min, max) => { SimulationScript.Instance.CoSh.MinAttackDamage = min; SimulationScript.Instance.CoSh.MaxAttackDamage = max; });
        AddDoubleSliderToSection("Fighting", "Defense Factor", SimulationScript.Instance.CoSh.MinDefenseFactor, SimulationScript.Instance.CoSh.MaxDefenseFactor, false, 0f, 1f, (min, max) => { SimulationScript.Instance.CoSh.MinDefenseFactor = min; SimulationScript.Instance.CoSh.MaxDefenseFactor = max; });

        AddDoubleSliderToSection("Food", "Food Nutrition", SimulationScript.Instance.CoSh.MinFoodNutritíon, SimulationScript.Instance.CoSh.MaxFoodNutrition, false, 10f, 250f, (min, max) => { SimulationScript.Instance.CoSh.MinFoodNutritíon = min; SimulationScript.Instance.CoSh.MaxFoodNutrition = max; });
        AddDoubleSliderToSection("Food", "Food Size", SimulationScript.Instance.CoSh.MinFoodRadius, SimulationScript.Instance.CoSh.MaxFoodRadius, false, 0.4f, 10f, (min, max) => { SimulationScript.Instance.CoSh.MinFoodRadius = min; SimulationScript.Instance.CoSh.MaxFoodRadius = max; });
        AddSliderToSection("Food", "Meat Decomposition Rate", SimulationScript.Instance.CoSh.MeatDecompositionRate, false, 30f, 60f * 20f, o => SimulationScript.Instance.CoSh.MeatDecompositionRate = o);

    }

    public void OpenMenu() {
        gameObject.SetActive(true);
    }

    public void AddSection(string sectionHeadline) {
        _menuSections.Add(Instantiate(MenuSectionPrefab, MenuSectionContent));
        _menuSections.Last().GetComponent<MenuSectionScript>().Headline.text = sectionHeadline;
        _menuSections.Last().name = sectionHeadline;

        //update content size
        MenuSectionContent.sizeDelta = new Vector2((25f + 250f) * MenuSectionContent.childCount + 25f, MenuSectionContent.sizeDelta.y);
    }

    public void AddSliderToSection(string section, string sliderDescription, float defaultValue, bool useInt, float minValue, float maxValue, UnityAction<float> call) {
        MenuSectionScript sectionScript = _menuSections.First(o => o.name == section).GetComponent<MenuSectionScript>();
        GameObject slider = Instantiate(SliderPrefab, sectionScript.ContentRect);
        slider.GetComponentInChildren<TMP_Text>().text = sliderDescription;

        Slider s = slider.GetComponentInChildren<Slider>();
        s.minValue = minValue;
        s.maxValue = maxValue;
        s.wholeNumbers = useInt;
        s.onValueChanged.AddListener(call);
        s.onValueChanged.AddListener(s.GetComponent<SliderUpdateScript>().OnValueChanged);
        s.value = defaultValue;

        sectionScript.UpdateContentSize();
    }

    public void AddDoubleSliderToSection(string section, string sliderDescription, float defaultValueMin, float defaultValueMax, bool useInt, float minValue, float maxValue, UnityAction<float, float> call) {
        MenuSectionScript sectionScript = _menuSections.First(o => o.name == section).GetComponent<MenuSectionScript>();
        GameObject slider = Instantiate(DoubleSliderPrefab, sectionScript.ContentRect);
        slider.GetComponentInChildren<TMP_Text>().text = sliderDescription;

        RangeSlider s = slider.GetComponentInChildren<RangeSlider>();
        s.MinValue = minValue;
        s.MaxValue = maxValue;
        s.WholeNumbers = useInt;
        s.OnValueChanged.AddListener(call);
        s.OnValueChanged.AddListener(s.GetComponent<SliderUpdateScript>().OnValueChanged);
        s.LowValue = defaultValueMin;
        s.HighValue = defaultValueMax;

        sectionScript.UpdateContentSize();
    }

}