using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Game : Singleton<Game>
{
    [SerializeField] private GameObject _originalEnemy;
    [SerializeField] private HPBar _healthBar;
    [SerializeField] private Transform _spawnPlayer;
    [SerializeField] private Player _player;
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private TextMeshProUGUI _scoreUI;
    [SerializeField] private TextMeshProUGUI _vagueUI;
    [SerializeField] private TextMeshProUGUI _LevelStrenghtUI;
    [SerializeField] private TextMeshProUGUI _LevelMaxHealthUI;
    [SerializeField] private TextMeshProUGUI _LevelRollUI;

    [SerializeField] private GameObject _gameOver;
    [SerializeField] private GameObject _hud;
    [SerializeField] private TextMeshProUGUI _scoreMaxUI;

    private int _scoreMax;
    private int _score;
    private int _vague;
    private bool _vagueFinish;
    private bool _vagueInProgress;
    private int _vagueNbSpawn;
    //private string _playerName;

    public int _playerStrenght;
    public int _playerStrenghtLevel;
    public int _playerMaxHealth;
    public int _playerMaxHealthLevel;
    public bool _playerRoll;
    //private int _playerSpeed;
    //private int _playerJump;
    //private int _playerExplosiveBullet;

    private int _enemyStrenght;
    private int _enemyMaxHealth;
    private float _enemySpeed;
    private float _enemySpeedLimite;
    private List<GameObject> _enemies;


    private float _spawnCoolDown;

    private void Start()
    {
        Setup();
    }

    public void Setup()
    {
        _player.gameObject.transform.position = _spawnPlayer.position;
        _player.gameObject.transform.rotation = _spawnPlayer.rotation;

        _score = 0;
        _vague = 1;
        _vagueFinish = false;
        _vagueInProgress = false;
        _vagueNbSpawn = 2;
        _vagueUI.text = "Vague : " + _vague.ToString();
        _scoreUI.text = "Score : " + _score.ToString();
        _LevelRollUI.text = "Locked";
        _LevelMaxHealthUI.text = "Level : " + _playerMaxHealthLevel.ToString();
        _LevelStrenghtUI.text = "Level : " + _playerStrenghtLevel.ToString();
        _spawnCoolDown = 1f;

        _playerStrenght = 10;
        _playerStrenghtLevel = 1;
        _player.SetStrength(_playerStrenght);
        _playerMaxHealth = 80;
        _playerMaxHealthLevel = 1;
        _player.SetMaxHealth(_playerMaxHealth);
        _player.SetHealth(_playerMaxHealth);
        _playerRoll = false;
        _player.SetRoll(_playerRoll);
        _healthBar.SetHPBarHealth(_playerMaxHealth);


        _enemyStrenght = 10;
        _enemyMaxHealth = 20;
        _enemySpeed = 1;
        _enemySpeedLimite = 3.2f;

        _gameOver.SetActive(false);
        _hud.SetActive(true);
    }

    private void Update()
    {
        if (!_player.isDead)
        {
            if (!_vagueFinish && !_vagueInProgress)
            {
                StartCoroutine(LaunchVague(_vagueNbSpawn));
            }
            else if (_vagueFinish)
            {
                _vague++;
                _vagueUI.text = "Vague : " + _vague.ToString();
                _vagueNbSpawn += 1 + Random.Range(0, 2) * (int)Math.Ceiling((double)_vague / 10);
                if (_vague == 2)
                {
                    _playerRoll = true;
                    _LevelRollUI.text = "Unlocked !";
                    _player.SetRoll(_playerRoll);
                }
                if (_vague % 10 == 0)
                {
                    _playerMaxHealth += 2;
                    _playerMaxHealthLevel += 1;
                    _LevelMaxHealthUI.text = "Level : " + _playerMaxHealthLevel.ToString();
                    _player.SetMaxHealth(_playerMaxHealth);
                    _healthBar.SetHPBarHealth(_player.GetHealth());
                }
                if (_vague % 5 == 0)
                {
                    _enemyStrenght += 2;
                    _enemySpeed += 0.2f;
                    if (_enemySpeed > _enemySpeedLimite)
                    {
                        _enemySpeed = _enemySpeedLimite;
                    }
                    _playerStrenght += 5;
                    _playerStrenghtLevel += 1;
                    _LevelStrenghtUI.text = "Level : " + _playerStrenghtLevel.ToString();
                    _player.SetStrength(_playerStrenght);

                }
                if (_vague % 2 == 0)
                {
                    _enemyMaxHealth += 5;
                }
                _vagueFinish = false;
                _vagueInProgress = false;
            }
        }
        else
        {
            if (_scoreMax < _score)
            {
                _scoreMax = _score;
            }
            //GAMEOVER
            _gameOver.SetActive(true);
            _hud.SetActive(false);
            foreach (var e in _enemies)
            {
                if (e != null)
                {
                    var eData = e.GetComponent<Enemy>();
                    eData.AutoKill();
                }
            }
            _scoreMaxUI.text = "Score max : " + _scoreMax.ToString();

        }
    }

    public void UpdateScore(int enemyMaxHealth, int enemyStrenght)
    {
        Game.Instance._score += enemyMaxHealth + (int)Math.Ceiling((double)enemyStrenght / 2);
        _scoreUI.text = "Score : " + _score.ToString();
    }

    IEnumerator SpawnEnemy(int nbEnemy)
    {
        int nbEnemySpawn = 0;
        _enemies = new List<GameObject>();
        while (nbEnemySpawn < nbEnemy)
        {
            int spawnPointIndex = Random.Range(0, 5);
            Transform spawnPoint = _spawnPoints[spawnPointIndex];
            var enemy = Instantiate(_originalEnemy, spawnPoint.position, spawnPoint.rotation);
            var enemyData = enemy.GetComponent<Enemy>();
            var enemySpeedData = enemy.GetComponent<NavMeshAgent>();
            enemyData.SetStrength(_enemyStrenght);
            enemyData.SetMaxHealth(_enemyMaxHealth);
            enemySpeedData.speed = _enemySpeed;
            enemy.SetActive(true);
            nbEnemySpawn++;
            _enemies.Add(enemy);
            yield return new WaitForSeconds(_spawnCoolDown);
        }
    }

    IEnumerator LaunchVague(int nbSpawn)
    {
        _vagueInProgress = true;
        StartCoroutine(SpawnEnemy(nbSpawn));

        while (_enemies.Where(x => x != null).Count() > 0)
        {
            yield return new WaitForSeconds(1f);
        }

        _vagueFinish = true;
    }
}
