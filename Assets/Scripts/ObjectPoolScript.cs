using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoodPool {

    private FoodScript[] _allFoods;
    public List<FoodScript> ActiveFoods = new ();

    private readonly int _foodCount;

    public FoodPool(GameObject prefab, Transform placeholder, int count) {
        _foodCount = count;
        _allFoods = new FoodScript[_foodCount];

        //pre-instantiate
        for (int i = 0; i < _foodCount; i++) {
            _allFoods[i] = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, placeholder).GetComponent<FoodScript>();
            _allFoods[i].gameObject.name = "Food" + i;
            _allFoods[i].gameObject.SetActive(false);
        }
    }

    public FoodScript SpawnFood(Vector2 pos, float nutValue, bool isMeat) {
        FoodScript fs = _allFoods.FirstOrDefault(o => !o.gameObject.activeSelf);

        //if no food available, use first one
        if (fs == null) {
            fs = _allFoods[0];
            DespawnFood(fs);
        }

        //resets food
        fs.gameObject.SetActive(true);
        fs.transform.position = (Vector3)pos + Vector3.back;

        fs.IsMeat = isMeat;
        fs.Density = SimulationScript.Instance.CoSh.FoodDensity;
        fs.NutritionalValue = nutValue;

        ActiveFoods.Add(fs);

        return fs;
    }

    public void DespawnFood(FoodScript fs) {
        ActiveFoods.Remove(fs);
        fs.gameObject.SetActive(false);
    }

}

public class EntityPool {

    private EntityScript[] _allEntities;
    public List<EntityScript> ActiveEntities = new ();

    private readonly int _entityCount;

    public EntityPool(GameObject prefab, Transform placeholder, int count) {
        _entityCount = count;
        _allEntities = new EntityScript[_entityCount];

        //pre-instantiate
        for (int i = 0; i < _entityCount; i++) {
            _allEntities[i] = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, placeholder).GetComponent<EntityScript>();
            _allEntities[i].gameObject.name = "Entity" + i;
            _allEntities[i].gameObject.SetActive(false);
        }
    }

    public EntityScript SpawnEntity(Vector2 pos, SerializableEntity se) {
        EntityScript es = _allEntities.FirstOrDefault(o => !o.gameObject.activeSelf);
        if (es == null) return null;

        es.gameObject.SetActive(true);
        es.transform.position = (Vector3)pos + Vector3.back;
        es.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        if (se.Network == null || se.Gene == null) {
            es.InitEntity();
        } else {
            es.InitEntity(se);
        }
        

        ActiveEntities.Add(es);

        return es;
    }

    public void DespawnEntity(EntityScript es) {
        es.StopAllCoroutines();
        ActiveEntities.Remove(es);
        SimulationScript.Instance.Neat.Species[es.Network.SpeciesID].RemoveFromSpecies(es.Network);

        es.gameObject.SetActive(false);
    }
}

public class PheromonePool {

    private PheromoneScript[] _allPheromones;
    public List<PheromoneScript> ActivePheromones;

    private readonly int _maxPheromoneCount;

    public PheromonePool(GameObject prefab, Transform placeholder, int maxCount) {
        _maxPheromoneCount = maxCount;
        _allPheromones = new PheromoneScript[_maxPheromoneCount];
        ActivePheromones = new List<PheromoneScript>(_maxPheromoneCount);

        //pre-instantiate
        for (int i = 0; i < _maxPheromoneCount; i++) {
            _allPheromones[i] = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, placeholder).GetComponent<PheromoneScript>();
            _allPheromones[i].gameObject.name = "Pheromone" + i;
            _allPheromones[i].gameObject.SetActive(false);
        }
    }

    public PheromoneScript SpawnPheromone(Vector2 pos, Vector2 direction, Color color, int senderID) {
        PheromoneScript ps = _allPheromones.FirstOrDefault(o => !o.gameObject.activeSelf);
        if (ps == null) return null;

        ps.gameObject.SetActive(true);
        ps.transform.position = pos;
        ps.PheromoneColor = color;
        ps.SenderID = senderID;
        ps.Direction = direction;

        ps.StartPheromone();

        ActivePheromones.Add(ps);

        return ps;
    }

    public void DespawnPheromone(PheromoneScript ps) {
        ps.StopAllCoroutines();
        ActivePheromones.Remove(ps);
        ps.gameObject.SetActive(false);
    }

}