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
    public int room;
    public bool changedRoom;

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
        room = 1;
        changedRoom = false;

    }

    public void FixedUpdate()
    {
        if(transform.rotation.z != 0)
        {
            //transform.position = lastOne;
            //Debug.Log(transform.rotation.z);
            //transform.rotation = Quaternion.identity;
            //Debug.Log(transform.rotation.z);
            //change.x *= -1;
            //change.y *= -1;
            //ServerSend.PlayerPosition(this);
            //ServerSend.AnimatorIsWalking(id, false);
        }
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

        if (change.x != Vector3.zero.x || change.y != Vector3.zero.y)
        {
            if(changedRoom)
            {
                transform.position += change;
                ServerSend.PlayerPosition(this);
                changedRoom = false;
                return;
            }
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
        transform.position = transform.position += change * speed * Time.deltaTime;
        

        ServerSend.PlayerPosition(this);
    }


}


