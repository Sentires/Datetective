using System.Collections;
using System.Collections.Generic;
using TheDates.Runtime.Experimental.MinigameCore;
using UnityEngine;

public class MaterialInstanceWrapper
{
    protected Renderer SourceRenderer;
    protected Material Material;
    
    // cache the original(s) for later
    public readonly int OriginalRenderingPriority;
    
    // use for same material priority
    public int currentRenderingPriority {get => SourceRenderer.rendererPriority; set => SourceRenderer.rendererPriority = value;}
    
    // use for different material priority
    public int currentSortingOrder {get => SourceRenderer.sortingOrder; set => SourceRenderer.sortingOrder = value;}
    
    // controls the rendering layer (ie, minigame vs rpg)
    public string currentSortingLayerName {get => SourceRenderer.sortingLayerName; set => SourceRenderer.sortingLayerName = value;}
    public int currentSortingLayerID {get => SourceRenderer.sortingLayerID; set => SourceRenderer.sortingLayerID = value;}

    public MaterialInstanceWrapper(Renderer renderer) {
        SourceRenderer = renderer;
        Material = renderer.material;
        
        OriginalRenderingPriority = SourceRenderer.rendererPriority;
    }
}

public class GridDisplay : MaterialInstanceWrapper
{
    private static readonly int GridTilingID = Shader.PropertyToID("_Grid_Tiling");
    private static readonly int LineThicknessID = Shader.PropertyToID("_Line_Thickness");
    private static readonly int LineColourID = Shader.PropertyToID("_Line_Colour");
    
    public GridDisplay(Renderer source) : base(source) { }

    public void SetLineColour(Color colour) {
        Material.SetColor(LineColourID, colour);
    }

    public void SetLineThickness(float thickness) {
        Material.SetFloat(LineThicknessID, thickness);
    }

    public void SetGridTiling(Vector2 tiling) {
        Material.SetVector(GridTilingID, tiling);
    }
    
}
