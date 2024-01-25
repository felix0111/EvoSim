using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class FoodPool {

    private FoodScript[] _allFoods;
    private List<FoodScript> _activeFoods = new ();

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
        fs.transform.position = pos;

        fs.IsMeat = isMeat;
        fs.NutritionalValue = nutValue;

        _activeFoods.Add(fs);

        return fs;
    }

    public void DespawnFood(FoodScript fs) {
        _activeFoods.Remove(fs);
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
        es.transform.position = pos;
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
        SimulationScript.Instance.Neat.RemoveEmptySpecies();

        es.gameObject.SetActive(false);
    }
}