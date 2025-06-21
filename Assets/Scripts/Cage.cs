using UnityEngine;

public class Cage : MonoBehaviour
{
    public static Cage Instance;
    public bool Breaked = false;
    [SerializeField] private GameObject _explosion;
    [SerializeField] private AudioSource _explosionSE;
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
            Instantiate(_explosion, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(_explosionSE.clip, transform.position);
            enemyBase.enemyState = EnemyBase.EnemyState.Wander;
            Breaked = true;
            this.gameObject.SetActive(false);
            Invoke(nameof(ActiveCage), 5f);
        }
    }

    private void ActiveCage()
    {
        this.gameObject.SetActive(true);
        Breaked = false;
    }
}
