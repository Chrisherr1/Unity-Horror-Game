using UnityEngine;

public class MonsterKillOnTouch : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerDeath death = collision.gameObject.GetComponent<PlayerDeath>();

            if (death != null)
            {
                death.Die();
            }
        }
    }
}