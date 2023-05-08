using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    [SerializeField, Range(0f, 40f)] private float delta;
    [SerializeField] private RectTransform selection;
    [SerializeField] private GameObject selector;

    private Camera mainCamera;
    private bool isSelection = false;
    private Vector2 startPos, endPos;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            startPos = mousePos;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (
                Vector2.Distance(mousePos, startPos) > delta
                )
            {
                isSelection = true;
                UpdateSelectionBox(mousePos);
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0) && isSelection)
        {
            ReleaseSelectionBox();
            isSelection = false;
        }
    }

    private void UpdateSelectionBox(Vector2 curMousePos)
    {
        if (!selection.gameObject.activeInHierarchy)
            selection.gameObject.SetActive(true);
        float width = curMousePos.x - startPos.x;
        float height = curMousePos.y - startPos.y;
        selection.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selection.position = startPos + new Vector2(width / 2, height / 2);
    }

    void ReleaseSelectionBox()
    {
        selection.gameObject.SetActive(false);
        Vector2 min = selection.anchoredPosition - (selection.sizeDelta / 2);
        Vector2 max = selection.anchoredPosition + (selection.sizeDelta / 2);
        GameObject selector_ = Instantiate(selector, startPos, Quaternion.Euler(0, 45, 0));

        Vector3[] worldSelection = GenerateBoxCoords(min, max);

        selector_.transform.position =
            (
                ((worldSelection[0] + worldSelection[1]) / 2.0f) +
                ((worldSelection[2] + worldSelection[3]) / 2.0f)
            ) / 2.0f;
        selector_.transform.localScale = new Vector3(
            Vector3.Distance(worldSelection[0], worldSelection[1]),
            1,
            Vector3.Distance(worldSelection[0], worldSelection[2])
            );

        Destroy(selector_, 0.05f);
    }

    Vector3[] GenerateBoxCoords(Vector2 a, Vector2 b)
    {
        Vector3[] worldSelection = new Vector3[4];
        int size = 0;
        Vector2 bottomLeft = new Vector2((a.x < b.x) ? a.x : b.x, (a.y < b.y) ? a.y : b.y);
        Vector2 topLeft = new Vector2((a.x < b.x) ? a.x : b.x, (a.y > b.y) ? a.y : b.y);
        Vector2 topRight = new Vector2((a.x > b.x) ? a.x : b.x, (a.y > b.y) ? a.y : b.y);
        Vector2 bottomRight = new Vector2((a.x > b.x) ? a.x : b.x, (a.y < b.y) ? a.y : b.y);

        Ray ray;
        RaycastHit hit;

        LayerMask mask = LayerMask.GetMask("Ground");
        ray = mainCamera.ScreenPointToRay(bottomLeft);
        if (Physics.Raycast(ray, out hit, 1000f, mask))
        {
            worldSelection[size++] = hit.point;
        }
        Debug.DrawRay(mainCamera.transform.position, ray.direction * 10f, Color.green, 10f);
        ray = mainCamera.ScreenPointToRay(bottomRight);
        if (Physics.Raycast(ray, out hit, 1000f, mask))
        {
            worldSelection[size++] = hit.point;
        }
        Debug.DrawRay(mainCamera.transform.position, ray.direction * 10f, Color.red, 10f);
        ray = mainCamera.ScreenPointToRay(topLeft);
        if (Physics.Raycast(ray, out hit, 1000f, mask))
        {
            worldSelection[size++] = hit.point;
        }
        Debug.DrawRay(mainCamera.transform.position, ray.direction * 10f, Color.blue, 10f);
        ray = mainCamera.ScreenPointToRay(topRight);
        if (Physics.Raycast(ray, out hit, 1000f, mask))
        {
            worldSelection[size++] = hit.point;
        }
        Debug.DrawRay(mainCamera.transform.position, ray.direction * 10f, Color.white, 10f);

        return worldSelection;
    }
}
