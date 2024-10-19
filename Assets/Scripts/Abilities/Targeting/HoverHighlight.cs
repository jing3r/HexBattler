using UnityEngine;
using System.Collections.Generic;

public class HoverHighlight : MonoBehaviour
{
    private TileSelector tileSelector;
    private HighlightController highlightController;
    private Tile lastHoveredTile;
    private List<Tile> currentlyHighlightedTiles = new List<Tile>();
    private int currentSelectionType = 0; // 0 - Circle/Tile, 1 - Triangle, 2 - Ray, 3 - Line
    private int radius = 0;

    void Start()
    {
        tileSelector = FindObjectOfType<TileSelector>();
        highlightController = FindObjectOfType<HighlightController>();
    }

    void Update()
    {
        HandleInput();
        HandleHighlighting();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Q)) { currentSelectionType = 0; radius = 0; } // Tile mode
        if (Input.GetKeyDown(KeyCode.W)) { currentSelectionType = 0; radius = 1; } // Circle mode
        if (Input.GetKeyDown(KeyCode.E)) { currentSelectionType = 1; } // Triangle mode
        if (Input.GetKeyDown(KeyCode.R)) { currentSelectionType = 2; } // Ray mode
        if (Input.GetKeyDown(KeyCode.T)) { currentSelectionType = 3; } // Line mode
    }

    void HandleHighlighting()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            Tile hoveredTile = hit.collider.GetComponent<Tile>();

            if (hoveredTile != null && hoveredTile != lastHoveredTile)
            {
                lastHoveredTile = hoveredTile;
                UpdateHighlights(hoveredTile);
            }
        }
        else
        {
            ClearHighlights();
        }
    }

    void UpdateHighlights(Tile hoveredTile)
    {
        ClearHighlights();

        switch (currentSelectionType)
        {
            case 0:
                HighlightTilesInCircle(hoveredTile, radius);
                break;
            case 1:
                HighlightTriangle(hoveredTile);
                break;
            case 2:
                HighlightRay(hoveredTile);
                break;
            case 3:
                HighlightLine(hoveredTile);
                break;
        }
    }

    void HighlightTilesInCircle(Tile centerTile, int radius)
    {
        List<Tile> tiles = TileSelector.GetTilesInCircle(centerTile, radius);
        foreach (var tile in tiles)
        {
            tile.HighlightAsTarget();
            currentlyHighlightedTiles.Add(tile);
        }
    }

    void HighlightTriangle(Tile targetTile)
    {
        List<Tile> triangleTiles = GetTriangleTiles(targetTile);
        foreach (var tile in triangleTiles)
        {
            tile.HighlightAsTarget();
            currentlyHighlightedTiles.Add(tile);
        }
    }

    void HighlightRay(Tile targetTile)
    {
        List<Tile> rayTiles = GetRayTiles(targetTile);
        foreach (var tile in rayTiles)
        {
            tile.HighlightAsTarget();
            currentlyHighlightedTiles.Add(tile);
        }
    }

    void HighlightLine(Tile targetTile)
    {
        List<Tile> lineTiles = GetLineTiles(targetTile);
        foreach (var tile in lineTiles)
        {
            tile.HighlightAsTarget();
            currentlyHighlightedTiles.Add(tile);
        }
    }

    void ClearHighlights()
    {
        foreach (var tile in currentlyHighlightedTiles)
        {
            tile.RemoveHighlight();
        }
        currentlyHighlightedTiles.Clear();
    }

    List<Tile> GetTriangleTiles(Tile targetTile)
    {
        List<Tile> result = new List<Tile>();
        Tile unitTile = tileSelector.targetSelector.currentUnit.currentTile;
        Vector3 direction = (targetTile.transform.position - unitTile.transform.position).normalized;

        int distance = 4;
        Vector3 farPoint = unitTile.transform.position + direction * distance;

        result.AddRange(GetTilesAlongPath(unitTile.transform.position, farPoint));

        float baseWidth = distance * 0.5f;
        Vector3 leftPoint = farPoint + Vector3.Cross(Vector3.up, direction) * baseWidth;
        Vector3 rightPoint = farPoint - Vector3.Cross(Vector3.up, direction) * baseWidth;

        result.AddRange(GetTilesAlongPath(farPoint, leftPoint));
        result.AddRange(GetTilesAlongPath(farPoint, rightPoint));

        return result;
    }

    List<Tile> GetRayTiles(Tile targetTile)
    {
        List<Tile> result = new List<Tile>();
        Tile unitTile = tileSelector.targetSelector.currentUnit.currentTile;
        Vector3 direction = (targetTile.transform.position - unitTile.transform.position).normalized;

        result.AddRange(GetTilesAlongPath(unitTile.transform.position, targetTile.transform.position));

        return result;
    }

    List<Tile> GetLineTiles(Tile targetTile)
    {
        List<Tile> result = new List<Tile>();
        Tile unitTile = tileSelector.targetSelector.currentUnit.currentTile;
        Vector3 direction = (targetTile.transform.position - unitTile.transform.position).normalized;

        int distance = 4;
        Vector3 farPoint = unitTile.transform.position + direction * distance;

        result.AddRange(GetTilesAlongPath(unitTile.transform.position, farPoint));

        return result;
    }

    List<Tile> GetTilesAlongPath(Vector3 start, Vector3 end)
    {
        List<Tile> pathTiles = new List<Tile>();
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        for (float i = 0; i <= distance; i += 0.5f)
        {
            Vector3 position = start + direction * i;
            Tile tile = FindTileAtPosition(position);

            if (tile != null)
            {
                pathTiles.Add(tile);
            }
        }

        return pathTiles;
    }

    Tile FindTileAtPosition(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, 0.1f);
        foreach (var hit in hits)
        {
            Tile tile = hit.GetComponent<Tile>();
            if (tile != null)
            {
                return tile;
            }
        }
        return null;
    }
}
