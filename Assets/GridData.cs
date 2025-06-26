using System;
using System.Collections.Generic;
using UnityEngine;


public class GridData
{
    private Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int size, int ID, int index, GridObjectType type)
    {
        if (!CanPlaceObjectAt(gridPosition, size, type))
        {
            Debug.LogWarning("Celda " + gridPosition + " ya está ocupada, no se puede colocar ahí.");
            return;
        }

        List<Vector3Int> occupiedPositions = CalculatePositions(gridPosition, size);

        PlacementData data = new PlacementData(occupiedPositions, ID, index, type);
        foreach (var pos in occupiedPositions)
        {
            placedObjects[pos] = data;
        }
    }

    public void RemoveObjectAt(Vector3Int cellPosition)
    {
        if (placedObjects.TryGetValue(cellPosition, out PlacementData data))
        {
            foreach (var pos in data.occupiedPositions)
            {
                placedObjects.Remove(pos);
            }
        }
        else
        {
            Debug.LogWarning($"No hay objeto en la celda {cellPosition} para remover.");
        }
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize, GridObjectType placingType)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionsToOccupy)
        {
            if (placedObjects.TryGetValue(pos, out var existingData))
            {
                //Quitar---------
                /* 
                 if (existingData.objectType != GridObjectType.Car || placingType != GridObjectType.Car)
                     return false;
                */
                if (existingData.objectType == GridObjectType.Car || existingData.objectType == GridObjectType.Obstacle)
                    return false;
            }
        }
        return true;
    }

    public bool HasObjectAt(Vector3Int gridPosition)
    {
        return placedObjects.ContainsKey(gridPosition);
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> result = new List<Vector3Int>();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int z = 0; z < objectSize.y; z++)
            {
                result.Add(gridPosition + new Vector3Int(x, 0, z));
            }
        }
        return result;
    }
}



public enum GridObjectType { Obstacle, Car }

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int placedObjectIndex { get; private set; }
    public GridObjectType objectType { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int ID, int placedObjectIndex, GridObjectType type)
    {
        this.occupiedPositions = occupiedPositions;
        this.ID = ID;
        this.placedObjectIndex = placedObjectIndex;
        this.objectType = type;
    }
}

