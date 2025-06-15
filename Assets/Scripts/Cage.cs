using UnityEngine;

public class Cage : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") &&
            collision.gameObject.TryGetComponent<EnemyBase>(out EnemyBase enemyBase))
        {
                enemyBase.enemyState = EnemyBase.EnemyState.Wander;
                this.gameObject.SetActive(false);
                Invoke(nameof(ActiveCage), 5f);
        }
    }
    private void ActiveCage()
    {
        this.gameObject.SetActive(true);
    }
}
