using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private string enemyType;
    
    private Animator anim;
    private Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 10 && enemyType == "jumper")
        {
            if(!InputManager.Instance.isItScore && !anim.GetBool("Jump"))
                anim.SetBool("Jump", true);
            else
                anim.SetBool("Jump", false);
        }

        if (Vector3.Distance(player.transform.position, transform.position) < 10 && enemyType == "runner")
        {
            anim.SetTrigger("RunJump");
        }
    }


}
