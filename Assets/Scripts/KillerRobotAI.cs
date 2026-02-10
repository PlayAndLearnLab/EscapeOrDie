using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class KillerRobotAI : MonoBehaviour
{
    public Transform[] waypoints;
    public float sightRange = 20f;
    public float attackRange = 5f;
    public int maxAttacksBeforeCapture = 3;
    public Animator animator;

    [SerializeField] private float attackDuration = 1f; // Time before capture

    private NavMeshAgent agent;
    private Transform player;
    private int currentWaypointIndex = 0;
    private int attackCount = 0;
    private bool playerDetected = false;
    private bool isAttacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (waypoints.Length > 0)
        {
            GoToNextWaypoint();
        }
    }

    void Update()
    {
        if (playerDetected)
        {
            PursuePlayer();
        }
        else
        {
            Patrol();
            DetectPlayer();
        }
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextWaypoint();
        }

        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f);
    }

    void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        agent.destination = waypoints[currentWaypointIndex].position;
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    void DetectPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Simple sight range check
        if (distance <= sightRange)
        {
            playerDetected = true;
        }
    }

    void PursuePlayer()
    {
        if (isAttacking)
        {
            // Ensure walking animation stops while attacking
            animator.SetBool("isWalking", false);
            return;
        }

        agent.isStopped = false; // Make sure it's moving if not attacking
        agent.SetDestination(player.position);
        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f);

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange && !isAttacking)
        {
            StartCoroutine(AttackPlayer());
        }
    }

    System.Collections.IEnumerator AttackPlayer()
    {
        isAttacking = true;

        agent.ResetPath();       // Clear current path so it stops immediately
        agent.velocity = Vector3.zero; // Hard stop
        agent.isStopped = true;

        animator.SetBool("isWalking", false); // Stop walk animation
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDuration);

        DamageOverlay damageOverlay = FindObjectOfType<DamageOverlay>();
        if (damageOverlay != null)
        {
            damageOverlay.ShowDamageEffect();
        }

        attackCount++;
        if (attackCount >= maxAttacksBeforeCapture)
        {
            CapturePlayer();
        }

        yield return new WaitForSeconds(1f); // Cooldown
        isAttacking = false;
    }

    void CapturePlayer()
    {
        Debug.Log("Player captured!");

        SceneFader fader = FindObjectOfType<SceneFader>();
        if (fader != null)
        {
            fader.FadeToScene("GameOver"); // Your scene name
        }
        else
        {
            // Fallback if fader not found
            SceneManager.LoadScene("GameOver");
        }
    }
}

