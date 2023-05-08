using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class POI : MonoBehaviour
{
    /*
     *  POI - Point of interest
     *  BASE CLASS:     MonoBehaviour
     *  TYPE:           Static AI logic controller
     *  LAST MODIFIED:  06.05.23 20:00
     *  
     *  Algorithm controls simple AI model based on units summary weight
     *  AI pushes it forces to the POI with highest difference between allies end enemies weights
     *  Also AI's strategy is being based on capture status
     *  100% captured points are no longer holded by forces, but being reinforced while under attack
     *  
     *  FORMULA:
     *  [look UnitController.cs in AI-connected functions]
     *  
     *  POI_Interest = (capture + 1) * (aiWeightA - (aiWeightE + aiEPredictedWeight))
     *  ->
     *      POI_Interest < 0 - Forces being moved to another points
     *      POI_Interest = 0 - Forces stay if there's no more interesting point
     *      POI_Interest > 0 - Point's being reinforced with more units
     *  
     *  Capture value is being clamped in range [-1, 1]
     *  _______________________________________________________________________________________________________________
     *                  TODO:
     *             1. Optimisation      [DONE]
     *  _______________________________________________________________________________________________________________
     */
    // -----              SERIALIZABLE            ----- //
    [Header("Зависимости UI")]                          //
    [SerializeField] private Slider slider;             // Control progress bar
    // -----                PRIVATE               ----- //
    //                                                  //
    [SerializeField] private uint    aiWeightA = 0;     // Allies summary
    [SerializeField] private uint    aiWeightE = 0;     // Enemies summary
    private uint    aiEPredictedWeight = 0;             // Enemies on their way to reinforce point
    private float   capture = 0;                        // Can be in range [-1, 1] where -1 is captured by enemy and 1 is captured by player;
    private Builder player;                             // Player handle to collect resource
    //                                                  //
    // -----                GET/SET               ----- //
    //                                                  //

    public List<UnitController> currentUnits;
    public List<UnitController> CurrentUnits { get { return currentUnits; } }

    public uint predictedEnemyWeight { get { return aiEPredictedWeight; } set { aiEPredictedWeight = value; } }
    public uint weightA { get { return aiWeightA; } set { aiWeightA = value; } }
    public uint weightE { get { return aiWeightE; } set { aiWeightE = value; } }
    public float Status { get { return capture; } }

    public void Remove(UnitController reference)
    {
        currentUnits.Remove(reference);
    }

    private void OnTriggerEnter(Collider other)
    {
        UnitController controller = other.gameObject.GetComponent<UnitController>();
        if (controller)
        {
            currentUnits.Add(controller);

            if (other.tag == "Team_A")
                aiWeightA++;
            else if (other.tag == "Team_E")
            {
                aiWeightE++;
                if (predictedEnemyWeight > 0) predictedEnemyWeight--;       // If there was one unit on the way and it had reached the trigger we need to update AI values properly
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // -- If someone left the trigger (e.g. moved to another POI) - update AI values -- //
        UnitController controller = other.gameObject.GetComponent<UnitController>();
        if (controller)
        {
            currentUnits.Remove(controller);

            if (other.tag == "Team_A" && weightA > 0) aiWeightA--;
            else if (other.tag == "Team_E" && aiWeightE > 0) aiWeightE--;
        }
    }

    private void Start()
    {
        player = GameObject.FindObjectOfType<Builder>();
    }

    private void Update()
    {
        capture += 0.1f * ((aiWeightA > aiWeightE) ? 1f : ((aiWeightA == aiWeightE) ? 0f : -1f)) * Time.deltaTime;
        capture = Mathf.Clamp(capture, -1f, 1f);
        slider.value = 0.5f * capture + 0.5f;

        if (capture == 1)
            player.PumpOil(Time.deltaTime);
        player.WinPoints += capture * Time.deltaTime * 0.25f;
    }
}
