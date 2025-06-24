using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _teleportPoint;

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
        _clearSE.PlayOneShot(_clearSE.clip);
        //テレポート実行
        PlayerTeleport();
        // エモート実行とタイトル戻りのコルーチンを開始
        //StartCoroutine(ClearSequence());
    }
    private void PlayerTeleport()
    {
        // PlayerMoverを無効化
        var playerMover = _player.GetComponent<PlayerMover>();
        if (playerMover != null)
        {
            playerMover.enabled = false;
        }

        // Rigidbodyの速度をリセット
        Rigidbody playerRb = _player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }
        Debug.Log($"テレポート前の位置: {_player.transform.position}");
        Debug.Log($"テレポート先の位置: {_teleportPoint.transform.position}");
        // テレポート実行
        _player.transform.position = _teleportPoint.transform.position;
        Debug.Log($"テレポート後の位置: {_player.transform.position}");
        // 少し待ってからPlayerMoverを再有効化（必要に応じて）
        StartCoroutine(ReenablePlayerMover(playerMover));
    }
    private IEnumerator ReenablePlayerMover(PlayerMover playerMover)
    {
        yield return new WaitForSeconds(10f); // 1秒待つ
        if (playerMover != null)
        {
            playerMover.enabled = true;
        }
    }

}
