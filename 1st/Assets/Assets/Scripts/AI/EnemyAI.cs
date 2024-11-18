using UnityEngine;
using TMPro;

public class EnemyAI : BaseAI
{
    [Header("Patrol Settings")]
    public Transform patrolPath; 
    [SerializeField] private float waypointTolerance = 0.5f; 

    private Transform[] waypoints; 
    private int currentWaypointIndex = 0;
    private bool isPatrolling = true; 
    private bool returningToPatrol = false; 

    [Header("Game Over Settings")]
    [SerializeField] private TMP_Text escapeCounterText; 
    [SerializeField] private int maxAllowedEscapes = 5; 
    private static int escapedEnemiesCount = 0; 

    public GameOverPanelMng gameOverManager; 

    protected override void Start()
    {
        base.Start();

        if (patrolPath != null)
        {
            waypoints = new Transform[patrolPath.childCount];
            for (int i = 0; i < patrolPath.childCount; i++)
            {
                waypoints[i] = patrolPath.GetChild(i);
            }
        }

        UpdateEscapeCounterUI();
    }

    protected override void Update()
    {
        base.Update();
        if (target == null)
        {
            if (returningToPatrol)
            {
                ReturnToPatrolPath();
            }
            else if (isPatrolling && waypoints != null && waypoints.Length > 0)
            {
                Patrol();
            }
        }
        CheckGameOver();
    }

    private void Patrol()
    {
        Transform currentWaypoint = waypoints[currentWaypointIndex];
        float distanceToWaypoint = Vector3.Distance(transform.position, currentWaypoint.position);

        if (distanceToWaypoint <= waypointTolerance)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                HandlePatrolEnd();
                return;
            }
        }

        MoveTowardsTarget(currentWaypoint);
        animator.SetFloat(verticalParam, 1f); 
    }

    private void ReturnToPatrolPath()
    {
        Transform closestWaypoint = FindClosestWaypoint();
        float distanceToWaypoint = Vector3.Distance(transform.position, closestWaypoint.position);

        if (distanceToWaypoint <= waypointTolerance)
        {
            returningToPatrol = false; 
            isPatrolling = true;      
        }
        else
        {
            MoveTowardsTarget(closestWaypoint); 
        }
    }

    private Transform FindClosestWaypoint()
    {
        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach (Transform waypoint in waypoints)
        {
            float distance = Vector3.Distance(transform.position, waypoint.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = waypoint;
            }
        }

        return closest;
    }

    private void HandlePatrolEnd()
    {
        escapedEnemiesCount++; 
        UpdateEscapeCounterUI();

        if (escapedEnemiesCount >= maxAllowedEscapes)
        {
            gameOverManager.isGameOver = true;
        }

        Destroy(gameObject); 
    }

    private void UpdateEscapeCounterUI()
    {
        if (escapeCounterText != null)
        {
            if(escapedEnemiesCount >= 5) { escapedEnemiesCount = 5; }
            escapeCounterText.text = $"{escapedEnemiesCount}/{maxAllowedEscapes}";
        }
    }

    private void CheckGameOver()
    {
        if (escapedEnemiesCount >= maxAllowedEscapes)
        {
            escapeCounterText.gameObject.SetActive(false);
            gameOverManager.isGameOver = true;
        }
    }

    public override void MoveTowardsTarget(Transform target)
    {
        float step = moveSpeed * Time.deltaTime;
        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        Quaternion currentRotation = transform.rotation;

        transform.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, lookRotation.eulerAngles.y, currentRotation.eulerAngles.z);
    }

    protected override bool IsValidTarget(Collider hitCollider)
    {
        return hitCollider.CompareTag("Player") || hitCollider.CompareTag("Soldier");
    }

    protected override void HandleAttack()
    {
        base.HandleAttack();
        returningToPatrol = true; 
    }
}
