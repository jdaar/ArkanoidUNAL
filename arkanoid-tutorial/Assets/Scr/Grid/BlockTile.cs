using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    Small,
    Big
}

public enum BlockColor
{ 
    Green,
    Blue,
    Orange,
    Red,
    Purple
}

public class BlockTile : MonoBehaviour
{
    private const string BLOCK_BIG_PATH = "Sprites/BlockTiles/Big/Big_{0}_{1}";

    [SerializeField]
    private BlockType _type = BlockType.Big;
    [SerializeField]
    private BlockColor _color = BlockColor.Blue;

    private SpriteRenderer _renderer;
    private Collider2D _collider;

    private int _id;
    private int _totalHits = 1;
    private int _currentHits = 0;

    [SerializeField]
    private int _score = 10;
    public int Score => _score;

    public void Init()
    { 
        _currentHits = 0;
        _totalHits = _type == BlockType.Big ? 2 : 1;

        _collider = GetComponent<Collider2D>();
        _collider.enabled = true;

        _renderer = GetComponentInChildren<SpriteRenderer>();
        _renderer.sprite = GetBlockSprite(_type, _color, 0);
    }

    public void SetData(int id, BlockColor color)
    {
        _id = id;
        _color = color;
    }

    static Sprite GetBlockSprite(BlockType type, BlockColor color, int state)
    {
        string path = string.Empty;
        if (type == BlockType.Big) 
        {
            path = string.Format(BLOCK_BIG_PATH, color, state);
        }

        if (string.IsNullOrEmpty(path)) 
        {
            return null;
        }

        return Resources.Load<Sprite>(path);
    }

    public void OnHitCollision(ContactPoint2D contactPoint)
    { 
        _currentHits++;
        if (_currentHits >= _totalHits)
        {
            _collider.enabled = false;
            gameObject.SetActive(false);
            ArkanoidEvent.OnBlockDestroyedEvent?.Invoke(_id);
        }
        else
        {
            _renderer.sprite = GetBlockSprite(_type, _color, _currentHits);
        }
    }
    private static string GetPowerUpPrefab()
    {
        return "Prefabs/PowerUp";
    }

    public void GeneratePowerUp()
    { 
        PowerUp powerUpPrefab = Resources.Load<PowerUp>(GetPowerUpPrefab());
        if (true)
        {
            Vector3 position = GetComponent<Transform>().position;
            if (Random.Range(0, 100) < 101)
            {
                PowerUp powerUp = Instantiate<PowerUp>(powerUpPrefab, position, Quaternion.identity);
                if (Random.Range(0, 2) == 0)
                    powerUp.Init(PowerUpType.Score);
                else if (Random.Range(0, 2) == 1)
                    powerUp.Init(PowerUpType.SmallLarge);
                else if (Random.Range(0, 2) == 2)
                    powerUp.Init(PowerUpType.SlowFast);
            }
        }
    }
}
