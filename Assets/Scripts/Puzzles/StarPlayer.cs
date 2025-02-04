using System;
using UnityEngine;

public class StarPlayer : MonoBehaviour
{

    public event Action OnCoinCollected;

    Rigidbody2D m_Rb;
    Collider2D m_Collider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<Collider2D>();    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Star")
        {
            collision.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        
    }

    
}
