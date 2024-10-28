using Combat;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBar;
    private Animator animator;
    private CharacterStats characterStats;
    private bool isDead = false;

    public float goldOnDeath = 10f;  // D��man �ld���nde kazand�rd��� alt�n miktar�

    void Start()
    {
        animator = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        if (characterStats != null)
        {
            maxHealth = characterStats.maxHealth;
            currentHealth = maxHealth;
        }

        UpdateHealthBar();
    }

    public void TakeDamage(float damage, string attackerTag)
    {
        if (attackerTag == gameObject.tag)
        {
            Debug.Log("Friendly fire prevented!");
            return;
        }

        if (characterStats != null && characterStats.hasShield && characterStats.shield > 0)
        {
            characterStats.shield -= damage;

            if (characterStats.shield < 0)
            {
                damage = -characterStats.shield;
                characterStats.shield = 0;
            }
            else
            {
                damage = 0;
            }
        }

        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead)
        {
            currentHealth = 0;
            Die(attackerTag);
        }
        else
        {
            TriggerImpact();
        }
        UpdateHealthBar();
    }

    void TriggerImpact()
    {
        if (animator != null)
        {
            animator.SetTrigger("Impact");
        }
    }

    void Die(string attackerTag)
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        DisableActions();

        // E�er Player karakteri bu d��man� �ld�rd�yse, �ld�r�len d��man say�s�n� art�r
        if (attackerTag == "Player")
        {
            GameStatsManager.Instance.AddKill();  // D��man �ld�rme say�s�n� art�r
            GameStatsManager.Instance.EarnGold(goldOnDeath);  // Alt�n kazan
        }

        Invoke(nameof(DestroyCharacter), 4f);
    }

    void DestroyCharacter()
    {
        Destroy(gameObject);
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
    }

    void DisableActions()
    {
        if (TryGetComponent(out PlayerCombat playerCombat))
        {
            playerCombat.enabled = false;
        }
        if (TryGetComponent(out EnemyAI enemyAI))
        {
            enemyAI.enabled = false;
        }
        if (TryGetComponent(out SoldierAI soldierAI))
        {
            soldierAI.enabled = false;
        }
        if (TryGetComponent(out BaseAI movement))
        {
            movement.enabled = false;
        }
    }
}
