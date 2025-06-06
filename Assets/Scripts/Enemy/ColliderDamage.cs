using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDamage : MonoBehaviour
{
    [SerializeField]
    public int attackDamage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trig");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player");
            Debug.Log(attackDamage);
            other.GetComponent<Player>().TakeDamage(attackDamage);
            gameObject.SetActive(false);
        }
    }
}
