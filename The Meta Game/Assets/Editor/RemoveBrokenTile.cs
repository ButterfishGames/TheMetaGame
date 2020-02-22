using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class RemoveBrokenTile : EditorWindow
{
    Vector2Int tilePos;
    Tilemap tilemap;
    public static RemoveBrokenTile win;

    const int MIN_TILE_Z = -2147483648;
    const int MAX_TILE_Z = 2147483646;

    [MenuItem("Window/Tools/Remove Broken Tile")]
    static void Init()
    {
        win = ScriptableObject.CreateInstance(typeof(RemoveBrokenTile)) as RemoveBrokenTile;
        win.minSize = new Vector2(300, 150);
        win.ShowUtility();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Tilemap:");
        tilemap = (Tilemap)EditorGUILayout.ObjectField(tilemap, typeof(Tilemap), true);
        tilePos = EditorGUILayout.Vector2IntField("Tile Pos: ", tilePos);
        if (GUILayout.Button("Remove Tile"))
        {
            RemoveTile(tilemap, tilePos);
        }
        GUILayout.EndVertical();
    }

    void RemoveTile(Tilemap map, Vector2Int pos)
    {
        Vector3Int currPos = new Vector3Int(pos.x, pos.y, MIN_TILE_Z);
        string info = "Removing all tiles at " + currPos.x + ", " + currPos.y + " on " + map.name;

        while (currPos.z <= MAX_TILE_Z)
        {
            /*if (EditorUtility.DisplayCancelableProgressBar(
                info, 
                "Deleting tile at Z " + currPos.z, 
                ((currPos.z / 10.0f) - (MIN_TILE_Z/10.0f)) / ((MAX_TILE_Z/10.0f) - (MIN_TILE_Z/10.0f))))
            {
                Debug.LogError("Cancelled");
                break;
            }*/
            map.SetTile(currPos, null);
            currPos.z++;
        }

        // EditorUtility.ClearProgressBar();
    }
}
