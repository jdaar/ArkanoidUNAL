using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    private const float BALL_VELOCITY_MIN_AXIS_VALUE = 0.5f;

    private bool SlowedDown = false;
    private bool Accelerated = false;
    private bool Modified = false;

    [SerializeField]
    private float _minSpeed = 4;
    [SerializeField]
    private float _maxSpeed = 7;
    [SerializeField]
    private float _initSpeed = 5;

    private Rigidbody2D _rb;
    private Collider2D _collider;

    void FixedUpdate()
    {
        CheckVelocity();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PowerUp powerUp;
        if (other.TryGetComponent(out powerUp))
        {
            powerUp.GrabPowerUp();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        BlockTile blockTileHit;
        if (other.collider.TryGetComponent(out blockTileHit))
        {
            ContactPoint2D contactPoint = other.contacts[0];
            blockTileHit.OnHitCollision(contactPoint);
        }
    }
    public void Init()
    { 
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        _collider.enabled = true;
        _rb.velocity = Random.insideUnitCircle.normalized * _initSpeed;
    }

    private void CheckVelocity()
    { 
        Vector2 velocity = _rb.velocity;
        float currentSpeed = velocity.magnitude;

        if (currentSpeed < _minSpeed && SlowedDown == false && Accelerated == false)
        {
            velocity = velocity.normalized * _minSpeed;
        }
        else if (currentSpeed > _maxSpeed && Accelerated == false && SlowedDown == false)
        {
            velocity = velocity.normalized * _maxSpeed;
        }

        if (Mathf.Abs(velocity.x) < BALL_VELOCITY_MIN_AXIS_VALUE)
        {
            float sign = velocity.x == 0 ? Mathf.Sign(-transform.position.x) : Mathf.Sign(velocity.x);
            velocity.x += sign * BALL_VELOCITY_MIN_AXIS_VALUE * Time.deltaTime;
        }
        else if (Mathf.Abs(velocity.y) < BALL_VELOCITY_MIN_AXIS_VALUE)
        {
            float sign = velocity.y == 0 ? Mathf.Sign(-transform.position.y) : Mathf.Sign(velocity.y);
            velocity.y += sign * BALL_VELOCITY_MIN_AXIS_VALUE * Time.deltaTime;
        }

        if (SlowedDown && Modified == false)
        {
            velocity -= new Vector2(5, 5);
            Modified = true;
        }
        if (Accelerated && Modified == false)
        {
            velocity += new Vector2(5, 5);
            Modified = false;
        }
        else
            _rb.velocity = velocity;
    }
    public void Hide()
    {
        _collider.enabled = false;
        gameObject.SetActive(false);
    }

    public void SlowDown()
    {
        SlowedDown = true;
        Modified = false;
        StartCoroutine(cancelSlowDown());
    }

    private IEnumerator cancelSlowDown()
    {
        yield return new WaitForSeconds(5);
        SlowedDown = false;
    }

    public void Accelerate()
    {
        Accelerated = true;
        Modified = false;
        StartCoroutine(cancelAccelerate());
    }
    private IEnumerator cancelAccelerate()
    {
        yield return new WaitForSeconds(5);
        Accelerated = false;
    }
}
