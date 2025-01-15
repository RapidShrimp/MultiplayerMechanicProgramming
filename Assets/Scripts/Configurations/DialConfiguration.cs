using Unity.Netcode;
using UnityEngine;

public class DialConfiguration : Configuration, IInteractable
{
    NetworkVariable<int> DialPosition = new NetworkVariable<int>(
        value:0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    [SerializeField] GameObject DialMesh;

    public void Awake()
    {
    }
    public override void OnNetworkSpawn()
    {
        DialPosition.OnValueChanged += OnDialTurned;
    }
    public override void OnNetworkDespawn()
    {
        DialPosition.OnValueChanged -= OnDialTurned;

    }
    override public void StartModule()
    {
     
    }

    private void OnDialTurned(int OldValue, int NewValue)
    {
        DialMesh.transform.rotation = Quaternion.Euler(0, 0, 90 * NewValue);
    }

    [Rpc(SendTo.Owner)]
    private void ChangeDialPosition_Rpc()
    {
        DialPosition.Value = Random.Range(0, 5);
    }

    public bool OnClick()
    {
        Debug.Log("CLikc");
        ChangeDialPosition_Rpc();
        return true;
    }

    public void OnDrag()
    {
        //Do Nothing :)
    }
}
