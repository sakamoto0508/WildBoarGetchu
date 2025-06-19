using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRun : EnemyBase
{
    [SerializeField] private float _runSpeed = 10f;
    [SerializeField] private float _chaseSpeed = 10f;
    [SerializeField] private float _wanderSpeed = 5f;
    [SerializeField] private Transform _targetPlayer;
    [SerializeField] private Transform _targetCage;
    [SerializeField] private Animator _animator;
    /// <summary>
    /// �ǐՉ\�Ȏ��Ԃ̊��o(�N�[���^�C��)
    /// </summary>
    [SerializeField] private float _trackingTimeLimit = 5f;
    /// <summary>
    /// ���Ƀv���C���[��ǐՂł��鎞��
    /// </summary>
    private float _trakingTime = 0f;
    //�����_���̍��W
    [SerializeField] private float _randX = 100;
    [SerializeField] private float _randZ = 100;
    [SerializeField] private float _mRandX = -100;
    [SerializeField] private float _mRandZ = -100;
    [SerializeField] private float _allowableDistance = 1f;

    /// <summary>
    /// ������ NavMeshAgent �R���|�[�l���g�i�����ړ��Ɏg�p�j
    /// </summary>
    private NavMeshAgent _myAgent;

    /// <summary>
    /// �G�̎��E����
    /// </summary>
    [SerializeField] private float _sightRange = 25f;

    /// <summary>
    /// ����p�i���ʂ��獶�E���x�܂Ō����邩�j
    /// </summary>
    [SerializeField] private float _viewAngleLimit = 45f;
    /// <summary>
    /// �v���C���[��10���[�g����
    /// </summary>
    [SerializeField] private float _predictionDistance = 10f;
    /// <summary>
    /// �v���C���[���瓦���鋗��
    /// </summary>
    [SerializeField] private float _runDistance = 10f;
    Collider _collider;
    /// <summary>
    /// Getchued��ԂɂȂ������Ԃ��L�^
    /// </summary>
    private float _getchuedStartTime = 0f;
    /// <summary>
    /// �R���C�_�[�𖳌�������܂ł̒x������
    /// </summary>
    [SerializeField] private float _colliderDisableDelay = 2f;
    /// <summary>
    /// �R���C�_�[�����ɖ��������ꂽ���ǂ����̃t���O
    /// </summary>
    private bool _colliderDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        _myAgent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<Collider>();
        _myAgent.speed = _wanderSpeed;
        SetNextWanderGoal();
    }

    // Update is called once per frame
    void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Wander:
                if (!_collider.enabled)
                {
                    _collider.enabled = true;
                }
                DetectPlayer();
                DetectCage();
                SetNextWanderGoal();
                break;

            case EnemyState.ChaseCage:
                if (!_targetCage.gameObject.activeSelf
                    || Cage.Instance.Breaked)
                {
                    ReturnToWander();
                }
                break;

            case EnemyState.RunFromPlayer:
                HandleFleeState();
                break;

            case EnemyState.Getchued:
                // Getchued��ԂɂȂ�������̏���
                if (_getchuedStartTime == 0f)
                {
                    _getchuedStartTime = Time.time;
                    _colliderDisabled = false;
                }

                // 2�b�o�ߌ�ɃR���C�_�[�𖳌���
                if (!_colliderDisabled && Time.time - _getchuedStartTime >= _colliderDisableDelay)
                {
                    _collider.enabled = false;
                    _colliderDisabled = true;
                    _getchuedStartTime = 0f;
                }

                if (!_targetCage.gameObject.activeSelf)
                {
                    ReturnToWander();
                    //Debug.Log("�����_�[�ɖ߂�");
                }
                break;

        }

        _animator.SetInteger("State", (int)enemyState);
    }

    /// <summary>
    /// ���o�Ńv���C���[�����m����
    /// </summary>
    void DetectPlayer()
    {
        //�B�������Ă���ꍇ����
        if (enemyState == EnemyState.ChaseCage) return;
        // �߂܂��Ă��ԂȂ疳��
        if (enemyState == EnemyState.Getchued) return;

        Vector3 toPlayer = _targetPlayer.position - transform.position;
        float distance = toPlayer.magnitude;

        // ���������E�O�Ȃ疳��
        if (distance > _sightRange) return;

        float angle = Vector3.Angle(transform.forward, toPlayer);
        // ����p�̊O�Ȃ疳��
        if (angle > _viewAngleLimit) return;

        // Raycast �Ńv���C���[���Ղ��Ă��Ȃ����m�F�iLayer13 �͖����j
        Vector3 eyePos = transform.position + Vector3.up;
        int layerMask = ~(1 << 13);

        // Ray���������ĂȂ������疳��
        if (!Physics.Raycast(eyePos, toPlayer.normalized, out RaycastHit hit, _sightRange, layerMask)) return;

        // �v���C���[�ȊO�̃I�u�W�F�N�g�ɎՂ��Ă���Ζ���
        if (!hit.collider.CompareTag("Player")) return;

        // �v���C���[������
        TryStartRunFromPlayer();

    }

    /// <summary>
    /// ���o�ŃP�[�W�����m����
    /// </summary>
    void DetectCage()
    {
        //�v���C���[��ǂ������Ă���ꍇ����
        if (enemyState == EnemyState.ChasePlayer) return;
        // �߂܂��Ă��ԂȂ疳��
        if (enemyState == EnemyState.Getchued) return;

        Vector3 toCage = _targetCage.position - transform.position;
        float distance = toCage.magnitude;

        // ���������E�O�Ȃ疳��
        if (distance > _sightRange) return;

        float angle = Vector3.Angle(transform.forward, toCage);
        // ����p�̊O�Ȃ疳��
        if (angle > _viewAngleLimit) return;

        // Raycast �Ńv���C���[���Ղ��Ă��Ȃ����m�F�iLayer13 �͖����j
        Vector3 eyePos = transform.position + Vector3.up;
        int layerMask = ~(1 << 13);
        // Ray���������ĂȂ������疳��
        if (!Physics.Raycast(eyePos, toCage.normalized, out RaycastHit hit, _sightRange, layerMask)) return;
        // �v���C���[�ȊO�̃I�u�W�F�N�g�ɎՂ��Ă���Ζ���
        if (!hit.collider.CompareTag("Cage")) return;
        // �B��������
        TryStartChaseCage();

    }

    /// <summary>
    /// �v���C���[���瓦���鏈���J�n
    /// </summary>
    void TryStartRunFromPlayer()
    {

        // �߂܂��Ă��ԂȂ疳��
        if (enemyState == EnemyState.Getchued) return;

        //enemyState = EnemyState.RunFromPlayer;
        enemyState = EnemyState.RunUP;
        _myAgent.speed = _runSpeed;
        SetFleeDestination();
        //SetChaseDestination();

        _trakingTime = Time.time;
        //Debug.Log("�v���C���[���݂������I");
    }

    /// <summary>
    /// �P�[�W�ǐՊJ�n����
    /// </summary>
    void TryStartChaseCage()
    {

        // �߂܂��Ă��ԂȂ疳��
        if (enemyState == EnemyState.Getchued) return;

        //enemyState = EnemyState.ChaseCage;
        enemyState = EnemyState.RunUP;
        _myAgent.speed = _chaseSpeed;
        Invoke(nameof(ChaseCage), 3f);

        //Invoke(nameof(SetChaseDestination), 3f);
        //SetChaseDestination();
        _trakingTime = Time.time;
        //Debug.Log("�B�����݂������I");
    }



    /// <summary>
    /// �v���C���[���瓦���鏈��
    /// </summary>
    void SetFleeDestination()
    {
        Vector3 awayFromPlayerDir = (transform.position - _targetPlayer.position).normalized;
        Vector3 fleeTargetPos = transform.position + awayFromPlayerDir * _runDistance;

        if (NavMesh.SamplePosition(fleeTargetPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            enemyState = EnemyState.RunFromPlayer;
            _myAgent.SetDestination(hit.position);
        }
        else
        {
            //Debug.Log("������ꏊ��������Ȃ������I");
            ReturnToWander();
        }
    }

    /// <summary>
    /// �p�j��Ԃɖ߂�
    /// </summary>
    void ReturnToWander()
    {
        enemyState = EnemyState.Wander;
        SetNextWanderGoal();
        _myAgent.speed = _wanderSpeed;
        
       
    }

    /// <summary>
    /// �����_���Ȓn�_�Ɉړ�
    /// </summary>
    void SetNextWanderGoal()
    {
        enemyState = EnemyState.Wander;

        // ���e���鋗���̌덷���ɋ��Ȃ���ΐV�����ړI�n���������Ȃ�
        if ((_myAgent.destination - transform.position).sqrMagnitude > _allowableDistance * _allowableDistance)
        {
            return;
        }

        //Debug.Log($"�ړI�n�ɒ��������I");

        Vector3 randomPos = new Vector3(Random.Range(_mRandX, _randX), 0, Random.Range(_mRandZ, _randZ));

        if (NavMesh.SamplePosition(randomPos, out NavMeshHit navMeshHit, 5f, NavMesh.AllAreas))
        {
            _myAgent.destination = navMeshHit.position;
            //Debug.Log(_myAgent.destination);
            //_myAgent.destination = _targetPlayer.position;
        }
    }

    void ChaseCage()
    {
        enemyState = EnemyState.ChaseCage;
        _myAgent.SetDestination(_targetCage.position);
    }
    void HandleFleeState()
    {
        if (_myAgent.remainingDistance <= _allowableDistance && !_myAgent.pathPending)
        {
            ReturnToWander();
            //Debug.Log("�����؂����̂Ŝp�j�ɖ߂�");
        }
    }

}
