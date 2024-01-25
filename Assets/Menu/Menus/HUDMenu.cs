using System.Collections;
using System.Collections.Generic;
using EasyNNFramework.NEAT;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDMenu : MonoBehaviour {

    public TMP_Text BestEntityText;

    void Update() {
        BestEntityText.text = "Best Entity: " + SimulationScript.Instance.BestEntity.Item1;
        BestEntityText.text += "<br>" + (SimulationScript.Instance.CoSh.AdaptionPhase ? "In Adaption-Phase" : "In Expanding-Phase");
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
