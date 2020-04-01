using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public float speed;
    public Vector3 change;
    public bool attackInput;
    public PlayerState currentState;

    public enum PlayerState
    {
        walk,
        attack,
        interact
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        currentState = PlayerState.walk;
        speed = 5f;
        attackInput = false;
    }

    public void FixedUpdate()
    {
        if (attackInput && currentState != PlayerState.attack)
        {
            StartCoroutine(AttackCo());
        }
        else if (currentState == PlayerState.walk)
        {
            UpdateAnimationAndMove();
        }

    }

    private IEnumerator AttackCo()
    {
        //animator.SetBool("attacking", true);
        //currentState = PlayerState.attack;
        //yield return null;
        //animator.SetBool("attacking", false);
        yield return new WaitForSeconds(.33f);
        //currentState = PlayerState.walk;

    }

    public void UpdateAnimationAndMove()
    {
        if (change != Vector3.zero)
        {
            MoveCharacter();
            ServerSend.AnimatorWalk(id, change);
            ServerSend.AnimatorIsWalking(id, true);
        }
        else
        {
            ServerSend.AnimatorIsWalking(id, false);
        }
    }

    void MoveCharacter()
    {
        change.Normalize();
        transform.position += change * speed * Time.deltaTime;
        ServerSend.PlayerPosition(this);
    }
}
