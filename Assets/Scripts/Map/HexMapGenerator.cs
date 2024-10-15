using UnityEngine;
using System.Collections.Generic;

public class HexMapGenerator : MonoBehaviour
{
    public GameObject normalTilePrefab;
    public GameObject difficultTerrainPrefab;
    public GameObject highGroundPrefab;
    public GameObject lowGroundPrefab;
    public GameObject partialCoverPrefab;
    public GameObject fullCoverPrefab;

    public GameObject[] team1Units;
    public GameObject[] team2Units;

    public int rings = 5;

    private List<GameObject> generatedTiles = new List<GameObject>();
    private List<GameObject> generatedUnits = new List<GameObject>();

    private Dictionary<Vector3, bool> tilePassability = new Dictionary<Vector3, bool>();

    public TurnManager turnManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            GenerateMap();
        }
    }

    void GenerateMap()
    {
        ClearMap();
        ResetCapturePoints();
        Vector3 centerPosition = Vector3.zero;
        GameObject centerTile = Instantiate(normalTilePrefab, centerPosition, Quaternion.identity);
        generatedTiles.Add(centerTile);

        Tile centerTileComponent = centerTile.GetComponent<Tile>();
        centerTileComponent.isFlagTile = true;

        tilePassability[centerPosition] = centerTileComponent.IsPassable();

        for (int ring = 1; ring <= rings; ring++)
        {
            GenerateRing(centerPosition, ring);
        }

        GenerateImpassableRing(centerPosition, rings + 1);
        PlaceUnits();

        turnManager.InitializeTurnCycle();
    }

    void GenerateRing(Vector3 centerPosition, int ring)
    {
        List<Vector3> ringPositions = GetHexRingPositions(centerPosition, ring);
        foreach (Vector3 position in ringPositions)
        {
            GameObject tilePrefab = SelectTilePrefab(ring);
            Vector3 tilePosition = position;

            GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
            generatedTiles.Add(tile);

            Tile tileComponent = tile.GetComponent<Tile>();

            if (ring == 1)
            {
                tileComponent.isFlagTile = true;
            }

            if (tilePrefab == highGroundPrefab)
            {
                tileComponent.isHighGround = true;
            }
            else if (tilePrefab == lowGroundPrefab)
            {
                tileComponent.isLowGround = true;
            }

            bool isPassable = (tilePrefab != partialCoverPrefab && tilePrefab != fullCoverPrefab);
            tilePassability[position] = isPassable;
        }
    }

    void GenerateImpassableRing(Vector3 centerPosition, int ring)
    {
        List<Vector3> ringPositions = GetHexRingPositions(centerPosition, ring);
        foreach (Vector3 position in ringPositions)
        {
            GameObject tilePrefab = Random.Range(0f, 1f) > 0.5f ? partialCoverPrefab : fullCoverPrefab;
            GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
            generatedTiles.Add(tile);
            tilePassability[position] = false;
        }
    }

    List<Vector3> GetHexRingPositions(Vector3 centerPosition, int ring)
    {
        List<Vector3> positions = new List<Vector3>();
        Vector3[] directions = {
            new Vector3(1, 0, -0.577f),
            new Vector3(0, 0, -1.154f),
            new Vector3(-1, 0, -0.577f),
            new Vector3(-1, 0, 0.577f),
            new Vector3(0, 0, 1.154f),
            new Vector3(1, 0, 0.577f)
        };

        Vector3 position = centerPosition + directions[4] * ring;

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < ring; j++)
            {
                positions.Add(position);
                position += directions[i];
            }
        }

        return positions;
    }

    GameObject SelectTilePrefab(int ring)
    {
        float randomValue = Random.Range(0f, 100f);
        if (randomValue < 40f) return normalTilePrefab;
        if (randomValue < 50f) return difficultTerrainPrefab;
        if (randomValue < 65f) return highGroundPrefab;
        if (randomValue < 80f) return lowGroundPrefab;
        if (randomValue < 90f) return partialCoverPrefab;
        return fullCoverPrefab;
    }

    void PlaceUnits()
    {
        List<Vector3> lastPlayableRingPositions = GetHexRingPositions(Vector3.zero, rings);

        int unitIndex = 0;
        for (int i = 0; i < lastPlayableRingPositions.Count / 2; i++)
        {
            Vector3 tilePosition = lastPlayableRingPositions[i];
            GameObject tile = generatedTiles.Find(t => t.transform.position == tilePosition);
            Tile tileComponent = tile.GetComponent<Tile>();

            if (tileComponent != null && tileComponent.IsPassable() && unitIndex < team1Units.Length)
            {
                Vector3 unitPosition = new Vector3(tilePosition.x, tilePosition.y + 1, tilePosition.z);
                GameObject unit = Instantiate(team1Units[unitIndex], unitPosition, Quaternion.identity);
                generatedUnits.Add(unit);
                turnManager.units.Add(unit.GetComponent<Unit>());

                Unit unitComponent = unit.GetComponent<Unit>();
                unitComponent.currentTile = tileComponent;
                unitIndex++;
            }
        }

        unitIndex = 0;
        for (int i = lastPlayableRingPositions.Count / 2; i < lastPlayableRingPositions.Count; i++)
        {
            Vector3 tilePosition = lastPlayableRingPositions[i];
            GameObject tile = generatedTiles.Find(t => t.transform.position == tilePosition);
            Tile tileComponent = tile.GetComponent<Tile>();

            if (tileComponent != null && tileComponent.IsPassable() && unitIndex < team2Units.Length)
            {
                Vector3 unitPosition = new Vector3(tilePosition.x, tilePosition.y + 1, tilePosition.z);
                GameObject unit = Instantiate(team2Units[unitIndex], unitPosition, Quaternion.identity);
                generatedUnits.Add(unit);
                turnManager.units.Add(unit.GetComponent<Unit>());

                Unit unitComponent = unit.GetComponent<Unit>();
                unitComponent.currentTile = tileComponent;
                unitIndex++;
            }
        }
    }

    void ClearMap()
    {
        foreach (GameObject tile in generatedTiles)
        {
            Tile tileComponent = tile.GetComponent<Tile>();

            Destroy(tile);
        }
        generatedTiles.Clear();
        tilePassability.Clear();

        foreach (GameObject unit in generatedUnits)
        {
            Destroy(unit);
        }
        generatedUnits.Clear();
        turnManager.units.Clear();
    }

    private void ResetCapturePoints()
    {
        GameEndManager gameEndManager = FindObjectOfType<GameEndManager>();
        if (gameEndManager != null)
        {
            gameEndManager.ResetPoints();
        }
    }
}
