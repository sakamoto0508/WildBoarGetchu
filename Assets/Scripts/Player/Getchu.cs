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
            //Debug.Log($"�G�ɓ������� {other.name}");

            // NavMeshAgent ���ꎞ�I�ɖ������i�ʒu�ύX�O�j
            NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = false;
            }

            // ��Ԃ� Getchued �ɕύX
            if (other.TryGetComponent<EnemyBase>(out EnemyBase enemyBase))
            {
                enemyBase.enemyState = EnemyBase.EnemyState.Getchued;
            }

            // Getchu���o�Ăяo���i�G�ƃe���|�[�g���n���j
            var getchuManager = FindObjectOfType<GetchuManager>();
            if (getchuManager != null)
            {
                getchuManager.PlayGetchuSequence(other.transform, _teleportPoint.transform, agent);
            }
        }
    }
}
