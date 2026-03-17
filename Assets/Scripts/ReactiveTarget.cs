using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ReactiveTarget : MonoBehaviour
{
    [SerializeField] float health = 100f;
    private bool isAlive = true;
    private Animator anim;

    [SerializeField] private GameObject deathSmokePrefab;

    public event Action<ReactiveTarget> OnDeath;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ReactToHit(float damage)
    {
        if (!isAlive) return;
        WanderingAI behavior = GetComponent<WanderingAI>();
        if (behavior != null)
            behavior.SetAlive(false);

        TakeDamage(damage);  //now uses players damage value
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;
        health -= damage;
        if (health <= 0f)
        {
            health = 0f;
            isAlive = false;
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        ZombieAI ai = GetComponent<ZombieAI>();
        if (ai != null) ai.enabled = false;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;
        GameEvents.EnemyKilled(); //broadcasting enemy killed event to update score and other systems
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(2f);
        if (deathSmokePrefab != null)
            Instantiate(deathSmokePrefab, transform.position, Quaternion.identity);
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

}