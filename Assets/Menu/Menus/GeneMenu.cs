using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GeneMenu : MonoBehaviour {
    public GameObject Content, SliderPrefab, CheckboxPrefab;

    public ConstantSheet CoSh;

    private EntityScript _entity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
        CoSh = SimulationScript.Instance.CoSh;
    }

    void Start() {
        AddSliderToSection("Oscillator Frequency", _entity.Gene.OscillatorFrequency, false, CoSh.MinOscillatorFrequency, CoSh.MaxOscillatorFrequency, o => _entity.Gene.OscillatorFrequency = o);
        AddSliderToSection("View Distance", _entity.Gene.ViewDistance, false, CoSh.MinViewDistance, CoSh.MaxViewDistance, o => _entity.Gene.ViewDistance = o);
        AddSliderToSection("Entity Size", _entity.Gene.EntitySize, false, CoSh.MinEntitySize, CoSh.MaxEntitySize, o => {
            _entity.Gene.EntitySize = o;
            _entity.Gene.UpdateAppearance(_entity);
        });
        AddSliderToSection("Field of View", _entity.Gene.FieldOfView, false, CoSh.MinFieldOfView, CoSh.MaxFieldOfView, o => _entity.Gene.FieldOfView = o);
        AddCheckboxToSection("Carnivore", _entity.Gene.Diet == EntityDiet.Carnivore, o => _entity.Gene.Diet = o ? EntityDiet.Carnivore : EntityDiet.Herbivore);
    }

    public void OpenMenu(EntityScript entity) {
        gameObject.SetActive(true);
        _entity = entity;
    }

    public void UpdateMenu(EntityScript entity) {
        _entity = entity;

        foreach (Transform t in Content.transform) {
            Destroy(t.gameObject);
        }

        Start();
    }

    public void AddSliderToSection(string sliderDescription, float defaultValue, bool useInt, float minValue, float maxValue, UnityAction<float> call) {
        GameObject slider = Instantiate(SliderPrefab, Content.transform);
        slider.GetComponentInChildren<TMP_Text>().text = sliderDescription;

        Slider s = slider.GetComponentInChildren<Slider>();
        s.minValue = minValue;
        s.maxValue = maxValue;
        s.wholeNumbers = useInt;
        s.onValueChanged.AddListener(call);
        s.onValueChanged.AddListener(s.GetComponent<SliderUpdateScript>().OnValueChanged);
        s.value = defaultValue;
    }

    public void AddCheckboxToSection(string checkboxDescription, bool defaultValue, UnityAction<bool> call) {
        GameObject checkbox = Instantiate(CheckboxPrefab, Content.transform);
        checkbox.GetComponentInChildren<TMP_Text>().text = checkboxDescription;

        Toggle t = checkbox.GetComponentInChildren<Toggle>();
        t.onValueChanged.AddListener(call);
        t.isOn = defaultValue;
    }
}
