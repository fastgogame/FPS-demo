using Mirror;
using UnityEngine;

public class Target : NetworkBehaviour
{
    float health = 100f;
    
    public void TakeDamage(float amount)
    {
        health -= amount;
        if(health <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        Destroy(gameObject);
    }
}
