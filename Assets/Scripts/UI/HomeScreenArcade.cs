using Unity.Netcode;
using UnityEngine;

public class HomeScreenArcade : NetworkBehaviour
{
    [SerializeField] MeshRenderer[] MenuArcadeUnits;

    private void OnEnable()
    {
        GameManager.OnPlayerCountUpdated += GameManager_OnPlayerCountUpdated;
    }

    private void GameManager_OnPlayerCountUpdated(int NewValue)
    {
        for (int i = 0; i < MenuArcadeUnits.Length; i++) 
        {
            if(i >= NewValue)
            {
                if (MenuArcadeUnits[i].materials[0].color == Color.white) { continue; }
                MenuArcadeUnits[i].materials[2].color = Color.black;
            }
            else
            {
                MenuArcadeUnits[i].materials[2].color = Color.white;   
            }
        }
    }

    private void OnDisable()
    {
        GameManager.OnPlayerCountUpdated -= GameManager_OnPlayerCountUpdated;
    }

}
