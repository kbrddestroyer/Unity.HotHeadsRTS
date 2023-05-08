using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fracture : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float maxHp;
    [SerializeField, Range(1f, 120f)] private float fractureLifetime;
    [SerializeField, Range(1f, 10f)] private float randomAspect;
    private float hp;

    private void Start()
    {
        hp = maxHp;
    }

    public void Damage(float damage)
    {
        hp -= Mathf.Abs(damage);
        if (hp <= 0f)
        {
            Canvas[] canvases = GetComponentsInChildren<Canvas>();
            foreach (Canvas canvas in canvases)
                Destroy(canvas.gameObject);

            Destroy(transform.GetChild(0).gameObject);
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                child.AddComponent<Rigidbody>().mass = 10f;
                child.AddComponent<NavMeshObstacle>();
                Destroy(child, fractureLifetime + Random.Range(-randomAspect, randomAspect));
            }
            transform.DetachChildren();
            Destroy(this.gameObject);
        }
    }
}
