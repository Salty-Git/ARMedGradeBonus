using UnityEngine;

public class SurgicalPointer : MonoBehaviour
{
    [Header("Setup")]
    public Transform toolTip;
    
    [Header("References")]
    public TargetManager targetManager;

    private void Update()
    {
        if (targetManager && toolTip)
        {
            targetManager.UpdateLineToCurrentTarget(toolTip.position);
            targetManager.UpdateTargetColors(toolTip.position);
        }
    }

    public void OnTargetReached(TargetOrb orb)
    {
        if (targetManager)
        {
            targetManager.OnTargetReached(orb);
        }
    }
}

