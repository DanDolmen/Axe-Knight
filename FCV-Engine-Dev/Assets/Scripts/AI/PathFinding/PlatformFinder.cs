using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

/// <summary>
/// Provides functionality to identify and manage walkable platform tiles in a tilemap.
/// Implements a singleton pattern to ensure only one instance exists and offers methods for detecting platforms below a specific position, identifying the closest connected platform, and visualizing walkable areas.
/// </summary>
public class PlatformFinder : MonoBehaviour
{
    public static PlatformFinder Instance { get; private set; }

    [SerializeField] private Tilemap tileMap;
    [FormerlySerializedAs("enemyCantWalkThrough")] [SerializeField] private LayerMask layerCantWalkThrough;

    private List<Vector3Int> walkablePositions;

    private Vector3 CellHalfSize => tileMap ? tileMap.cellSize / 2f : Vector3.zero;
    public Tilemap TileMap => tileMap;
    private static Vector3Int MaxV3Int => new(int.MaxValue, int.MaxValue, int.MaxValue);
    
    /// <summary>
    /// Does a singleton check and if one already exists deletes itself
    /// </summary>
    private void Awake()
    {
        SingletonCheck();
    }
    
    /// <summary>
    /// Finds the tile directly below a specified position within the tilemap.
    /// Returns a maximum vector value if no tile is found within a certain threshold.
    /// </summary>
    /// <param name="position">The position to search from.</param>
    /// <returns>The tile position directly below, or a max vector if none exists.</returns>
    public Vector3Int GetPlatformBelow(Vector3 position)
    {
        int i = 0;
        while (!tileMap.HasTile(tileMap.WorldToCell(position)))
        {
            position += Vector3.down;
            
            if (i == 9)
            {
                print("Null");
                return MaxV3Int;
            }

            i++;
        }
        
        return tileMap.WorldToCell(position);
    }
    
    /// <summary>
    /// Retrieves all tiles forming the closest connected platform beneath a given position.
    /// Includes tiles to the left and right, as long as they are walkable and uninterrupted.
    /// </summary>
    /// <param name="position">The position to search from.</param>
    /// <returns>A list of tile positions representing the closest connected platform.</returns>
    public List<Vector3Int> GetClosestPlatform(Vector3 position)
    {
        List<Vector3Int> platformPositions = new();
        
        Vector3Int tilePosition = GetPlatformBelow(position);

        if (tilePosition == MaxV3Int) return null;

        platformPositions.Add(tilePosition);

        platformPositions.AddRange(CheckPlatformInDirection(tilePosition, Vector2Int.left));
        platformPositions.AddRange(CheckPlatformInDirection(tilePosition, Vector2Int.right));
        return platformPositions.Select(x => x + Vector3Int.up).ToList();
    }
    
    /// <summary>
    /// Checks for all valid platform tiles in a specified direction, ensuring there are no overlapping obstacles above them.
    /// Continues searching until an invalid tile or obstacle is encountered.
    /// </summary>
    /// <param name="start">The starting tile position.</param>
    /// <param name="direction">The direction to search in (e.g., left or right).</param>
    /// <returns>A list of tile positions in the specified direction.</returns>
    private List<Vector3Int> CheckPlatformInDirection(Vector3Int start, Vector2Int direction)
    {
        List<Vector3Int> platforms = new();
        Vector3Int nextTile = start;
        direction.Clamp(-Vector2Int.one, Vector2Int.one);
        
        while (tileMap.HasTile(nextTile) && !tileMap.HasTile(nextTile + Vector3Int.up))
        {
            if (nextTile != start) platforms.Add(nextTile);
            
            
            if (Physics2D.OverlapBox((Vector2Int)nextTile + new Vector2(0.5f ,1.5f), Vector2.one * .9f, 0, layerCantWalkThrough))
                return platforms;
            
            nextTile += (Vector3Int)direction;
        }

        return platforms;
    }
    /// <summary>
    /// Will display gizmos for all walkable positions if list is valid and has elements
    /// </summary>
    private void OnDrawGizmos()
    {
        if (walkablePositions == null || walkablePositions.Count <= 0) return;

        foreach (var position in walkablePositions)
        {
            Gizmos.DrawWireSphere(position + CellHalfSize, .2f);
        }
    }

    #region Singleton
    private void SingletonCheck()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }
    #endregion
}
