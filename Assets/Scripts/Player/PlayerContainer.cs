using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// プレイヤーに関するクラスを管理するクラス
/// </summary>
public class PlayerContainer : MonoBehaviour
{
    public MyState state;

    IPlayerMover[] _playerMovers;
    bool _isPlayerMover = true;


    public bool IsPlayerMover { set => _isPlayerMover = value; }
    /// <summary>
    /// Awakeメソッド
    /// 初期化処理を行う
    /// </summary>
    void Start()
    {
        _playerMovers = GetComponents<IPlayerMover>();
    }
    /// <summary>
    /// プレイヤーの移動処理を実行する
    /// </summary>
    void Update()
    {
        if (!_isPlayerMover) return;
        // プレイヤーの移動処理を実行
        foreach (var playerMover in _playerMovers)
        {
            playerMover.PlayerUpdate();
        }
    }
    public enum MyState
    {
        Normal,
        Stan,
        Attack,
        Victory
    };
}
