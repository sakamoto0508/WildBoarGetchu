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

    // �ړ�������ۑ����邽�߂̕ϐ�
    private Vector3 _moveDirection;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        //FixedUpdate  LateUpdate �̃Y������
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

        // �ړ�������ۑ��i��]�p�j
        if (moveDirection.magnitude > 0.1f)
        {
            _moveDirection = moveDirection.normalized;
        }

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
            //AudioSource.PlayClipAtPoint(_damageSE, Camera.main.transform.position);
            _damageSE.PlayOneShot(_damageSE.clip);
            _canAttack = false;
        }
    }
    
    private void RotateTest()
    {
        // �ړ����Ă���ꍇ�̂݉�]
        if (_moveDirection.magnitude > 0.1f && _isRunning)
        {
            // �ړ���������ɂ����ڕW��]���v�Z
            Quaternion targetRot = Quaternion.LookRotation(_moveDirection);

            // Rigidbody �� MoveRotation �ŉ�]FixedDeltaTime ���g���Ċ��炩�ɕ��
            Quaternion newRot = Quaternion.Slerp(
                _rb.rotation,//���݂̉�]
                targetRot,//�ڕW��]
                _rotateSpeed * Time.fixedDeltaTime//�⊮���x
            );
            //MoveRotation: �������Z�Ɛ�������ۂ�����]���@
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
