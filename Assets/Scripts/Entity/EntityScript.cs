using System;
using System.Collections;
using NeuraSuite.NeatExpanded;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class EntityScript : Interactable {

    //vision
    public VisionHandler VisionHandler;
    public Vector2 VisionVector => (Quaternion.Euler(0f, 0f, VisionAngle) * transform.up).normalized;
    public int VisionMask, EatMask, AttackMask;
    public float VisionAngle;

    //neural network
    public Network Network => SimulationScript.Instance.Neat.NetworkCollection[_brainID];
    private int _brainID;
    public NeuralNetHandler NeuralNetHandler;
    public float Fitness {
        get { return CalculateFitness();}
        set { _fitness = value; }
    }
    private float _fitness;
    public Species Species {
        get {
            if(Network.SpeciesID == -1) SimulationScript.Instance.Neat.SpeciateSingle(Network);
            return SimulationScript.Instance.Neat.Species[Network.SpeciesID];
        }
    }

    //movement
    public float AimedRotationDir;
    public Vector2 AimedMovementDir;    //relative to transform.up vector

    //energy
    public EnergyHandler EnergyHandler;

    //self
    public float Health {
        get => _health;
        set {
            _health = Mathf.Clamp(value, 0f, ScaledMaxHealth);
            if (Health <= 0f) Kill(true);
        }
    }
    private float _health;
    public int Age;
    public bool IsMutated;

    //reproduction
    public bool IsPregnant;
    public SerializableEntity SexualPartner;

    //other
    public Gene Gene;
    public Rigidbody2D Rigidbody;
    public PolygonCollider2D Collider;
    public ParticleSystem ParticleSystem;
    public float PassedTime;
    public float Radius => SimulationScript.Instance.CoSh.UnscaledEntityRadius * Gene.EntitySize;

    //raw values
    private float _nutrientsEaten;
    private float _attackedOther;
    private float _damageTaken;
    private int _reproduced;

    //scaled properties
    public float ScaledMovementSpeedFactor => ScaledToSizeValue(SimulationScript.Instance.CoSh.MinMovementSpeed, SimulationScript.Instance.CoSh.MaxMovementSpeed, true);
    public float ScaledMaxEnergy => ScaledToSizeValue(SimulationScript.Instance.CoSh.MinActiveEnergy, SimulationScript.Instance.CoSh.MaxActiveEnergy, false);
    public float ScaledMaxHealth => ScaledToSizeValue(SimulationScript.Instance.CoSh.MinHealth, SimulationScript.Instance.CoSh.MaxHealth, false);
    public float ScaledFoodIntake => ScaledToSizeValue(SimulationScript.Instance.CoSh.MinNutrientIntake, SimulationScript.Instance.CoSh.MaxNutrientIntake, false);
    public float ScaledAttackDamage => ScaledToSizeValue(SimulationScript.Instance.CoSh.MinAttackDamage, SimulationScript.Instance.CoSh.MaxAttackDamage, false);
    public float ScaledDefenseFactor => ScaledToSizeValue(SimulationScript.Instance.CoSh.MinDefenseFactor, SimulationScript.Instance.CoSh.MaxDefenseFactor, false);
    public float ScaledBaseEnergyLoss => ScaledToSizeValue(SimulationScript.Instance.CoSh.MinBaseEnergyConsumption, SimulationScript.Instance.CoSh.MaxBaseEnergyConsumption, false);
    public float ScaledStomachContent => ScaledToSizeValue(SimulationScript.Instance.CoSh.MinStomachSize, SimulationScript.Instance.CoSh.MaxStomachSize, false);
    public float ScaledDigestionSpeed => ScaledToSizeValue(SimulationScript.Instance.CoSh.MinDigestionRate, SimulationScript.Instance.CoSh.MaxDigestionRate, false);
    public float ScaledEnergyToReproduce => ScaledToSizeValue(SimulationScript.Instance.CoSh.MinEnergyToReproduce, SimulationScript.Instance.CoSh.MaxEnergyToReproduce, false);

    public override void Awake() {
        base.Awake();

        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<PolygonCollider2D>();
        VisionMask = LayerMask.GetMask("Food", "Entity");
        EatMask = LayerMask.GetMask("Food");
        AttackMask = LayerMask.GetMask("Entity");

        //init neural network
        _brainID = SimulationScript.Instance.Neat.NetworkCollection.Count;
        SimulationScript.Instance.Neat.AddNetwork(_brainID);
        NeuralNetHandler = new NeuralNetHandler(this);
    }

    public void InitEntity() {
        StopAllCoroutines();

        //generate empty data
        Gene = new Gene(SimulationScript.Instance.CoSh);
        Gene.UpdateAppearance(this);
        SimulationScript.Instance.Neat.SpeciateSingle(Network);
        Fitness = 0f;

        Age = 0;
        VisionAngle = 0f;
        AimedRotationDir = 0f;
        AimedMovementDir = Vector2.zero;
        Health = ScaledMaxHealth;
        IsMutated = false;
        IsPregnant = false;
        SexualPartner.Network = null;
        SexualPartner.Gene = null;
        PassedTime = 0f;
        _eatCooldown = 0f;
        _attackCooldown = 0f;
        _pheromoneCooldown = 0f;
        _checkInAreaCooldown = 0f;

        _nutrientsEaten = 0f;
        _attackedOther = 0;
        _damageTaken = 0;
        _reproduced = 0;

        EnergyHandler = new EnergyHandler(this);
        VisionHandler = new VisionHandler(this, VisionMask);
        _smelledPherosBuffer = new PheromoneScript[2];

        //init pheromones
        var m = ParticleSystem.main;
        m.startColor = Gene.PheromoneColor;
        ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        StartCoroutine(Ageing());
    }

    public void InitEntity(SerializableEntity se) {
        StopAllCoroutines();

        //transfer data from serializable entity
        Gene = new Gene(se.Gene);
        Gene.UpdateAppearance(this);
        SimulationScript.Instance.Neat.ChangeNetwork(_brainID, se.Network); //also speciates
        Fitness = 0f;

        Age = 0;
        VisionAngle = 0f;
        AimedRotationDir = 0f;
        AimedMovementDir = Vector2.zero;
        Health = SimulationScript.Instance.CoSh.MinHealth;
        IsMutated = false;
        IsPregnant = false;
        SexualPartner.Network = null;
        SexualPartner.Gene = null;
        PassedTime = 0f;
        _eatCooldown = 0f;
        _attackCooldown = 0f;
        _pheromoneCooldown = 0f;
        _checkInAreaCooldown = 0f;

        _nutrientsEaten = 0f;
        _attackedOther = 0;
        _damageTaken = 0;
        _reproduced = 0;

        EnergyHandler = new EnergyHandler(this);
        VisionHandler = new VisionHandler(this, VisionMask);
        _smelledPherosBuffer = new PheromoneScript[2];

        //init pheromones
        var m = ParticleSystem.main;
        m.startColor = Gene.PheromoneColor;
        ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        StartCoroutine(Ageing());
    }

    private int _step;
    void FixedUpdate() {
        PassedTime += Time.deltaTime;

        _pheromoneCooldown += Time.deltaTime;
        _eatCooldown += Time.deltaTime;
        _attackCooldown += Time.deltaTime;
        _checkInAreaCooldown += Time.deltaTime;

        //update vision
        if (_step >= SimulationScript.Instance.CoSh.CheckVisionStep) {
            VisionHandler.UpdateVision(VisionVector, Gene.FieldOfView, SimulationScript.Instance.CoSh.RaycastVision, true);
            _step = 0;
        } else {
            VisionHandler.UpdateVision(VisionVector, Gene.FieldOfView, SimulationScript.Instance.CoSh.RaycastVision, false);
        }
        _step++;

        //calculate neural network
        Network.Fitness = Fitness;
        NeuralNetHandler.GetInputNeuronValues(Network.InputValues);
        Network.CalculateNetwork();
        NeuralNetHandler.ComputeOutputs(Network.OutputValues);

        //check if outside of a food area
        CheckInFoodArea();

        //process neural network outputs
        Move();

        //update energy
        EnergyHandler.ConsumeEnergy();

        //check if starving
        if (EnergyHandler.ActiveEnergy == 0f) Health -= 10f * Time.deltaTime;
    }

    private void Move() {
        Rigidbody.inertia = 0.08f;
        Rigidbody.AddForce(AimedMovementDir * ScaledMovementSpeedFactor);
        Rigidbody.AddTorque(AimedRotationDir * SimulationScript.Instance.CoSh.MaxRotationSpeed);
    }

    public void Reproduce() {
        if (SimulationScript.Instance.OffspringBudget.TryGetValue(Network.SpeciesID, out var budget)) {
            //if no budget, dont reproduce
            if (budget <= Species.AllNetworks.Count) return;
        } else {
            Debug.Log($"Could not find offspring budget for species: {Network.SpeciesID}");
            return;
        }

        Vector2 spawnPos = transform.position - transform.up * 2f;

        //use self if no sexual partner defined
        if (SexualPartner.Network == null) SexualPartner = new SerializableEntity(this);

        //inherit genes from fittest and crossover networks
        SerializableEntity child = Network.Fitness > SexualPartner.Network.Fitness ? new SerializableEntity(this) : SexualPartner;

        //spawn entity
        EntityScript es = SimulationScript.Instance.EntityPool.SpawnEntity(spawnPos, child);
        if (es == null) return; //if entity spawn pool is full, return

        //change network to the crossover network
        SimulationScript.Instance.Neat.ChangeNetwork(es.Network.NetworkID, SimulationScript.Instance.Neat.CrossoverNetworks(es.Network.NetworkID, SexualPartner.Network, Network));
        
        //mutate child randomly
        if(Random.value <= SimulationScript.Instance.CoSh.ChildMutationChance) es.Mutate(Random.Range(1, SimulationScript.Instance.CoSh.MaxChildMutations + 1));
        es.Gene.Generation++;

        _reproduced++;

        SexualPartner.Network = null;
        SexualPartner.Gene = null;
    }

    //TODO check if in radioactive area
    public void Mutate(int count) {
        if (count == 0) return;

        for (int i = 0; i < count; i++) {
            Network.Mutate(SimulationScript.Instance.Neat, SimulationScript.Instance.CoSh.MutateOptions);
            Gene.MutateGenes(this);
        }

        SimulationScript.Instance.Neat.SpeciateSingle(Network);

        IsMutated = true;
    }

    //returns the actual damage
    public float Damage(float amount) {
        float damageAmount = amount * (1f - ScaledDefenseFactor);
        Health -= damageAmount;

        _damageTaken += damageAmount;

        return damageAmount;
    }

    public void Kill(bool spawnLeftover) {

        //spawn meat on death
        if (spawnLeftover) {
            float energyLeft = SimulationScript.Instance.CoSh.LeftoverBaseEnergy + EnergyHandler.AmountInStomach + EnergyHandler.ActiveEnergy/2f;
            while (energyLeft > 0f) {
                if (energyLeft <= SimulationScript.Instance.CoSh.MaxFoodNutrition) {
                    SimulationScript.Instance.FoodPool.SpawnFood(transform.position, energyLeft, true);
                    break;
                }

                FoodScript f = SimulationScript.Instance.FoodPool.SpawnFood(transform.position, SimulationScript.Instance.CoSh.MaxFoodNutrition, true);
                energyLeft -= f.NutritionalValue;
            }
        }

        if (Network.Fitness > SimulationScript.Instance.BestEntity.Item1) SimulationScript.Instance.BestEntity = new Tuple<float, SerializableEntity>(Network.Fitness, new SerializableEntity(this));
        SimulationScript.Instance.EntityPool.DespawnEntity(this);
    }

    private PheromoneScript[] _smelledPherosBuffer;
    public PheromoneScript Smell() {
        int amount = CollisionWatcher.All(_smelledPherosBuffer);

        for (int i = 0; i < amount; i++) {
            if (_smelledPherosBuffer[i].SenderID != gameObject.GetInstanceID()) return _smelledPherosBuffer[i];
        }

        return null;
    }

    private float _pheromoneCooldown;
    public void ProducePheromone() {
        if(_pheromoneCooldown < SimulationScript.Instance.CoSh.PheromoneCooldown) return;

        if(SimulationScript.Instance.CoSh.ShowParticles) ParticleSystem.Emit(3);
        SimulationScript.Instance.PheromonePool.SpawnPheromone(transform.position, Rigidbody.linearVelocity.normalized, Gene.PheromoneColor, gameObject.GetInstanceID());

        _pheromoneCooldown = 0f;
    }

    private float _eatCooldown;
    public void Eat() {
        if (_eatCooldown < SimulationScript.Instance.CoSh.EatCooldown) return;

        if (VisionHandler.InFrontCollider != null && VisionHandler.InFrontCollider.TryGetComponent(out FoodScript fs)) {
            float actualIntake = fs.NutritionalValue < ScaledFoodIntake ? fs.NutritionalValue : ScaledFoodIntake;
            actualIntake = EnergyHandler.ToStomach(actualIntake, fs.IsMeat);
            fs.NutritionalValue -= actualIntake;

            _nutrientsEaten += actualIntake;

            _eatCooldown = 0f;
        }
    }

    private float _attackCooldown;
    public void Attack() {
        if (_attackCooldown < SimulationScript.Instance.CoSh.AttackCooldown) return;

        if (VisionHandler.InFrontCollider != null && VisionHandler.InFrontCollider.TryGetComponent(out EntityScript es)) {
            float factor = Network.OutputValues[(int)ActionNeuron.ActionAttack];
            float dealedDamage = es.Damage(ScaledAttackDamage * factor);
            EnergyHandler.ToStomach(dealedDamage, true);

            _attackedOther += dealedDamage;

            _attackCooldown = 0f;
        }
    }

    private IEnumerator Ageing() {
        while (Age <= SimulationScript.Instance.CoSh.MaxAge) {
            yield return new WaitForSeconds(SimulationScript.Instance.CoSh.AgeingFrequency);
            Age++;
        }

        Kill(true);
    }

    void OnMouseDown() {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (SimulationScript.Instance.MenuManager.EntityMenu.isActiveAndEnabled) {
            SimulationScript.Instance.MenuManager.EntityMenu.UpdateEntity(this);
        } else {
            SimulationScript.Instance.MenuManager.EntityMenu.OpenMenu(this);
        }
    }

    public float ScaledToSizeValue(float min, float max, bool smallestSizeIsMax) {
        if (smallestSizeIsMax) {
            return Mathf.Lerp(min, max, 1f - Mathf.InverseLerp(SimulationScript.Instance.CoSh.MinEntitySize, SimulationScript.Instance.CoSh.MaxEntitySize, Gene.EntitySize));
        }

        return Mathf.Lerp(min, max, Mathf.InverseLerp(SimulationScript.Instance.CoSh.MinEntitySize, SimulationScript.Instance.CoSh.MaxEntitySize, Gene.EntitySize));
    }

    public float CalculateFitness() {
        //if (_reproduced == 0) return 0f;

        float f = 0f;

        f += _nutrientsEaten / 250f;
        f += _attackedOther / 100f;
        f -= _damageTaken / 100f;
        f += (float)_reproduced / 2f;
        f += (float)Age / (SimulationScript.Instance.CoSh.MaxAge/2f);

        return f;
    }

    private float _checkInAreaCooldown;
    private void CheckInFoodArea() {
        if (_checkInAreaCooldown < 5f || !SimulationScript.Instance.CoSh.RotateToMainArea || CollisionWatcher.TryGet(out FoodSpawnAreaScript _)) return;

        //if not in a food area, rotate to nearest food area
        var fsa = Utility.GetNearestFoodArea(transform.position);
        float rot = Vector2.SignedAngle(Rigidbody.linearVelocity, fsa.transform.position - transform.position) / 180f;
        Rigidbody.AddTorque(rot * 5f, ForceMode2D.Impulse);

        _checkInAreaCooldown = 0f;
    }
}

