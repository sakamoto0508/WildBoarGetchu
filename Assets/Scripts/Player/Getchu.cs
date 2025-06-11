using UnityEngine;
using UnityEngine.AI;

public class Getchu : MonoBehaviour
{
    PlayerContainer _playerContainer;
    EnemyBase _enemyContainer;
    [SerializeField] private GameObject _teleportPoint;
    private void Start()
    {
        _playerContainer = FindAnyObjectByType<PlayerContainer>();
        _enemyContainer = FindAnyObjectByType<EnemyBase>();
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Enemy" && _playerContainer.state == PlayerContainer.MyState.Attack)
        {
            Debug.Log("“G‚É“–‚½‚Á‚½");
            other.transform.position = _teleportPoint.transform.position;
            _enemyContainer.enemyState = EnemyBase.EnemyState.Getchued;
            other.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        }
    }
}
