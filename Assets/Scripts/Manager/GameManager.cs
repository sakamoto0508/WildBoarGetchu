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
    [SerializeField] private string _particleObjectName = "root";
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private ParticleSystem _playerEmoteEffect; // �p�[�e�B�N���ǉ�
    private List<EnemyBase> _enemies;
    private bool _hasCleared = false;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("onenable");
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
        _enemies = FindObjectsOfType<EnemyBase>().ToList();
    }
    private void Update()
    {
        if (_hasCleared)
            return;
        bool allCaught = _enemies.All(e => e.enemyState == EnemyState.Getchued);
        if (allCaught)
        {
            _hasCleared = true;
            OnGameCleared();
        }
    }
    private void OnGameCleared()
    {
        _clearSE.PlayOneShot(_clearSE.clip);
        StartCoroutine(ClearSequence());
    }
    private IEnumerator ClearSequence()
    {
        PlayPlayerEmote();
        yield return new WaitForSeconds(_delayBeforeReturnToTitle);
        ReturnToTitle();
    }
    private void PlayPlayerEmote()
    {
        // �v���C���[��Animator�Ď擾
        var player = GameObject.FindAnyObjectByType<PlayerMover>();
        if (player != null)
        {
            _playerAnimator = player.GetComponent<Animator>();
            // �����̕��@�Ńp�[�e�B�N����T��
            _playerEmoteEffect = player.GetComponentInChildren<ParticleSystem>();
            
        }
        if (_playerAnimator != null)
        {
            _playerAnimator.SetTrigger("Clear");
        }
            _playerEmoteEffect.Play();
    }
    private void ReturnToTitle()
    {
        SceneManager.LoadScene(_titleSceneName);
    }
}