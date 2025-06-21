using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : EnemyBase
{
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
    [SerializeField] private GameObject _xclamation;
    [SerializeField] private AudioSource _findSE;
    private float _lastSETime = -10f;
    [SerializeField] private float _seCooldown = 1f;

    // Start is called before the first frame update
    void Start()
    {
        _myAgent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<Collider>();
        _findSE = GetComponent<AudioSource>();
        _myAgent.speed = _wanderSpeed;
        _xclamation.SetActive(false);
        SetNextWanderGoal();
    }

    // Update is called once per frame
    void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Wander:
                _xclamation.SetActive(false);
                if (!_collider.enabled)
                {
                    _collider.enabled = true;
                }
                DetectPlayer();
                DetectCage();
                SetNextWanderGoal();
                break;

            case EnemyState.ChaseCage:
                _xclamation.SetActive(true);
                if (!_targetCage.gameObject.activeSelf || Cage.Instance.Breaked)
                {
                    ReturnToWander();
                }
                break;

            case EnemyState.ChasePlayer:
                _xclamation.SetActive(true);
                HandleChaseStatePlayer();
                break;

            case EnemyState.Getchued:
                _xclamation.SetActive(false);

                // NavMeshAgent���L���ȏꍇ�͊��S�ɒ�~
                if (_myAgent.enabled)
                {
                    _myAgent.isStopped = true;
                    _myAgent.ResetPath();
                    _myAgent.velocity = Vector3.zero;
                }

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

                    // NavMeshAgent���ēx��~���m���ɂ���
                    if (_myAgent.enabled)
                    {
                        _myAgent.isStopped = true;
                        _myAgent.ResetPath();
                    }
                }

                // �B�������Ȃ����烏���_�[�ɖ߂�
                if (!_targetCage.gameObject.activeSelf)
                {
                    ReturnToWander();
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
        //SE���Đ�
        PlayFindSEOnce();
        // �v���C���[������
        TryStartChasePlayer();

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
        //SE���Đ�
        PlayFindSEOnce();
        // �B��������
        TryStartChaseCage();

    }

    /// <summary>
    /// �v���C���[�ǐՊJ�n����
    /// </summary>
    void TryStartChasePlayer()
    {
        // �߂܂��Ă��ԂȂ疳��
        if (enemyState == EnemyState.Getchued) return;

        enemyState = EnemyState.RunUP;
        _myAgent.speed = _chaseSpeed;
        Invoke(nameof(SetChaseDestination), 3f);

        _trakingTime = Time.time;
    }

    /// <summary>
    /// �P�[�W�ǐՊJ�n����
    /// </summary>
    void TryStartChaseCage()
    {
        // �߂܂��Ă��ԂȂ疳��
        if (enemyState == EnemyState.Getchued) return;

        enemyState = EnemyState.RunUP;
        _myAgent.speed = _chaseSpeed;
        Invoke(nameof(ChaseCage), 3f);

        
        //�g���b�L���O�^�C���Ɉ�x���̎��Ԃ�����
        _trakingTime = Time.time;
    }



    /// <summary>
    /// �v���C���[�ǐՏ�Ԃ̏���
    /// </summary>
    void HandleChaseStatePlayer()
    {
        Vector3 toPlayer = _targetPlayer.position - transform.position;
        float distance = toPlayer.magnitude;

        float angle = Vector3.Angle(transform.forward, toPlayer);
        Vector3 eyePos = transform.position + Vector3.up;
        int layerMask = ~(1 << 13);

        bool canSeePlayer = false;
        // Raycast �Ńv���C���[�����E�ɓ����Ă��邩���m�F
        if (distance <= _sightRange && angle <= _viewAngleLimit &&
            Physics.Raycast(eyePos, toPlayer.normalized, out RaycastHit hit, _sightRange, layerMask) &&
            hit.collider.CompareTag("Player"))
        {
            canSeePlayer = true;
        }
        //�g���b�L���O�^�C�����瑝�������Ԃƃ��~�b�g���ׂ�
        if (canSeePlayer && Time.time - _trakingTime >= _trackingTimeLimit)
        {
            enemyState = EnemyState.RunUP;
            Invoke(nameof(SetChaseDestination), 3f);
            _trakingTime += _trackingTimeLimit;
        }
        else if (!canSeePlayer && Time.time - _trakingTime >= _trackingTimeLimit)
        {
            ReturnToWander();
        }
    }

    /// <summary>
    /// �p�j��Ԃɖ߂�
    /// </summary>
    void ReturnToWander()
    {
        enemyState = EnemyState.Wander;
        _myAgent.speed = _wanderSpeed;

        // NavMeshAgent������������Ă���ꍇ�͗L����
        if (!_myAgent.enabled)
        {
            _myAgent.enabled = true;
        }

        _myAgent.isStopped = false; // �ړ����ĊJ
        SetNextWanderGoal();

        // Getchued�֘A�̕ϐ������Z�b�g
        _getchuedStartTime = 0f;
        _colliderDisabled = false;

        // �R���C�_�[���L����
        if (!_collider.enabled)
        {
            _collider.enabled = true;
        }
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

        Vector3 randomPos = new Vector3(Random.Range(_mRandX, _randX), 0, Random.Range(_mRandZ, _randZ));

        if (NavMesh.SamplePosition(randomPos, out NavMeshHit navMeshHit, 5f, NavMesh.AllAreas))
        {
            _myAgent.destination = navMeshHit.position;
            
        }
    }
    /// <summary>
    /// �ǐՏ�Ԃ̎��A�v���C���[�̌��������ɍs������
    /// </summary>
    void SetChaseDestination()
    {
        enemyState = EnemyState.ChasePlayer;
        Vector3 toPlayer = (_targetPlayer.position - transform.position).normalized;
        Vector3 predictedPosition = _targetPlayer.position + toPlayer * _predictionDistance;
        // NavMesh�o�H�擾�p
        NavMeshPath path = new NavMeshPath();
        // �o�H���v�Z�ł��āA�ڕW�n�_���L���Ȃ�Z�b�g
        if (NavMesh.CalculatePath(transform.position, predictedPosition, NavMesh.AllAreas, path) &&
            path.status == NavMeshPathStatus.PathComplete)
        {
            _myAgent.SetPath(path); // �o�H�S�̂��Z�b�g
        }
        else
        {
            // �o�H���s���ȏꍇ�̓v���C���[�̌��ݒn�Ɍ�����
            _myAgent.SetDestination(_targetPlayer.position);
        }
    }

    void ChaseCage()
    {
        enemyState = EnemyState.ChaseCage;
        _myAgent.SetDestination(_targetCage.position);
    }

    void PlayFindSEOnce()
    {
        if (Time.time - _lastSETime >= _seCooldown)
        {
            _findSE.PlayOneShot(_findSE.clip);
            _lastSETime = Time.time;
        }
    }
}
