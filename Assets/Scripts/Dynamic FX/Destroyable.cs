using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] private float hp;
    [SerializeField, Range(0f, 10f)] private float lifetime;
    public void Damage(float damage)
    {
        hp -= Mathf.Abs(damage);

        if (hp <= 0f)
        {
            if (!this.GetComponent<Rigidbody>())
            {
                this.AddComponent<Rigidbody>().mass = 10f;
                Destroy(this.gameObject, lifetime);
            }
        }
    }
}
