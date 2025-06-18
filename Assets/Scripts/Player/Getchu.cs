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

            // �G���e���|�[�g
            other.transform.position = _teleportPoint.transform.position;

            // ��Ԃ� Getchued �ɕύX
            if (other.TryGetComponent<EnemyBase>(out EnemyBase enemyBase))
            {
                enemyBase.enemyState = EnemyBase.EnemyState.Getchued;
            }

            // NavMeshAgent ���ĂїL�����i�K�v�Ȃ�j
            if (agent != null)
            {
                agent.enabled = true;
                agent.Warp(_teleportPoint.transform.position); // NavMesh �ɍ��킹��Ȃ� Warp ����
            }
        }
    }
}
