using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class a_democracykeeper : MonoBehaviour
{
    [SerializeField] private GameObject bomb;
    [SerializeField, Range(0f, 10f)] private float bombingRadius;
    [SerializeField, Range(0, 10)] private int bombs;
    [SerializeField, Range(0f, 5f)] private float bombingDelay;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, bombingRadius);
    }

    private void Start()
    {
        Destroy(this.gameObject, GetComponent<AudioSource>().clip.length);
    }

    float secPassed = 0f;
    // Update is called once per frame
    void Update()
    {
        secPassed += Time.deltaTime;
        if (secPassed > bombingDelay && bombs > 0)
        {
            Instantiate(bomb, transform.position + new Vector3(Random.Range(-bombingRadius, bombingRadius), 0, Random.Range(-bombingRadius, bombingRadius)), Quaternion.identity);
            secPassed = 0;
            bombs--;
        }
    }
}
