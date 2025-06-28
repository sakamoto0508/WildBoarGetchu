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
    /// 追跡可能な時間の感覚(クールタイム)
    /// </summary>
    [SerializeField] private float _trackingTimeLimit = 5f;
    /// <summary>
    /// 次にプレイヤーを追跡できる時刻
    /// </summary>
    private float _trakingTime = 0f;
    //ランダムの座標
    [SerializeField] private float _randX = 100;
    [SerializeField] private float _randZ = 100;
    [SerializeField] private float _mRandX = -100;
    [SerializeField] private float _mRandZ = -100;
    [SerializeField] private float _allowableDistance = 1f;

    /// <summary>
    /// 自分の NavMeshAgent コンポーネント（自動移動に使用）
    /// </summary>
    private NavMeshAgent _myAgent;

    /// <summary>
    /// 敵の視界距離
    /// </summary>
    [SerializeField] private float _sightRange = 25f;

    /// <summary>
    /// 視野角（正面から左右何度まで見えるか）
    /// </summary>
    [SerializeField] private float _viewAngleLimit = 45f;
    /// <summary>
    /// プレイヤーの10メートル先
    /// </summary>
    [SerializeField] private float _predictionDistance = 10f;
    /// <summary>
    /// プレイヤーから逃げる距離
    /// </summary>
    [SerializeField] private float _runDistance = 10f;
    Collider _collider;
    /// <summary>
    /// Getchued状態になった時間を記録
    /// </summary>
    private float _getchuedStartTime = 0f;
    /// <summary>
    /// コライダーを無効化するまでの遅延時間
    /// </summary>
    [SerializeField] private float _colliderDisableDelay = 2f;
    /// <summary>
    /// コライダーが既に無効化されたかどうかのフラグ
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
                if (!_collider.enabled)
                {
                    _collider.enabled = true;
                }
                _xclamation.SetActive(false);
                DetectPlayer();
                DetectCage();
                SetNextWanderGoal();
                break;

            case EnemyState.ChaseCage:
                
                _xclamation.SetActive(true);
                if (!_targetCage.gameObject.activeSelf
                    || Cage.Instance.Breaked)
                {
                    ReturnToWander();
                }
                break;

            case EnemyState.RunFromPlayer:
                
                _xclamation.SetActive(true);
                HandleFleeState();
                break;

            case EnemyState.Getchued:
                _xclamation.SetActive(false);

                // NavMeshAgentが有効な場合は完全に停止
                if (_myAgent.enabled)
                {
                    _myAgent.isStopped = true;
                    _myAgent.ResetPath();
                    _myAgent.velocity = Vector3.zero;
                }

                // Getchued状態になった直後の処理
                if (_getchuedStartTime == 0f)
                {
                    _getchuedStartTime = Time.time;
                    _colliderDisabled = false;
                }

                // 2秒経過後にコライダーを無効化
                if (!_colliderDisabled && Time.time - _getchuedStartTime >= _colliderDisableDelay)
                {
                    _collider.enabled = false;
                    _colliderDisabled = true;

                    // NavMeshAgentも再度停止を確実にする
                    if (_myAgent.enabled)
                    {
                        _myAgent.isStopped = true;
                        _myAgent.ResetPath();
                    }
                }

                // 檻が無くなったらワンダーに戻る
                if (!_targetCage.gameObject.activeSelf)
                {
                    ReturnToWander();
                }
                break;
        }
        _animator.SetInteger("State", (int)enemyState);
    }

    /// <summary>
    /// 視覚でプレイヤーを検知する
    /// </summary>
    void DetectPlayer()
    {
        //檻を見つけている場合無視
        if (enemyState == EnemyState.ChaseCage) return;
        // 捕まってる状態なら無視
        if (enemyState == EnemyState.Getchued) return;

        Vector3 toPlayer = _targetPlayer.position - transform.position;
        float distance = toPlayer.magnitude;

        // 距離が視界外なら無視
        if (distance > _sightRange) return;

        float angle = Vector3.Angle(transform.forward, toPlayer);
        // 視野角の外なら無視
        if (angle > _viewAngleLimit) return;

        // Raycast でプレイヤーが遮られていないか確認（Layer13 は無視）
        Vector3 eyePos = transform.position + Vector3.up;
        int layerMask = ~(1 << 13);

        // Rayが当たってなかったら無視
        if (!Physics.Raycast(eyePos, toPlayer.normalized, out RaycastHit hit, _sightRange, layerMask)) return;

        // プレイヤー以外のオブジェクトに遮られていれば無視
        if (!hit.collider.CompareTag("Player")) return;
        //SEを再生
        PlayFindSEOnce();
        // プレイヤー見つけた
        TryStartRunFromPlayer();

    }

    /// <summary>
    /// 視覚でケージを検知する
    /// </summary>
    void DetectCage()
    {
        //プレイヤーを追いかけている場合無視
        if (enemyState == EnemyState.ChasePlayer) return;
        // 捕まってる状態なら無視
        if (enemyState == EnemyState.Getchued) return;

        Vector3 toCage = _targetCage.position - transform.position;
        float distance = toCage.magnitude;

        // 距離が視界外なら無視
        if (distance > _sightRange) return;

        float angle = Vector3.Angle(transform.forward, toCage);
        // 視野角の外なら無視
        if (angle > _viewAngleLimit) return;

        // Raycast でプレイヤーが遮られていないか確認（Layer13 は無視）
        Vector3 eyePos = transform.position + Vector3.up;
        int layerMask = ~(1 << 13);
        // Rayが当たってなかったら無視
        if (!Physics.Raycast(eyePos, toCage.normalized, out RaycastHit hit, _sightRange, layerMask)) return;
        // プレイヤー以外のオブジェクトに遮られていれば無視
        if (!hit.collider.CompareTag("Cage")) return;
        //SEを再生
        PlayFindSEOnce();
        // 檻を見つけた
        TryStartChaseCage();

    }

    /// <summary>
    /// プレイヤーから逃げる処理開始
    /// </summary>
    void TryStartRunFromPlayer()
    {
        // 捕まってる状態なら無視
        if (enemyState == EnemyState.Getchued) return;

        //enemyState = EnemyState.RunFromPlayer;
        enemyState = EnemyState.RunUP;
        _myAgent.speed = _runSpeed;
        SetFleeDestination();
        //SetChaseDestination();

        _trakingTime = Time.time;
        //Debug.Log("プレイヤーをみつけたぜ！");
    }

    /// <summary>
    /// ケージ追跡開始処理
    /// </summary>
    void TryStartChaseCage()
    {
        // 捕まってる状態なら無視
        if (enemyState == EnemyState.Getchued) return;

        //enemyState = EnemyState.ChaseCage;
        enemyState = EnemyState.RunUP;
        _myAgent.speed = _chaseSpeed;
        Invoke(nameof(ChaseCage), 3f);

        //Invoke(nameof(SetChaseDestination), 3f);
        //SetChaseDestination();
        _trakingTime = Time.time;
        //Debug.Log("檻ををみつけたぜ！");
    }



    /// <summary>
    /// プレイヤーから逃げる処理
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
            //Debug.Log("逃げる場所が見つからなかった！");
            ReturnToWander();
        }
    }

    /// <summary>
    /// 徘徊状態に戻す
    /// </summary>
    void ReturnToWander()
    {
        enemyState = EnemyState.Wander;
        _myAgent.speed = _wanderSpeed;

        // NavMeshAgentが無効化されている場合は有効化
        if (!_myAgent.enabled)
        {
            _myAgent.enabled = true;
        }

        _myAgent.isStopped = false; // 移動を再開
        SetNextWanderGoal();

        // Getchued関連の変数をリセット
        _getchuedStartTime = 0f;
        _colliderDisabled = false;

        // コライダーも有効化
        if (!_collider.enabled)
        {
            _collider.enabled = true;
        }
    }

    /// <summary>
    /// ランダムな地点に移動
    /// </summary>
    void SetNextWanderGoal()
    {
        enemyState = EnemyState.Wander;

        // 許容する距離の誤差内に居なければ新しい目的地を検索しない
        if ((_myAgent.destination - transform.position).sqrMagnitude > _allowableDistance * _allowableDistance)
        {
            return;
        }

        //Debug.Log($"目的地に着いたぜ！");

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
        if (_myAgent == null || !_myAgent.enabled)
        {
            
            return;
        }

        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            
            return;
        }

        enemyState = EnemyState.ChaseCage;
        _myAgent.SetDestination(_targetCage.position);
    }
    void HandleFleeState()
    {
        if (_myAgent.remainingDistance <= _allowableDistance && !_myAgent.pathPending)
        {
            ReturnToWander();
            //Debug.Log("逃げ切ったので徘徊に戻る");
        }
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
