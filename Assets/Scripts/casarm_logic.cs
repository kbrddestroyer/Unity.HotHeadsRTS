using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class casarm_logic : MonoBehaviour
{
    [SerializeField] private GameObject medicPrefab;
    [SerializeField, Range(0f, 10f)] private float medicDelay;
    [SerializeField, Range(10f, 1000f)] private float medicOilCost;
    [SerializeField] private GameObject scoutPrefab;
    [SerializeField, Range(0f, 10f)] private float scoutDelay;
    [SerializeField, Range(10f, 1000f)] private float scoutOilCost;

    [SerializeField] private GameObject panel;
    [SerializeField] private Slider progress;
    [SerializeField] private Text scoutLabel;
    [SerializeField] private Text medicLabel;
    
    private bool selected = false;
    private bool mouseOver = false;
    private Builder player;

    private bool isCreatingUnit = false;

    private void OnMouseEnter()
    {
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        mouseOver = false; 
    }

    private void Awake()
    {
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
        player = FindObjectOfType<Builder>();

        scoutLabel.text = scoutOilCost.ToString();
        medicLabel.text = medicOilCost.ToString();
    }

    private void OnMouseDown()
    {
        selected = true;
        panel.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !mouseOver)
        {
            selected = false;
            panel.SetActive(false);
        }
    }

    private IEnumerator createUnit(GameObject prefab, float delay)
    {
        isCreatingUnit = true;
        for (float i = 0f; i <= delay; i += Time.deltaTime)
        {
            yield return new WaitForEndOfFrame();
            progress.value = i / delay;
        }
        Instantiate(prefab, transform.position, Quaternion.identity);
        isCreatingUnit = false;
    }

    public void CreateScout()
    {
        if (player.OilLevel >= scoutOilCost && !isCreatingUnit)
        {
            player.OilLevel -= scoutOilCost;
            StartCoroutine(createUnit(scoutPrefab, scoutDelay));
        }
    }

    public void CreateMedic()
    {
        if (player.OilLevel >= medicOilCost && !isCreatingUnit)
        {
            player.OilLevel -= medicOilCost;
            StartCoroutine(createUnit(medicPrefab, medicDelay));
        }
    }
}
