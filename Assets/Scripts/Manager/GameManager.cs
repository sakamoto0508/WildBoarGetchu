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
    [SerializeField] private GameObject _clearUIPanel;
    [SerializeField] private AudioSource _clearSE;

    private List<EnemyBase> _enemies;
    private bool _hasCleared = false;

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

        // クリア用UIを非表示
        if (_clearUIPanel != null)
            _clearUIPanel.SetActive(false);
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
        Debug.Log("全ての敵を捕まえた！ゲームクリア！");
        _clearSE.PlayOneShot(_clearSE.clip);
        // クリアUIを表示
        if (_clearUIPanel != null)
            _clearUIPanel.SetActive(true);

        // 次のシーンへ遷移したい場合は以下を有効化
        // SceneManager.LoadScene("NextSceneName");
    }
}
