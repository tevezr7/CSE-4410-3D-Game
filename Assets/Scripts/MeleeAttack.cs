using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private float damage = 50f;
    [SerializeField] private float range = 2f;
    [SerializeField] private LayerMask enemyLayer;
    private Animator knifeAnimator;

    void Start()
    {
        knifeAnimator = GetComponentInChildren<Animator>();
    }

    public void Slash()
    {
        if (knifeAnimator != null)
            knifeAnimator.SetTrigger("Slash");

        Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyLayer);
        foreach (Collider hit in hits)
        {
            ReactiveTarget target = hit.GetComponentInParent<ReactiveTarget>();
            if (target != null)
                target.ReactToHit(damage);
        }
    }
}