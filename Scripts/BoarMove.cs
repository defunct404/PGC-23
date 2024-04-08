using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class BoarMove : MonoBehaviour
{
    public float speed;
    private float waitTime;           //время отдыха между передвижениями
    public float startWaitTime;
    bool wait = false;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private UnityEngine.AI.NavMeshAgent agent;
    private Rigidbody2D rb;
    private static readonly int animIsWalk = Animator.StringToHash("isWalk");
    private static readonly int animSpeedX = Animator.StringToHash("speedX");
    private static readonly int animSpeedY = Animator.StringToHash("speedY");
    private float horizontal = 0;
    private float vertical = 0;
    private float prevX = 0;
    private float prevY = 0;

    private Vector3 lastPosition;
    private Vector3 moveTarget;

    private float nActTime = 0.0f;
    private float period = 5000.0f;

    private void GetComps()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        waitTime = startWaitTime;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        lastPosition = transform.position;
        moveTarget = transform.position;
    }

    void Start()
    {
        GetComps();
    }

    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            Vector3 currentPosition = transform.position;

            Vector3 movementDirection = currentPosition - lastPosition;
            if (Input.GetKeyUp("r"))
            {
                GetComps();
            }
            if (wait)
            {
                waitTime -= Time.deltaTime;
                if (waitTime < 0)
                {
                    horizontal = UnityEngine.Random.Range(-10f, 10f);
                    vertical = UnityEngine.Random.Range(-10f, 10f);
                    wait = false;
                    moveTarget = new Vector3(lastPosition.x + horizontal, lastPosition.y + vertical, GetComponent<Transform>().position.z);
                }
            }
            else
            {
                agent.SetDestination(moveTarget);
                /*if (Time.time > nActTime)
                {
                    nActTime += period;
                    wait = true;
                    waitTime = startWaitTime;
                }
                else */
                if (agent.remainingDistance <= 0.3f)
                {
                    //nActTime = 0.0f;
                    agent.ResetPath();
                    wait = true;
                    waitTime = startWaitTime;
                }
            }
            int h;
            int v;

            if ((movementDirection.x) > 0.01)
                h = 1;
            else if ((movementDirection.x) < -0.01)
                h = -1;
            else h = 0;

            if ((movementDirection.y) > 0.01)
                v = 1;
            else if ((movementDirection.y) < -0.01)
                v = -1;
            else v = 0;

            // Обновляем lastPosition
            lastPosition = currentPosition;

            horizontal = h;
            vertical = v;

            if (prevX == horizontal && prevY == vertical) return;
            prevX = horizontal;
            prevY = vertical;

            animator.SetBool(animIsWalk, horizontal != 0 || vertical != 0);
            if (horizontal > 0) spriteRenderer.flipX = false;
            else if (horizontal < 0) spriteRenderer.flipX = true;
            animator.SetInteger(animSpeedX, horizontal != 0 ? 1 : 0);

            if (vertical > 0) animator.SetInteger(animSpeedY, 1);
            else if (vertical < 0) animator.SetInteger(animSpeedY, -1);
            else animator.SetInteger(animSpeedY, 0);
        }
        else
        {
            if (agent.hasPath) agent.ResetPath();
            
        }
    }
}