using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PowerUp powerUp;
        if (other.TryGetComponent(out powerUp))
        {
            powerUp.GrabPowerUp();
        }
    }
}
