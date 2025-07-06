using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class FoodSpawnAreaScript : MonoBehaviour {

    public float Radius {
        get => _spriteRenderer.bounds.extents.x;
        set => transform.localScale = new Vector3(value * 2f, value * 2f);
    }

    private SpriteRenderer _spriteRenderer;

    public float MinNutrition, MaxNutrition;
    public int MaxFoodAmount;
    public float SpawnFrequency;
    public bool SpawnPlant, SpawnMeat;

    public int SpawnCount;

    void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start() {
        StartCoroutine(CheckAmountRoutine());
        StartCoroutine(SpawnRoutine());
    }


    private IEnumerator SpawnRoutine() {
        while (true) {
            yield return new WaitForSeconds(SpawnFrequency);
            if (SpawnCount >= MaxFoodAmount || !SpawnPlant && !SpawnMeat) continue;
            
            Vector2 pos = Utility.RandomPosInRadius(transform.position, Radius);
            float randomNutrition = Random.Range(MinNutrition, MaxNutrition);

            if (Utility.IsCollidingOnPos(pos, FoodScript.NutritionToSize(randomNutrition), LayerMask.GetMask("Food", "Entity", "Obstacle"))) continue;

            SimulationScript.Instance.FoodPool.SpawnFood(pos, randomNutrition, SpawnPlant && SpawnMeat ? Random.value > 0.5f : SpawnMeat);
            SpawnCount++;
        }
    }

    private IEnumerator CheckAmountRoutine() {
        while (true) {
            yield return new WaitForSeconds(15f);
            SpawnCount = Physics2D.OverlapCircleAll(transform.position, Radius, LayerMask.GetMask("Food")).Length;
        }
    }

    void OnMouseOver() {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        //if right clicked
        if (!Input.GetMouseButtonDown(1)) return;
        SimulationScript.Instance.MenuManager.FoodSpawnerMenu.OpenMenu(this);
    }
}