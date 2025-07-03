using UnityEngine;
using UnityEngine.Tilemaps; // Necesario para trabajar con Tilemaps

public class PelletSpawner : MonoBehaviour
{
    // Asigna estos prefabs de pellet en el Inspector
    public GameObject pelletPrefab;
    public GameObject powerPelletPrefab;

    // Asigna el Tilemap que contiene los tiles de pellet como referencia
    public Tilemap referencePelletTilemap; 
    // Opcional: Si tienes un Tilemap de PowerPellets separado como referencia
    public Tilemap referencePowerPelletTilemap; 

    // Asigna los tiles específicos que representan los pellets en tus Tilemaps de referencia
    public TileBase normalPelletTile;
    public TileBase powerPelletTile;

    // Referencia al Tilemap de las paredes (esto es si quieres contar los tiles que no son pellets)
    // No es estrictamente necesario para la generación de pellets, pero útil si quieres saber el total de puntos.
    // public Tilemap wallTilemap; 

    void Start()
    {
        // Asegúrate de que las referencias estén asignadas en el Inspector
        if (pelletPrefab == null || powerPelletPrefab == null)
        {
            Debug.LogError("PelletSpawner: Asegúrate de asignar los Prefabs de Pellet y Power Pellet en el Inspector.");
            return;
        }
        if (referencePelletTilemap == null)
        {
            Debug.LogError("PelletSpawner: Asegúrate de asignar el Tilemap de Referencia de Pellets en el Inspector.");
            return;
        }

        // Generar los pellets normales
        SpawnPellets(referencePelletTilemap, normalPelletTile, pelletPrefab);

        // Generar los power pellets (si hay un Tilemap de referencia separado para ellos)
        if (referencePowerPelletTilemap != null)
        {
            SpawnPellets(referencePowerPelletTilemap, powerPelletTile, powerPelletPrefab);
        }
        else // Si los PowerPellets están en el mismo Tilemap de referencia que los normales
        {
            SpawnPellets(referencePelletTilemap, powerPelletTile, powerPelletPrefab);
        }

        // Una vez que todos los pellets han sido instanciados, puedes limpiar los Tilemaps de referencia
        // para que no sigan generando colliders o renderizando tiles.
        // Si no los necesitas más, puedes simplemente deshabilitar los Tilemaps de referencia
        // o borrarlos programáticamente si no los vas a usar más.
        referencePelletTilemap.gameObject.SetActive(false);
        if (referencePowerPelletTilemap != null)
        {
            referencePowerPelletTilemap.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Itera sobre un Tilemap para encontrar tiles específicos y reemplazarlos por GameObjects.
    /// </summary>
    /// <param name="tilemap">El Tilemap de referencia a escanear.</param>
    /// <param name="tileToFind">El TileBase específico que representa el tipo de pellet a generar.</param>
    /// <param name="prefabToSpawn">El GameObject Prefab del pellet a instanciar.</param>
    void SpawnPellets(Tilemap tilemap, TileBase tileToFind, GameObject prefabToSpawn)
    {
        // Itera sobre los límites del Tilemap
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            // Obtiene el tile en la posición actual
            TileBase tile = tilemap.GetTile(pos);

            // Si el tile no es nulo y coincide con el tile que estamos buscando
            if (tile != null && tile == tileToFind)
            {
                // Convierte la posición de la celda a la posición mundial del centro de la celda
                Vector3 worldPos = tilemap.GetCellCenterWorld(pos);

                // Instancia el prefab en esa posición
                Instantiate(prefabToSpawn, worldPos, Quaternion.identity, transform); // Instancia como hijo del Spawner

                // Opcional: Borra el tile del Tilemap de referencia para que no se renderice
                // y para que no interfiera si tiene un collider.
                // Es importante hacer esto si el Tilemap de referencia tiene un Tilemap Collider 2D
                // para evitar doble detección o colliders innecesarios.
                tilemap.SetTile(pos, null); 
            }
        }
    }
}