using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5;

    [SerializeField]
    private float _movementLimit = 7;

    private Vector3 _targetPosition;

    private Camera _cam;
    private Camera Camera
    {
        get 
        { 
            if (_cam == null)
            {
                _cam = Camera.main;
            }
            return _cam;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("collision");
        PowerUp powerUp;
        if (other.TryGetComponent(out powerUp))
        {
            powerUp.GrabPowerUp();
        }
    }

    void Update()
    {
        _targetPosition.x = Camera.ScreenToWorldPoint(Input.mousePosition).x;
        _targetPosition.x = Mathf.Clamp(_targetPosition.x, -_movementLimit, _movementLimit);

        _targetPosition.y = this.transform.position.y;

        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _speed);
    }
}
