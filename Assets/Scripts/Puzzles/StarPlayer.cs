using System;
using Unity.Netcode;
using UnityEngine;

public class StarPlayer : NetworkBehaviour
{

    public event Action OnStarCollected;
    public float step = 0.05f;
    Rigidbody2D m_Rb;
    Collider2D m_Collider;
    [SerializeField] GameObject[] StarsList;

    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<Collider2D>();
    }

    public void Handle_PlayerMove(Vector2 Dir, bool Performed)
    {
        transform.position += new Vector3 (Dir.x,Dir.y,0) * step;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Star")
        { 
            //Find Child Index

            for(int i = 0; i < StarsList.Length; i++)
            {
                if(collision.gameObject == StarsList[i])
                {
                    CollectedStar_Rpc(i);
                }
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    protected void CollectedStar_Rpc(int StarIndex)
    {
        StarsList[StarIndex].SetActive(false);
        OnStarCollected?.Invoke();
    }

    private void FixedUpdate()
    {
        
    }

    
}
