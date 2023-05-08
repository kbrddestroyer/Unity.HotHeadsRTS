using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class shooting : UnitController
{
    [Header("Настройка локальной логики")]
    [SerializeField] protected GameObject muzzleflash;
    [SerializeField, Range(1f, 15f)] protected float seekDistance;
    [SerializeField, Range(1f, 100f)] protected float dealingDamage;
    [SerializeField, Range(0.1f, 5f)] protected float checkRangeSec;
    [Header("Настройки Gizmo")]
    [SerializeField] protected Color seekDistance_gizmoColor;

    public List<UnitController> enemies; // Enemies in range
    protected UnitController active;
    protected bool checkerRunning = false;
    public bool IsShooting { get { return checkerRunning; } }

    protected void OnDrawGizmos()
    {
        Gizmos.color = seekDistance_gizmoColor;
        Gizmos.DrawWireSphere(transform.position, seekDistance);
    }

    protected IEnumerator damageDiceChecker()
    {
        checkerRunning = true;
        animation.SetBool("shooting", true);
        if (!muzzleflash.activeInHierarchy) muzzleflash.SetActive(true);
        while (true)
        {
            if (enemies.Count == 0) break;

            UnitController controller = active.transform.GetComponent<UnitController>();
            if (controller)
            {
                if (Random.Range(0, 2) == 1)
                    controller.Damage(dealingDamage);
                if (controller.HP <= 0) enemies.Remove(controller);
            }
            yield return new WaitForSeconds(checkRangeSec);
        }
        checkerRunning = false;
        animation.SetBool("shooting", false);
        if (muzzleflash.activeInHierarchy) muzzleflash.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Collider[] hitting = Physics.OverlapSphere(transform.position, seekDistance);
        enemies.Clear();
        foreach (Collider collider in hitting)
        {
            if ((collider.tag == "Team_E" && this.gameObject.tag == "Team_A") ||
                (collider.tag == "Team_A" && this.gameObject.tag == "Team_E"))
            {
                UnitController controller = collider.GetComponent<UnitController>();
                if (!enemies.Contains(controller))
                {
                    enemies.Add(controller);
                }
            }
        }

        if (this.tag == "Team_E")
        {
            if (enemies.Count > 0)
            {
                capturedAgentControl = true;
                SwitchAgent(false);
            }
            else capturedAgentControl = false;
        }

        if (!agent.hasPath && enemies.Count > 0 && hp > 0)
        {
            active = enemies[0];
            foreach (UnitController controller in enemies)
            {
                if (controller.HP < active.HP)
                {
                    Ray ray = new Ray(transform.position + transform.up * 2f, (controller.transform.position + Vector3.up * 2f) - (transform.position + Vector3.up * 2f));
                    Debug.DrawRay(transform.position, ray.direction, Color.red, 10f);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 1000f))
                    {
                        Debug.Log("Hit: " + hit.transform.name);
                        Debug.Log("Expected: " + controller.transform.name);
                        if (hit.transform.name == controller.transform.name)
                            active = controller;
                    }
                }
            }

            transform.LookAt(active.transform.position);
            if (!checkerRunning) StartCoroutine(damageDiceChecker());
        }
        else if (animation.GetBool("shooting"))
        {
            animation.SetBool("shooting", false);
            if (muzzleflash.activeInHierarchy) muzzleflash.SetActive(false);
            active = null;
            StopCoroutine(damageDiceChecker());
            checkerRunning = false;
        }
    }
}
