using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSectionData", menuName = "Game Data/Section Data")]
public class SectionData : ScriptableObject
{
    public int sectionId;
    public string sectionName;
    public List<LevelData> levels;
}
