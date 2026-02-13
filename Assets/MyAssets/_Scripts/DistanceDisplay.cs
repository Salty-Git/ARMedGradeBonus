using UnityEngine;
using TMPro;

public class DistanceDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform textAnchorLeft;
    [SerializeField] private Transform textAnchorRight;
    [SerializeField] private Transform trackerTransform;
    [SerializeField] private Transform tipTransform;
    [SerializeField] private TargetManager targetManager;
    [SerializeField] private TextMeshProUGUI distanceText;

    [Header("Settings")]
    [SerializeField] private bool lookAtCamera = true;
    [SerializeField] private string unitLabel = "mm";
    [SerializeField] private float metersToUnits = 1000f; // Convert Unity meters to mm

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        
        if (!distanceText)
        {
            Debug.LogError("DistanceDisplay: TextMeshProUGUI not assigned!");
        }
        if (!trackerTransform)
        {
            Debug.LogError("DistanceDisplay: ToolTip Transform not assigned!");
        }
        if (!targetManager)
        {
            Debug.LogError("DistanceDisplay: TargetManager not assigned!");
        }
    }

    private void Update()
    {
        if (!trackerTransform || !tipTransform || !targetManager || !distanceText)
            return;

        // Determine side based on tool's right direction
        // If tool points right, text goes to positive Z (right anchor)
        // If tool points left, text goes to negative Z (left anchor)
        float rightDot = Vector3.Dot(trackerTransform.right, Vector3.right);
        Transform activeAnchor = rightDot >= 0 ? textAnchorLeft : textAnchorRight;
        
        // Position with dynamic offset
        transform.position = activeAnchor.position;

        // Make text face camera
        if (lookAtCamera && _mainCamera)
        {
            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                _mainCamera.transform.rotation * Vector3.up);
        }

        // Calculate and display distance
        float distance = targetManager.GetDistanceToClosestTarget(tipTransform.position);
        
        if (distance >= 0)
        {
            float displayDistance = distance * metersToUnits;
            distanceText.text = $"{displayDistance:F1} {unitLabel}";
    
            // Color based on distance
            distanceText.color = displayDistance < 10f ? Color.green :
                displayDistance < 30f ? Color.yellow : Color.white;
        }
        else
        {
            distanceText.text = "Complete!";
        }
    }
}