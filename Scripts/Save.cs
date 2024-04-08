using System.IO;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.ComponentModel.Design;

public class Save : MonoBehaviour
{
    public GameObject Grid;
    public List<GameObject> prefabObjects;
    public List<Tilemap> tilemaps;
    public int grassSeed;
    public MapGen2 MG2;
    public Tile[] tiles;
    public Tile tileRock;
    public Tile[] diffTiles;

    void initLists()
    {
        Tilemap[] foundTilemaps = Grid.GetComponentsInChildren<Tilemap>();
        tilemaps.AddRange(foundTilemaps);
        GameObject[] objectsWithTag1 = GameObject.FindGameObjectsWithTag("Tree");
        prefabObjects.AddRange(objectsWithTag1);
        GameObject[] objectsWithTag2 = GameObject.FindGameObjectsWithTag("Boar");
        prefabObjects.AddRange(objectsWithTag2);
        GameObject[] objectsWithTag3 = GameObject.FindGameObjectsWithTag("Barrel");
        prefabObjects.AddRange(objectsWithTag3);
        grassSeed = MG2.newNoise;
    }
    void Start()
    {
        initLists();
    }
    public void SaveGame()
    {
        var MapPos = new Vector3Int(0, 0, 0);
        WorldData worldData = new WorldData();
        worldData.savedObjects = new List<SavedObject>();
        worldData.tilemapPositions = new List<Vector3>();
        worldData.grassSeed = grassSeed;
        worldData.groundTiles = new List<Vector3Int>();
        worldData.rocksTiles = new List<Vector2Int>();
        worldData.diffTiles = new List<Vector3Int>();
        for (MapPos.x = -51; MapPos.x <= 51; MapPos = MapPos + Vector3Int.right)
            for (MapPos.y = -51; MapPos.y <= 51; MapPos = MapPos + Vector3Int.up)
            {
                if (tilemaps[0].HasTile(MapPos))
                {
                    Vector3Int gT = new Vector3Int(MapPos.x, MapPos.y, int.Parse(tilemaps[0].GetSprite(MapPos).name));
                    worldData.groundTiles.Add(gT);
                }
                if (tilemaps[1].HasTile(MapPos))
                {
                    Vector2Int rT = new Vector2Int(MapPos.x, MapPos.y);
                    worldData.rocksTiles.Add(rT);
                }
                if (tilemaps[2].HasTile(MapPos))
                {
                    
                    for (int i = 0; i < diffTiles.Length; i++)
                    {
                        if (tilemaps[2].GetSprite(MapPos) == diffTiles[i].sprite)
                        {
                            Vector3Int dT = new Vector3Int(MapPos.x, MapPos.y, i);
                            worldData.diffTiles.Add(dT);
                        }
                    }
                }
            }
        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }
        foreach (GameObject prefab in prefabObjects)
        {
            if (prefab != null)
            {
                SavedObject savedObject = new SavedObject();
                savedObject.prefabName = prefab.name.Remove(prefab.name.IndexOf('('));
                savedObject.position = prefab.transform.position;
                worldData.savedObjects.Add(savedObject);
            }
        }
        foreach (Tilemap tilemap in tilemaps)
        {
            worldData.tilemapPositions.Add((tilemap.transform.position));
        }
        string jsonData = JsonUtility.ToJson(worldData);
        File.WriteAllText(Application.persistentDataPath + "/worldData.json", jsonData);
        Debug.Log("Game Saved!");
    }

    public void LoadGround(List<Vector3Int> groundTiles)
    {
        var MapPos = new Vector3Int(0, 0, 0);
        foreach (Vector3Int gT in groundTiles)
        {
            MapPos.x = gT.x;
            MapPos.y = gT.y;
            MapPos.z = 0;
            var tile = tiles[gT.z - 1];
            tilemaps[0].SetTile(MapPos, tile);
        }
    }

    public void LoadRocks(List<Vector2Int> rocksTiles)
    {
        var MapPos = new Vector3Int(0, 0, 0);
        foreach (Vector3Int rT in rocksTiles)
        {
            MapPos.x = rT.x;
            MapPos.y = rT.y;
            MapPos.z = 0;
            var tile = tileRock;
            tilemaps[1].SetTile(MapPos, tile);
        }
    }

    public void LoadDTiles(List<Vector3Int> dTiles)
    {
        var MapPos = new Vector3Int(0, 0, 0);
        foreach (Vector3Int dT in dTiles)
        {
            MapPos.x = dT.x;
            MapPos.y = dT.y;
            MapPos.z = 0;
            var tile = diffTiles[dT.z];
            tilemaps[2].SetTile(MapPos, tile);
        }
    }

    public void LoadGrass(int grassSeed)
    {
        float[,] noiseMap = MG2.GenerateNoiseMap(MG2.mapWidth, MG2.mapHeight, MG2.noiseScale, grassSeed);
        var MapPos = new Vector3Int(0, 0, 0);
        for (MapPos.x = -50; MapPos.x <= 50; MapPos = MapPos + Vector3Int.right)
            for (MapPos.y = -50; MapPos.y <= 50; MapPos = MapPos + Vector3Int.up)
            {
                if ((tilemaps[1].HasTile(MapPos) == false) && (tilemaps[2].HasTile(MapPos) == false) && (tilemaps[3].HasTile(MapPos) == false))
                {
                    float currentHeight = noiseMap[MapPos.x + 50, MapPos.y + 50];
                    Color clr = Color.white;
                    Texture2D txtr;
                    Sprite sprt;
                    Tile tile3 = MG2.ChooseTile(currentHeight);
                    if (tile3 != null)
                    {
                        sprt = tilemaps[0].GetSprite(MapPos);
                        txtr = sprt.texture;
                        clr = txtr.GetPixel(100, 50);
                        clr.g = clr.g + 0.2f;
                        clr.r = clr.r + 0.1f;
                        tile3.color = clr;
                        if (currentHeight > MG2.heightThreshold)
                        {
                            tilemaps[3].SetTile(MapPos, tile3);
                        }
                        tile3.color = Color.white;
                    }
                }
            }
    }

    public void LoadGame()
    {
        MG2.ClearGen();
        string filePath = Application.persistentDataPath + "/worldData.json";
        if (File.Exists(filePath))
        {
            prefabObjects.Clear();
            string jsonData = File.ReadAllText(filePath);
            WorldData worldData = JsonUtility.FromJson<WorldData>(jsonData);
            LoadGround(worldData.groundTiles);
            LoadRocks(worldData.rocksTiles);
            LoadDTiles(worldData.diffTiles); 
            LoadGrass(worldData.grassSeed);
            foreach (SavedObject savedObject in worldData.savedObjects)
            {
                GameObject prefab = Resources.Load<GameObject>(savedObject.prefabName) as GameObject;
                if (prefab != null)
                {
                    //GameObject newObject = Instantiate(Resources.Load("Assets/Prefabs/Boar.prefab"), typeof(GameObject))) as GameObject);
                    GameObject newObject = Instantiate(prefab, savedObject.position, Quaternion.identity);
                    Debug.Log("prefab Loaded.");
                }
                else
                {
                    Debug.LogError("Prefab not found: " + savedObject.prefabName);
                }
            }
            Debug.Log("Game Loaded.");
            initLists();
        }
        else
        {
            Debug.Log("No saved game found...");
        }
    }
}
