using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{ 
    Score,
    SlowFast,
    SmallLarge
}

public class PowerUp : MonoBehaviour
{
    private const string POWER_UP_SPRITE_PATH = "Sprites/PowerUps/{0}_{1}";


    [SerializeField]
    private PowerUpType _powerUpType;
    private SpriteRenderer _renderer;

    private int _variation = 0;

    public void Init(PowerUpType powerUpType)
    {
        _powerUpType = powerUpType;
        _renderer = GetComponentInChildren<SpriteRenderer>();

        if (powerUpType == PowerUpType.Score) 
        {
            _variation = Random.Range(0, 3);
        }
        else if (powerUpType == PowerUpType.SmallLarge) 
        {
            _variation = Random.Range(0, 1);
        }
        else if (powerUpType == PowerUpType.SlowFast) 
        {
            _variation = Random.Range(0, 1);
        }

        _renderer.sprite = GetSprite(_powerUpType);
    }

    public void Update()
    {
        Transform objTransform = GetComponent<Transform>();
        objTransform.Translate(new Vector3(0, -0.3f * Time.deltaTime, 0));
    }

    public void GrabPowerUp()
    {
        ArkanoidEvent.OnPowerUpEvent?.Invoke(_powerUpType, _variation);
        Destroy(gameObject);
    }
    private Sprite GetSprite(PowerUpType powerUpType)
    {
        string path = string.Empty;

        path = string.Format(POWER_UP_SPRITE_PATH, powerUpType, _variation);

        if (string.IsNullOrEmpty(path)) 
        {
            return null;
        }

        return Resources.Load<Sprite>(path);
    }
}
