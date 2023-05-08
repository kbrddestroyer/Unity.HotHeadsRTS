using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] private float cameraSpeed;
    [SerializeField, Range(0f, 1f)] private float cameraSmoothness;
    [SerializeField, Range(0f, 100f)] private float cameraDistanseSpeed;
    [SerializeField, Range(0f, 10f)] private float cameraDistanseSmoothvalue;
    [SerializeField] private GameObject explodion;

    private Vector3 clickPoint = Vector3.zero;
    private bool hitSuccess = false;
    private Camera mainCamera;
    private AudioSource audioSource;
    public List<UnitController> allies, enemies;
    public Vector3 hitPoint { get { return clickPoint; } }
    public bool Hit { get { return hitSuccess; } }

    public List<UnitController> Allies { get { return allies; } }
    public List<UnitController> Enemies { get { return enemies; } }

    public void AddUnit(UnitController reference)
    {
        if (reference.tag == "Team_A")
            allies.Add(reference);
        else if (reference.tag == "Team_E")
            enemies.Add(reference);
        else
            Debug.LogError("Тег UnitController " + reference.name + " назначен неверно!");
    }

    public void RemoveUnit(UnitController reference)
    {
        if (reference.tag == "Team_A")
            allies.Remove(reference);
        else if (reference.tag == "Team_E")
            enemies.Remove(reference);
        else
            Debug.LogError("Тег UnitController " + reference.name + " назначен неверно!");
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        mainCamera = GetComponent<Camera>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Движение камеры по WASD и реакция на действия мыши

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            audioSource.Play();
        }

        float axisX = Input.GetAxis("Horizontal");
        float axisY = Input.GetAxis("Vertical");
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            LayerMask mask = LayerMask.GetMask("Ground");
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (hitSuccess = Physics.Raycast(ray, out hit, 1000f, mask))
                clickPoint = hit.point;
            else clickPoint = Vector3.zero;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            LayerMask mask = LayerMask.GetMask("Ground");
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, mask))
                Instantiate(explodion, hit.point, Quaternion.identity);
        }

        Vector3 newPosition = Vector3.Lerp(
                transform.position, 
                transform.position + new Vector3(axisX + axisY, 0, axisY - axisX) * transform.position.y * cameraSpeed,
                cameraSmoothness * Time.deltaTime
            );

        if (
            (mouseWheel > 0 && transform.position.y > 1) ||
            (mouseWheel < 0 && transform.position.y < 10)
            )
        {
            newPosition = Vector3.Lerp(
                transform.position,
                transform.position + transform.forward * mouseWheel * cameraDistanseSpeed,
                cameraDistanseSmoothvalue * Time.deltaTime
                );
        }

        transform.position = new Vector3(newPosition.x, Mathf.Clamp(newPosition.y, 1, 10), newPosition.z);
    }

}
