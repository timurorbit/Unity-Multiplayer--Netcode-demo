using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerColorDisplay : MonoBehaviour
{
    [SerializeField] private TeamColorLookup teamColorLookup;
    [SerializeField] private PlanePlayer player;

    [SerializeField]
    private SpriteRenderer[] spriteRenderers;

    private void Start()
    {
        HandlePlayerTeamChanged(-1, player.TeamIndex.Value);
        player.TeamIndex.OnValueChanged += HandlePlayerTeamChanged;
    }

    private void HandlePlayerTeamChanged(int oldIndex, int newIndex)
    {
        Color teamColor = teamColorLookup.GetTeamColor(newIndex);
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = teamColor;
        }
    }

    private void OnDestroy()
    {
        player.TeamIndex.OnValueChanged -= HandlePlayerTeamChanged;
    }
    
    
}
