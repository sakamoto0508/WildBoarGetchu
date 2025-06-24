using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EnemyBase;
/// <summary>
/// 全ての敵が捕まったらゲームクリアを判定するマネージャー
/// </summary>
public class GameManager : MonoBehaviour
{
    // シングルトンインスタンス
    public static GameManager Instance { get; private set; }

    [Header("クリア後の表示")]
    [SerializeField] private AudioSource _clearSE;
    [SerializeField] private float _delayBeforeReturnToTitle = 5f; // タイトルに戻るまでの秒数
    [SerializeField] private string _titleSceneName = "Title";
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private ParticleSystem[] _particles; 
    [SerializeField] private float _interval = 0.5f; // ずらす間隔
    [SerializeField] private float _loopInterval = 2f; // 1周のサイクル時間

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


            // プレイヤーのAnimator再取得
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                _playerAnimator = player.GetComponent<Animator>();
            }
            else
            {
                Debug.LogWarning("Playerがシーン内に見つかりません");
            }
        }
    }
    private void Awake()
    {
        _clearSE = GetComponent<AudioSource>();
        // シングルトン初期化
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
        // シーン内の全EnemyBaseを収集
        _enemies = FindObjectsOfType<EnemyBase>().ToList();

        
    }

    private void Update()
    {
        if (_hasCleared)
            return;

        // 全ての敵の状態がGetchued（捕まった）かチェック
        bool allCaught = _enemies.All(e => e.enemyState == EnemyState.Getchued);
        if (allCaught)
        {
            _hasCleared = true;
            OnGameCleared();
        }
    }

    /// <summary>
    /// ゲームクリア時の処理
    /// </summary>
    private void OnGameCleared()
    {
        _clearSE.PlayOneShot(_clearSE.clip);
        // エモート実行とタイトル戻りのコルーチンを開始
        StartCoroutine(ClearSequence());
    }
    /// <summary>
    /// クリア後のシーケンス（エモート→タイトル戻り）
    /// </summary>
    private IEnumerator ClearSequence()
    {

        // プレイヤーのエモートを実行
        PlayPlayerEmote();

        // 指定秒数待機
        yield return new WaitForSeconds(_delayBeforeReturnToTitle);

        // タイトルシーンに戻る
        ReturnToTitle();
    }

    /// <summary>
    /// プレイヤーのエモートを実行
    /// </summary>
    private void PlayPlayerEmote()
    {
        Animator playerAnimator = _playerAnimator.GetComponent<Animator>();
        // アニメーションを再生
        playerAnimator.SetTrigger("Clear");

        // 例：パーティクルエフェクト、追加のSE再生など
        StartCoroutine(LoopPlayParticles());
    }

    /// <summary>
    /// タイトルシーンに戻る
    /// </summary>
    private void ReturnToTitle()
    {
        // フェードアウト効果などがある場合はここで実行

        // タイトルシーンをロード
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

            // 全て再生した後、再度ループまでの待機時間
            float waitTime = _loopInterval - (_interval * _particles.Length);
            if (waitTime > 0)
            {
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
