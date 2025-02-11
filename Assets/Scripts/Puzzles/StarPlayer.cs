using System;
using Unity.Netcode;
using UnityEngine;

public class StarPlayer : NetworkBehaviour
{

    public event Action OnStarCollected;
    public float step = 1;
    Rigidbody2D m_Rb;
    Collider2D m_Collider;
    [SerializeField] GameObject StarsList;

    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<Collider2D>();
        StarsList = transform.parent.GetChild(1).gameObject;
    }

    [Rpc(SendTo.Everyone)]
    public void Handle_PlayerMove_Rpc(Vector2 Dir, bool Performed)
    {
        //transform.localPosition += new Vector3 (Dir.x,Dir.y,0) * step;
        float XPos = Mathf.Clamp(transform.localPosition.x + (Dir.x * step) , -500.0f, 500.0f);
        float YPos = Mathf.Clamp(transform.localPosition.y + (Dir.y * step) , -350.0f, 300.0f);

        transform.localPosition = new Vector3(XPos, YPos, transform.localPosition.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner) { return; }
        if(collision.tag == "Star")
        {
            //Find Child Index
            for (int i = 0; i < StarsList.transform.childCount; i++)
            {
                if(StarsList.transform.GetChild(i).gameObject == collision.transform.gameObject)
                {
                    Debug.Log("Trigger Event Loop");
                    OnStarCollected?.Invoke();
                    CollectedStar_Rpc(i);
                    return;
                }
            }

        }
    }

    [Rpc(SendTo.Everyone)]
    protected void CollectedStar_Rpc(int StarIndex)
    {
        StarsList.transform.GetChild(StarIndex).gameObject.SetActive(false);
    }
    
}
