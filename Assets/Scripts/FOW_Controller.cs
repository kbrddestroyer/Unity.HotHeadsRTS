using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOW_Controller : MonoBehaviour
{
    private Renderer[] renderers;
    private Canvas[] canvases;
    private shooting controller;

    [SerializeField] private bool fowEnabled = false;
    [SerializeField] private int visibleBy = 0;

    public int Visible
    {
        get { return visibleBy; }
        set {
            if (value < 0) visibleBy = 0;
            Enabled = visibleBy > 0;
        }
    }

    public bool Enabled { 
        get { return fowEnabled; } 
        set 
        { 
            fowEnabled = value;
            Debug.Log(name + value);
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = value;
            }
            foreach (Canvas canvas in canvases)
            {
                canvas.enabled = value;
            }
        } 
    }

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        canvases = GetComponentsInChildren<Canvas>();
        controller = GetComponent<shooting>();

    }

    private void FixedUpdate()
    {
        if (controller)
        {
            Enabled = controller.IsShooting;
        }
        else Enabled = false;
    }
}
