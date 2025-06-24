using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EnemyBase;
/// <summary>
/// �S�Ă̓G���߂܂�����Q�[���N���A�𔻒肷��}�l�[�W���[
/// </summary>
public class GameManager : MonoBehaviour
{
    // �V���O���g���C���X�^���X
    public static GameManager Instance { get; private set; }

    [Header("�N���A��̕\��")]
    [SerializeField] private AudioSource _clearSE;
    [SerializeField] private float _delayBeforeReturnToTitle = 5f; // �^�C�g���ɖ߂�܂ł̕b��
    [SerializeField] private string _titleSceneName = "Title";
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private ParticleSystem[] _particles; 
    [SerializeField] private float _interval = 0.5f; // ���炷�Ԋu
    [SerializeField] private float _loopInterval = 2f; // 1���̃T�C�N������

    private List<EnemyBase> _enemies;
    private bool _hasCleared = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != _titleSceneName)
        {
            _enemies = FindObjectsOfType<EnemyBase>().ToList();
            _hasCleared = false;


            // �v���C���[��Animator�Ď擾
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                _playerAnimator = player.GetComponent<Animator>();
            }
            else
            {
                Debug.LogWarning("Player���V�[�����Ɍ�����܂���");
            }
        }
    }
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
        // �G���[�g���s�ƃ^�C�g���߂�̃R���[�`�����J�n
        StartCoroutine(ClearSequence());
    }
    /// <summary>
    /// �N���A��̃V�[�P���X�i�G���[�g���^�C�g���߂�j
    /// </summary>
    private IEnumerator ClearSequence()
    {

        // �v���C���[�̃G���[�g�����s
        PlayPlayerEmote();

        // �w��b���ҋ@
        yield return new WaitForSeconds(_delayBeforeReturnToTitle);

        // �^�C�g���V�[���ɖ߂�
        ReturnToTitle();
    }

    /// <summary>
    /// �v���C���[�̃G���[�g�����s
    /// </summary>
    private void PlayPlayerEmote()
    {
        Animator playerAnimator = _playerAnimator.GetComponent<Animator>();
        // �A�j���[�V�������Đ�
        playerAnimator.SetTrigger("Clear");

        // ��F�p�[�e�B�N���G�t�F�N�g�A�ǉ���SE�Đ��Ȃ�
        StartCoroutine(LoopPlayParticles());
    }

    /// <summary>
    /// �^�C�g���V�[���ɖ߂�
    /// </summary>
    private void ReturnToTitle()
    {
        // �t�F�[�h�A�E�g���ʂȂǂ�����ꍇ�͂����Ŏ��s

        // �^�C�g���V�[�������[�h
        SceneManager.LoadScene(_titleSceneName);
    }
    private IEnumerator LoopPlayParticles()
    {
        while (true)
        {
            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i].Play();
                yield return new WaitForSeconds(_interval);
            }

            // �S�čĐ�������A�ēx���[�v�܂ł̑ҋ@����
            float waitTime = _loopInterval - (_interval * _particles.Length);
            if (waitTime > 0)
            {
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
