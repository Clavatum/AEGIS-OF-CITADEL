using UnityEngine;

public abstract class BaseUnit : MonoBehaviour
{
    [SerializeField] protected float detectionRange = 10f; // Tespit aral���
    [SerializeField] protected float attackRange = 2f; // Sald�r� mesafesi
    [SerializeField] protected float attackInterval = 1.5f; // Sald�r� aral���
    [SerializeField] protected int damage = 10; // Hasar
    protected float attackCooldown = 0f; // Sald�r� i�in bekleme s�resi
    protected Transform target; // Hedef

    protected virtual void Update()
    {
        if (target == null)
        {
            FindTarget(); // E�er hedef yoksa tespit et
        }
        else
        {
            HandleMovementAndAttack(); // Hedefe do�ru hareket et ve sald�r
        }

        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime; // Sald�r� geri say�m�n� g�ncelle
        }
    }

    protected virtual void FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);

        foreach (Collider hitCollider in hitColliders)
        {
            if (IsValidTarget(hitCollider)) // Ge�erli bir hedef mi?
            {
                target = hitCollider.transform; // Hedefi belirle
                Debug.Log($"Target detected: {target.name}");
                break;
            }
        }
    }

    protected abstract bool IsValidTarget(Collider hitCollider); // Ge�erli hedef ko�ulu, asker/d��man �zelle�tirilecek

    protected virtual void HandleMovementAndAttack()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= attackRange)
        {
            if (attackCooldown <= 0f)
            {
                Attack();
                attackCooldown = attackInterval; // Bir sonraki sald�r� i�in geri say�m
            }
        }
        else
        {
            MoveTowardsTarget(); // Hedefe yakla�
        }
    }

    protected virtual void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * 2f); // Y�r�me h�z�
    }

    protected virtual void Attack()
    {
        Debug.Log($"Attacking target for {damage} damage!");
        // Hedefe zarar ver (�rnek)
        // target.GetComponent<BaseUnit>().TakeDamage(damage);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Hasar alma i�lemi �rne�i
    public virtual void TakeDamage(int amount)
    {
        Debug.Log($"{gameObject.name} took {amount} damage.");
        // Sa�l�k g�ncellemesi yap�labilir
    }
}
