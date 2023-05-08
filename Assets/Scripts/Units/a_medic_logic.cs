using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class a_medic_logic : shooting
{
    private new void Update()
    {
        base.Update();
    
        foreach (UnitController controller in camController.Allies)
        {
            if (Vector3.Distance(transform.position, controller.transform.position) < seekDistance)
            {
                controller.HP += Time.deltaTime * 0.2f;
            }
        }
    }
}
