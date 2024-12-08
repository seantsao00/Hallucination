using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(TilemapSkinChanger))]
public class TilemapSkinChangerInspector : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        TilemapSkinChanger tilemapSkinChanger = (TilemapSkinChanger)target;
        if (GUILayout.Button("CopyTilemap")) {
            tilemapSkinChanger.GenerateCopiedTilemapWithSkin();
        }
    }
}
#endif

public class TilemapSkinChanger : MonoBehaviour {
    [SerializeField] Tilemap[] sourceTilemaps;
    [SerializeField] TileBase targetTileType;

    public void GenerateCopiedTilemapWithSkin() {
        foreach (var sourceTilemap in sourceTilemaps) {
            sourceTilemap.CompressBounds();
            var copiedTilemapGameObject = Instantiate(sourceTilemap.gameObject, sourceTilemap.transform);
            copiedTilemapGameObject.name = sourceTilemap.name + "_skin";
            var copiedTilemap = copiedTilemapGameObject.GetComponent<Tilemap>();
            BoundsInt bounds = copiedTilemap.cellBounds;

            for (int x = bounds.xMin; x < bounds.xMax; x++) {
                for (int y = bounds.yMin; y < bounds.yMax; y++) {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    TileBase tile = copiedTilemap.GetTile(position);
                    if (tile != null) {
                        copiedTilemap.SetTile(position, targetTileType);
                    }
                }
            }
            copiedTilemap.RefreshAllTiles();
        }
    }
}
