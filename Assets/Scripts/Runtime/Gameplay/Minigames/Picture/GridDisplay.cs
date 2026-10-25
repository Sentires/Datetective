using System.Collections;
using System.Collections.Generic;
using TheDates.Runtime.Experimental.MinigameCore;
using UnityEngine;

public class GridDisplay
{
    private Renderer _sourceRenderer;
    public Material material {get; private set;}
    
    private static int _gridTilingID = Shader.PropertyToID("_Grid_Tiling");
    private static int _lineThicknessID = Shader.PropertyToID("_Line_Thickness");
    private static int _lineColourID = Shader.PropertyToID("_Line_Colour");
    
    
    public GridDisplay(Renderer source) {
        _sourceRenderer = source;
        material = source.material;
    }

    public void SetLineColour(Color colour) {
        material.SetColor(_lineColourID, colour);
    }

    public void SetLineThickness(float thickness) {
        material.SetFloat(_lineThicknessID, thickness);
    }

    public void SetGridTiling(Vector2 tiling) {
        material.SetVector(_gridTilingID, tiling);
    }
}
