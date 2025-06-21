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
    [SerializeField] private GameObject _clearUIPanel;
    [SerializeField] private AudioSource _clearSE;

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
        Debug.Log("�S�Ă̓G��߂܂����I�Q�[���N���A�I");
        _clearSE.PlayOneShot(_clearSE.clip);
        // �N���AUI��\��
        if (_clearUIPanel != null)
            _clearUIPanel.SetActive(true);

        // ���̃V�[���֑J�ڂ������ꍇ�͈ȉ���L����
        // SceneManager.LoadScene("NextSceneName");
    }
}
