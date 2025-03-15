using UnityEngine;

public class TrailController : MonoBehaviour
{
    private TrailRenderer trail;
    private float defaultTrailTime;
    private float fadeSpeed = 2f; // Speed at which the trail fades out

    void Start()
    {
        trail = GetComponent<TrailRenderer>();
        defaultTrailTime = trail.time; // Store the original trail time
        trail.time = 0f; // Start with no trail
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space)) // Boosting
        {
            trail.time = defaultTrailTime; // Show trail
        }
        else if (trail.time > 0) // Smoothly reduce trail time when not boosting
        {
            trail.time -= Time.deltaTime * fadeSpeed;
            if (trail.time < 0) trail.time = 0; // Prevent negative values
        }
    }
}