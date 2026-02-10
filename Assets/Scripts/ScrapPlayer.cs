using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapPlayer : MonoBehaviour
{
    private CharacterController _controller;
    private Animator _animator;

    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float runBonusSpeed = 3f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private string damageAnimationName = "TakeDamage";
    [SerializeField] private DamageOverlay damageOverlay; // Link this in Inspector

    private float currentSpeed;
    private bool isRunning = false;

    private StaminaBar staminaBar;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        staminaBar = Object.FindFirstObjectByType<StaminaBar>(); // Assumes there's one in the scene
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = direction.magnitude;

        // Normalize direction for consistent movement
        direction = direction.normalized;

        // Handle running
        if (inputMagnitude > 0.1f && Input.GetKey(KeyCode.LeftShift) && staminaBar.CanRun())
        {
            isRunning = true;
            currentSpeed = walkSpeed + runBonusSpeed;
            staminaBar.UseStamina(Time.deltaTime);
        }
        else
        {
            isRunning = false;
            currentSpeed = walkSpeed;
        }

        // Convert movement to world space
        Vector3 move = transform.TransformDirection(direction) * currentSpeed;

        // Apply gravity only when not grounded
        if (!_controller.isGrounded)
        {
            move.y -= gravity * Time.deltaTime;
        }

        _controller.Move(move * Time.deltaTime);

        // Animation
        _animator.SetFloat("Speed", inputMagnitude * (isRunning ? 2f : 1f));
        _animator.SetBool("IsRunning", isRunning);
    }

    public void TakeDamage()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("TakeDamage");
        }

        if (damageOverlay != null)
        {
            damageOverlay.ShowDamageEffect();
        }
    }
}
