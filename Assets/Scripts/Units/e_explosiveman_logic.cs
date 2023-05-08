using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class e_explosiveman_logic : UnitController
{
    [Header("Настройки локальной логики")]
    [SerializeField, Range(0f, 15f), Tooltip("Задержка перед взрывом")] private float explosionDelay;
    [SerializeField, Range(0f, 1f), Tooltip("Время жизни динамического эффекта")] private float explosionLifetime;
    [SerializeField, Range(0f, 10f), Tooltip("Радиус, в котором юнит видит врага")] private float explosionTriggerRange;
    [SerializeField] protected Slider progress;
    [SerializeField] private GameObject explosion;
    [Header("Настройки Gizmo")]
    [SerializeField] private Color triggerRangeColor;
    private GameObject minimal = null;

    private bool exploding = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = triggerRangeColor;
        Gizmos.DrawWireSphere(transform.position, explosionTriggerRange);
    }

    private IEnumerator Explode()
    {
        for (float i = 0; i < explosionDelay; i += 0.01f)
        {
            progress.value += 1.0f / (explosionDelay * 100);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(GameObject.Instantiate(explosion, transform.position, transform.rotation), explosionLifetime);
        Die();
    }

    new void Update()
    {
        base.Update();

        if (!exploding)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Team_A");
            if (enemies.Length != 0)
            {
                minimal = enemies[0];
                foreach (GameObject enemy in enemies)
                {
                    if (Vector3.Distance(transform.position, enemy.transform.position) <
                        Vector3.Distance(transform.position, minimal.transform.position))
                    {
                        minimal = enemy;
                    }

                }


                if (Vector3.Distance(transform.position, minimal.transform.position) < explosionTriggerRange)
                {
                    exploding = true;
                    audioSource.Play();
                    StartCoroutine(Explode());
                }
            }
        }
        else
        {
            if (!agent.enabled)
            {
                SwitchAgent(true);
            }
            StartCoroutine(MoveTowards(minimal.transform.position));
        }
    }
}
