using System;
using UnityEngine;

public class TargetOrb : MonoBehaviour
{
    [Header("Settings")]
    public Color successColor = Color.green;
    public Color targetColor = Color.red;
    public Color idleColor = Color.blue;
    
    // private Color _originalColor;
    private Renderer _renderer;
    private State _state = State.Idle;

    public State CurrentState => _state;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer)
        {
            _renderer.material.color = idleColor;
        }
        else
        {
            Debug.LogError($"TargetOrb {gameObject.name} requires a Renderer component!");
        }
    }

    public void SetState(State state)
    {
        if (_state == State.Activated)
            return; // can't cange state anymore

        _state = state;

        if (!_renderer)
            return;

        switch (_state)
        {
            case State.Idle:
                _renderer.material.color = idleColor;
                break;
            case State.Targeted:
                _renderer.material.color = targetColor;
                break;
            case State.Activated:
                _renderer.material.color = successColor;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the pointer's tool tip entered this trigger
        if (other.CompareTag("ToolTip"))
        {
            Debug.Log($"Target {gameObject.name} should have changed color!");
            var pointerScript = other.GetComponentInParent<SurgicalPointer>();
            if (pointerScript)
            {
                pointerScript.OnTargetReached(this);
            }
        }
        else
        {
            Debug.Log($"Tag {other.tag} entered {gameObject.name} trigger, but it's not a ToolTip.");
        }
    }
}

public enum State
{
    Idle,
    Targeted,
    Activated
}
