using UnityEngine;

[CreateAssetMenu(fileName = "SO_GameSettings", menuName = "Scriptable Objects/SO_GameSettings")]
public class SO_GameSettings : ScriptableObject
{
    protected string DifficultyName = "Default";
    public int GameTime = 120;
    public bool RequiresConfigurationsForNextPuzzle = true;
    public bool SabotageGivesScore = true;
}
