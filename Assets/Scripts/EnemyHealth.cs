using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Tooltip("Salud inicial del enemigo.")]
    public int health = 100;

    [Tooltip("Evento que se dispara cuando el enemigo es destruido.")]
    public UnityEvent OnDeath;

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath.Invoke();
        Destroy(gameObject);
    }
}
