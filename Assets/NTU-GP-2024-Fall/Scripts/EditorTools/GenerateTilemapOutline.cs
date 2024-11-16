using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(GenerateTilemapOutline))]
public class GenerateTilemapOutlineInspector : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        GenerateTilemapOutline generateTilemapOutline = (GenerateTilemapOutline)target;
        if (GUILayout.Button("Generate Outline")) {
            generateTilemapOutline.GenerateOutline();
        }
    }
}
#endif

public class GenerateTilemapOutline : MonoBehaviour {
    [SerializeField] Tilemap[] sourceTilemaps;
    [SerializeField] Tile outlineTile;
    [SerializeField] Color outlineColor = Color.black;
    [SerializeField] string outlineName = "Outline";

    public void GenerateOutline() {
        foreach (var sourceTilemap in sourceTilemaps) {
            Tilemap outlineTilemap;
            sourceTilemap.CompressBounds();
            GameObject outlineTilemapGameObject = new GameObject();
            outlineTilemapGameObject.name = outlineName;
            outlineTilemapGameObject.transform.position = sourceTilemap.transform.position;
            outlineTilemapGameObject.transform.parent = sourceTilemap.transform;
            outlineTilemap = outlineTilemapGameObject.AddComponent<Tilemap>();
            outlineTilemapGameObject.AddComponent<TilemapRenderer>();
            // outlineTilemap.ClearAllTiles();
            outlineTilemap.color = outlineColor;
            BoundsInt bounds = sourceTilemap.cellBounds;

            for (int x = bounds.xMin; x < bounds.xMax; x++) {
                for (int y = bounds.yMin; y < bounds.yMax; y++) {
                    Vector3Int position = new Vector3Int(x, y, 0);

                    TileBase tile = sourceTilemap.GetTile(position);
                    if (tile != null) {
                        // Copy tile to the target tilemap
                        outlineTilemap.SetTile(position, outlineTile);
                        // Debug.Log($"{position}({x}, {y}): {tile != null}");

                        // Optionally, copy other tile data (like color or transform)
                        // outlineTilemap.SetTransformMatrix(position, sourceTilemap.GetTransformMatrix(position));
                        // outlineTilemap.SetTileFlags(position, sourceTilemap.GetTileFlags(position));
                        // outlineTilemap.SetColor(position, sourceTilemap.GetColor(position));
                    }
                }
            }
            outlineTilemap.RefreshAllTiles();
        }
    }
}
