using UnityEngine;

public class MonsterAttackHit : MonoBehaviour
{
    public Transform player;
    public float hitRange = 2f;

    public void AttackHit()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= hitRange)
        {
            PlayerDeath playerDeath = player.GetComponent<PlayerDeath>();

            if (playerDeath != null)
                playerDeath.Die();
        }
    }
}