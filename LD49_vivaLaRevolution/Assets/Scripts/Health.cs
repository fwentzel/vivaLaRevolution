
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public UnityEvent<float> onHeal;
    public UnityEvent<float> onTakeDamage;
    public int maxHealth = 100;
    public int currentHealth { get; private set; }
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        onHeal?.Invoke(amount);
    }
    public void takeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            onTakeDamage?.Invoke(amount);
        }

    }


}
