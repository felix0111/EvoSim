using System.Collections;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public EntityMenu EntityMenu;
    public HUDMenu HUDMenu;
    public SpeciesMenu SpeciesMenu;
    public SettingsMenu SettingsMenu;
    public FoodSpawnerMenu FoodSpawnerMenu;

    public int FocusedEntityIndex {
        get => _focusedEntityIndex;
        set {
            if (SimulationScript.Instance.EntityPool.ActiveEntities.Count == 0) {
                EntityMenu.gameObject.SetActive(false);
                _focusedEntityIndex = -1;
                return;
            }

            _focusedEntityIndex = Mathf.Clamp(value, 0, SimulationScript.Instance.EntityPool.ActiveEntities.Count - 1);
            var e = SimulationScript.Instance.EntityPool.ActiveEntities[_focusedEntityIndex];
            if (EntityMenu.isActiveAndEnabled) {
                EntityMenu.UpdateEntity(e);
            } else {
                EntityMenu.OpenMenu(e);
            }
        }
    }
    private int _focusedEntityIndex;

    private int _loggingStep = 0;

    void Start() {
        CloseMenus();

        StartCoroutine(UpdateSpeciesMenu());
    }

    void Update() {
        HandleInputs();
    }

    private void HandleInputs() {
        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (EntityMenu.isActiveAndEnabled) {
                if (EntityMenu.NNVisualizerMenu.isActiveAndEnabled) {
                    if (EntityMenu.NNVisualizerMenu.IsDragging) {
                        EntityMenu.NNVisualizerMenu.InterruptDragging();
                    } else {
                        EntityMenu.NNVisualizerMenu.gameObject.SetActive(false);
                    }
                } else {
                    EntityMenu.gameObject.SetActive(false);
                }
            } else if (SpeciesMenu.isActiveAndEnabled) {
                SpeciesMenu.gameObject.SetActive(false);
            } else if (SettingsMenu.isActiveAndEnabled) {
                SettingsMenu.gameObject.SetActive(false);
            } else if (FoodSpawnerMenu.isActiveAndEnabled) {
                FoodSpawnerMenu.gameObject.SetActive(false);
            }
        }
        
        if (Input.GetKeyUp(KeyCode.LeftArrow)) {
            FocusedEntityIndex--;
        } else if (Input.GetKeyUp(KeyCode.RightArrow)) {
            FocusedEntityIndex++;
        }
    }

    public void CloseMenus() {
        EntityMenu.NNVisualizerMenu.gameObject.SetActive(false);
        EntityMenu.gameObject.SetActive(false);
        SpeciesMenu.gameObject.SetActive(false);
        SettingsMenu.gameObject.SetActive(false);
        FoodSpawnerMenu.gameObject.SetActive(false);
    }

    IEnumerator UpdateSpeciesMenu() {
        yield return new WaitForSeconds(2f);

        SpeciesMenu.LogSpecies(_loggingStep);
        _loggingStep++;
        SpeciesMenu.LogSpecies(_loggingStep);
        _loggingStep++;

        while (true) {
            yield return new WaitForSeconds(SimulationScript.Instance.CoSh.SpeciesLoggingRate);

            SpeciesMenu.LogSpecies(_loggingStep);
            if(SpeciesMenu.isActiveAndEnabled) SpeciesMenu.UpdateMenu();
            _loggingStep++;
            
        }
    }

}
