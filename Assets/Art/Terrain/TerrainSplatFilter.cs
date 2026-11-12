using UnityEngine;

public class TerrainSplatFilter : MonoBehaviour
{
    public Terrain terrain;

    void Start()
    {
        foreach (Texture2D alphamap in terrain.terrainData.alphamapTextures)
        {
            alphamap.filterMode = FilterMode.Point;
        }
    }
}
