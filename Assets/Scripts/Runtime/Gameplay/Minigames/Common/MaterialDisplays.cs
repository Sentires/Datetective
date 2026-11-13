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
    
    public Bounds rendererBounds => SourceRenderer.bounds;
    public Bounds rendererLocalBounds => SourceRenderer.localBounds;

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

public class PuzzleTileDisplay : MaterialInstanceWrapper
{
    private static readonly int AlbedoColourID = Shader.PropertyToID("_AlbedoColour");
    private static readonly int AlbedoMapID = Shader.PropertyToID("_AlbedoMap");
    private static readonly int PaddingEnabledID = Shader.PropertyToID("_PaddingEnabled");
    private static readonly int PaddingTilingID = Shader.PropertyToID("_PaddingTiling");
    private static readonly int PaddingThicknessID = Shader.PropertyToID("_PaddingThickness");
    private static readonly int PaddingColourID = Shader.PropertyToID("_PaddingColour");
    
    
    
    public PuzzleTileDisplay(Renderer source) : base(source) { }

    public void SetAlbedoColour(Color colour) => Material.SetColor(AlbedoColourID, colour);
    public void SetAlbedoMap(Texture2D map) => Material.SetTexture(AlbedoMapID, map);
    public void SetPaddingEnabled(bool toggle) => Material.SetFloat(PaddingEnabledID, toggle ? 1 : 0);
    public void SetPaddingTiling(Vector2 tiling) => Material.SetVector(PaddingTilingID, tiling);
    public void SetPaddingThickness(float thickness) => Material.SetFloat(PaddingThicknessID, thickness);
    public void SetPaddingColour(Color colour) => Material.SetColor(PaddingColourID, colour);
    
    
}

public class PuzzlePieceDisplay : MaterialInstanceWrapper
{
    private static readonly int AlbedoColourID = Shader.PropertyToID("_AlbedoColour");
    private static readonly int AlbedoMapID = Shader.PropertyToID("_AlbedoMap");
    
    
    
    public PuzzlePieceDisplay(Renderer source) : base(source) { }

    public void SetAlbedoColour(Color colour) => Material.SetColor(AlbedoColourID, colour);
    public void SetAlbedoMap(Texture2D map) => Material.SetTexture(AlbedoMapID, map);
    
    
}