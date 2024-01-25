using System.Collections;
using UnityEngine;

public class FoodSpawnAreaScript : MonoBehaviour {

    public float Radius => _spriteRenderer.bounds.extents.x;

    private SpriteRenderer _spriteRenderer;

    public float SpawnFrequency;
    public bool SpawnMeat;
    public int MaxSpawnCount;

    public int SpawnCount;

    void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start() {
        StartCoroutine(SpawnRoutine());
    }


    IEnumerator SpawnRoutine() {
        while (true) {
            yield return new WaitForSeconds(SpawnFrequency);

            if (SpawnCount >= MaxSpawnCount) {
                SpawnCount = Physics2D.OverlapCircleAll(transform.position, Radius, LayerMask.GetMask("Food")).Length;
                if (SpawnCount >= MaxSpawnCount) continue;  //if still enough food in radius, don't spawn new
            }

            Vector2 pos = Utility.RandomPosInRadius(transform.position, Radius);
            if(Utility.IsCollidingOnPos(pos, SimulationScript.Instance.CoSh.MaxFoodRadius, LayerMask.GetMask("Food", "Entity", "Obstacle"))) continue;

            SimulationScript.Instance.FoodPool.SpawnFood(pos, Utility.RandomNutritionalValue, SpawnMeat);
            SpawnCount++;
        }
    }
}