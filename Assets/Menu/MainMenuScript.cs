using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {

    void Awake() {
        LoadEntityAssets();
    }

    public void StartSimulationButton() {

        //settings
        PlayerPrefs.SetInt("EntityCount", (int)transform.Find("Main").Find("EntityCount").GetComponentInChildren<Slider>().value);
        PlayerPrefs.SetInt("MutationCount", (int)transform.Find("Main").Find("MutationCount").GetComponentInChildren<Slider>().value);
        PlayerPrefs.SetString("DefaultEntity", transform.Find("Main").Find("DefaultEntity").GetComponentInChildren<TMP_Dropdown>().captionText.text);

        SceneManager.LoadScene("MainScene");
    }

    public void LoadEntityAssets() {
        var info = new DirectoryInfo(Application.streamingAssetsPath);
        transform.Find("Main").Find("DefaultEntity").GetComponentInChildren<TMP_Dropdown>().AddOptions(info.GetFiles("*.brain").Select(o => o.Name).ToList());
    }

}
