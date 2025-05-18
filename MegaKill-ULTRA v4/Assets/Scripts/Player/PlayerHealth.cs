using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHitable
{
    [SerializeField] float health;
    float maxHealth = 100;
    UEye uEye;

    void Awake()
    {
        uEye = FindObjectOfType<UEye>();
    }

    void Start()
    {
        health = maxHealth;
    }

    public void Heal(float heal)
    {
        if (StateManager.State != StateManager.GameState.DEAD)
        {
            health = Mathf.Min(health + heal, maxHealth);
            uEye.UpdateHealth(health);
        }
    }

    public void Hit(float dmg)
    {
        if (StateManager.State != StateManager.GameState.DEAD)
        {
           // health -= dmg;
            uEye.UpdateHealth(health);

        }
        if (health <= 0)
        {
            StateManager.LoadState(StateManager.GameState.DEAD);
            StartCoroutine(Return());
        }
    }

    IEnumerator Return()
    {
        yield return new WaitForSeconds(3f);
        StateManager.LoadState(StateManager.GameState.TITLE);
        Debug.Log("title called");
    }
}
