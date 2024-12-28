using UnityEngine;

[CreateAssetMenu(fileName = "ProjectData", menuName = "Voting/ProjectData")]
public class ProjectData : ScriptableObject
{
    public int projectNumber;
    public string organizationName;
    public string projectName;
    public Sprite icon;
}