[Serializable]
public class Gene {

    public int Generation;
    public EntityDiet Diet;

    public float OscillatorFrequency {
        get => _oscillatorFrequency;
        set => _oscillatorFrequency = Mathf.Clamp(value, SimulationScript.Instance.CoSh.MinOscillatorFrequency, SimulationScript.Instance.CoSh.MaxOscillatorFrequency);
    }
    private float _oscillatorFrequency;

    public float ViewDistance {
        get => _viewDistance;
        set => _viewDistance = Mathf.Clamp(value, SimulationScript.Instance.CoSh.MinViewDistance, SimulationScript.Instance.CoSh.MaxViewDistance);
    }
    private float _viewDistance;

    public float EntitySize {
        get => _entitySize;
        set => _entitySize = Mathf.Clamp(value, SimulationScript.Instance.CoSh.MinEntitySize, SimulationScript.Instance.CoSh.MaxEntitySize);
    }
    private float _entitySize;

    public float FieldOfView {
        get => _fieldOfView;
        set => _fieldOfView = Mathf.Clamp(value, SimulationScript.Instance.CoSh.MinFieldOfView, SimulationScript.Instance.CoSh.MaxFieldOfView);
    }
    private float _fieldOfView;

    public Color EntityColor {
        get => new Color(_entityColor[0], _entityColor[1], _entityColor[2], _entityColor[3]);
        set { _entityColor = new[] { value.r, value.g, value.b, value.a }; }
    }
    private float[] _entityColor = new[] { 1f, 1f, 1f, 1f };

