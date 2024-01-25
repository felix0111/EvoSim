using EasyNNFramework.NEAT;

public class ConstantSheet
{
    //global variables for food
    //size
    public float MinFoodNutritíon = 25f;
    public float MaxFoodNutrition = 120f;
    public float MaxFoodRadius = 2f;
    public float MinFoodRadius = 0.6f;
    public float MeatDecompositionRate = 60f * 15f;

    //global variables for entity
    //health stuff
    public float HealingRate = 0.4f;
    public float MinEnergyToHeal = 20f;
    public float MinHealth = 100f;
    public float MaxHealth = 500f;

    //eating stuff
    public float MaxEatDistance = 0.4f;
    public float MaxNutrientIntake = 50f;
    public float MinNutrientIntake = 4f;
    public float EatCooldown = 0.8f;

    //energy losses (every 1 second)
    public float MoveEnergyConsumption = 0.5f;
    public float AttackEnergyConsumption = 0.15f;
    public float PheromoneEnergyConsumption = 0.05f;
    public float MinBaseEnergyConsumption = 0.2f;
    public float MaxBaseEnergyConsumption = 0.5f;
    public float EnergyConsumptionMultiplier = 1f;

    //energy
    public float MinActiveEnergy = 100f;
    public float MaxActiveEnergy = 500f;
    public float MinStomachSize = 100f;
    public float MaxStomachSize = 500f;

    //digestion
    public float MinDigestionRate = 2f;
    public float MaxDigestionRate = 4f;
    public float MeatToEnergyFactor = 1.5f;
    public float PlantToEnergyFactor = 1f;

    //ageing
    public float AgeingFrequency = 10f;  //x sec. = 1 yrs
    public int MaxAge = 100;

    //reproduction
    public int MaxChildMutations = 8;
    public float ChildMutationChance = 0.3f;
    public float MaxColorDifference = 0.15f;
    public float MaxPheromoneDifference = 0.2f;
    public float MinEnergyToReproduce = 100f;
    public float MaxEnergyToReproduce = 200f;
    public float TimeToReproduce = 100f;

    //fighting stuff
    public float MaxAttackDistance = 1f;
    public float AttackCooldown = 1f;
    public float MaxAttackDamage = 100f;
    public float MinAttackDamage = 20f;
    public float MinDefenseFactor = 0f;
    public float MaxDefenseFactor = 0.8f;
    public float MaxKillDropDistance = 1.5f;
    
    //movement
    public float MaxRotationSpeed = 4f;
    public float MaxMovementSpeed = 100f;
    public float MinMovementSpeed = 20f;

    //neural network
    public float ActionThreshold = 0.2f;
    public SpeciationOptions SpeciationOptions => new SpeciationOptions(1f, 0.0f, 0.75f, 10, false);

    //mutation factors
    public float AddConnectionChance = 0.12f;
    public float RemoveConnectionChance = 0.12f;
    public float AddNeuronChance = 0.04f;
    public float RemoveNeuronChance = 0.04f;
    public float RandomFunctionChance = 0.02f;
    public float AddRecurrentConnectionChance = 0.02f;
    public float AdjustWeightChance = 0.6f;
    public float ToggleConnectionChance = 0.04f;
    //public MutateOptions MutateOptions => new MutateOptions(AddConnectionChance, RemoveConnectionChance, AdjustWeightChance, ToggleConnectionChance, AddNeuronChance, RemoveNeuronChance, RandomFunctionChance, AddRecurrentConnectionChance, default, HiddenFunctions, true);
    public float CheckImprovementRate = 60f * 30f;
    public bool AdaptionPhase = true;
    public MutateOptions MutateOptions => AdaptionPhase ? AdaptionMutateOptions : ExpandingMutateOptions;
    public MutateOptions AdaptionMutateOptions = new MutateOptions(0.10f, 0.10f, 0.70f, 0.01f, 0.02f, 0.02f, 0.04f, 0.01f, default, HiddenFunctions, true);
    public MutateOptions ExpandingMutateOptions = new MutateOptions(0.24f, 0.24f, 0.10f, 0.06f, 0.12f, 0.12f, 0.08f, 0.04f, default, HiddenFunctions, true);


    //gene mutations
    public float SizeMutationFactor = 0.4f;
    public float ViewDistanceMutationFactor = 3f;
    public float OscillatorMutationFactor = 5f;
    public static ActivationFunction[] HiddenFunctions = new[] { ActivationFunction.SIGMOID, ActivationFunction.MULT, ActivationFunction.LATCH, ActivationFunction.IDENTITY, ActivationFunction.GAUSS, ActivationFunction.ABS, ActivationFunction.BINARYSTEP };
    
    //pheromone
    public float PheromoneDegradeTime = 12f;
    public float PheromoneCooldown = 1f;
    public float PheromoneSmellDistance = 4f;

    //vision
    public float FieldOfView = 80f;
    public float MinViewDistance = 5f;
    public float MaxViewDistance = 20f;

    //entity misc
    public float UnscaledEntityRadius = 0.45f;   //size in radius
    public float MinEntitySize = 1f;
    public float MaxEntitySize = 4f;
    public float MinOscillatorFrequency = 0.5f;
    public float MaxOscillatorFrequency = 40f;
    public float LeftoverBaseEnergy = 20f;

    //other
    public float SpeciesLoggingRate = 60f * 5f;
    public int CheckVisionStep = 1;

}
