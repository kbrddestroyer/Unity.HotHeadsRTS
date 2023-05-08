using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirstrikeCaller : MonoBehaviour
{
    [SerializeField] private GameObject airstrike;
    [SerializeField, Range(10f, 100f)] public float airstrikeCost;
    private Builder player;

    private void Start()
    {
        player = GameObject.FindObjectOfType<Builder>();
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Ground")))
        {
            transform.position = hit.point;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Instantiate(airstrike, transform.position + airstrike.transform.position, Quaternion.identity);
            player.OilLevel -= airstrikeCost;
            Destroy(this.gameObject);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Destroy(this.gameObject);
        }
    }
}
