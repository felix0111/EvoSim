using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SettingsMenu : MonoBehaviour {

    //ui
    public GameObject MenuSectionPrefab, SliderPrefab, DoubleSliderPrefab, CheckboxPrefab;
    public RectTransform MenuSectionContent;
    private List<GameObject> _menuSections = new();

    void Start() {
        AddSection("Simulation");
        AddSection("Neural Network");
        AddSection("Neural Network Mutation Chances");
        AddSection("Entity");
        AddSection("Entity Gene");
        AddSection("Entity Body Size Dependencies");
        AddSection("Eating & Digestion");
        AddSection("Energy Consumption");
        AddSection("Reproduction");
        AddSection("Fighting");
        AddSection("Pheromone");
        AddSection("Speciation");
        AddSection("Species");
        AddSection("Food");

        AddSliderToSection("Simulation", "Minimum Population", SimulationScript.Instance.CoSh.MinPopulation, true, 10, 150, o => SimulationScript.Instance.CoSh.MinPopulation = (int)o);
        AddSliderToSection("Simulation", "Vision Step", SimulationScript.Instance.CoSh.CheckVisionStep, true, 1, 8, o => SimulationScript.Instance.CoSh.CheckVisionStep = (int)o);
        AddCheckboxToSection("Simulation", "Raycast Vision", SimulationScript.Instance.CoSh.RaycastVision, o => SimulationScript.Instance.CoSh.RaycastVision = o);
        AddCheckboxToSection("Simulation", "Show Particles", SimulationScript.Instance.CoSh.ShowParticles, o => SimulationScript.Instance.CoSh.ShowParticles = o);
        AddCheckboxToSection("Simulation", "Rotate to main Area", SimulationScript.Instance.CoSh.RotateToMainArea, o => SimulationScript.Instance.CoSh.RotateToMainArea = o);

        AddSliderToSection("Neural Network", "Action Threshold", ConstantSheet.ActionThreshold, false, 0f, 1f, o => ConstantSheet.ActionThreshold = o);
        AddSliderToSection("Neural Network", "Weight Adjustment Magnitude", ConstantSheet.WeightAdjustmentMagnitude, false, 0f, 4f, o => ConstantSheet.WeightAdjustmentMagnitude = o);
        AddSliderToSection("Neural Network", "Check Improvement Rate", ConstantSheet.CheckImprovementRate, false, 60f, 60f * 60f, o => ConstantSheet.CheckImprovementRate = o);
        AddSliderToSection("Neural Network", "Expansion Phase Multiplier", ConstantSheet.ExpansionMult, false, 1f, 6f, o => ConstantSheet.ExpansionMult = o);

        AddSliderToSection("Neural Network Mutation Chances", "Add Connection", ConstantSheet.AddConnectionChance, false, 0f, 1f, o => ConstantSheet.AddConnectionChance = o);
        AddSliderToSection("Neural Network Mutation Chances", "Remove Connection", ConstantSheet.RemoveConnectionChance, false, 0f, 1f, o => ConstantSheet.RemoveConnectionChance = o);
        AddSliderToSection("Neural Network Mutation Chances", "Adjust Weight", ConstantSheet.AdjustWeightChance, false, 0f, 1f, o => ConstantSheet.AdjustWeightChance = o);
        AddSliderToSection("Neural Network Mutation Chances", "Toggle Connection", ConstantSheet.ToggleConnectionChance, false, 0f, 1f, o => ConstantSheet.ToggleConnectionChance = o);
        AddSliderToSection("Neural Network Mutation Chances", "Add Neuron", ConstantSheet.AddNeuronChance, false, 0f, 1f, o => ConstantSheet.AddNeuronChance = o);
        AddSliderToSection("Neural Network Mutation Chances", "Remove Neuron", ConstantSheet.RemoveNeuronChance, false, 0f, 1f, o => ConstantSheet.RemoveNeuronChance = o);
        AddSliderToSection("Neural Network Mutation Chances", "Random Function", ConstantSheet.RandomFunctionChance, false, 0f, 1f, o => ConstantSheet.RandomFunctionChance = o);

        AddSliderToSection("Entity", "Max. Age", SimulationScript.Instance.CoSh.MaxAge, true, 10, 1000, o => SimulationScript.Instance.CoSh.MaxAge = (int)o);
        AddSliderToSection("Entity", "Ageing Frequency", SimulationScript.Instance.CoSh.AgeingFrequency, false, 1f, 10f, o => SimulationScript.Instance.CoSh.AgeingFrequency = o);
        AddSliderToSection("Entity", "Healing Rate", SimulationScript.Instance.CoSh.HealingRate, false, 0f, 100f, o => SimulationScript.Instance.CoSh.HealingRate = o);
        AddSliderToSection("Entity", "Min. Energy to Heal", SimulationScript.Instance.CoSh.MinEnergyToHeal, false, 0f, 100f, o => SimulationScript.Instance.CoSh.MinEnergyToHeal = o);
        AddSliderToSection("Entity", "Max. Vision Angle", SimulationScript.Instance.CoSh.MaxVisionAngle, false, 1f, 89f, o => SimulationScript.Instance.CoSh.MaxVisionAngle = o);
        AddSliderToSection("Entity", "Rotation Speed", SimulationScript.Instance.CoSh.MaxRotationSpeed, false, 1f, 8f, o => SimulationScript.Instance.CoSh.MaxRotationSpeed = o);

        AddDoubleSliderToSection("Entity Gene", "Body Size", SimulationScript.Instance.CoSh.MinEntitySize, SimulationScript.Instance.CoSh.MaxEntitySize, false, 0.5f, 15f, (min, max) => { SimulationScript.Instance.CoSh.MinEntitySize = min; SimulationScript.Instance.CoSh.MaxEntitySize = max; });
        AddDoubleSliderToSection("Entity Gene", "View Distance", SimulationScript.Instance.CoSh.MinViewDistance, SimulationScript.Instance.CoSh.MaxViewDistance, false, 5f, 60f, (min, max) => { SimulationScript.Instance.CoSh.MinViewDistance = min; SimulationScript.Instance.CoSh.MaxViewDistance = max; });
        AddDoubleSliderToSection("Entity Gene", "Field Of View", SimulationScript.Instance.CoSh.MinFieldOfView, SimulationScript.Instance.CoSh.MaxFieldOfView, false, 1f, 89f, (min, max) => { SimulationScript.Instance.CoSh.MinFieldOfView = min; SimulationScript.Instance.CoSh.MaxFieldOfView = max; });
        AddDoubleSliderToSection("Entity Gene", "Oscillator Frequency", SimulationScript.Instance.CoSh.MinOscillatorFrequency, SimulationScript.Instance.CoSh.MaxOscillatorFrequency, false, 0.2f, 1000f, (min, max) => { SimulationScript.Instance.CoSh.MinOscillatorFrequency = min; SimulationScript.Instance.CoSh.MaxOscillatorFrequency = max; });
        AddDoubleSliderToSection("Entity Gene", "Pregnancy Time", SimulationScript.Instance.CoSh.MinPregnancyTime, SimulationScript.Instance.CoSh.MaxPregnancyTime, false, 0f, 60f * 4f, (min, max) => { SimulationScript.Instance.CoSh.MinPregnancyTime = min; SimulationScript.Instance.CoSh.MaxPregnancyTime = max; });
        AddDoubleSliderToSection("Entity Gene", "Pregnancy Energy Invest", SimulationScript.Instance.CoSh.MinPregnancyEnergyInvest, SimulationScript.Instance.CoSh.MaxPregnancyEnergyInvest, false, 0f, 1000f, (min, max) => { SimulationScript.Instance.CoSh.MinPregnancyEnergyInvest = min; SimulationScript.Instance.CoSh.MaxPregnancyEnergyInvest = max; });

        AddDoubleSliderToSection("Entity Body Size Dependencies", "Movement Speed", SimulationScript.Instance.CoSh.MinMovementSpeed, SimulationScript.Instance.CoSh.MaxMovementSpeed, false, 1f, 200f, (min, max) => { SimulationScript.Instance.CoSh.MinMovementSpeed = min; SimulationScript.Instance.CoSh.MaxMovementSpeed = max; });
        AddDoubleSliderToSection("Entity Body Size Dependencies", "Health", SimulationScript.Instance.CoSh.MinHealth, SimulationScript.Instance.CoSh.MaxHealth, false, 5f, 1000f, (min, max) => { SimulationScript.Instance.CoSh.MinHealth = min; SimulationScript.Instance.CoSh.MaxHealth = max; });
        AddDoubleSliderToSection("Entity Body Size Dependencies", "Active Energy", SimulationScript.Instance.CoSh.MinActiveEnergy, SimulationScript.Instance.CoSh.MaxActiveEnergy, false, 5f, 1000f, (min, max) => { SimulationScript.Instance.CoSh.MinActiveEnergy = min; SimulationScript.Instance.CoSh.MaxActiveEnergy = max; });
        AddDoubleSliderToSection("Entity Body Size Dependencies", "Base Energy Drain", SimulationScript.Instance.CoSh.MinBaseEnergyConsumption, SimulationScript.Instance.CoSh.MaxBaseEnergyConsumption, false, 0f, 5f, (min, max) => { SimulationScript.Instance.CoSh.MinBaseEnergyConsumption = min; SimulationScript.Instance.CoSh.MaxBaseEnergyConsumption = max; });
        AddDoubleSliderToSection("Entity Body Size Dependencies", "Stomach Size", SimulationScript.Instance.CoSh.MinStomachSize, SimulationScript.Instance.CoSh.MaxStomachSize, false, 5f, 1000f, (min, max) => { SimulationScript.Instance.CoSh.MinStomachSize = min; SimulationScript.Instance.CoSh.MaxStomachSize = max; });
        AddDoubleSliderToSection("Entity Body Size Dependencies", "Nutrition Intake", SimulationScript.Instance.CoSh.MinNutrientIntake, SimulationScript.Instance.CoSh.MaxNutrientIntake, false, 1f, 200f, (min, max) => { SimulationScript.Instance.CoSh.MinNutrientIntake = min; SimulationScript.Instance.CoSh.MaxNutrientIntake = max; });
        AddDoubleSliderToSection("Entity Body Size Dependencies", "Digestion Rate", SimulationScript.Instance.CoSh.MinDigestionRate, SimulationScript.Instance.CoSh.MaxDigestionRate, false, 0.2f, 10f, (min, max) => { SimulationScript.Instance.CoSh.MinDigestionRate = min; SimulationScript.Instance.CoSh.MaxDigestionRate = max; });
        AddDoubleSliderToSection("Entity Body Size Dependencies", "Attack Damage", SimulationScript.Instance.CoSh.MinAttackDamage, SimulationScript.Instance.CoSh.MaxAttackDamage, false, 1f, 100f, (min, max) => { SimulationScript.Instance.CoSh.MinAttackDamage = min; SimulationScript.Instance.CoSh.MaxAttackDamage = max; });
        AddDoubleSliderToSection("Entity Body Size Dependencies", "Defense Factor", SimulationScript.Instance.CoSh.MinDefenseFactor, SimulationScript.Instance.CoSh.MaxDefenseFactor, false, 0f, 1f, (min, max) => { SimulationScript.Instance.CoSh.MinDefenseFactor = min; SimulationScript.Instance.CoSh.MaxDefenseFactor = max; });

        AddSliderToSection("Eating & Digestion", "Eating Distance", SimulationScript.Instance.CoSh.MaxEatDistance, false, 0.2f, 5f, o => SimulationScript.Instance.CoSh.MaxEatDistance = o);
        AddSliderToSection("Eating & Digestion", "Eating Cooldown", SimulationScript.Instance.CoSh.EatCooldown, false, 0.1f, 5f, o => SimulationScript.Instance.CoSh.EatCooldown = o);
        AddSliderToSection("Eating & Digestion", "Plant To Energy Factor", SimulationScript.Instance.CoSh.PlantToEnergyFactor, false, 0.1f, 5f, o => SimulationScript.Instance.CoSh.PlantToEnergyFactor = o);
        AddSliderToSection("Eating & Digestion", "Meat To Energy Factor", SimulationScript.Instance.CoSh.MeatToEnergyFactor, false, 0.1f, 5f, o => SimulationScript.Instance.CoSh.MeatToEnergyFactor = o);

        AddDoubleSliderToSection("Energy Consumption", "Base Energy Consumption", SimulationScript.Instance.CoSh.MinBaseEnergyConsumption, SimulationScript.Instance.CoSh.MaxBaseEnergyConsumption, false, 0f, 5f, (min, max) => { SimulationScript.Instance.CoSh.MinBaseEnergyConsumption = min; SimulationScript.Instance.CoSh.MaxBaseEnergyConsumption = max; });
        AddSliderToSection("Energy Consumption", "Moving Energy Consumption", SimulationScript.Instance.CoSh.MoveEnergyConsumption, false, 0f, 5f, o => SimulationScript.Instance.CoSh.MoveEnergyConsumption = o);
        AddSliderToSection("Energy Consumption", "Attacking Energy Consumption", SimulationScript.Instance.CoSh.AttackEnergyConsumption, false, 0f, 5f, o => SimulationScript.Instance.CoSh.AttackEnergyConsumption = o);
        AddSliderToSection("Energy Consumption", "Pheromone Energy Consumption", SimulationScript.Instance.CoSh.PheromoneEnergyConsumption, false, 0f, 5f, o => SimulationScript.Instance.CoSh.PheromoneEnergyConsumption = o);
        AddSliderToSection("Energy Consumption", "Energy Consumption Multiplier", SimulationScript.Instance.CoSh.EnergyConsumptionMultiplier, false, 0f, 5f, o => SimulationScript.Instance.CoSh.EnergyConsumptionMultiplier = o);

        AddCheckboxToSection("Reproduction", "Sexual Reproduction", false, o => SimulationScript.Instance.CoSh.SexualReproduction = o);
        AddSliderToSection("Reproduction", "Max. Child Mutations", SimulationScript.Instance.CoSh.MaxChildMutations, true, 0, 20, o => SimulationScript.Instance.CoSh.MaxChildMutations = (int)o);
        AddSliderToSection("Reproduction", "Child Mutation Chance", SimulationScript.Instance.CoSh.ChildMutationChance, false, 0f, 1f, o => SimulationScript.Instance.CoSh.ChildMutationChance = o);
        AddSliderToSection("Reproduction", "Minimum Reproduction Age", SimulationScript.Instance.CoSh.MinAgeToReproduce, true, 1, 99, o => SimulationScript.Instance.CoSh.MinAgeToReproduce = (int)o);
        
        AddSliderToSection("Fighting", "Attack Distance", SimulationScript.Instance.CoSh.MaxAttackDistance, false, 0.2f, 5f, o => SimulationScript.Instance.CoSh.MaxAttackDistance = o);
        AddSliderToSection("Fighting", "Attack Cooldown", SimulationScript.Instance.CoSh.AttackCooldown, false, 0.1f, 5f, o => SimulationScript.Instance.CoSh.AttackCooldown = o);
        
        AddSliderToSection("Pheromone", "Pheromone Degrade Time", SimulationScript.Instance.CoSh.PheromoneDegradeTime, false, 4f, 20f, o => SimulationScript.Instance.CoSh.PheromoneDegradeTime = o);
        AddSliderToSection("Pheromone", "Pheromone Cooldown", SimulationScript.Instance.CoSh.PheromoneCooldown, false, 0.5f, 8f, o => SimulationScript.Instance.CoSh.PheromoneCooldown = o);
        AddSliderToSection("Pheromone", "Pheromone Radius", SimulationScript.Instance.CoSh.PheromoneSmellDistance, false, 2f, 10f, o => SimulationScript.Instance.CoSh.PheromoneSmellDistance = o);

        AddSliderToSection("Speciation", "Disjoint Factor", ConstantSheet.DisjointFactor, false, 0f, 5f, o => ConstantSheet.DisjointFactor = o);
        AddSliderToSection("Speciation", "Weight Factor", ConstantSheet.WeightFactor, false, 0f, 5f, o => ConstantSheet.WeightFactor = o);
        AddSliderToSection("Speciation", "Compatability Threshold", ConstantSheet.CompatabilityThreshold, false, 0f, 5f, o => ConstantSheet.CompatabilityThreshold = o);
        AddCheckboxToSection("Speciation", "Use Adjusted Fitness", ConstantSheet.UseAdjustedFitness, o => ConstantSheet.UseAdjustedFitness = o);

        AddCheckboxToSection("Species", "Use Species Budget", SimulationScript.Instance.CoSh.UseSpeciesBudget, o => SimulationScript.Instance.CoSh.UseSpeciesBudget = o);
        AddSliderToSection("Species", "Minimum Species Budget", SimulationScript.Instance.CoSh.MinSpeciesBudget, true, 0, 20, o => SimulationScript.Instance.CoSh.MinSpeciesBudget = (int)o);
        AddSliderToSection("Species", "Species Logging Rate", SimulationScript.Instance.CoSh.SpeciesLoggingRate, false, 60f, 60f * 60f, o => SimulationScript.Instance.CoSh.SpeciesLoggingRate = o);

        AddDoubleSliderToSection("Food", "Food Nutrition", SimulationScript.Instance.CoSh.MinFoodNutrition, SimulationScript.Instance.CoSh.MaxFoodNutrition, false, 10f, 250f, (min, max) => { SimulationScript.Instance.CoSh.MinFoodNutrition = min; SimulationScript.Instance.CoSh.MaxFoodNutrition = max; });
        AddDoubleSliderToSection("Food", "Food Size", SimulationScript.Instance.CoSh.MinFoodRadius, SimulationScript.Instance.CoSh.MaxFoodRadius, false, 0.4f, 10f, (min, max) => { SimulationScript.Instance.CoSh.MinFoodRadius = min; SimulationScript.Instance.CoSh.MaxFoodRadius = max; });
        AddSliderToSection("Food", "Food Density", SimulationScript.Instance.CoSh.FoodDensity, false, 0.4f, 5f, o => SimulationScript.Instance.CoSh.FoodDensity = o);
        AddSliderToSection("Food", "Meat Decomposition Rate", SimulationScript.Instance.CoSh.FoodDecompositionRate, false, 30f, 60f * 20f, o => SimulationScript.Instance.CoSh.FoodDecompositionRate = o);
        

    }

    public void OpenMenu() {
        gameObject.SetActive(true);
    }

    public void AddSection(string sectionHeadline) {
        _menuSections.Add(Instantiate(MenuSectionPrefab, MenuSectionContent));
        _menuSections.Last().GetComponent<MenuSectionScript>().Headline.text = sectionHeadline;
        _menuSections.Last().name = sectionHeadline;
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
    }

    public void AddCheckboxToSection(string section, string checkboxDescription, bool defaultValue, UnityAction<bool> call) {
        MenuSectionScript sectionScript = _menuSections.First(o => o.name == section).GetComponent<MenuSectionScript>();
        GameObject checkbox = Instantiate(CheckboxPrefab, sectionScript.ContentRect);
        checkbox.GetComponentInChildren<TMP_Text>().text = checkboxDescription;

        Toggle t = checkbox.GetComponentInChildren<Toggle>();
        t.onValueChanged.AddListener(call);
        t.isOn = defaultValue;
    }

}