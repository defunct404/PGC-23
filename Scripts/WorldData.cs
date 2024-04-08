using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedObject
{
    public string prefabName; //  префаб.name
    public Vector3 position; // pos объекта
}

[System.Serializable]
public class WorldData
{
    public List<SavedObject> savedObjects;
    public List<Vector3> tilemapPositions;
    public int grassSeed;
    public List<Vector3Int> groundTiles;
    public List<Vector3Int> diffTiles;
    public List<Vector2Int> rocksTiles;
    //public int[,] rocksTiles;
    //public int[,] roadTiles;
}
