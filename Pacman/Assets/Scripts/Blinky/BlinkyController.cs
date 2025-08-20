using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BlinkyController : MonoBehaviour
{
    public float moveSpeed = 4.5f; 
    public Transform pacManTransform; 
    public LayerMask wallLayerMask; 
    private Animator animator;

    public Tilemap mazeTilemap; 

    private Vector2 currentDirection = Vector2.left; 
    private Vector2 targetTile; 
    private Rigidbody2D rb;

    public float raycastOffset = 0.2f; 
    public float raycastDistance = 0.6f; 

    public Color targetLineColor = Color.magenta; 
    public float targetLineDuration = 0.1f; 

    public Color pathLineColor = Color.yellow; 
    private List<Vector3> currentPath = new List<Vector3>(); 

    private Vector2 nextWaypoint; 
    private bool reachedWaypoint = true; 

    void Start(){
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("BlinkyController requiere un componente Rigidbody2D!");
        }

        if (pacManTransform == null)
        {
            Debug.LogError("BlinkyController: ¡Asigna el Transform de Pac-Man en el Inspector!");
            GameObject pacManObj = GameObject.FindGameObjectWithTag("Player"); 
            if (pacManObj != null)
            {
                pacManTransform = pacManObj.transform;
            }
        }

        if (mazeTilemap == null)
        {
            Debug.LogError("BlinkyController: ¡Asigna el Tilemap del laberinto en el Inspector!");
        }

        nextWaypoint = GetGridCenterWorldPosition(transform.position); 
    }

    void FixedUpdate()
    {
        if (pacManTransform != null)
        {
            targetTile = pacManTransform.position;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distanceToWaypoint = Vector2.Distance(transform.position, nextWaypoint);

        if (distanceToWaypoint < 0.05f) 
        {
            transform.position = nextWaypoint; 
            reachedWaypoint = true;
            rb.linearVelocity = Vector2.zero; 
        }
        else
        {
            reachedWaypoint = false;
        }

        if (reachedWaypoint)
        {
            DecideNextDirection();
            nextWaypoint = GetGridCenterWorldPosition((Vector2)transform.position + currentDirection);
        }

        if (!reachedWaypoint)
        {
            Vector2 moveVector = (nextWaypoint - (Vector2)transform.position).normalized;
            rb.linearVelocity = moveVector * moveSpeed;
            // Solo actualiza la dirección para la animación/rotación si realmente se está moviendo hacia una nueva celda
            // y no es solo un ajuste menor al centro.
            // Opcional: Podrías mantener la currentDirection elegida en DecideNextDirection().
            // Para la rotación visual, 'moveVector' es más preciso en cada momento.
            // Para la lógica de IA y restricción de giro, 'currentDirection' es la que importa.
            // Aquí, queremos que el sprite mire hacia donde se mueve.
            if (moveVector.magnitude > 0.01f) // Evitar actualizar si el vector es casi cero
            {
                // Si la dirección del movimiento ha cambiado significativamente, rota.
                // Esto es para que la animación/rotación siga el movimiento inter-tile.
                RotateGhost(moveVector); 
            }
        }


        // DEPURACIÓN VISUAL
        Debug.DrawLine(transform.position, targetTile, targetLineColor, targetLineDuration);
        DrawCalculatedPath();
    }

    void DecideNextDirection()
    {
        Vector2 currentGridPos = GetGridPosition(transform.position);

        if (mazeTilemap != null)
        {
            currentPath = FindPathBFS(currentGridPos, GetGridPosition(targetTile));
        }
        else
        {
            currentPath.Clear(); 
        }

        Vector2[] possibleDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        
        Vector2 bestDirection = currentDirection; 
        float minDistance = float.MaxValue; 

        // Origen de los raycasts: siempre el centro del tile actual de Blinky
        Vector2 rayOriginForDecision = GetGridCenterWorldPosition(transform.position); 

        foreach (Vector2 dir in possibleDirections)
        {
            // Evitar girar 180 grados (a menos que no haya otra opción)
            if (dir == -currentDirection && !CanOnlyMoveBackwards(currentGridPos, currentDirection, wallLayerMask) && currentDirection != Vector2.zero)
            {
                continue; 
            }
            
            // Usamos rayOriginForDecision para la verificación del camino
            if (CheckPathClear(rayOriginForDecision, dir, raycastDistance, wallLayerMask, raycastOffset)) 
            {
                Vector2 nextCellCenter = GetGridCenterWorldPosition((Vector2)transform.position + dir);
                float distance = (targetTile - nextCellCenter).sqrMagnitude; 

                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestDirection = dir;
                }
            }
        }

        // --- Lógica de recuperación si no se encuentra una "mejor" dirección ---
        // Esto cubre casos donde Blinky podría quedarse quieto si no hay un camino claro
        // o si la dirección actual se volvió inválida y no hay otra opción.
        if (minDistance == float.MaxValue) // No se encontró ninguna dirección válida (todas bloqueadas o inválidas)
        {
            // Intentar ir hacia atrás como último recurso si es un callejón sin salida
            if (CheckPathClear(rayOriginForDecision, -currentDirection, raycastDistance, wallLayerMask, raycastOffset))
            {
                bestDirection = -currentDirection;
            }
            else // Si incluso ir hacia atrás está bloqueado, esto indica un problema serio en el laberinto o lógica.
            {
                Debug.LogWarning("Blinky está completamente atascado. No se encontró dirección válida.");
                bestDirection = Vector2.zero; // Se detiene
            }
        }
        // Si se estaba moviendo pero la dirección actual se volvió inválida y se llegó al waypoint
        else if (reachedWaypoint && bestDirection == currentDirection && !CheckPathClear(rayOriginForDecision, currentDirection, raycastDistance, wallLayerMask, raycastOffset))
        {
            // Si la dirección actual ya no es válida desde el centro de este tile,
            // y Blinky llegó a este tile, entonces necesita elegir una nueva dirección.
            // bestDirection ya contendrá la mejor de las opciones válidas.
            // No necesitamos un bucle extra aquí, ya que el bucle principal ya lo hizo.
        }
        
        currentDirection = bestDirection;
        // La rotación del sprite se maneja en FixedUpdate con el moveVector para un movimiento más suave.
        // Pero si quieres que rote *inmediatamente* al decidir, podrías llamarlo aquí también.
        // RotateGhost(currentDirection); 
    }

    Vector2 GetGridCenterWorldPosition(Vector3 worldPosition)
    {
        Vector3Int cellPos = mazeTilemap.WorldToCell(worldPosition);
        return mazeTilemap.GetCellCenterWorld(cellPos);
    }

    Vector2 GetGridPosition(Vector3 worldPosition)
    {
        return new Vector2(Mathf.Round(worldPosition.x), Mathf.Round(worldPosition.y));
    }

    bool CheckPathClear(Vector2 origin, Vector2 direction, float dist, LayerMask layer, float offset)
    {
        Vector2 centerRayOrigin = origin + direction * 0.1f; 
        Vector2 perpendicularOffset = Vector2.zero;

        if (direction == Vector2.up || direction == Vector2.down)
        {
            perpendicularOffset = Vector2.right * offset;
        }
        else if (direction == Vector2.left || direction == Vector2.right)
        {
            perpendicularOffset = Vector2.up * offset;
        }

        Vector2 leftRayOrigin = centerRayOrigin - perpendicularOffset;
        Vector2 rightRayOrigin = centerRayOrigin + perpendicularOffset;

        RaycastHit2D hitCenter = Physics2D.Raycast(centerRayOrigin, direction, dist, layer);
        RaycastHit2D hitLeft = Physics2D.Raycast(leftRayOrigin, direction, dist, layer);
        RaycastHit2D hitRight = Physics2D.Raycast(rightRayOrigin, direction, dist, layer);
        
        Debug.DrawRay(centerRayOrigin, direction * dist, (hitCenter.collider != null) ? Color.red : Color.cyan, 0.1f);
        Debug.DrawRay(leftRayOrigin, direction * dist, (hitLeft.collider != null) ? Color.red : Color.cyan, 0.1f);
        Debug.DrawRay(rightRayOrigin, direction * dist, (hitRight.collider != null) ? Color.red : Color.cyan, 0.1f);

        return hitCenter.collider == null && hitLeft.collider == null && hitRight.collider == null;
    }

    bool CanOnlyMoveBackwards(Vector2 currentGridPos, Vector2 currentDir, LayerMask wallLayer)
    {
        int availableDirections = 0;
        Vector2[] checkDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector2 rayOrigin = GetGridCenterWorldPosition(transform.position); 

        foreach (Vector2 dir in checkDirections)
        {
            if (dir == -currentDir) continue; 

            if (CheckPathClear(rayOrigin, dir, raycastDistance, wallLayer, raycastOffset))
            {
                availableDirections++;
            }
        }
        return availableDirections == 0; 
    }

    void RotateGhost(Vector2 direction){
        // Solo rota si hay un movimiento significativo para evitar rotaciones erráticas con pequeños valores
        if (direction.magnitude > 0.01f) 
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // Horizontal
            {
                if (direction.x > 0) // Derecha
                {
                    animator.SetInteger("Direccion", 2);
                }
                else // Izquierda
                {
                    animator.SetInteger("Direccion", 0);
                }
            }
            else // Vertical
            {
                if (direction.y > 0) // Arriba
                {
                    animator.SetInteger("Direccion", 3);
                }
                else // Abajo
                {
                    animator.SetInteger("Direccion", 1);
                }
            }
        }
    }

    private List<Vector3> FindPathBFS(Vector2 startGridPos, Vector2 targetGridPos)
    {
        if (mazeTilemap == null) return new List<Vector3>();

        Vector3Int startCell = new Vector3Int(Mathf.RoundToInt(startGridPos.x), Mathf.RoundToInt(startGridPos.y), 0);
        Vector3Int targetCell = new Vector3Int(Mathf.RoundToInt(targetGridPos.x), Mathf.RoundToInt(targetGridPos.y), 0);
        
        if (mazeTilemap.GetTile(startCell) != null || mazeTilemap.GetTile(targetCell) != null)
        {
             return new List<Vector3>(); 
        }

        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> parentMap = new Dictionary<Vector3Int, Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        queue.Enqueue(startCell);
        visited.Add(startCell);

        Vector3Int[] directions = {
            new Vector3Int(0, 1, 0),   // Up
            new Vector3Int(0, -1, 0),  // Down
            new Vector3Int(-1, 0, 0),  // Left
            new Vector3Int(1, 0, 0)    // Right
        };

        bool pathFound = false;
        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            if (current == targetCell)
            {
                pathFound = true;
                break;
            }

            foreach (Vector3Int dir in directions)
            {
                Vector3Int neighbor = current + dir;

                if (!visited.Contains(neighbor) && mazeTilemap.GetTile(neighbor) == null) 
                {
                    visited.Add(neighbor);
                    parentMap[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }

        List<Vector3> path = new List<Vector3>();
        if (pathFound)
        {
            Vector3Int current = targetCell;
            while (current != startCell)
            {
                path.Add(mazeTilemap.GetCellCenterWorld(current));
                current = parentMap[current];
            }
            path.Add(mazeTilemap.GetCellCenterWorld(startCell));
            path.Reverse(); 
        }
        return path;
    }

    void DrawCalculatedPath()
    {
        if (currentPath.Count < 2) return; 

        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Debug.DrawLine(currentPath[i], currentPath[i + 1], pathLineColor, targetLineDuration);
        }
    }
}