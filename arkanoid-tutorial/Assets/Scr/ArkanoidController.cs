using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArkanoidController : MonoBehaviour
{
    private const string BALL_PREFAB_PATH = "Prefabs/Ball";
    private readonly Vector2 BALL_INIT_POSITION = new Vector2(0, -0.86f);
    private int[] POWER_UP_SCORE_POINTS = {50, 100, 250, 500};

    private Ball _ballPrefab = null;
    private List<Ball> _balls = new List<Ball>();
    private Ball _currentBall;
    private int _totalScore = 0;

    [SerializeField]
    private GridController _gridController;

    [Space(20)]
    [SerializeField]
    private List<LevelData> _levels = new List<LevelData>();

    [SerializeField]
    private Paddle _paddle;

    private int _currentLevel = 0;

    private void Start()
    {
        ArkanoidEvent.OnBallReachDeadZoneEvent += OnBallReachDeadZone;
        ArkanoidEvent.OnBlockDestroyedEvent += OnBlockDestroyed;
        ArkanoidEvent.OnPowerUpEvent += OnPowerUpEvent;
    }

    private void OnDestroy()
    {
        ArkanoidEvent.OnBallReachDeadZoneEvent -= OnBallReachDeadZone;
        ArkanoidEvent.OnBlockDestroyedEvent -= OnBlockDestroyed;
        ArkanoidEvent.OnPowerUpEvent -= OnPowerUpEvent;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            InitGame();
        }
    }

    private void InitGame()
    { 
        _currentLevel = 0;
        _totalScore = 0;
        _gridController.BuildGrid(_levels[_currentLevel]);
        ClearBalls();
        SetInitialBall();
        removeAllPowerUps();
        ArkanoidEvent.OnGameStartEvent?.Invoke();
        ArkanoidEvent.OnScoreUpdatedEvent?.Invoke(0, _totalScore);
    }

    private void SetInitialBall()
    {
        Ball ball = CreateBallAt(BALL_INIT_POSITION);
        _currentBall = ball;
        ball.Init();
        _balls.Add(ball);
    }

    private void ClearBalls()
    {
        for (int i = _balls.Count - 1; i >= 0; i--)
        {
            _balls[i].gameObject.SetActive(false);
            Destroy(_balls[i]);
        }
    
        _balls.Clear();
    }
    private Ball CreateBallAt(Vector2 position)
    {
        if (_ballPrefab == null)
        {
            _ballPrefab = Resources.Load<Ball>(BALL_PREFAB_PATH);
        }

        return Instantiate(_ballPrefab, position, Quaternion.identity);
    }

    private void OnBallReachDeadZone(Ball ball)
    {
        ball.Hide();
        _balls.Remove(ball);
        Destroy(ball.gameObject);

        CheckGameOver();
    }

    private void CheckGameOver()
    {
        //Game over
        if (_balls.Count == 0)
        {
            ClearBalls();
            ArkanoidEvent.OnGameOverEvent?.Invoke();
        }
    }

    private void OnBlockDestroyed(int blockId)
    {
        BlockTile blockDestroyed = _gridController.GetBlockBy(blockId);
        if (blockDestroyed != null)
        {
            _totalScore += blockDestroyed.Score;
            blockDestroyed.GeneratePowerUp();
            ArkanoidEvent.OnScoreUpdatedEvent?.Invoke(blockDestroyed.Score, _totalScore);
        }
        if (_gridController.GetBlocksActive() == 0)
        {
            _currentLevel++;
            ArkanoidEvent.OnLevelUpdatedEvent?.Invoke(_currentLevel);
            if (_currentLevel >= _levels.Count)
            {
                ClearBalls();
            }
            else
            {
                ClearBalls();
                SetInitialBall();
                _gridController.BuildGrid(_levels[_currentLevel]);
            }

        }
    }

    private void OnPowerUpEvent(PowerUpType powerUpType, int _variation)
    { 
        if (powerUpType == PowerUpType.Score)
        {
            _totalScore += POWER_UP_SCORE_POINTS[_variation];
            ArkanoidEvent.OnScoreUpdatedEvent?.Invoke(0, _totalScore);
        }
        else if (powerUpType == PowerUpType.SmallLarge)
        {
            _paddle.transform.localScale += new Vector3(0.5f, 0, 0);
            StartCoroutine(removeScale());
        }
        else if (powerUpType == PowerUpType.SlowFast)
        {
            if (_variation == 0)
                _currentBall.SlowDown();
            if (_variation == 1)
                _currentBall.Accelerate();
        }
    }

    private void removeAllPowerUps()
    {
        PowerUp[] powerUp = FindObjectsOfType<PowerUp>();
        for (int i = powerUp.Length - 1; i >= 0; i--)
        {
            powerUp[i].gameObject.SetActive(false);
            Destroy(powerUp[i].gameObject);
        }
    }

    private IEnumerator removeScale()
    {
        yield return new WaitForSeconds(5);
        _paddle.transform.localScale -= new Vector3(0.5f, 0, 0);
    }
}
