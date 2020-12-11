using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSenser : MonoBehaviour
{
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<Player>();    
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Ground")
        {
            player.isGround = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Ground")
        {
            player.isGround = false;
        }
    }
}
