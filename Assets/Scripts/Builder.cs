using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.EventSystems;
using TMPro;

public class Builder : MonoBehaviour
{
    [SerializeField] private GameObject casarmPreview;
    [SerializeField] private GameObject airstrikePreview;
    [SerializeField, Range(100f, 1000f)] public float initOilLevel;
    [SerializeField] private TMP_Text oilLevelText;
    [SerializeField] private string mainMenu;
    private GameObject current = null;
    private float oilLevel;
    public float winPoints = 0f;

    [SerializeField] private GameObject escapeSection;
    [SerializeField] private GameObject endgameSection;
    [SerializeField] private TMP_Text endgameText;
    [SerializeField] private TMP_Text airstrikeText;
    [SerializeField] private TMP_Text casarmText;
    [SerializeField] private Slider progress;

    public float WinPoints
    {
        get { return winPoints; }     
        set 
        { 
            winPoints = value;
            progress.value = winPoints / 100f + 0.5f;

            if (winPoints >= 50f)
            {
                endgameSection.SetActive(true);
            }
            else if (winPoints <= -50f)
            {
                endgameText.color = Color.red;
                endgameText.text = "Поражение";

                endgameSection.SetActive(true);
            }
        }
    }

    public float OilLevel { 
        get { return oilLevel; } 
        set
        {
            oilLevel = value;

            oilLevelText.text = ((int)oilLevel).ToString();
        }
    }

    private void Start()
    {
        OilLevel = initOilLevel;

        airstrikeText.text = airstrikePreview.GetComponent<AirstrikeCaller>().airstrikeCost.ToString();
        casarmText.text = casarmPreview.GetComponent<Buildable>().buildCost.ToString();
    }

    public void Continue()
    {
        escapeSection.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }

    public void BuildCasarm()
    {
        if (oilLevel < 100 || current != null) return;
        current = Instantiate(casarmPreview, Vector3.zero, casarmPreview.transform.rotation);
    }

    public void AirstrikeCall()
    {
        if (oilLevel >= airstrikePreview.GetComponent<AirstrikeCaller>().airstrikeCost)
        {
            Instantiate(airstrikePreview, Vector3.zero, Quaternion.identity);
        }
    }

    private float sec = 0;
    private void Update()
    {
        sec += Time.deltaTime;

        if (sec >= 2f)
        {
            sec = 0;
            PumpOil(1f);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escapeSection.SetActive(!escapeSection.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.T))
            BuildCasarm();
        if (Input.GetKeyDown(KeyCode.E))
            AirstrikeCall();
    }

    public void PumpOil(float delta)
    {
        OilLevel += 1f * delta;
    }
}