    public Color PheromoneColor {
        get => new Color(_pheromoneColor[0], _pheromoneColor[1], _pheromoneColor[2], _pheromoneColor[3]);
        set { _pheromoneColor = new[] { value.r, value.g, value.b, value.a }; }
    }
    private float[] _pheromoneColor = new[] { 1f, 1f, 1f, 1f };

    //init default gene for entity
    //using parameter because unity would call this constructor in editor before SimulationScript is created
    public Gene(ConstantSheet cs) {
        Generation = 0;
        EntitySize = SimulationScript.Instance.CoSh.MinEntitySize;
        ViewDistance = SimulationScript.Instance.CoSh.MinViewDistance;
        FieldOfView = SimulationScript.Instance.CoSh.MinFieldOfView;
        EntityColor = Color.white;
        PheromoneColor = Color.white;
        Diet = default;
        OscillatorFrequency = SimulationScript.Instance.CoSh.MinOscillatorFrequency;
    }

    //copy gene
    public Gene(Gene toCopy) {
        Generation = toCopy.Generation;
        EntitySize = toCopy.EntitySize;
        ViewDistance = toCopy.ViewDistance;
        FieldOfView = toCopy.FieldOfView;
        EntityColor = toCopy.EntityColor;
        PheromoneColor = toCopy.PheromoneColor;
        Diet = toCopy.Diet;
        OscillatorFrequency = toCopy.OscillatorFrequency;
    }

