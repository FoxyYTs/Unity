using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDebugger : MonoBehaviour
{
    // Asigna aquí tu Tilemap principal del mapa (el que tiene las paredes)
    public Tilemap mazeTilemap; 

    // Opcional: Si tienes un Tilemap de pellets separado y quieres asegurarte
    // de que esas celdas se cuenten como 'despejadas' antes de generar los prefabs.
    // Aunque el PelletSpawner ya vacía esas celdas.
    // public Tilemap pelletTilemap; 

    public Color wallColor = Color.red;    // Color para las "X" (paredes)
    public Color pathColor = Color.green;  // Color para las "O" (pasillos despejados)

    public float debugDrawDuration = 0.5f; // Duración de las líneas de depuración
    public float offset = 0.2f;            // Pequeño offset para que los símbolos no se superpongan a otros objetos

    void Start()
    {
        if (mazeTilemap == null)
        {
            Debug.LogError("MapDebugger: ¡Asigna el Tilemap del laberinto en el Inspector!");
            return;
        }

        DrawMapDebug();
    }

    void DrawMapDebug()
    {
        // Itera sobre los límites de la cuadrícula del Tilemap del laberinto
        // Estos límites definen el área de tu mapa.
        BoundsInt bounds = mazeTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                
                // Obtiene la posición central del mundo de esta celda
                Vector3 worldPosition = mazeTilemap.GetCellCenterWorld(cellPosition);
                
                // Aplica un pequeño offset para que los símbolos no se superpongan a sprites o colliders
                worldPosition += new Vector3(offset, offset, 0);

                // Verifica si hay un tile en esta posición en el Tilemap de paredes
                TileBase wallTile = mazeTilemap.GetTile(cellPosition);

                if (wallTile != null)
                {
                    // Es un tile de pared - dibuja una "X"
                    Debug.DrawLine(worldPosition + new Vector3(-0.3f, -0.3f, 0), worldPosition + new Vector3(0.3f, 0.3f, 0), wallColor, debugDrawDuration);
                    Debug.DrawLine(worldPosition + new Vector3(-0.3f, 0.3f, 0), worldPosition + new Vector3(0.3f, -0.3f, 0), wallColor, debugDrawDuration);
                }
                else
                {
                    // Es un espacio vacío/pasillo - dibuja una "O" (círculo simple)
                    Debug.DrawLine(worldPosition + new Vector3(-0.3f, 0, 0), worldPosition + new Vector3(0.3f, 0, 0), pathColor, debugDrawDuration);
                    Debug.DrawLine(worldPosition + new Vector3(0, -0.3f, 0), worldPosition + new Vector3(0, 0.3f, 0), pathColor, debugDrawDuration);
                    Debug.DrawLine(worldPosition + new Vector3(-0.2f, 0.2f, 0), worldPosition + new Vector3(0.2f, 0.2f, 0), pathColor, debugDrawDuration); // Para darle forma de círculo
                    Debug.DrawLine(worldPosition + new Vector3(-0.2f, -0.2f, 0), worldPosition + new Vector3(0.2f, -0.2f, 0), pathColor, debugDrawDuration);
                }
            }
        }
    }
}