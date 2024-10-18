using UnityEngine;
using System.Collections.Generic;

public class HoverHighlight : MonoBehaviour
{
    private TileSelector tileSelector;
    private Tile lastHoveredTile;
    private bool highlightModeSingle = true;
    private bool isHighlightingArea = false;
    void Start()
    {
        tileSelector = FindObjectOfType<TileSelector>();
    }

   void Update()
{
    if (Input.GetKeyDown(KeyCode.F))
    {
        isHighlightingArea = !isHighlightingArea;
        ClearTileHighlights();
    }

    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
    {
        Tile hoveredTile = hit.collider.GetComponent<Tile>();

        if (hoveredTile != null)
        {
            ClearTileHighlights();

            if (isHighlightingArea)
            {
                List<Tile> tilesInRadius = TileSelector.GetTilesInCircle(hoveredTile, 1);

                foreach (var tile in tilesInRadius)
                {
                    tile.marker.SetActive(true);
                }
            }
            else
            {
                hoveredTile.marker.SetActive(true);
            }
        }
    }
}


    private void HandleHoverHighlight()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Tile hoveredTile = hit.collider.GetComponent<Tile>();

            if (hoveredTile != null)
            {
                if (lastHoveredTile != null && lastHoveredTile != hoveredTile)
                {
                    lastHoveredTile.marker.SetActive(false);
                }

                if (highlightModeSingle)
                {
                    hoveredTile.marker.SetActive(true);
                }
                else
                {
                    List<Tile> tilesInRadius = TileSelector.GetTilesInCircle(hoveredTile, 1); // Радиус 1 для теста
                    foreach (var tile in tilesInRadius)
                    {
                        tile.marker.SetActive(true);
                    }
                }

                lastHoveredTile = hoveredTile;
            }
        }
        else
        {
            if (lastHoveredTile != null)
            {
                lastHoveredTile.marker.SetActive(false);
                lastHoveredTile = null;
            }
        }
    }

void ClearTileHighlights()
{
    Tile[] allTiles = FindObjectsOfType<Tile>();
    foreach (Tile tile in allTiles)
    {
        tile.marker.SetActive(false);
    }
}

    private void ToggleHighlightMode()
    {
        highlightModeSingle = !highlightModeSingle;
        ClearTileHighlights();
    }
}
