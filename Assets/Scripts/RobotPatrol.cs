using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RobotPatrol : MonoBehaviour
{
    [Header("Attack Settings")]
    public string attackAnimationName = "Bounce Attack";
    public float detectionRange = 10f;
    public LayerMask obstacleMask;
    public Transform player;

    [Header("Patrolling")]
    public Transform[] waypoints;
    public float rotationSpeed = 5f;

    private int currentWaypoint = 0;
    private float switchDelay = 0.5f;
    private float waypointCooldown = 0f;

    private Animator animator;
    private NavMeshAgent agent;
    private bool isAttacking = false;
    private float encounterCooldown = 0f;
    [SerializeField] private StaminaBar playerStamina;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            Debug.LogError("Player not assigned and no GameObject with tag 'Player' found!");

        if (playerStamina == null)
        {
            playerStamina = Object.FindFirstObjectByType<StaminaBar>();
            if (playerStamina == null)
                Debug.LogError("StaminaBar not found in scene!");
        }

        GoToNextWaypoint();
    }

    void Update()
    {
        if (isAttacking) return;

        if (encounterCooldown > 0f)
        {
            encounterCooldown -= Time.deltaTime;
        }
        else
        {
            TryDetectAndShootPlayer();
        }

        Patrol();
    }

    void TryDetectAndShootPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f; // Slightly elevated ray origin
            Vector3 direction = (player.position - transform.position).normalized;

            // Draw ray for visual debugging in Scene view
            Debug.DrawRay(origin, direction * detectionRange, Color.red, 1f);

            //Debug.Log("Raycasting toward player. Distance: " + distanceToPlayer);

            // Fallback to simplified logic: skip obstacle check for debugging
            if (Physics.Raycast(origin, direction, out RaycastHit hit, detectionRange))
            {
                //Debug.Log("Raycast hit: " + hit.transform.name);

                if (hit.transform == player)
                {
                    Debug.Log("Player detected by robot!");

                    if (!isAttacking)
                    {
                        isAttacking = true;
                        animator.SetBool("IsAttacking", true);

                        // Drain stamina
                        if (playerStamina != null)
                            playerStamina.UseStamina(playerStamina.GetMaxStamina() * 0.5f);

                        // Damage player visually
                        ScrapPlayer playerScript = player.GetComponent<ScrapPlayer>();
                        if (playerScript != null)
                            playerScript.TakeDamage();

                        StartCoroutine(HandleAttack());
                    }
                }
                //else
                //{
                    //Debug.Log("Hit something else: " + hit.transform.name);
                //}
            }
            else
            {
                Debug.Log("Raycast did not hit anything");
            }
        }
    }


    IEnumerator HandleAttack()
    {
        float animDuration = 1.5f;

        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == attackAnimationName)
            {
                animDuration = clip.length;
                break;
            }
        }

        float timer = 0f;
        while (timer < animDuration)
        {
            timer += Time.deltaTime;

            // Rotate smoothly toward the player during attack
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0f;
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * 100f);
            }

            yield return null;
        }

        animator.SetBool("IsAttacking", false);
        isAttacking = false;
        encounterCooldown = 1f;
        agent.isStopped = false;
        GoToNextWaypoint();
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f && waypointCooldown <= 0f)
        {
            GoToNextWaypoint();
            waypointCooldown = switchDelay;
        }

        if (waypointCooldown > 0f)
            waypointCooldown -= Time.deltaTime;
    }

    void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        agent.destination = waypoints[currentWaypoint].position;
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    void OnDrawGizmos()
    {
        if (waypoints == null) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
                Gizmos.DrawSphere(waypoints[i].position, 0.2f);

            if (i < waypoints.Length - 1 && waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}

