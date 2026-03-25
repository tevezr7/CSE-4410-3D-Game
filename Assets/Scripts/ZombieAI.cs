using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Transform specimen;
    public LayerMask whatIsPlayer;
    private Animator anim;

    public float aggroRange = 8f;
    public float attackRange = 2f;
    public float damage = 10f;
    public float timeBetweenAttacks = 1.5f;
    private bool alreadyAttacked;

    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float closeRangeSpeed = 4.5f;
    public float speedModifier = 1f;

    private bool isAggroed = false;
    public Transform currentTarget;
    public static List<ZombieAI> allZombies = new List<ZombieAI>();
    private void OnEnable() { allZombies.Add(this); }
    private void OnDisable() { allZombies.Remove(this); }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (player == null)
        {
            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
        if (specimen == null)
        {
            var specimenObj = GameObject.FindWithTag("Specimen");
            if (specimenObj != null) specimen = specimenObj.transform;
        }

        currentTarget = specimen;
        agent.stoppingDistance = attackRange - 0.1f;
    }

    private void Update()
    {
        if (currentTarget == null) return;

        UpdateAggro();

        float distToTarget = Vector3.Distance(transform.position, currentTarget.position);
        bool inAttackRange = distToTarget <= attackRange;

        if (inAttackRange)
            AttackTarget();
        else
            ChaseTarget();

        if (anim)
            anim.SetBool("isWalking", !inAttackRange);
    }

    private void UpdateAggro()
    {
        if (player == null) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer <= aggroRange)
        {
            isAggroed = true;
            currentTarget = player;
        }
        else if (isAggroed && distToPlayer > aggroRange * 1.5f)
        {
            isAggroed = false;
            currentTarget = specimen;
        }
    }

    private void ChaseTarget()
    {
        agent.isStopped = false;
        agent.speed = chaseSpeed * speedModifier;
        agent.stoppingDistance = attackRange - 0.1f;
        agent.SetDestination(currentTarget.position);

        float dist = Vector3.Distance(transform.position, currentTarget.position);
        if (dist < 3f) agent.speed = closeRangeSpeed;
    }

    private void AttackTarget()
    {
        agent.isStopped = true;
        agent.ResetPath();

        var flatTarget = new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z);
        transform.LookAt(flatTarget);

        if (!alreadyAttacked)
        {
            if (anim) anim.SetTrigger("Attack");
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    public void DealDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, whatIsPlayer);
        foreach (Collider hit in hits)
        {
            PlayerCharacter p = hit.GetComponent<PlayerCharacter>();
            if (p != null) p.Hurt(damage);
        }

        if (currentTarget == specimen)
        {
            Specimen s = specimen.GetComponent<Specimen>();
            if (s != null) s.Hurt(damage);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.isStopped = false;
    }

    public void TriggerAggro()
    {
        isAggroed = true;
        currentTarget = player;
    }
}