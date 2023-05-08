using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildable : MonoBehaviour
{
    [SerializeField] private GameObject resultingPrefab;
    [SerializeField] private Material okMaterial;
    [SerializeField] private Material failMaterial;
    [SerializeField, Range(10f, 100f)] public float buildCost;

    private Builder player;
    private MeshRenderer mesh;
    private int ok = 0;
    private void Start()
    {
        player = GameObject.FindObjectOfType<Builder>();
        mesh = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Ground")
        {
            ok++;
            Debug.Log(other.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Ground" && ok > 0)
            ok--;
    }

    private void Update()
    {
        Material[] materials = new Material[mesh.materials.Length];
        for (int i = 0; i < mesh.materials.Length; i++)
        {
            materials[i] = (ok == 0) ? okMaterial : failMaterial;
        }
        mesh.materials = materials;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Ground")))
        {
            transform.position = hit.point + resultingPrefab.transform.position;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && ok == 0)
        {
            Instantiate(resultingPrefab, transform.position - resultingPrefab.transform.position, Quaternion.identity);
            player.OilLevel -= buildCost;
            Destroy(this.gameObject);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Destroy(this.gameObject);
        }
    }
}

