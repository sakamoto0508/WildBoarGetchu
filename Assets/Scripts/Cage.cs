using UnityEngine;

public class Cage : MonoBehaviour
{
    //EnemyBase _enemyContainer;
    private void Start()
    {
         //_enemyContainer = FindAnyObjectByType<EnemyBase>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") &&
            collision.gameObject.TryGetComponent<EnemyBase>(out EnemyBase enemyBase))
        {
            //if (enemyBase.enemyState == EnemyBase.EnemyState.ChaseCage)
            //{
                enemyBase.enemyState = EnemyBase.EnemyState.Wander;
                Invoke(nameof(ActiveCage), 5f);
                this.gameObject.SetActive(false);
            //}
        }
    }
    private void ActiveCage()
    {
        this.gameObject.SetActive(true);
    }
}
