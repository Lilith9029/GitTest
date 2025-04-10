using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    // ------------------- SETTINGS -------------------
    [Header("Health Settings")]
    public float maxHealth = 100f;
    [SerializeField] private Slider _slider;

    // ------------------- STATES -------------------
    private float currentHealth;

    // ------------------- UNITY METHODS -------------------
    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Update()
    {
        HealthCheck();
    }

    // ------------------- HEALTH LOGIC -------------------
    private void HealthCheck()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (_slider != null)
        {
            _slider.value = currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // Thêm xử lý chết tùy theo loại object (player, enemy,...)
        // Ví dụ: Destroy(gameObject);
    }
}
