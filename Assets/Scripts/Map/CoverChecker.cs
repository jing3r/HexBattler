using UnityEngine;

public class CoverChecker
{
    public static bool IsTargetInCover(Unit shooter, Unit target)
    {
        Tile[] adjacentTiles = GetAdjacentTiles(target.currentTile);
        Tile closestTileToShooter = GetClosestTileToShooter(shooter, adjacentTiles);
        if (closestTileToShooter != null && closestTileToShooter.isImpassable)
        {
            return true;
        }

        return false;
    }

    private static Tile[] GetAdjacentTiles(Tile targetTile)
    {
        return targetTile.GetAdjacentTiles();
    }
    private static Tile GetClosestTileToShooter(Unit shooter, Tile[] adjacentTiles)
    {
        Tile closestTile = null;
        float closestDistance = Mathf.Infinity;

        foreach (Tile tile in adjacentTiles)
        {
            float distance = Vector3.Distance(shooter.transform.position, tile.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = tile;
            }
        }

        return closestTile;
    }
}
