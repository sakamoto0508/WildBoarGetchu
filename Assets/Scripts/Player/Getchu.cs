using UnityEngine;
using UnityEngine.AI;

public class Getchu : MonoBehaviour
{
    PlayerContainer _playerContainer;
    //EnemyBase _enemyContainer;
    [SerializeField] private GameObject _teleportPoint;
    private void Start()
    {
        _playerContainer = FindAnyObjectByType<PlayerContainer>();
        //_enemyContainer = FindAnyObjectByType<EnemyBase>();
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Enemy" && _playerContainer.state == PlayerContainer.MyState.Attack)
        {
            Debug.Log($"“G‚É“–‚½‚Á‚½ {other.name}");
            other.transform.position = _teleportPoint.transform.position;

            if(other.gameObject.TryGetComponent<EnemyBase>(out EnemyBase enemyBase))
            {
                enemyBase.enemyState = EnemyBase.EnemyState.Getchued;
            }
            
            other.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        }
    }
}
