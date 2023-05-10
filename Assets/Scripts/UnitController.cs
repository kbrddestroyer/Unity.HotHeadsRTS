using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NavMeshObstacle))]
[RequireComponent(typeof(Collider))]
public abstract class UnitController : MonoBehaviour
{
    // -----            SERIALIZED          ----- //
    [Header("Настройки базовой логики")]
    [SerializeField, Range(0f, 100f), Tooltip("Максимальное значение здоровья")]   
    protected float         maxHp;
    [SerializeField, Tooltip("Префаб ragdoll")]
    protected GameObject    ragdoll;
    [Header("Настройки UI")]
    [SerializeField] protected RawImage selectionIcon;
    [SerializeField] protected Slider hpBar;
    [Header("Звуковые эффекты")]
    [SerializeField] protected AudioClip selection;
    [SerializeField] protected AudioClip running;
    // -----            PROTECTED VARIABLES         ----- //
    protected POI currentPOI = null;                        // AI-only - current point of interest
    protected bool capturedAgentControl = false;
    protected float hp;                                     // Current HP
    protected bool selected = false;                        // True if was clicked and active
    protected bool isMouseOver = false;
    protected bool dead = false;
    protected NavMeshAgent agent;
    protected NavMeshObstacle obstacle;
    protected new Animator animation;                       // Animation Controller        
    protected Camera mainCamera;
    protected AudioSource audioSource;
    protected LayerMask groundMask;                         // Layer mask is used in raycast to detect the destination point for the NavMesh agent script
    protected CameraController camController;
    protected Canvas canvas;

    private Quaternion canvasInitialRotation;

    protected POI[] points;

    // -----            GET/SET         ----- //
    public bool isSelected { get { return selected; } }
    public float HP { get { return hp; } set { hp = value; } }

    protected void Start()
    {
        hp = maxHp;
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        agent.enabled = true;

        mainCamera = Camera.main;
        camController = mainCamera.gameObject.GetComponent<CameraController>();
        audioSource = GetComponent<AudioSource>();
        groundMask = LayerMask.GetMask("Ground");
        animation = GetComponentInChildren<Animator>();

        camController.AddUnit(this);
        canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = mainCamera;

        canvasInitialRotation = canvas.transform.rotation;

        points = GameObject.FindObjectsOfType<POI>();
    }

    protected void OnMouseEnter()
    {
        isMouseOver = true;
    }

    protected void OnMouseExit()
    {
        isMouseOver = false;
    }

    protected void OnMouseDown()
    {
        ToggleSelected(true);
        audioSource.PlayOneShot(selection);
    }

    public void Damage(float damage) { hp -= Mathf.Abs(damage); }

    protected void Die()
    {
        if (dead) return;
        dead = true;
        Destroy(GameObject.Instantiate(ragdoll, transform.position, transform.rotation), 60);
        camController.RemoveUnit(GetComponent<UnitController>());
        UnitController instance = GetComponent<UnitController>();
        foreach (POI poi in points)
        {
            if (poi.CurrentUnits.Contains(instance))
            {
                if (this.tag == "Team_A")
                    poi.weightA--;
                else if (this.tag == "Team_E")
                    poi.weightE--;
                poi.Remove(instance);
                break;
            }
        }

        Destroy(this.gameObject);
    }

    private IEnumerator switchAgent(bool status)
    {
        // Это костыль
        if (status)
        {
            obstacle.enabled = false;
            yield return new WaitForSeconds(0.1f);
            agent.enabled = true;
        }
        else
        {
            agent.enabled = false;
            obstacle.enabled = true;
        }
    }

    protected void SwitchAgent(bool status)
    {
        StartCoroutine(switchAgent(status));
    }

    public IEnumerator MoveTowards(Vector3 position)
    {
        while (!agent.enabled || obstacle.enabled) yield return new WaitForEndOfFrame();
        agent.SetDestination(position);
    }

    protected void Update()
    {
        canvas.transform.rotation = canvasInitialRotation;
        hpBar.value = hp / maxHp;
        animation.SetBool("moving", agent.hasPath);


        if (hp <= 0) Die();
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isMouseOver) ToggleSelected(false);

        if (!agent.hasPath && agent.enabled)
        {
            SwitchAgent(false);
        }

        if (this.gameObject.tag == "Team_A")
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) && isSelected)
            {
                SwitchAgent(true);

                if (camController.Hit)
                {
                    StartCoroutine(MoveTowards(camController.hitPoint));
                }
            }
        }
        else
        {
            if ((!agent.enabled || !agent.hasPath) && !capturedAgentControl)
            {
                
                POI point = null;

                foreach (POI _point in points)
                {
                    if (_point.Status > -1)
                        if (
                            point == null ||
                            (int)point.weightA - (int)point.weightE - (int)point.predictedEnemyWeight <
                            (int)_point.weightA - (int)_point.weightE - (int)_point.predictedEnemyWeight
                            )
                            point = _point;
                }
                if (point && currentPOI != point)
                {
                    if (currentPOI && currentPOI.predictedEnemyWeight > 0) currentPOI.predictedEnemyWeight--;
                    currentPOI = point; 
                }
                if (currentPOI)
                {
                    currentPOI.predictedEnemyWeight++;
                    SwitchAgent(true);
                    StartCoroutine(MoveTowards(currentPOI.transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5, 5))));
                }
                else
                {
                    SwitchAgent(false);
                }
            }
        }
    }
    protected void OnTriggerStay(Collider other)
    {
        if (other.tag == "Selector" && !isSelected)
        {
            ToggleSelected(true);
        }
    }

    public void ToggleSelected(bool selected_)
    {
        this.selected = selected_;
        selectionIcon.enabled = selected_;
    }
}
