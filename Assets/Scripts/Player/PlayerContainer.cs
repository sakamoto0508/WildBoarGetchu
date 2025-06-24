using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �v���C���[�Ɋւ���N���X���Ǘ�����N���X
/// </summary>
public class PlayerContainer : MonoBehaviour
{
    public MyState state;

    IPlayerMover[] _playerMovers;
    bool _isPlayerMover = true;


    public bool IsPlayerMover { set => _isPlayerMover = value; }
    /// <summary>
    /// Awake���\�b�h
    /// �������������s��
    /// </summary>
    void Start()
    {
        _playerMovers = GetComponents<IPlayerMover>();
    }
    /// <summary>
    /// �v���C���[�̈ړ����������s����
    /// </summary>
    void Update()
    {
        if (!_isPlayerMover) return;
        // �v���C���[�̈ړ����������s
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
