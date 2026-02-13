using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [Header("Setup")]
    public List<TargetOrb> targetOrbs = new List<TargetOrb>();
    public LineRenderer lineRenderer;
    
    private bool _allTargetsCompleted;
    private int _targetsCompletedCount;

    private void Start()
    {
        if (targetOrbs.Count == 0)
        {
            Debug.LogError("TargetManager: No target orbs assigned!");
            return;
        }

        Debug.Log($"TargetManager initialized with {targetOrbs.Count} targets. Will guide to closest unactivated target.");
        
        if (!lineRenderer)
        {
            Debug.LogError("LineRenderer not assigned to TargetManager!");
        }
    }
    
    public void UpdateTargetColors(Vector3 toolTipPosition)
    {
        TargetOrb closestTarget = _getClosestUnactivatedTarget(toolTipPosition);
        foreach (var target in targetOrbs)
        {
            // If activated stops in TargetOrb already
            if (target == closestTarget)
            {
                target.SetState(State.Targeted);
            }
            else
            {
                target.SetState(State.Idle);
            }
        }
    }

    public void UpdateLineToCurrentTarget(Vector3 toolTipPosition)
    {
        if (_failAndExitChecks()) return;
        // Find closest unactivated target
        TargetOrb closestTarget = _getClosestUnactivatedTarget(toolTipPosition);
        
        if (closestTarget)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, toolTipPosition);
            lineRenderer.SetPosition(1, closestTarget.transform.position);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private bool _failAndExitChecks()
    {
        if (_allTargetsCompleted || !lineRenderer || targetOrbs.Count == 0)
        {
            if (lineRenderer)
            {
                lineRenderer.enabled = false;
            }

            return true;
        }

        return false;
    }

    private TargetOrb _getClosestUnactivatedTarget(Vector3 position)
    {
        TargetOrb closest = null;
        float closestDistance = float.MaxValue;
        
        foreach (var target in targetOrbs)
        {
            if (target.CurrentState != State.Activated)
            {
                float distance = Vector3.Distance(position, target.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = target;
                }
            }
        }
        
        return closest;
    }

    public void OnTargetReached(TargetOrb orb)
    {
        if (_allTargetsCompleted)
            return;

        // Activate the target if not already activated
        if (orb.CurrentState != State.Activated)
        {
            orb.SetState(State.Activated);
            _targetsCompletedCount++;

            // Check if all targets completed
            if (_targetsCompletedCount >= targetOrbs.Count)
            {
                _allTargetsCompleted = true;
                if (lineRenderer)
                {
                    lineRenderer.enabled = false;
                }
                Debug.Log("All targets completed! Surgical procedure finished.");
            }
            else
            {
                Debug.Log($"Target completed! Targets remaining: {targetOrbs.Count - _targetsCompletedCount}");
            }
        }
    }
    
    public float GetDistanceToClosestTarget(Vector3 toolTipPosition)
    {
        TargetOrb closest = _getClosestUnactivatedTarget(toolTipPosition);
    
        if (closest)
        {
            return Vector3.Distance(toolTipPosition, closest.transform.position);
        }
    
        return -1f; // All targets activated
    }
}

