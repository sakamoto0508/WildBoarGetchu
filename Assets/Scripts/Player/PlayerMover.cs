using UnityEngine;

public class PlayerMover : MonoBehaviour, IPlayerMover
{
    private Rigidbody _rb;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpPower = 10f;
    private bool _canJump = true;
    private bool _isRunning = false;
    [SerializeField] private float _rotateSpeed = 10f;
    [SerializeField] private Transform _camera;
    [SerializeField] private Animator _animator;
    private bool _canAttack = true;
    [SerializeField] private float _coolTime = 1.5f;
    private float _stanTime = 0f;
    [SerializeField] private float _stanTimeLimit = 3f;
    private float _attackLimit = 0f;
    PlayerContainer _container;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        //FixedUpdate  LateUpdate のズレを補間
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _container = FindAnyObjectByType<PlayerContainer>();
    }

    public void PlayerUpdate()
    {
        Jump();  
        Attack();
        StateChange();
    }
    private void FixedUpdate()
    {
        Move();
        Rotate();
    }

    private void Move()
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
            _canAttack = false;
        }
    }
    /// <summary>
    /// カメラの向きに基づいてプレイヤーを回転させる
    /// </summary>
    private void Rotate()
    {
        /*Vector3 direction = _camera.forward;
        direction.y = 0;
        if (direction.magnitude > 0.1f)
        {
            // カメラの向いている方向をプレイヤーの目標回転角度として設定
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // 現在の向きと目標の向きの間を補間（滑らかな回転）
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
        }*/

        // カメラ向きに基づいた目標回転を計算
        Vector3 dir = _camera.forward; dir.y = 0;
        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        // Rigidbody の MoveRotation で回転FixedDeltaTime を使って滑らかに補間
        Quaternion newRot = Quaternion.Slerp(
            _rb.rotation,
            targetRot,
            _rotateSpeed * Time.fixedDeltaTime
        );
        _rb.MoveRotation(newRot);
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