    //TODO change color depending on species
    public void MutateGenes(EntityScript es) {
        if (Random.value < 0.2f) EntitySize += Random.Range(-1f, 1f) * SimulationScript.Instance.CoSh.SizeMutationFactor;
        if (Random.value < 0.2f) ViewDistance += Random.Range(-1f, 1f) * SimulationScript.Instance.CoSh.ViewDistanceMutationFactor;
        if (Random.value < 0.2f) FieldOfView += Random.Range(-1f, 1f) * SimulationScript.Instance.CoSh.FieldOfViewMutationFactor;
        if (Random.value < 0.2f) OscillatorFrequency += Random.Range(-1f, 1f) * SimulationScript.Instance.CoSh.OscillatorMutationFactor;
        if (Random.value < 0.08f) Diet = Diet == EntityDiet.Carnivore ? EntityDiet.Herbivore : EntityDiet.Carnivore;

        //mutate pheromone
        if (Random.value < 0.2f) {
            float rndR = Random.Range(-1f, 1f) * SimulationScript.Instance.CoSh.MaxPheromoneDifference;
            float rndG = Random.Range(-1f, 1f) * SimulationScript.Instance.CoSh.MaxPheromoneDifference;
            float rndB = Random.Range(-1f, 1f) * SimulationScript.Instance.CoSh.MaxPheromoneDifference;
            PheromoneColor += new Color(rndR, rndG, rndB);
        }

        //mutate entity color
        if (Random.value < 0.2f) {
            float rndR = Random.Range(-1f, 1f) * SimulationScript.Instance.CoSh.MaxColorDifference;
            float rndG = Random.Range(-1f, 1f) * SimulationScript.Instance.CoSh.MaxColorDifference;
            float rndB = Random.Range(-1f, 1f) * SimulationScript.Instance.CoSh.MaxColorDifference;
            EntityColor += new Color(rndR, rndG, rndB);
        }

        //update entity appearance
        UpdateAppearance(es);
    }

    public void UpdateAppearance(EntityScript es) {
        es.GetComponent<SpriteRenderer>().color = EntityColor;
        es.transform.localScale = Vector3.one * EntitySize;
    }

}

[Serializable]
public struct SerializableEntity {

    public Network Network;
    public Gene Gene;

    public SerializableEntity(EntityScript es) {
        Network = new Network(es.Network.NetworkID, es.Network);
        Gene = new Gene(es.Gene);
    }

}

[Serializable]
public enum EntityDiet {Herbivore, Carnivore}