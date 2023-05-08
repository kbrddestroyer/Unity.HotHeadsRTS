using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject rifler;
    [SerializeField] private GameObject shaheed;
    [SerializeField, Range(0f, 10f)] private float delay;
    private CameraController controller;
    private POI[] points;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindObjectOfType<CameraController>();
        points = FindObjectsOfType<POI>();
    }

    // Update is called once per frame
    private float time = 0f;
    void Update()
    {
        time += Time.deltaTime;
        if (time >= delay)
        {
            time = 0f;
            if (controller.Allies.Count > controller.Enemies.Count)
            {
                if (controller.Allies.Count - controller.Enemies.Count < 1)
                {
                    Instantiate(rifler, transform.position, Quaternion.identity);
                }
                else 
                    Instantiate(shaheed, transform.position, Quaternion.identity);
            }
            else if (controller.allies.Count == controller.enemies.Count)
            {
                bool allCaptured = true;
                foreach (POI poi in points)
                {
                    if (poi.Status > -1)
                    {
                        allCaptured = false;
                    }
                }
                if (!allCaptured)
                {
                    Instantiate(rifler, transform.position, Quaternion.identity);
                }
            }
        }
    }
}
