using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTeamColorLookup", menuName = "Team Color Lookup")]
public class TeamColorLookup : ScriptableObject
{
    [SerializeField]
    private Color[] teamColours;

    public Color GetTeamColor(int teamIndex)
    {
        if (teamIndex < 0 || teamIndex >= teamColours.Length)
        {
            return Random.ColorHSV(0f,1f,1f,1f,0.5f,1f);
        }
        else
        {
            return teamColours[teamIndex];
        }
    }
}
