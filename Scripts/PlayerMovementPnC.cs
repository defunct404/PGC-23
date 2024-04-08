using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerMovementPnC : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject AS;

    private Rigidbody2D rb;
    private AudioSource aus;

    private UnityEngine.AI.NavMeshAgent agent;

    public bool showPath;
    public bool showAhead;

    private float horizontal = 0;
    private float vertical = 0;
    private float prevX = 0;
    private float prevY = 0;

    private static int animIsWalk = Animator.StringToHash("isWalk");
    private static int animSpeedX = Animator.StringToHash("speedX");
    private static int animSpeedY = Animator.StringToHash("speedY");

    private Vector3 lastPosition;

    void Start()
    {
        aus = AS.GetComponent<AudioSource>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        lastPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = 0;
            agent.destination = target;
        }
        Vector3 currentPosition = transform.position;

        // Вычисляем вектор направления движения
        Vector3 movementDirection = currentPosition - lastPosition;
        float h;
        float v;

        if ((movementDirection.x) > 0.0f)
            h = 1.0f;
        else if ((movementDirection.x) < 0.0f)
            h = -1.0f;
        else h = 0.0f;

        if ((movementDirection.y) > 0.0f)
            v = 1.0f;
        else if ((movementDirection.y) < 0.0f)
            v = -1.0f;
        else v = 0.0f;

        // Обновляем lastPosition
        lastPosition = currentPosition;

        //if (agent.remainingDistance <= 0.3f)
        //{
        //    agent.ResetPath();
        //    h = 0.0f;
        //    v = 0.0f;
        //}

        horizontal = h;
        vertical = v;

        if (prevX == horizontal && prevY == vertical) return;
        if ((horizontal != 0) || (vertical != 0)) aus.enabled = true;
        prevX = horizontal;
        prevY = vertical;

        animator.SetBool(animIsWalk, horizontal != 0 || vertical != 0);
        if (horizontal > 0) spriteRenderer.flipX = false;
        else if (horizontal < 0) spriteRenderer.flipX = true;
        animator.SetInteger(animSpeedX, horizontal != 0 ? 1 : 0);

        if (vertical > 0) animator.SetInteger(animSpeedY, 1);
        else if (vertical < 0) animator.SetInteger(animSpeedY, -1);
        else animator.SetInteger(animSpeedY, 0);

        if ((horizontal == 0) && (vertical == 0)) aus.enabled = false;
    }

    //private void OnDrawGizmos()
    //{
    // Navigate.DrawGizmos(agent, showPath, showAhead);
    //}
}
