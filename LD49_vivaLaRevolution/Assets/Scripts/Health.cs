
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public bool randomizeHealth = true;
    public MeshRenderer meshRenderer;
    public UnityEvent<float> onHeal;
    public UnityEvent<float> onTakeDamage;

    public int currentHealth { get; private set; }

    public Damage lastDamage;

    private Transform meshTransform;
    private bool isPolice;
    private Vector3 initialScale;

    private void Awake()
    {

        meshRenderer = GetComponent<MeshRenderer>();
        if (!meshRenderer)
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshTransform = meshRenderer.transform;
        isPolice = tag.Equals("Police");
        currentHealth = randomizeHealth ? Mathf.FloorToInt(maxHealth - (Random.Range(1, maxHealth * 0.15f))) : maxHealth;
        initialScale = meshTransform.localScale;
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
    public void takeDamage(Damage damage)
    {
        currentHealth -= damage.amount;
        if (currentHealth <= 0)
        {
            if (TryGetComponent(out RTSUnit rts))
            {
                rts.OnKill();
            }
            else
            {

                Destroy(gameObject);

            }
        }
        else
        {
            lastDamage = damage;
            onTakeDamage?.Invoke(damage.amount / (float)maxHealth);
            meshTransform.DOScale(Vector3.one * 0.5f, 0.1f).OnComplete(() =>
                    {
                        if (meshTransform != null)
                        {
                            meshTransform.DOScale(initialScale, 0.2f);
                        }
                    });
            UpdateColor();
        }


    }

    private void UpdateColor()
    {
        Color color = meshRenderer.material.color;
        float ratio = 1f - HealthRatio();
        meshRenderer.material.color = isPolice ? new Color(color.b * ratio, color.b * ratio, color.b) : new Color(color.r, color.r * ratio, color.r * ratio);
    }

    public float HealthRatio()
    {
        return currentHealth / (float)maxHealth;
    }


}

public struct Damage
{
    public DamageType damageType;
    public int amount;
    public Transform originTransform;

    public Damage(DamageType damageType, int amount, Transform originTransform)
    {
        this.damageType = damageType;
        this.amount = amount;
        this.originTransform = originTransform;
    }
}

public enum DamageType
{
    Molotov,
    Meele,
    Ranged
}
