using Unity.Netcode;
using UnityEngine;

public class HomeScreenArcade : NetworkBehaviour
{
    [SerializeField] MeshRenderer[] MenuArcadeUnits;
    [SerializeField] Material[] UI_MenuMats;
    [SerializeField] GameObject[] UI_RenderTargets;

    private void OnEnable()
    {

        foreach (GameObject go in UI_RenderTargets)
        {
            Instantiate(go);
            go.SetActive(true);
        }

        GameManager.OnPlayerCountUpdated += GameManager_OnPlayerCountUpdated;
        GameManager_OnPlayerCountUpdated(0);
    }

    private void GameManager_OnPlayerCountUpdated(int NewValue)
    {
        for (int i = 0; i < MenuArcadeUnits.Length; i++) 
        {
            if(i >= NewValue)
            {
                if (MenuArcadeUnits[i].material == UI_MenuMats[1]) { continue; }
                MenuArcadeUnits[i].material = UI_MenuMats[0] ;
            }
            else
            {
                MenuArcadeUnits[i].material = UI_MenuMats[1];   
            }
        }
    }

    private void OnDisable()
    {
        GameManager.OnPlayerCountUpdated -= GameManager_OnPlayerCountUpdated;
    }

}
