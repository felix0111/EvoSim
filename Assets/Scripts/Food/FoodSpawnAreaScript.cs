using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class FoodSpawnAreaScript : MonoBehaviour {

    public float Radius {
        get => _spriteRenderer.bounds.extents.x;
        set => transform.localScale = new Vector3(value * 2f, value * 2f);
    }

    private SpriteRenderer _spriteRenderer;

    public int MaxFoodAmount;
    public float SpawnFrequency;
    public bool SpawnMeat;
    public bool HoldMaxAmount;

    public int SpawnCount;

    void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start() {
        StartCoroutine(CheckAmountRoutine());
        StartCoroutine(SpawnRoutine());
    }


    IEnumerator SpawnRoutine() {
        while (true) {
            yield return new WaitForSeconds(SpawnFrequency);

            if (SpawnCount >= MaxFoodAmount) continue;

            if (HoldMaxAmount) {

                int amountLeft = MaxFoodAmount - SpawnCount;
                for (int i = 0; i < amountLeft; i++) {
                    Vector2 pos = Utility.RandomPosInRadius(transform.position, Radius);
                    if (Utility.IsCollidingOnPos(pos, SimulationScript.Instance.CoSh.MaxFoodRadius, LayerMask.GetMask("Food", "Entity"))) continue;

                    SimulationScript.Instance.FoodPool.SpawnFood(pos, Utility.RandomNutritionalValue, SpawnMeat);
                    SpawnCount++;
                }

            } else {
                Vector2 pos = Utility.RandomPosInRadius(transform.position, Radius);
                if (Utility.IsCollidingOnPos(pos, SimulationScript.Instance.CoSh.MaxFoodRadius, LayerMask.GetMask("Food", "Entity"))) continue;

                SimulationScript.Instance.FoodPool.SpawnFood(pos, Utility.RandomNutritionalValue, SpawnMeat);
                SpawnCount++;
            }
        }
    }

    IEnumerator CheckAmountRoutine() {
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