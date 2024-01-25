using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyNNFramework.NEAT;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Random = UnityEngine.Random;

public class SimulationScript : MonoBehaviour {

    public static SimulationScript Instance;

    //other
    public ConstantSheet CoSh = new ();
    public MenuManager MenuManager;

    public Tuple<float, SerializableEntity> BestEntity {
        get => _bestEntity;
        set {
            if (value.Item1 > _bestEntity.Item1) CoSh.AdaptionPhase = true;
            _bestEntity = value;
        }
    }
    private Tuple<float, SerializableEntity> _bestEntity = new (0f, default);

    //map
    public GameObject Map;
    public FoodSpawnAreaScript[] SpawnAreas;

    //prefabs
    public GameObject FoodPrefab, EntityPrefab;

    //object pooling
    public FoodPool FoodPool;
    public EntityPool EntityPool;

    //neural net
    public Neat Neat;

    //assets
    private SerializableEntity _defaultEntity;

    void Awake() {

        Instance = this;

        //load assets
        if(PlayerPrefs.GetString("DefaultEntity") != "empty") _defaultEntity = Serializer.ReadFromBinaryFile<SerializableEntity>(Application.streamingAssetsPath + "/" + PlayerPrefs.GetString("DefaultEntity"));

        //init neural net
        NeuralNetHandler.GetNeuronTemplates(out Neuron[] ins, out Neuron[] outs);
        Neat = new Neat(ins, outs, CoSh.SpeciationOptions);

        //init object pooling
        FoodPool = new FoodPool(FoodPrefab, transform, 1500);
        EntityPool = new EntityPool(EntityPrefab, transform, 300);
    }

    void Start() {

        SpawnAreas = Map.GetComponentsInChildren<FoodSpawnAreaScript>();

        //spawn entities
        for (int i = 0; i < PlayerPrefs.GetInt("EntityCount"); i++) {
            EntityScript es = EntityPool.SpawnEntity(Utility.RandomPosInRadius(transform.position, 400), _defaultEntity);
            es.Mutate(PlayerPrefs.GetInt("MutationCount"));
        }

        StartCoroutine(CheckImprovement());
    }

    void FixedUpdate() {
        //hold steady population of min. 20 entities
        if (EntityPool.ActiveEntities.Count < 20) {
            EntityScript es = EntityPool.SpawnEntity(Utility.RandomPosInRadius(transform.position, 400), BestEntity.Item2.Network != null ? BestEntity.Item2 : _defaultEntity);
            if(Random.value <= CoSh.ChildMutationChance) es.Mutate(Random.Range(1, CoSh.MaxChildMutations + 1));
        }
    }

    IEnumerator CheckImprovement() {
        while (true) {

            float currentFitness = BestEntity.Item1;

            yield return new WaitForSeconds(CoSh.CheckImprovementRate);

            CoSh.AdaptionPhase = BestEntity.Item1 > currentFitness;
        }
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

    public static FoodSpawnAreaScript GetNearestArea(Vector2 position) {

        float sqr = 0f;
        float minDist = float.MaxValue;
        FoodSpawnAreaScript minDistArea = null;
        FoodSpawnAreaScript[] areas = SimulationScript.Instance.SpawnAreas;

        for (int i = 0; i < areas.Length; i++) {
            sqr = (position - (Vector2)areas[i].transform.position).sqrMagnitude;
            if (sqr < minDist) {
                minDist = sqr;
                minDistArea = areas[i];
            }
        }

        return minDistArea;
    }
}