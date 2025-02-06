using System;
using UnityEngine;

public class StarPlayer : MonoBehaviour
{

    public event Action OnCoinCollected;
    public float step = 0.05f;
    Rigidbody2D m_Rb;
    Collider2D m_Collider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<Collider2D>();
    }

    public void Handle_PlayerMove(Vector2 Dir, bool Performed)
    {
        transform.position += new Vector3 (Dir.x,Dir.y,0) * step;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Collided {collision.gameObject.name}");
        if(collision.tag == "Star")
        {
            collision.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        
    }

    
}
