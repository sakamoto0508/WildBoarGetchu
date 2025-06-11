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
        _container = FindAnyObjectByType<PlayerContainer>();
    }

    public void PlayerUpdate()
    {
        Move();
        Jump();
        Rotate();
        Attack();
        StateChange();
    }

    private void Move()
    {
        //移動処理
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        // 入力に基づいたワールド空間の移動方向
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical);
        //プレイヤーのローカル空間に変換
        Vector3 moveDirection = transform.TransformDirection(inputDirection.normalized) * _moveSpeed;
        if (_container.state == PlayerContainer.MyState.Normal)
        {

            _rb.AddForce((moveDirection) - _rb.velocity);
        }
        else if (_container.state == PlayerContainer.MyState.Attack)
        {
            _rb.AddForce((moveDirection) / 15 - _rb.velocity);
        }
        else if (_container.state == PlayerContainer.MyState.Stan)
        {
            _rb.velocity = Vector3.zero;
        }

        //Debug.Log(_rb.velocity);

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

    private void Rotate()
    {
        Vector3 direction = _camera.forward;
        direction.y = 0;
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
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
