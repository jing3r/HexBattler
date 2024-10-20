using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HoverHighlight : MonoBehaviour
{
    private TileSelector tileSelector;
    private HighlightController highlightController;
    private Tile lastHoveredTile;
    private List<Tile> currentlyHighlightedTiles = new List<Tile>();
    private List<Tile> selectedTiles = new List<Tile>();
    public int maxTargets = 3;
    public int radius = 0;
    public int distance = 5;
    private int currentSelectionType = -1;

    void Start()
    {
        tileSelector = FindObjectOfType<TileSelector>();
        highlightController = FindObjectOfType<HighlightController>();
    }

    void Update()
    {
        HandleInput();
        HandleHighlighting();

        if (Input.GetMouseButtonDown(0) && currentSelectionType != -1)
        {
            SelectTile();
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Q)) ToggleSelectionType(0);
        if (Input.GetKeyDown(KeyCode.W)) ToggleSelectionType(1);
        if (Input.GetKeyDown(KeyCode.E)) ToggleSelectionType(2);
        if (Input.GetKeyDown(KeyCode.R)) ToggleSelectionType(3);
    }

    void ToggleSelectionType(int type)
    {
        if (currentSelectionType == type)
        {
            currentSelectionType = -1;
            ClearHighlights();
            ClearHexMarkers();
            selectedTiles.Clear();
        }
        else
        {
            currentSelectionType = type;
            UpdateHighlights(lastHoveredTile);
            selectedTiles.Clear();
        }
    }

    void HandleHighlighting()
    {
        if (currentSelectionType == -1) return;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            Tile hoveredTile = hit.collider.GetComponent<Tile>();

            if (hoveredTile != null && hoveredTile != lastHoveredTile)
            {
                if (lastHoveredTile != null)
                {
                    lastHoveredTile.RemoveHighlight();
                }

                lastHoveredTile = hoveredTile;
                hoveredTile.HighlightAsTarget();
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
        if (hoveredTile == null || currentSelectionType == -1) return;

        switch (currentSelectionType)
        {
            case 0:
                HighlightTilesInCircle(hoveredTile, radius);
                break;
            case 1:
            case 2:
            case 3:
                HighlightDirectional(hoveredTile);
                break;
        }
    }

    void HighlightTilesInCircle(Tile centerTile, int radius)
    {
        List<Tile> tiles = TileSelector.GetTilesInCircle(centerTile, radius);
        foreach (var tile in tiles)
        {
            if (!selectedTiles.Contains(tile))
            {
                tile.HighlightAsTarget();
                currentlyHighlightedTiles.Add(tile);
            }
        }
    }

    void HighlightDirectional(Tile targetTile)
    {
        Tile unitTile = tileSelector.targetSelector.currentUnit.currentTile;
        Vector3 direction = (targetTile.transform.position - unitTile.transform.position).normalized;
        Vector3 farPoint = unitTile.transform.position + direction * distance;
        float baseWidth = distance * 0.5f;
        Vector3 leftPoint = farPoint + Vector3.Cross(Vector3.up, direction) * baseWidth;
        Vector3 rightPoint = farPoint - Vector3.Cross(Vector3.up, direction) * baseWidth;

        List<Tile> edgeTiles = GetTilesAlongPath(unitTile.transform.position, farPoint);

        if (currentSelectionType == 1)
        {
            edgeTiles.RemoveAll(tile => tile == unitTile);
            edgeTiles.AddRange(GetTilesAlongPath(farPoint, leftPoint));
            edgeTiles.AddRange(GetTilesAlongPath(farPoint, rightPoint));
            FillTriangle(edgeTiles, leftPoint, rightPoint);
        }
        else if (currentSelectionType == 2)
        {
            edgeTiles = GetTilesAlongPath(unitTile.transform.position, farPoint);
            edgeTiles.RemoveAll(tile => tile == unitTile);
            edgeTiles = edgeTiles.GetRange(0, Mathf.Min(edgeTiles.Count, distance));
        }
        else if (currentSelectionType == 3)
        {
            edgeTiles.Clear();
            edgeTiles.AddRange(GetTilesAlongPath(leftPoint, rightPoint));
        }

        foreach (var tile in edgeTiles)
        {
            if (!selectedTiles.Contains(tile))
            {
                tile.HighlightAsTarget();
                currentlyHighlightedTiles.Add(tile);
            }
        }
    }

    void FillTriangle(List<Tile> edgeTiles, Vector3 leftPoint, Vector3 rightPoint)
    {
        foreach (Tile edgeTile in edgeTiles)
        {
            List<Tile> lineToLeft = GetTilesAlongPath(edgeTile.transform.position, leftPoint);
            List<Tile> lineToRight = GetTilesAlongPath(edgeTile.transform.position, rightPoint);
            AddUniqueTiles(lineToLeft);
            AddUniqueTiles(lineToRight);
        }
    }

    void AddUniqueTiles(List<Tile> tiles)
    {
        foreach (var tile in tiles)
        {
            if (!currentlyHighlightedTiles.Contains(tile))
            {
                currentlyHighlightedTiles.Add(tile);
                tile.HighlightAsTarget();
            }
        }
    }

    void SelectTile()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            Tile selectedTile = hit.collider.GetComponent<Tile>();

            if (selectedTile != null)
            {
                if (selectedTiles.Contains(selectedTile))
                {
                    selectedTiles.Remove(selectedTile);
                    Transform hexMarker = selectedTile.transform.Find("HexMarker");
                    if (hexMarker != null) hexMarker.gameObject.SetActive(false);
                }
                else if (selectedTiles.Count < maxTargets)
                {
                    foreach (var tile in currentlyHighlightedTiles)
                    {
                        if (!selectedTiles.Contains(tile))
                        {
                            selectedTiles.Add(tile);
                            Transform hexMarker = tile.transform.Find("HexMarker");
                            if (hexMarker != null) hexMarker.gameObject.SetActive(true);
                        }
                    }

                    if (selectedTiles.Count >= maxTargets)
                    {
                        StartCoroutine(UseAbilityCoroutine());
                    }
                }
            }
        }
    }

    void ActivateHexMarkers()
    {
        foreach (var tile in currentlyHighlightedTiles)
        {
            Transform hexMarker = tile.transform.Find("HexMarker");
            if (hexMarker != null) hexMarker.gameObject.SetActive(true);
        }
    }

    void ClearHexMarkers()
    {
        Tile[] allTiles = FindObjectsOfType<Tile>();
        foreach (var tile in allTiles)
        {
            Transform hexMarker = tile.transform.Find("HexMarker");
            if (hexMarker != null) hexMarker.gameObject.SetActive(false);
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

    IEnumerator UseAbilityCoroutine()
    {
        Debug.Log("Ability activated with " + selectedTiles.Count + " targets!");
        yield return new WaitForSeconds(1f);
        ClearHexMarkers();
        selectedTiles.Clear();
        Debug.Log("Ability finished.");
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

            if (tile != null && !pathTiles.Contains(tile))
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
