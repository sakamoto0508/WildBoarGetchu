using UnityEngine;

public class PlayerMover : MonoBehaviour, IPlayerMover
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpPower = 10f;
    [SerializeField] private float _stanTimeLimit = 3f;
    [SerializeField] private float _rotateSpeed = 10f;
    [SerializeField] private Transform _camera;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _coolTime = 1.5f;
    [SerializeField] private AudioSource _damageSE;

    private Rigidbody _rb;
    private bool _canJump = true;
    private bool _isRunning = false;
    private bool _canAttack = true;
    private float _stanTime = 0f;
    private float _attackLimit = 0f;
    PlayerContainer _container;

    // 移動方向を保存するための変数
    private Vector3 _moveDirection;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        //FixedUpdate  LateUpdate のズレを補間
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _container = FindAnyObjectByType<PlayerContainer>();
        _damageSE = GetComponent<AudioSource>();
    }

    public void PlayerUpdate()
    {
        Jump();
        Attack();
        StateChange();
    }
    private void FixedUpdate()
    {
        //Move();
        MoveTest();
        // Rotate();
        RotateTest();
    }


    private void MoveTest()
    {
        // 入力取得
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // カメラの前方と右方向を使って入力方向をワールド空間に変換
        Vector3 forward = _camera.forward;
        Vector3 right = _camera.right;

        // Y軸方向の影響を除外（平面移動のため）
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // カメラの向きをベースにした移動ベクトル
        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized * _moveSpeed;

        // 移動方向を保存（回転用）
        if (moveDirection.magnitude > 0.1f)
        {
            _moveDirection = moveDirection.normalized;
        }

        moveDirection = new Vector3(moveDirection.x, _rb.velocity.y, moveDirection.z);

        // 状態に応じた移動の制御
        if (_container.state == PlayerContainer.MyState.Normal)
        {
            _rb.AddForce(moveDirection - _rb.velocity);
        }
        else if (_container.state == PlayerContainer.MyState.Attack)
        {
            _rb.AddForce(moveDirection / 15f - _rb.velocity);
        }
        else if (_container.state == PlayerContainer.MyState.Stan)
        {
            _rb.velocity = Vector3.zero;
        }

        // アニメーション：一定以上速度が出ていれば走っていると判定
        _isRunning = _rb.velocity.magnitude > 0.2f;
        _animator.SetBool("IsRunning", _isRunning);
    }

    private void Jump()
    {
        if (Input.GetButton("Jump") && _canJump == true)
        {
            _rb.AddForce(transform.up * _jumpPower, ForceMode.Impulse);
            _canJump = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            _canJump = true;
        }
        if (collision.gameObject.tag == "Enemy")
        {
            _container.state = PlayerContainer.MyState.Stan;
            //AudioSource.PlayClipAtPoint(_damageSE, Camera.main.transform.position);
            _damageSE.PlayOneShot(_damageSE.clip);
            _canAttack = false;
        }
    }
    
    private void RotateTest()
    {
        // 移動している場合のみ回転
        if (_moveDirection.magnitude > 0.1f && _isRunning)
        {
            // 移動方向を基準にした目標回転を計算
            Quaternion targetRot = Quaternion.LookRotation(_moveDirection);

            // Rigidbody の MoveRotation で回転FixedDeltaTime を使って滑らかに補間
            Quaternion newRot = Quaternion.Slerp(
                _rb.rotation,//現在の回転
                targetRot,//目標回転
                _rotateSpeed * Time.fixedDeltaTime//補完速度
            );
            //MoveRotation: 物理演算と整合性を保った回転方法
            _rb.MoveRotation(newRot);
        }
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0) && _canAttack)
        {
            _animator.SetTrigger("Attack");
            _container.state = PlayerContainer.MyState.Attack;
            _canAttack = false;
        }

        if (Time.time > _attackLimit)
        {

            _canAttack = true;
            _attackLimit += _coolTime;
        }
    }

    private void StateChange()
    {
        if (_container.state == PlayerContainer.MyState.Stan)
        {
            _stanTime += Time.deltaTime;
            if (_stanTime >= _stanTimeLimit)
            {
                _container.state = PlayerContainer.MyState.Normal;
                _canAttack = true;
                _stanTime = 0;
            }
        }
    }
}
