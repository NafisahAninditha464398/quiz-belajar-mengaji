using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game Data/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelId;
    public string levelName;
    public List<QnA> QnAs;
}
