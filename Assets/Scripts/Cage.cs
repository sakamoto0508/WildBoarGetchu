using UnityEngine;

public class Cage : MonoBehaviour
{
    public static Cage Instance;
    public bool Breaked=false;
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") &&
            collision.gameObject.TryGetComponent<EnemyBase>(out EnemyBase enemyBase))
        {
                enemyBase.enemyState = EnemyBase.EnemyState.Wander;
                this.gameObject.SetActive(false);
                Breaked = true;
                Invoke(nameof(ActiveCage), 5f);
        }
    }
    private void ActiveCage()
    {
        this.gameObject.SetActive(true);
        Breaked = false;
    }
}
