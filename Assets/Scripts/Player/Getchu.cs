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
            // �G�̏�Ԃ��ɕύX�i�d�v�FNavMeshAgent�𖳌�������O�Ɂj
            EnemyBase enemyBase = other.GetComponent<EnemyBase>();
            if (enemyBase == null) return;

            // ����Getchued��Ԃ̏ꍇ�͏������Ȃ��i�d�����s�h�~�j
            if (enemyBase.enemyState == EnemyBase.EnemyState.Getchued) return;

            // ��Ԃ𑦍���Getchued�ɕύX
            enemyBase.enemyState = EnemyBase.EnemyState.Getchued;
            // �`���[�g���A���p�F�G���߂܂������Ƃ�AttackTask�ɒʒm�i�`���[�g���A�������݂���ꍇ�̂݁j
            var tutorialManager = FindObjectOfType<TutorialManager>();
            if (tutorialManager != null)
            {
                AttackTask.OnEnemyCaught();
            }
            // SE�Đ�
            _getchuSE.PlayOneShot(_getchuSE.clip);

            // NavMeshAgent����
            NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.ResetPath(); // ���݂̃p�X���N���A
                agent.velocity = Vector3.zero; // ���x�����Z�b�g
                agent.isStopped = true; // �ړ����~
                // �ꎞ�I�ɖ�������GetchuManager�ōs��
            }

            // Getchu���o�Ăяo��
            var getchuManager = FindObjectOfType<GetchuManager>();
            if (getchuManager != null)
            {
                getchuManager.PlayGetchuSequence(other.transform, _teleportPoint.transform, agent);
            }
        }
    }
}