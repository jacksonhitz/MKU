using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHands : Enemy
{
    Vector3 movementRange = new Vector3(0.2f, 0.2f, 0.2f);
    float spd = 1f; 
    float minDelay = 0f;
    float maxDelay = 0.5f;

    Transform[] children;
    Dictionary<Transform, (Vector3 range, float speed)> movementData;

    protected override void Pathfind(){}
    protected override void DropItem(){}
    protected override void CallAttack(){}
    protected override void Start()
    {
        int childCount = transform.childCount;
        children = new Transform[childCount];
        movementData = new Dictionary<Transform, (Vector3, float)>();

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            children[i] = child;
            StartCoroutine(PlayAnim(child));

            Vector3 randomRange = new Vector3(
                Random.Range(0.1f, movementRange.x),
                Random.Range(0.1f, movementRange.y),
                0
            );
            float randomSpd = Random.Range(0.5f, spd * 1.5f);

            movementData[child] = (randomRange, randomSpd);
        }
    }
    protected override void Hit(bool lethal)
    {
        player.rooted = false;
        Dead();
    }

    IEnumerator PlayAnim(Transform child)
    {
        Animator animator = child.GetComponent<Animator>();
        float delay = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);
        animator.Play("grab");
    }
}
