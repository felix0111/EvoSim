using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDMenu : MonoBehaviour {

    public TMP_Text BestEntityText, GeneralInfoText;

    void Start() {
        StartCoroutine(UpdateTextsRoutine());
    }

    private int _fps = 0;
    void Update() {
        _fps = Mathf.CeilToInt(1f / Time.unscaledDeltaTime);
    }

    IEnumerator UpdateTextsRoutine() {
        while (true) {
            yield return new WaitForSeconds(2f);

            BestEntityText.text = "Best Entity: " + SimulationScript.Instance.BestEntity.Item1;
            BestEntityText.text += "<br>" + (SimulationScript.Instance.CoSh.AdaptionPhase ? "In Adaption-Phase" : "In Expanding-Phase");

            GeneralInfoText.text = "FPS: " + _fps;
            GeneralInfoText.text += "<br>Entity Amount: " + SimulationScript.Instance.EntityPool.ActiveEntities.Count + "/" + 400;
            GeneralInfoText.text += "<br>Food Amount: " + SimulationScript.Instance.FoodPool.ActiveFoods.Count + "/" + 2500;
            GeneralInfoText.text += "<br>Pheromone Amount: " + SimulationScript.Instance.PheromonePool.ActivePheromones.Count + "/" + 1500;

        }
    }

    public void OnTimeSliderChanged(Slider slider) {
        Time.timeScale = slider.value;
    }

    public void OnSpeciesMenuButton() {
        if (SimulationScript.Instance.MenuManager.SpeciesMenu.isActiveAndEnabled) {
            SimulationScript.Instance.MenuManager.SpeciesMenu.gameObject.SetActive(false);
        } else {
            SimulationScript.Instance.MenuManager.SpeciesMenu.OpenMenu();
        }
    }

    public void OnSettingsMenuButton() {
        if (SimulationScript.Instance.MenuManager.SettingsMenu.isActiveAndEnabled) {
            SimulationScript.Instance.MenuManager.SettingsMenu.gameObject.SetActive(false);
        } else {
            SimulationScript.Instance.MenuManager.SettingsMenu.OpenMenu();
        }
    }

    public void OnFullscreenButton(TMP_Text buttonText) {
        Screen.fullScreenMode = Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;
        buttonText.text = Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? "Windowed" : "Fullscreen";
    }
    
}
