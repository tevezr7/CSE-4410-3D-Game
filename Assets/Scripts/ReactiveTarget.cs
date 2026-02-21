using UnityEngine;
using System.Collections;

public class ReactiveTarget : MonoBehaviour
{
    [SerializeField] float health = 100f;
    private PlayerCharacter playerCharacter;
    private float damageAmount = 10f;
    private bool isAlive = true;

    //added enemy health system, that way they dont just die in one hit! variable damage so that in the future, multiple weapon types can be added. :D

    void Start()
    {
        playerCharacter = FindFirstObjectByType<PlayerCharacter>();
        if (playerCharacter != null)
            damageAmount = playerCharacter.damage;  //pull damage from playercharacter
    }

    public void ReactToHit()
    {
        if (!isAlive) return;
        WanderingAI behavior = GetComponent<WanderingAI>();
        if (behavior != null)
            behavior.SetAlive(false);

        TakeDamage(damageAmount);  //now uses players damage value
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
        transform.Rotate(-75, 0, 0);
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    void Update() { }
}