using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FOW : MonoBehaviour
{
    [Header("Настройка тумана войны")]
    [SerializeField, Range(0f, 100f)] private float range;
    [Header("Настройка Gizmo")]
    [SerializeField] private Color gizmoColor;

    [SerializeField] private bool debug;
    private CameraController camController;

    private FOW[] other;

    private void Start()
    {
        camController = Camera.main.GetComponent<CameraController>();
        other = FindObjectsOfType<FOW>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UnitController[] enemies = camController.Enemies.ToArray();
        LayerMask mask = LayerMask.GetMask("FOW");
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, mask);

        foreach (Collider collider in colliders)
        {
            FOW_Controller controller = collider.gameObject.GetComponent<FOW_Controller>();
            controller.Enabled = true;
            if (debug)
            {
                Debug.Log("Enabling: " + collider.name + " from: " + this.gameObject.name);
            }
        }
    }

}
