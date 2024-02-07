using System.Collections;
using UnityEngine;

public class FoodScript : Interactable {

    public float Radius => transform.localScale.x / 2f;

    public Sprite PlantSprite, MeatSprite;

    public bool IsMeat {
        get => _isMeat;
        set {
            _isMeat = value;
            if (_isMeat) {
                _spriteRenderer.sprite = MeatSprite;
                StartCoroutine(MeatDecomposition());
            } else {
                _spriteRenderer.sprite = PlantSprite;
                StopAllCoroutines();
            }
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
            if(value < SimulationScript.Instance.CoSh.MinFoodNutritíon) DestroyFood();
            _nutritionalValue = value;
            float a = Mathf.InverseLerp(SimulationScript.Instance.CoSh.MinFoodNutritíon, SimulationScript.Instance.CoSh.MaxFoodNutrition, _nutritionalValue);
            float b = Mathf.Lerp(SimulationScript.Instance.CoSh.MinFoodRadius, SimulationScript.Instance.CoSh.MaxFoodRadius, a);
            transform.localScale = new Vector2(b, b);
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
        NutritionalValue = SimulationScript.Instance.CoSh.MinFoodNutritíon;
        IsMeat = false;
    }

    IEnumerator MeatDecomposition() {
        yield return new WaitForSeconds(SimulationScript.Instance.CoSh.MeatDecompositionRate);
        if(IsMeat) DestroyFood();
    }

    public void DestroyFood() {
        SimulationScript.Instance.FoodPool.DespawnFood(this);
    }
}
