using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NeuraSuite.NeatExpanded;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimulationScript : MonoBehaviour {

    public static SimulationScript Instance;

    //other
    public ConstantSheet CoSh = new ();
    public MenuManager MenuManager;

    public Tuple<float, SerializableEntity> BestEntity {
        get => _bestEntity;
        set => _bestEntity = value;
    }
    private Tuple<float, SerializableEntity> _bestEntity = new (0f, default);

    //map
    public GameObject Map;
    [HideInInspector] public FoodSpawnAreaScript[] FoodAreas;

    //prefabs
    public GameObject FoodPrefab, EntityPrefab, PheromonePrefab;

    //object pooling
    public FoodPool FoodPool;
    public EntityPool EntityPool;
    public PheromonePool PheromonePool;

    //reproduction
    public Dictionary<int, int> OffspringBudget;

    //neural net
    public Neat Neat;

    //assets
    private SerializableEntity _defaultEntity;

    void Awake() {

        Instance = this;

        //load assets
        if(PlayerPrefs.GetString("DefaultEntity") != "empty") _defaultEntity = Serializer.ReadFromBinaryFile<SerializableEntity>(Application.persistentDataPath + "/" + PlayerPrefs.GetString("DefaultEntity"));

        //init neural net
        NeuralNetHandler.GetNeuronTemplates(out Neuron[] ins, out Neuron[] outs);
        Neat = new Neat(ins, outs, CoSh.SpeciationOptions);

        //init object pooling
        FoodPool = new FoodPool(FoodPrefab, transform, 2500);
        EntityPool = new EntityPool(EntityPrefab, transform, 400);
        PheromonePool = new PheromonePool(PheromonePrefab, transform, 1500);

        OffspringBudget = new Dictionary<int, int>();
    }

    void Start() {

        FoodAreas = Map.GetComponentsInChildren<FoodSpawnAreaScript>();

        //spawn entities
        for (int i = 0; i < PlayerPrefs.GetInt("EntityCount"); i++) {
            EntityScript es = EntityPool.SpawnEntity(Utility.RandomPosInRadius(transform.position, 400), _defaultEntity);
            es.Mutate(PlayerPrefs.GetInt("MutationCount"));
        }

        //spawn food
        for (int i = 0; i < PlayerPrefs.GetInt("FoodCount"); i++) {
            FoodPool.SpawnFood(Utility.RandomPosInRadius(transform.position, 400), Utility.RandomNutritionalValue, false);
        }

        StartCoroutine(CheckImprovement());
        StartCoroutine(UpdateOffspringBudget());

    }

    void FixedUpdate() {

        //hold steady population of min. CoSh.MinPopulation entities
        if (EntityPool.ActiveEntities.Count < CoSh.MinPopulation) {
            EntityScript es = EntityPool.SpawnEntity(Utility.RandomPosInRadius(transform.position, 400), BestEntity.Item2.Network != null ? BestEntity.Item2 : _defaultEntity);
            es.Mutate(PlayerPrefs.GetInt("MutationCount"));
        }

        //Parallel.For(0, EntityPool.ActiveEntities.Count, (i) => EntityPool.ActiveEntities[i].Network.CalculateNetwork());
    }

    /// <summary>
    /// Updates the budget each species has. When the population of a species is over the budget, it cannot reproduce.
    /// </summary>
    private IEnumerator UpdateOffspringBudget() {
        while (true) {
            yield return new WaitForSeconds(5f);
            OffspringBudget = Neat.GetOffspringAmount(120).ToDictionary(o => o.Item1, o => Mathf.Max(o.Item2, CoSh.MinSpeciesBudget));
        }
    }

    /// <summary>
    /// Checks periodically if the fitness of the entities are improving or stagnating. Changes mutation to be more aggressive when stagnated.
    /// </summary>
    private IEnumerator CheckImprovement() {
        float currentFitness = 0f;
        while (true) {

            yield return new WaitForSeconds(CoSh.CheckImprovementRate);

            CoSh.AdaptionPhase = BestEntity.Item1 > currentFitness;

            currentFitness = BestEntity.Item1;

            //reset
            BestEntity = new(0f, default);
        }
    }

    /// <summary>
    /// Picks a random point in a random food area.
    /// </summary>
    private Vector2 RandomSpawnPosition() {
        var fsa = FoodAreas[Random.Range(0, FoodAreas.Length)];
        return Utility.RandomPosInRadius(fsa.transform.position, fsa.Radius);
    }

    void OnApplicationQuit() {
        Debug.Log("Quit");
        PlayerPrefs.DeleteAll();
    }
}

public static class Utility {

    public static float RandomNutritionalValue => Random.Range(SimulationScript.Instance.CoSh.MinFoodNutritíon, SimulationScript.Instance.CoSh.MaxFoodNutrition);

    public static Vector2 RandomPosInRadius(Vector2 position, float radius) => position + Random.insideUnitCircle * radius;

    public static bool IsCollidingOnPos(Vector2 pos, float radius, int layerMask) => Physics2D.OverlapCircle(pos, radius, layerMask) != null;

    public static LineRenderer MakeVisualizerLine(Transform parent, Color color, Material mat) {
        GameObject lineObj = new("Line" + parent.transform.childCount);
        lineObj.transform.parent = parent;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.startColor = color;
        lr.endColor = color;
        lr.sharedMaterial = mat;

        return lr;
    }

    public static FoodSpawnAreaScript GetNearestFoodArea(Vector2 position) {

        float sqr = 0f;
        float minDist = float.MaxValue;
        FoodSpawnAreaScript minDistArea = null;

        for (int i = 0; i < SimulationScript.Instance.FoodAreas.Length; i++) {
            sqr = (position - (Vector2)SimulationScript.Instance.FoodAreas[i].transform.position).sqrMagnitude;
            if (sqr < minDist) {
                minDist = sqr;
                minDistArea = SimulationScript.Instance.FoodAreas[i];
            }
        }

        return minDistArea;
    }
}