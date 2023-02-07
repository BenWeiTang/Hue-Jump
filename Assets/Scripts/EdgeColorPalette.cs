using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create Edge Color Palette", fileName = "Edge Color Palette")]
public class EdgeColorPalette : ScriptableObject
{
    public List<EdgeColor> EdgeColors => _edgeColors;
    
    [SerializeField] private List<EdgeColor> _edgeColors;
}

[System.Serializable]
public struct EdgeColor
{
    public Color Color;
    public ColorLayer ColorLayer;
}

public enum ColorLayer
{
    Red = 6,
    Blue,
    Yellow,
    Purple,
    Green,
    Orange
}