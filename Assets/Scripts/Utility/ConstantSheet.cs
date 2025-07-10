
using NeuraSuite.NeatExpanded;

public class ConstantSheet
{
    //global variables for food
    //size
    public float MinFoodNutrition = 25f;
    public float MaxFoodNutrition = 160f;
    public float MaxFoodRadius = 4f;
    public float MinFoodRadius = 0.6f;
    public float FoodDecompositionRate = 60f * 15f;
    public float FoodDensity = 2f;

    //global variables for entity
    //health stuff
    public float HealingRate = 0.4f;
    public float MinEnergyToHeal = 20f;
    public float MinHealth = 100f;
    public float MaxHealth = 1000f;

    //eating stuff
    public float MaxEatDistance = 0.4f;
    public float MaxNutrientIntake = 50f;
    public float MinNutrientIntake = 5f;
    public float EatCooldown = 1f;

    //energy losses (every 1 second)
    public float MinMoveEnergyConsumption = 3f;
    public float MaxMoveEnergyConsumption = 8f;
    public float AttackEnergyConsumption = 0.15f;
    public float PheromoneEnergyConsumption = 0.1f;
    public float MinBaseEnergyConsumption = 0.2f;
    public float MaxBaseEnergyConsumption = 1f;
    public float EnergyConsumptionMultiplier = 1f;

    //energy
    public float MinActiveEnergy = 100f;
    public float MaxActiveEnergy = 800f;
    public float MinStomachSize = 100f;
    public float MaxStomachSize = 500f;

    //digestion
    public float MinDigestionRate = 2f;
    public float MaxDigestionRate = 10f;
    public float MeatToEnergyFactor = 1.5f;
    public float PlantToEnergyFactor = 1f;

    //ageing
    public float AgeingFrequency = 10f;  //seconds per year
    public int MaxAge = 100;

    //reproduction
    public bool SexualReproduction = false;
    public int MaxChildMutations = 1;
    public float ChildMutationChance = 1f;
    public float MaxColorDifference = 0.15f;
    public float MaxPheromoneDifference = 0.2f;
    public int MinAgeToReproduce = 15;
    public float MinPregnancyEnergyInvest = 10f;
    public float MaxPregnancyEnergyInvest = 800f;
    public float MinPregnancyTime = 5f;
    public float MaxPregnancyTime = 60f * 2f;
    
    //fighting stuff
    public float MaxAttackDistance = 1f;
    public float AttackCooldown = 1f;
    public float MaxAttackDamage = 100f;
    public float MinAttackDamage = 20f;
    public float MinDefenseFactor = 0f;
    public float MaxDefenseFactor = 0.8f;
    
    //movement
    public float MaxRotationSpeed = 16f;
    public float MaxMovementSpeed = 200f;
    public float MinMovementSpeed = 20f;

    //neural network
    public static float ActionThreshold = 0.2f;
    public static float WeightAdjustmentMagnitude = 1f;
    public static float CheckImprovementRate = 60f * 30f;
    public bool AdaptionPhase = true;
    public static float ExpansionMult = 2f;
    public MutateOptions MutateOptions => AdaptionPhase ? AdaptionMutateOptions : ExpandingMutateOptions;
    public MutateOptions AdaptionMutateOptions => new MutateOptions(AddConnectionChance, RemoveConnectionChance, AdjustWeightChance, ToggleConnectionChance, AddNeuronChance, RemoveNeuronChance, RandomFunctionChance, WeightAdjustmentMagnitude, default, HiddenFunctions, true);
    public MutateOptions ExpandingMutateOptions => new MutateOptions(AddConnectionChance * ExpansionMult, RemoveConnectionChance * ExpansionMult, AdjustWeightChance * ExpansionMult, ToggleConnectionChance * ExpansionMult, AddNeuronChance * ExpansionMult, RemoveNeuronChance * ExpansionMult, RandomFunctionChance * ExpansionMult, WeightAdjustmentMagnitude * ExpansionMult, default, HiddenFunctions, true);
    public static ActivationFunction[] HiddenFunctions = new[] { ActivationFunction.SIGMOID, ActivationFunction.TANH, ActivationFunction.MULT, ActivationFunction.LATCH, ActivationFunction.IDENTITY, ActivationFunction.GAUSS, ActivationFunction.ABS, ActivationFunction.BINARYSTEP };

    //neural network mutation chances
    public static float AddConnectionChance = 0.2f;
    public static float RemoveConnectionChance = 0f;
    public static float AdjustWeightChance = 0.3f;
    public static float ToggleConnectionChance = 0.02f;
    public static float AddNeuronChance = 0.05f;
    public static float RemoveNeuronChance = 0f;
    public static float RandomFunctionChance = 0.04f;

    //speciation
    public static SpeciationOptions SpeciationOptions => new SpeciationOptions(DisjointFactor, WeightFactor, CompatabilityThreshold, UseAdjustedFitness);
    public static float DisjointFactor = 1f;
    public static float WeightFactor = 0.05f;
    public static float CompatabilityThreshold = 0.5f;
    public static bool UseAdjustedFitness = true;

    //species
    public float SpeciesLoggingRate = 60f;
    public bool UseSpeciesBudget = false;
    public int MinSpeciesBudget = 5;
    public static int TargetedPopulationSize = 120;

    //gene mutations
    public float SizeMutationFactor = 0.4f;
    public float ViewDistanceMutationFactor = 5f;
    public float FieldOfViewMutationFactor = 5f;
    public float OscillatorMutationFactor = 5f;
    public float PregnancyTimeMutationFactor = 5f;
    public float PregnancyEnergyInvestMutationFactor = 15f;
    
    //pheromone
    public float PheromoneDegradeTime = 12f;
    public float PheromoneCooldown = 1.5f;
    public float PheromoneSmellDistance = 8f;

    //vision
    public float MaxVisionAngle = 80f;
    public float MinFieldOfView = 5f;
    public float MaxFieldOfView = 30f;
    public float MinViewDistance = 20f;
    public float MaxViewDistance = 40f;

    //entity misc
    public float UnscaledEntityRadius = 0.45f;
    public float MinEntitySize = 1f;
    public float MaxEntitySize = 6f;
    public float MinOscillatorFrequency = 0.5f;
    public float MaxOscillatorFrequency = 40f;
    public float LeftoverBaseEnergy = 50f;

    //other
    public int MinPopulation = 20;
    public int CheckVisionStep = 1;
    public bool RaycastVision = true;
    public bool ShowParticles = false;
    public bool RotateToMainArea = true;

}
