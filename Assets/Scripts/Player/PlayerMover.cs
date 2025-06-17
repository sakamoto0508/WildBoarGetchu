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
        //FixedUpdate  LateUpdate �̃Y������
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
        // ���͎擾
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // �J�����̑O���ƉE�������g���ē��͕��������[���h��Ԃɕϊ�
        Vector3 forward = _camera.forward;
        Vector3 right = _camera.right;

        // Y�������̉e�������O�i���ʈړ��̂��߁j
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // �J�����̌������x�[�X�ɂ����ړ��x�N�g��
        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized * _moveSpeed;
        moveDirection = new Vector3(moveDirection.x, _rb.velocity.y, moveDirection.z);

        // ��Ԃɉ������ړ��̐���
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

        // �A�j���[�V�����F���ȏ㑬�x���o�Ă���Α����Ă���Ɣ���
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
    /// �J�����̌����Ɋ�Â��ăv���C���[����]������
    /// </summary>
    private void Rotate()
    {
        /*Vector3 direction = _camera.forward;
        direction.y = 0;
        if (direction.magnitude > 0.1f)
        {
            // �J�����̌����Ă���������v���C���[�̖ڕW��]�p�x�Ƃ��Đݒ�
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // ���݂̌����ƖڕW�̌����̊Ԃ��ԁi���炩�ȉ�]�j
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
        }*/

        // �J���������Ɋ�Â����ڕW��]���v�Z
        Vector3 dir = _camera.forward; dir.y = 0;
        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        // Rigidbody �� MoveRotation �ŉ�]FixedDeltaTime ���g���Ċ��炩�ɕ��
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
