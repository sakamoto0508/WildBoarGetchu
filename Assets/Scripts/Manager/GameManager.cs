using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnemyBase;

/// <summary>
/// �S�Ă̓G���߂܂�����Q�[���N���A�𔻒肷��}�l�[�W���[
/// </summary>
public class GameManager : MonoBehaviour
{
    // �V���O���g���C���X�^���X
    public static GameManager Instance { get; private set; }

    [Header("�N���A��̕\��")]
    [SerializeField] private GameObject _clearUIPanel;
    [SerializeField] private AudioSource _clearSE;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _teleportPoint;

    private List<EnemyBase> _enemies;
    private bool _hasCleared = false;

    private void Awake()
    {
        _clearSE = GetComponent<AudioSource>();
        // �V���O���g��������
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // �V�[�����̑SEnemyBase�����W
        _enemies = FindObjectsOfType<EnemyBase>().ToList();

        // �N���A�pUI���\��
        if (_clearUIPanel != null)
            _clearUIPanel.SetActive(false);
    }

    private void Update()
    {
        if (_hasCleared)
            return;

        // �S�Ă̓G�̏�Ԃ�Getchued�i�߂܂����j���`�F�b�N
        bool allCaught = _enemies.All(e => e.enemyState == EnemyState.Getchued);
        if (allCaught)
        {
            _hasCleared = true;
            OnGameCleared();
        }
    }

    /// <summary>
    /// �Q�[���N���A���̏���
    /// </summary>
    private void OnGameCleared()
    {
        _clearSE.PlayOneShot(_clearSE.clip);
        //�e���|�[�g���s
        PlayerTeleport();
        // �G���[�g���s�ƃ^�C�g���߂�̃R���[�`�����J�n
        //StartCoroutine(ClearSequence());
    }
    private void PlayerTeleport()
    {
        // PlayerMover�𖳌���
        var playerMover = _player.GetComponent<PlayerMover>();
        if (playerMover != null)
        {
            playerMover.enabled = false;
        }

        // Rigidbody�̑��x�����Z�b�g
        Rigidbody playerRb = _player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }
        Debug.Log($"�e���|�[�g�O�̈ʒu: {_player.transform.position}");
        Debug.Log($"�e���|�[�g��̈ʒu: {_teleportPoint.transform.position}");
        // �e���|�[�g���s
        _player.transform.position = _teleportPoint.transform.position;
        Debug.Log($"�e���|�[�g��̈ʒu: {_player.transform.position}");
        // �����҂��Ă���PlayerMover���ėL�����i�K�v�ɉ����āj
        StartCoroutine(ReenablePlayerMover(playerMover));
    }
    private IEnumerator ReenablePlayerMover(PlayerMover playerMover)
    {
        yield return new WaitForSeconds(10f); // 1�b�҂�
        if (playerMover != null)
        {
            playerMover.enabled = true;
        }
    }

}
