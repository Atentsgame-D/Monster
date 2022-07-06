using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public int damage = 5;
    public float moveSpeed = 5.0f;
    float inputSide = 0.0f;

    PlayerInputActions actions;
    Vector3 inputDir = Vector3.zero;

    Rigidbody rigid;

    private void Awake()
    {
        actions = new();
        rigid = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        actions.Player.Enable();
        actions.Player.Move.performed += OnMoveInput;
        actions.Player.Move.canceled += OnMoveInput;
        actions.Player.SideMove.performed += OnSideMoveInput;
        actions.Player.SideMove.canceled += OnSideMoveInput;
    }


    private void OnDisable()
    {
        actions.Player.Move.canceled -= OnMoveInput;
        actions.Player.Move.performed -= OnMoveInput;
        actions.Player.SideMove.canceled -= OnSideMoveInput;
        actions.Player.SideMove.performed -= OnSideMoveInput;
        actions.Player.Disable();
    }

    private void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + moveSpeed * Time.fixedDeltaTime * (inputDir.y * transform.forward + inputSide * transform.right));
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        inputDir = context.ReadValue<Vector2>();
    }
    private void OnSideMoveInput(InputAction.CallbackContext context)
    {
        inputSide = context.ReadValue<float>();
    }

}
