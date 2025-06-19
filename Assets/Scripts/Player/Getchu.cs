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
            //Debug.Log($"敵に当たった {other.name}");

            // NavMeshAgent を一時的に無効化（位置変更前）
            NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = false;
            }

            // 状態を Getchued に変更
            if (other.TryGetComponent<EnemyBase>(out EnemyBase enemyBase))
            {
                enemyBase.enemyState = EnemyBase.EnemyState.Getchued;
            }

            // Getchu演出呼び出し（敵とテレポート先を渡す）
            var getchuManager = FindObjectOfType<GetchuManager>();
            if (getchuManager != null)
            {
                getchuManager.PlayGetchuSequence(other.transform, _teleportPoint.transform, agent);
            }
        }
    }
}
