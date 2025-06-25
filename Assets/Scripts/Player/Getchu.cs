using UnityEngine;
using UnityEngine.AI;

public class Getchu : MonoBehaviour
{
    PlayerContainer _playerContainer;
    [SerializeField] private GameObject _teleportPoint;
    [SerializeField] private AudioSource _getchuSE;

    private void Start()
    {
        _playerContainer = FindAnyObjectByType<PlayerContainer>();
        _getchuSE = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && _playerContainer.state == PlayerContainer.MyState.Attack)
        {
            // 敵の状態を先に変更（重要：NavMeshAgentを無効化する前に）
            EnemyBase enemyBase = other.GetComponent<EnemyBase>();
            if (enemyBase == null) return;

            // 既にGetchued状態の場合は処理しない（重複実行防止）
            if (enemyBase.enemyState == EnemyBase.EnemyState.Getchued) return;

            // 状態を即座にGetchuedに変更
            enemyBase.enemyState = EnemyBase.EnemyState.Getchued;
            // チュートリアル用：敵が捕まったことをAttackTaskに通知（チュートリアルが存在する場合のみ）
            var tutorialManager = FindObjectOfType<TutorialManager>();
            if (tutorialManager != null)
            {
                AttackTask.OnEnemyCaught();
            }
            // SE再生
            _getchuSE.PlayOneShot(_getchuSE.clip);

            // NavMeshAgent処理
            NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.ResetPath(); // 現在のパスをクリア
                agent.velocity = Vector3.zero; // 速度をリセット
                agent.isStopped = true; // 移動を停止
                // 一時的に無効化はGetchuManagerで行う
            }

            // Getchu演出呼び出し
            var getchuManager = FindObjectOfType<GetchuManager>();
            if (getchuManager != null)
            {
                getchuManager.PlayGetchuSequence(other.transform, _teleportPoint.transform, agent);
            }
        }
    }
}