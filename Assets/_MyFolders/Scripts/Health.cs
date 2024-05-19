using System;
using UnityEngine;

public class Health : MonoBehaviour
{

    public event Action OnDie;

    [field: SerializeField] public int maxHealth { get; private set; }
    public Observer<int> CurrentHealth = new Observer<int>(0);

    void Start()
    {
        CurrentHealth.Value = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth.Value -= damage;
        if (CurrentHealth.Value <= 0)
            Die();
    }

    void Die()
    {
        OnDie?.Invoke();
        Destroy(gameObject);
    }

}
