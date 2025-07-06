using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class FoodScript : Interactable {

    public float Radius => transform.localScale.x / 2f;

    public Sprite PlantSprite, MeatSprite;

    public bool IsMeat {
        get => _isMeat;
        set {
            _isMeat = value;
            _spriteRenderer.sprite = _isMeat ? MeatSprite : PlantSprite;
            StopAllCoroutines();
            StartCoroutine(Decomposition());
        }
    }
    private bool _isMeat;

    public float Density {
        get => _collider.density;
        set => _collider.density = value;
    }

    public float NutritionalValue {
        get => _nutritionalValue;
        set {
            if(value < SimulationScript.Instance.CoSh.MinFoodNutrition) DestroyFood();
            if(value > SimulationScript.Instance.CoSh.MaxFoodNutrition) value = SimulationScript.Instance.CoSh.MaxFoodNutrition;
            _nutritionalValue = value;

            float r = NutritionToSize(value);
            transform.localScale = new Vector2(r, r);
        }
    }
    private float _nutritionalValue;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    //called only once
    public override void Awake() {
        base.Awake();

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    //resetting to default values
    void OnEnable() {
        NutritionalValue = SimulationScript.Instance.CoSh.MinFoodNutrition;
        IsMeat = false;
    }

    IEnumerator Decomposition() {
        yield return new WaitForSeconds(SimulationScript.Instance.CoSh.FoodDecompositionRate);
        DestroyFood();
    }

    public void DestroyFood() {
        SimulationScript.Instance.FoodPool.DespawnFood(this);
    }

    public static float NutritionToSize(float nutrition) {
        return Mathf.Lerp(SimulationScript.Instance.CoSh.MinFoodRadius, SimulationScript.Instance.CoSh.MaxFoodRadius, Mathf.InverseLerp(SimulationScript.Instance.CoSh.MinFoodNutrition, SimulationScript.Instance.CoSh.MaxFoodNutrition, nutrition));
    }

    void OnMouseOver() {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        //if right clicked
        if (!Input.GetMouseButtonDown(1)) return;
        Debug.Log(NutritionalValue);
    }
}
