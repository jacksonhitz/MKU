using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    public Vector3 movementRange = new Vector3(0.2f, 0.2f, 0); // Max movement range
    public float movementSpeed = 1f; // Base speed of the floating effect
    public float minDelay = 0.5f;
    public float maxDelay = 2f;

    private Transform[] children;
    private Dictionary<Transform, (Vector3 range, float speed, float offset)> movementData;

    void Start()
    {
        int childCount = transform.childCount;
        children = new Transform[childCount];
        movementData = new Dictionary<Transform, (Vector3, float, float)>();

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            children[i] = child;
            StartCoroutine(PlayAnimationWithDelay(child));

            // Assign unique movement parameters for each child
            Vector3 randomRange = new Vector3(
                Random.Range(0.1f, movementRange.x),
                Random.Range(0.1f, movementRange.y),
                0
            );
            float randomSpeed = Random.Range(0.5f, movementSpeed * 1.5f);
            float randomOffset = Random.Range(0f, Mathf.PI * 2); // Phase offset for variation

            movementData[child] = (randomRange, randomSpeed, randomOffset);
        }
    }

    IEnumerator PlayAnimationWithDelay(Transform child)
    {
        Animator animator = child.GetComponent<Animator>();
        if (animator != null)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
            animator.Play("grab");
        }
    }

    void Update()
    {
        foreach (Transform child in children)
        {
            if (child != null && movementData.ContainsKey(child))
            {
                var (range, speed, offset) = movementData[child];

                float xOffset = Mathf.Sin(Time.time * speed + offset) * range.x;
                float yOffset = Mathf.Cos(Time.time * speed + offset) * range.y;
                Vector3 targetPosition = child.position + new Vector3(xOffset, yOffset, 0);

                child.position = Vector3.Lerp(child.position, targetPosition, Time.deltaTime);
            }
        }
    }
}
