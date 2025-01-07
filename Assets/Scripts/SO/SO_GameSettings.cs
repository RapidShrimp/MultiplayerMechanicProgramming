using UnityEngine;

[CreateAssetMenu(fileName = "SO_GameSettings", menuName = "Scriptable Objects/SO_GameSettings")]
public class SO_GameSettings : ScriptableObject
{
    protected string DifficultyName = "Normal";
    public int GameTime = 120;
    public int DefaultHealth = 100;
}
