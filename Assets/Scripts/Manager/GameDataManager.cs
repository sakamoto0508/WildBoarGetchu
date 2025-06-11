using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    [SerializeField] int _score;
    int _defaultHp;
    int _defaultRemaing;
    int _progress;//進行度
    GameObject _player;
    public GameObject Player { set => _player = value; }
    [SerializeField] private int _hp = 3;
    [SerializeField] private int _remaing = 3;//残機
    [SerializeField] private AudioClip _damageSE;//ダメージSE
    [SerializeField] private AudioClip _DeadSE; //死亡SE
    public int Score { get => _score; }
    public int Reaming { get => _remaing; }
    public int HP { get => _hp; }
    public event Action<int> OnScoreChanged;//スコアが変化したときに呼び出されるイベント
    public static GameDataManager Instance;

    // Start is called before the first frame update
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _defaultHp = _hp;
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            return;
        }
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Init()
    {
        OnScoreChanged -= OnScoreChanged;
        _score = 0;
        _hp = _defaultHp;
        _remaing = 3;
       
    }
    public void Damage(int damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            PlayerDead();
            return;
        }
        SoundManager.Instance.PlaySE(_damageSE);
    }
    public void PlayerDead()
    {
        _remaing--;
        if (_remaing <= 0)
        {
            SceneManager.LoadScene("GameOver");
            return;
        }
        PlayerRestart();
        SoundManager.Instance.PlaySE(_DeadSE);
    }
    void PlayerRestart()
    {
        _hp = _defaultHp;
        
    }
}
