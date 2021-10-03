
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Health : MonoBehaviour
{
    public UnityEvent<float> onHeal;
    public UnityEvent<float> onTakeDamage;
    public int maxHealth = 100;
    public int currentHealth { get; private set; }
    MeshRenderer meshRenderer;
    bool isPolice;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        isPolice = tag.Equals("Police");
        currentHealth = maxHealth-(Random.Range(0,maxHealth));
        UpdateColor();
    }

    public void heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        onHeal?.Invoke(amount / (float)maxHealth);
        UpdateColor();
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
            onTakeDamage?.Invoke(amount / (float)maxHealth);
        }

        transform.DOScale(Vector3.one * 0.5f, 0.1f).OnComplete(() => transform.DOScale(Vector3.one, 0.2f));
        UpdateColor();
    }

    private void UpdateColor()
    {
        Color color = meshRenderer.material.color;
        float ratio=currentHealth/(float)maxHealth;
        meshRenderer.material.color = isPolice ? new Color(color.b * ratio, color.b * ratio, color.b) : new Color(color.r, color.r * ratio, color.r * ratio);
    }


}
