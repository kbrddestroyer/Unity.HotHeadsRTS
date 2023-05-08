using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject explodion;
    [SerializeField, Range(0f, 1f)] private float explosionLifetime;
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(Instantiate(explodion, transform.position, Quaternion.identity), explosionLifetime);
        Destroy(this.gameObject);
    }
}
