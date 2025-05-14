using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHitable
{
    [SerializeField] float health;
    float maxHealth = 100;

    void Start()
    {
        health = maxHealth;
    }

    public void Heal(float heal)
    {
        if (StateManager.State != StateManager.GameState.DEAD)
            health = Mathf.Min(health + heal, maxHealth);
    }

    public void Hit(float dmg)
    {
        if (StateManager.State != StateManager.GameState.DEAD)
            health -= dmg;
        if (health <= 0)
            StartCoroutine(Dead());

        Debug.Log("player hit");
    }

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(3f);
        StateManager.LoadState(StateManager.GameState.DEAD);
    }
}
