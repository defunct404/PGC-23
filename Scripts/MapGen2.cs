using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using NavMeshPlus.Components;
using Unity.VisualScripting;
using static UnityEditor.PlayerSettings;

public class MapGen2 : MonoBehaviour
{
    public Grid grid;

    public Tile[] tiles, puddles;
    public Tile NE_SW, NE_SE, NE_SW_SE, NW_NE, NW_NE_SE, NW_NE_SW, NW_NE_SW_SE, NW_SE, NW_SW, NW_SW_SE, SW_SE;
    public Tile NW_SE_fence;

    public Tilemap GroundTilemap, CollidiableObjectsTilemap, NonCollidiableObjectsTilemap, GrassTilemap, AnomaliesTilemap;

    public Tile tileRock, tileRocks;
    public Tile[] Grass;
    public Transform[] trees;
    public Transform treeTriggerCollider;

    //public GameObject mc;

    public GameObject boar;
    public GameObject layout;
    public GameObject boars;
    public GameObject navmesh;

    public int mapWidth = 50;
    public int mapHeight = 50;

    public int GrassNoise = 0, ElectraNoise;

    public float noiseScale = 0.1f;
    public float heightThreshold = 0.0f;

    //GameManager gameManager;

    //private void Awake()
    //{
    //    gameManager = FindObjectOfType<GameManager>();
    //    //Cursor.visible = false;
    //    //Gen();
    //}

    void Start()
    {
        //ClearGen();
        //Gen();
        navmesh.GetComponent<NavMeshSurface>().BuildNavMeshAsync();
        //navmesh.GetComponent<NavMeshSurface>().BuildNavMeshAsync();
    }


    private void Update()
    {
        //if (Input.GetKeyUp("escape"))
        //{
        //    Application.Quit();
        //}
        if (Input.GetKeyUp("r"))
        {
            Gen();
            navmesh.GetComponent<NavMeshSurface>().BuildNavMeshAsync();
        }
        //if (Input.GetKeyUp("f"))
        //{
        //    Save mcs = mc.GetComponent<Save>();
        //    if (mcs.enabled == false)
        //        mcs.enabled = true;
        //    else if (mcs.enabled == true)
        //        mcs.enabled = false;
        //}
        //navmesh.GetComponent<NavMeshSurface>().UpdateNavMesh(navmesh.GetComponent<NavMeshSurface>().navMeshData);
        //navmesh.GetComponent<NavMeshSurface>().RemoveData();
    }

    private Vector3 IsoToN(Vector3Int mps)
    {
        float row = mps.x + mps.y + 1;
        float column = mps.x - mps.y;

        row /= 4;
        column /= 2;

        return new Vector3(column, row, 0);
    }

    private void SetTilePro(Vector3Int MP, Vector3Int MPL1, Vector3Int MPL2)
    {
        for (int i = 0; i < tiles.Length; i++)
            for (int j = 0; j < tiles.Length; j++)
                if ((GroundTilemap.GetTile(MPL1) == tiles[i]) && (GroundTilemap.GetTile(MPL2) == tiles[j]))
                {
                    int r1, r2;
                    if (i == j)
                    {
                        if (i <= 4) r1 = 0;
                        else r1 = i - 5;
                        if (i >= tiles.Length - 4) r2 = tiles.Length;
                        else r2 = i + 5;
                        var tile = tiles[Random.Range(r1, r2)];
                        GroundTilemap.SetTile(MP, tile);
                    }
                    else if (i > j)
                    {
                        if (j <= 4) r1 = 0;
                        else r1 = j - 5;
                        if (i >= tiles.Length - 4) r2 = tiles.Length;
                        else r2 = i + 5;
                        var tile = tiles[Random.Range(r1, r2)];
                        GroundTilemap.SetTile(MP, tile);
                    }
                    else if (i < j)
                    {
                        if (i <= 4) r1 = 0;
                        else r1 = i - 5;
                        if (j >= tiles.Length - 4) r2 = tiles.Length;
                        else r2 = j + 5;
                        var tile = tiles[Random.Range(r1, r2)];
                        GroundTilemap.SetTile(MP, tile);
                    }
                }
    }

    private void SpiralGen(Vector3Int MP)
    {
        var MPL1 = MP + Vector3Int.left;
        var MPL2 = MP + Vector3Int.left + Vector3Int.down;
        while (((MP.x <= 50) && (MP.x >= -50)) && ((MP.y <= 50) && (MP.y >= -50)))
        {
            if ((MP.x == MP.y) && (MP.x > 0) && (GroundTilemap.HasTile(MP + Vector3Int.down) == false) && (GroundTilemap.HasTile(MP + Vector3Int.left) == true))
            {
                SetTilePro(MP, MPL1, MPL2);
                MPL1 = MP;
                MP.y--;
            }
            else if ((Math.Abs(MP.x) == MP.y) && (MP.x < 0) && (MP.y > 0) && (GroundTilemap.HasTile(MP + Vector3Int.down) == true) && (GroundTilemap.HasTile(MP + Vector3Int.up) == false))
            {
                SetTilePro(MP, MPL1, MPL2);
                MPL1 = MP;
                MP.y++;
                if (GroundTilemap.HasTile(MPL2 + Vector3Int.up) == true) MPL2.y++;
            }
            else if ((MP.x == Math.Abs(MP.y)) && (MP.x > 0) && (MP.y < 0) && (GroundTilemap.HasTile(MP + Vector3Int.up) == true) && (GroundTilemap.HasTile(MP + Vector3Int.left) == false))
            {
                SetTilePro(MP, MPL1, MPL2);
                MPL1 = MP;
                MP.x--;
            }
            else if ((MP.x == MP.y) && (MP.x < 0) && (MP.y < 0) && (GroundTilemap.HasTile(MP + Vector3Int.right) == true) && (GroundTilemap.HasTile(MP + Vector3Int.up) == false))
            {
                SetTilePro(MP, MPL1, MPL2);
                MPL1 = MP;
                MP.y++;
            }
            else if ((GroundTilemap.HasTile(MP + Vector3Int.left) == true) && (GroundTilemap.HasTile(MP + Vector3Int.down) == false) && (GroundTilemap.HasTile(MP + Vector3Int.up) == true) && (MP.x > 0))
            {
                SetTilePro(MP, MPL1, MPL2);
                if ((Math.Abs(MP.y - 1) != MP.x) && (MP.x > 1))
                {
                    MPL2.y--;
                    //if ((MPL1.y == MPL1.x) && (MP.x > 1)) MPL2.y++;
                }
                MPL1 = MP;
                MP.y--;
            }
            else if ((GroundTilemap.HasTile(MP + Vector3Int.up) == true) && (GroundTilemap.HasTile(MP + Vector3Int.left) == false) && (GroundTilemap.HasTile(MP + Vector3Int.right) == true) && (MP.y < 0))
            {
                SetTilePro(MP, MPL1, MPL2);
                if (((MP.x - 1) != MP.y) && (MP.y < -1))
                {
                    MPL2.x--;
                    //if ((Math.Abs(MPL1.y) == MPL1.x) && (MP.y < -1)) MPL2.x++;
                }
                MPL1 = MP;
                MP.x--;
            }
            else if ((GroundTilemap.HasTile(MP + Vector3Int.right) == true) && (GroundTilemap.HasTile(MP + Vector3Int.up) == false) && (GroundTilemap.HasTile(MP + Vector3Int.down) == true) && (MP.x < 0))
            {
                SetTilePro(MP, MPL1, MPL2);
                if (MP.x < -1)
                {
                    if (GroundTilemap.HasTile(MPL2 + Vector3Int.up) == true) MPL2.y++;
                    //if ((Math.Abs(MP.x) == MP.y + 1) && (MP.y > 0)) 
                    //MPL2.y--;
                    //if ((MPL1.y == MPL1.x) && (MP.x < -1)) MPL2.y--;
                }
                MPL1 = MP;
                MP.y++;
            }
            else if ((GroundTilemap.HasTile(MP + Vector3Int.down) == true) && (GroundTilemap.HasTile(MP + Vector3Int.right) == false) && (GroundTilemap.HasTile(MP + Vector3Int.left) == true) && (MP.y > 0))
            {
                SetTilePro(MP, MPL1, MPL2);
                if ((MP.y != (MP.x + 1)) && (MP.y > 1))
                {
                    MPL2.x++;
                    //if ((MPL1.y == Math.Abs(MPL1.x)) && (MP.y > 1)) MPL2.x--;
                }
                MPL1 = MP;
                MP.x++;
            }
            else if ((MP.y - (Math.Abs(MP.x)) == 1) && (MP.x < 0) && (MP.y > 0) && (GroundTilemap.HasTile(MP + Vector3Int.down) == true) && (GroundTilemap.HasTile(MP + Vector3Int.right) == false))
            {
                SetTilePro(MP, MPL1, MPL2);
                MPL1 = MP;
                MP.x++;
            }
            else break;
            /*Debug.Log("MP:");
            Debug.Log(MP);
            Debug.Log("MPL1:");
            Debug.Log(MPL1);
            Debug.Log("MPL2:");
            Debug.Log(MPL2);*/
        }
    }

    public void GroundTilemapGen(Vector3Int MapPos)
    {
        var tile0 = tiles[Random.Range(0, tiles.Length)];

        GroundTilemap.SetTile(MapPos, tile0);

        MapPos = MapPos + Vector3Int.up;

        for (int i = 0; i < tiles.Length; i++)
        {
            if (GroundTilemap.GetTile(MapPos + Vector3Int.down) == tiles[i])
            {
                int r1, r2;
                if ((i == 0) || (i == 1)) r1 = 0;
                else r1 = i - 2;
                if ((i == tiles.Length) || (i == tiles.Length - 1)) r2 = tiles.Length;
                else r2 = i + 2;
                var tile = tiles[Random.Range(r1, r2)];
                GroundTilemap.SetTile(MapPos, tile);
            }
        }

        MapPos = MapPos + Vector3Int.right;

        SpiralGen(MapPos);
    }
    
    public void FenceGen(Vector3Int MapPos)
    {
        for (MapPos.x = -50; MapPos.x <= 50; MapPos = MapPos + Vector3Int.right)
            for (MapPos.y = -50; MapPos.y <= 50; MapPos = MapPos + Vector3Int.up)
            {
                if (Random.Range(0f, 100f) <= 40f)
                    if (NonCollidiableObjectsTilemap.GetTile(MapPos) == NW_SE)
                    {
                        CollidiableObjectsTilemap.SetTile(MapPos, NW_SE_fence); //using with NonCollidiableObjectsTilemap replaces tile instead of adding
                    }
            }
    }

    public void RoadGen(Vector3Int MapPos)
    {
        int rx = Random.Range(-20, 20);
        int ry = Random.Range(-20, 20);

        MapPos.x = rx;
        MapPos.y = ry;

        var road = NW_NE_SW_SE;

        NonCollidiableObjectsTilemap.SetTile(MapPos, road);

        int r;

        MapPos.x = rx;

        for (MapPos.y = ry - 1; MapPos.y >= -50; MapPos = MapPos + Vector3Int.down)
        {
            if (
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NE_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NE_SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_NE_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_NE_SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == SW_SE)
                )

                if (Random.Range(0f, 100f) <= 97f)
                {
                    road = NW_SE;
                    NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                }
                else
                {
                    r = Random.Range(0, 6);
                    if (r == 0)
                        road = NW_NE;
                    else if (r == 1)
                        road = NW_NE_SE;
                    else if (r == 2)
                        road = NW_NE_SW;
                    else if (r == 3)
                        road = NW_NE_SW_SE;
                    else if (r == 4)
                        road = NW_SW;
                    else if (r == 5)
                        road = NW_SW_SE;
                    NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                }
            else if (Random.Range(0f, 100f) >= 99f)
            {
                r = Random.Range(0, 4);
                if (r == 0)
                    road = NE_SE;
                else if (r == 1)
                    road = NE_SW;
                else if (r == 2)
                    road = SW_SE;
                else if (r == 3)
                    road = NE_SW_SE;
                NonCollidiableObjectsTilemap.SetTile(MapPos, road);
            }
        }

        MapPos.y = ry;

        for (MapPos.x = rx - 1; MapPos.x >= -50; MapPos = MapPos + Vector3Int.left)
        {
            if (
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NE_SW) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NE_SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_NE_SW) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_NE_SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_SW) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_SW_SE)
                )

                if (Random.Range(0f, 100f) <= 97f)
                {
                    road = NE_SW;
                    NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                }
                else
                {
                    r = Random.Range(0, 6);
                    if (r == 0)
                        road = NE_SE;
                    else if (r == 1)
                        road = NE_SW_SE;
                    else if (r == 2)
                        road = NW_NE;
                    else if (r == 3)
                        road = NW_NE_SW;
                    else if (r == 4)
                        road = NW_NE_SW_SE;
                    else if (r == 5)
                        road = NW_NE_SE;
                    NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                }
            else if (Random.Range(0f, 100f) >= 99f)
            {
                r = Random.Range(0, 4);
                if (r == 0)
                    road = NW_SE;
                else if (r == 1)
                    road = NW_SW;
                else if (r == 2)
                    road = SW_SE;
                else if (r == 3)
                    road = NW_SW_SE;
                NonCollidiableObjectsTilemap.SetTile(MapPos, road);
            }
        }

        MapPos.y = ry;

        for (MapPos.x = rx + 1; MapPos.x <= 50; MapPos = MapPos + Vector3Int.right)
        {
            if (
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NE_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NE_SW) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NE_SW_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE_SW) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE_SW_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE_SE)
               )

                if (Random.Range(0f, 100f) <= 97f)
                {
                    road = NE_SW;
                    NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                }
                else
                {
                    r = Random.Range(0, 6);
                    if (r == 0)
                        road = SW_SE;
                    else if (r == 1)
                        road = NE_SW_SE;
                    else if (r == 2)
                        road = NW_NE_SW;
                    else if (r == 3)
                        road = NW_NE_SW_SE;
                    else if (r == 4)
                        road = NW_SW;
                    else if (r == 5)
                        road = NW_SW_SE;
                    NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                }
            else if (Random.Range(0f, 100f) >= 99f)
            {
                r = Random.Range(0, 4);
                if (r == 0)
                    road = NE_SE;
                else if (r == 1)
                    road = NW_NE;
                else if (r == 2)
                    road = NW_SE;
                else if (r == 3)
                    road = NW_NE_SE;
                NonCollidiableObjectsTilemap.SetTile(MapPos, road);
            }
        }

        MapPos.x = rx;

        for (MapPos.y = ry + 1; MapPos.y <= 50; MapPos = MapPos + Vector3Int.up)
        {
            if (
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE_SW) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE_SW_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_SW_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_SW)
               )
                if (Random.Range(0f, 100f) <= 97f)
                {
                    road = NW_SE;
                    NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                }
                else
                {
                    r = Random.Range(0, 6);
                    if (r == 0)
                        road = NE_SE;
                    else if (r == 1)
                        road = NW_NE_SE;
                    else if (r == 2)
                        road = NE_SW_SE;
                    else if (r == 3)
                        road = NW_NE_SW_SE;
                    else if (r == 4)
                        road = NW_NE_SE;
                    else if (r == 5)
                        road = NW_SW_SE;
                    NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                }
            else if (Random.Range(0f, 100f) >= 99f)
            {
                r = Random.Range(0, 4);
                if (r == 0)
                    road = NE_SW;
                else if (r == 1)
                    road = NW_NE;
                else if (r == 2)
                    road = NW_SW;
                else if (r == 3)
                    road = NW_NE_SW;
                NonCollidiableObjectsTilemap.SetTile(MapPos, road);
            }
        }

        float rank = 0f;

        for (MapPos.x = rx - 1; MapPos.x >= -50; MapPos = MapPos + Vector3Int.left)
            for (MapPos.y = ry - 1; MapPos.y >= -50; MapPos = MapPos + Vector3Int.down)
            {
                float ran = Random.Range(rank, 100f);

                if (
                    ((NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NE_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NE_SW_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_NE_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_NE_SW_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_SW_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == SW_SE)) &&
                    ((NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NE_SW) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NE_SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_NE_SW) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_NE_SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_SW) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_SW_SE))
                )
                {
                    r = Random.Range(0, 4);
                    if (r == 0)
                        road = NW_NE;
                    else if (r == 1)
                        road = NW_NE_SE;
                    else if (r == 2)
                        road = NW_NE_SW;
                    else if (r == 3)
                        road = NW_NE_SW_SE;
                    NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                }
                else if (((NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NE_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NE_SW_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_NE_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_NE_SW_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_SW_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == SW_SE)))
                {
                    if (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) != NW_SE)
                    {
                        road = NW_SE;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank += 5f;
                    }
                    else if (ran <= 97f)
                    {
                        road = NW_SE;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank += 5f;
                    }
                    else if (ran >= 97f)
                    {
                        r = Random.Range(0, 2);
                        if (r == 0)
                            road = NW_SW;
                        else if (r == 1)
                            road = NW_SW_SE;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank = 0f;
                    }
                }
                else if (((NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NE_SW) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NE_SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_NE_SW) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_NE_SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_SW) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_SW_SE)))
                {
                    if (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) != NE_SW)
                    {
                        road = NE_SW;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank += 5f;
                    }
                    else if (ran <= 97f)
                    {
                        road = NE_SW;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank += 5f;
                    }
                    else if (ran >= 97f)
                    {
                        r = Random.Range(0, 2);
                        if (r == 0)
                            road = NE_SE;
                        else if (r == 1)
                            road = NE_SW_SE;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank = 0f;
                    }
                }
                else if (Random.Range(0f, 400f) >= 399f)
                {
                    road = SW_SE;
                    NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                }
            }

        rank = 0f;

        for (MapPos.x = rx + 1; MapPos.x <= 50; MapPos = MapPos + Vector3Int.right)
            for (MapPos.y = ry - 1; MapPos.y >= -50; MapPos = MapPos + Vector3Int.down)
            {
                float ran = Random.Range(rank, 100f);
                if (((NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NE_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NE_SW_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_NE_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_NE_SW_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_SW_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == SW_SE)) &&
                    ((NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NE_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NE_SW) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NE_SW_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE_SW) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE_SW_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE_SE)))
                {
                    r = Random.Range(0, 4);
                    if (r == 0)
                        road = NW_SW;
                    else if (r == 1)
                        road = NW_NE_SW;
                    else if (r == 2)
                        road = NW_SW_SE;
                    else if (r == 3)
                        road = NW_NE_SW_SE;
                    NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                }
                else if (((NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NE_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NE_SW_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_NE_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_NE_SW_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == NW_SW_SE) ||
                    (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == SW_SE)))
                {
                    if (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) != NW_SE)
                    {
                        road = NW_SE;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank += 5f;
                    }
                    else if (ran <= 97f)
                    {
                        road = NW_SE;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank += 5f;
                    }
                    else if (ran >= 97f)
                    {
                        r = Random.Range(0, 2);
                        if (r == 0)
                            road = NW_NE;
                        else if (r == 1)
                            road = NW_NE_SE;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank = 0f;
                    }
                }
                else if (((NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NE_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NE_SW) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NE_SW_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE_SW) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE_SW_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE_SE)))
                {
                    if (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) != NE_SW)
                    {
                        road = NE_SW;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank += 5f;
                    }
                    else if (ran <= 97f)
                    {
                        road = NE_SW;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank += 5f;
                    }
                    else if (ran >= 97f)
                    {
                        r = Random.Range(0, 2);
                        if (r == 0)
                            road = SW_SE;
                        else if (r == 1)
                            road = NE_SW_SE;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank = 0f;
                    }
                }
                else if (Random.Range(0f, 400f) >= 399f)
                {
                    road = NE_SE;
                    NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                }
            }

        rank = 0f;

        for (MapPos.x = rx - 1; MapPos.x >= -50; MapPos = MapPos + Vector3Int.left)
            for (MapPos.y = ry + 1; MapPos.y <= 50; MapPos = MapPos + Vector3Int.up)
            {
                float ran = Random.Range(rank, 100f);
                if (((NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE_SE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE_SW) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE_SW_SE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_SE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_SW_SE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_SW)) &&
                    ((NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NE_SW) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NE_SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_NE_SW) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_NE_SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_SW) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_SW_SE)))
                {
                    r = Random.Range(0, 4);
                    if (r == 0)
                        road = NE_SE;
                    else if (r == 1)
                        road = NE_SW_SE;
                    else if (r == 2)
                        road = NW_NE_SE;
                    else if (r == 3)
                        road = NW_NE_SW_SE;
                    NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                }
                else if (((NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE_SE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE_SW) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE_SW_SE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_SE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_SW_SE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_SW)))
                {
                    if (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) != NW_SE)
                    {
                        road = NW_SE;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank += 5f;
                    }
                    else if (ran <= 97f)
                    {
                        road = NW_SE;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank += 5f;
                    }
                    else if (ran >= 97f)
                    {
                        r = Random.Range(0, 2);
                        if (r == 0)
                            road = SW_SE;
                        else if (r == 1)
                            road = NW_SW_SE;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank = 0f;
                    }
                }
                else if (((NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NE_SW) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NE_SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_NE_SW) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_NE_SW_SE) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_SW) ||
                (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == NW_SW_SE)))
                {
                    if (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) != NE_SW)
                    {
                        road = NE_SW;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank += 5f;
                    }
                    else if (ran <= 97f)
                    {
                        road = NE_SW;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank += 5f;
                    }
                    else if (ran >= 97f)
                    {
                        r = Random.Range(0, 2);
                        if (r == 0)
                            road = NW_NE;
                        else if (r == 1)
                            road = NW_NE_SW;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank = 0f;
                    }
                }
                else if (Random.Range(0f, 400f) >= 399f)
                {
                    road = NW_SW;
                    NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                }
            }

        rank = 0f;

        for (MapPos.x = rx + 1; MapPos.x <= 50; MapPos = MapPos + Vector3Int.right)
            for (MapPos.y = ry + 1; MapPos.y <= 50; MapPos = MapPos + Vector3Int.up)
            {
                float ran = Random.Range(rank, 100f);
                if (((NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE_SE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE_SW) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE_SW_SE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_SE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_SW_SE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_SW)) &&
                    ((NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NE_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NE_SW) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NE_SW_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE_SW) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE_SW_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE_SE)))
                {
                    r = Random.Range(0, 4);

                    if (r == 0)
                        road = SW_SE;
                    else if (r == 1)
                        road = NE_SW_SE;
                    else if (r == 2)
                        road = NW_SW_SE;
                    else if (r == 3)
                        road = NW_NE_SW_SE;

                    NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                }
                else if (((NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE_SE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE_SW) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_NE_SW_SE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_SE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_SW_SE) ||
              (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == NW_SW)))
                {
                    if (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) != NW_SE)
                    {
                        road = NW_SE;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank += 5f;
                    }
                    else if (ran <= 97f)
                    {
                        road = NW_SE;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank += 5f;
                    }
                    else if (ran >= 97f)
                    {
                        r = Random.Range(0, 2);
                        if (r == 0)
                            road = NE_SE;
                        else if (r == 1)
                            road = NW_NE_SE;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank = 0f;
                    }
                }
                else if (((NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NE_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NE_SW) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NE_SW_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE_SW) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE_SW_SE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE) ||
               (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == NW_NE_SE)))
                {
                    if (NonCollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) != NE_SW)
                    {
                        road = NE_SW;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank += 5f;
                    }
                    else if (ran <= 97f)
                    {
                        road = NE_SW;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank += 5f;
                    }
                    else if (ran >= 97f)
                    {
                        r = Random.Range(0, 2);
                        if (r == 0)
                            road = NW_SW;
                        else if (r == 1)
                            road = NW_NE_SW;
                        NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                        rank = 0f;
                    }
                }
                else if (Random.Range(0f, 400f) >= 399f)
                {
                    road = NW_NE;
                    NonCollidiableObjectsTilemap.SetTile(MapPos, road);
                }
            }
    }

    public void RockGen(Vector3Int MapPos)
    {
        for (MapPos.x = -51; MapPos.x <= 51; MapPos = MapPos + Vector3Int.right)
            for (MapPos.y = -51; MapPos.y <= 51; MapPos = MapPos + Vector3Int.up)
            {
                if (Math.Abs(MapPos.x) == 51 || Math.Abs(MapPos.y) == 51)
                {
                    var tile1 = tileRock;
                    CollidiableObjectsTilemap.SetTile(MapPos, tile1);
                }
                else if ((Random.Range(0f, 100f) >= 99f) && (NonCollidiableObjectsTilemap.HasTile(MapPos) == false))
                {
                    var tile1 = tileRock;
                    CollidiableObjectsTilemap.SetTile(MapPos, tile1);
                }
            }

        for (MapPos.x = -50; MapPos.x <= 50; MapPos = MapPos + Vector3Int.right)
            for (MapPos.y = -50; MapPos.y <= 50; MapPos = MapPos + Vector3Int.up)
            {
                if ((Random.Range(0f, 100f) >= 80f) && (CollidiableObjectsTilemap.GetTile(MapPos) != tileRock) && (NonCollidiableObjectsTilemap.HasTile(MapPos) == false) && ((CollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down) == tileRock)
            || (CollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up) == tileRock)
            || (CollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.left) == tileRock)
            || (CollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.right) == tileRock)
            || (CollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down + Vector3Int.right) == tileRock)
            || (CollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.down + Vector3Int.left) == tileRock)
            || (CollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up + Vector3Int.right) == tileRock)
            || (CollidiableObjectsTilemap.GetTile(MapPos + Vector3Int.up + Vector3Int.left) == tileRock)))
                {
                    var tile2 = tileRocks;
                    NonCollidiableObjectsTilemap.SetTile(MapPos, tile2);
                }
            }

        for (MapPos.x = -50; MapPos.x <= 50; MapPos = MapPos + Vector3Int.right)
            for (MapPos.y = -50; MapPos.y <= 50; MapPos = MapPos + Vector3Int.up)
            {
                if ((Random.Range(0f, 100f) >= 99f) && (CollidiableObjectsTilemap.GetTile(MapPos) != tileRock) && (NonCollidiableObjectsTilemap.HasTile(MapPos) == false))
                {
                    var tile2 = puddles[Random.Range(0, puddles.Length)];
                    NonCollidiableObjectsTilemap.SetTile(MapPos, tile2);
                }
            }
    }

    public void GrassGen(Vector3Int MapPos, float[,] noiseMap)
    {
        for (MapPos.x = -50; MapPos.x <= 50; MapPos = MapPos + Vector3Int.right)
            for (MapPos.y = -50; MapPos.y <= 50; MapPos = MapPos + Vector3Int.up)
            {
                if (/*(Random.Range(0f, 100f) >= 85f) && */(CollidiableObjectsTilemap.HasTile(MapPos) == false) && (NonCollidiableObjectsTilemap.HasTile(MapPos) == false) && (GrassTilemap.HasTile(MapPos) == false))
                {
                    float currentHeight = noiseMap[MapPos.x + 50, MapPos.y + 50];

                    Color clr = Color.white;
                    Texture2D txtr;
                    Sprite sprt;

                    Tile tile3 = ChooseTile(currentHeight);
                    if (tile3 != null)
                    {
                        sprt = GroundTilemap.GetSprite(MapPos);
                        txtr = sprt.texture;
                        clr = txtr.GetPixel(100, 50);
                        clr.g = clr.g + 0.2f;
                        clr.r = clr.r + 0.1f;
                        tile3.color = clr;
                        if (currentHeight > heightThreshold)
                        {
                            GrassTilemap.SetTile(MapPos, tile3);
                        }
                        tile3.color = Color.white;
                    }
                }
            }
    }

    public void LayoutGen(Vector3Int MapPos)
    {
        for (MapPos.x = -50; MapPos.x <= 50; MapPos = MapPos + Vector3Int.right)
            for (MapPos.y = -50; MapPos.y <= 50; MapPos = MapPos + Vector3Int.up)
            {
                /*if ((Random.Range(0f, 100f) >= 98f) && (CollidiableObjectsTilemap.HasTile(MapPos) == false) && (NonCollidiableObjectsTilemap.HasTile(MapPos) == false))
                {
                    var tile3 = tileTree;
                    GrassTilemap.SetTile(MapPos, tile3);
                    Instantiate(treeTriggerCollider, IsoToN(MapPos), Quaternion.identity);
                }*/
                if ((Random.Range(0f, 100f) >= 98f) && (CollidiableObjectsTilemap.HasTile(MapPos) == false) && (NonCollidiableObjectsTilemap.HasTile(MapPos) == false) && 
                    (GrassTilemap.HasTile(MapPos) == false) && (!AnomaliesTilemap.HasTile(MapPos)))
                {
                    int rn = Random.Range(0, trees.Length);
                    Instantiate(trees[rn], IsoToN(MapPos), Quaternion.identity, layout.transform);
                }
            }
    }

    public void BoarGen(Vector3Int MapPos)
    {
        for (MapPos.x = -50; MapPos.x <= 50; MapPos = MapPos + Vector3Int.right)
            for (MapPos.y = -50; MapPos.y <= 50; MapPos = MapPos + Vector3Int.up)
            {
                if (Random.Range(0f, 100f) >= 99.9f)
                    Instantiate(boar, IsoToN(MapPos), Quaternion.identity, boars.transform);
            }
    }

    public void ElectraGen(Vector3Int MapPos, float[,] noiseMap)
    {
        for (MapPos.x = -50; MapPos.x <= 50; MapPos = MapPos + Vector3Int.right)
            for (MapPos.y = -50; MapPos.y <= 50; MapPos = MapPos + Vector3Int.up)
            {
               if((Random.Range(0f, 100f) >= 98f) && !CollidiableObjectsTilemap.HasTile(MapPos))
                {

                }
            }
    }

    public Tile ChooseTile(float height)
    {
        // Реализуйте свою логику выбора тайла в зависимости от значения высоты
        // Например, можно разбить высоты на диапазоны и возвращать определенные тайлы для каждого диапазона
        // Пример:
        if (height > 0.9f)
        {
            return Grass[5];
        }
        else if (height > 0.8f)
        {
            return Grass[4];
        }
        else if (height > 0.7f)
        {
            return Grass[3];
        }
        else if (height > 0.6f)
        {
            return Grass[2];
        }
        else if (height > 0.5f)
        {
            return Grass[1];
        }
        else if (height > 0.4f)
        {
            return Grass[0];
        }
        // Добавьте другие условия по необходимости

        // Если не соответствует ни одному условию, возвращаем null
        return null;
    }

    public float[,] GenerateNoiseMap(int width, int height, float scale, int seed)
    {
        int Width = 2 * width + 1;
        int Height = 2 * height + 1;

        float[,] noiseMap = new float[Width, Height];

        // Генерация шума Перлина
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int X = x - 50;
                int Y = y - 50;
                float xCoord = X * scale;
                float yCoord = Y * scale;
                float dnewNoise = seed * scale;
                float sample = Mathf.PerlinNoise(xCoord + dnewNoise, yCoord + dnewNoise);
                noiseMap[x, y] = sample;
            }
        }

        return noiseMap;
    }
    
    public void ClearGen()
    {
        navmesh.GetComponent<NavMeshSurface>().RemoveData();

        GroundTilemap.ClearAllTiles();
        CollidiableObjectsTilemap.ClearAllTiles();
        NonCollidiableObjectsTilemap.ClearAllTiles();
        GrassTilemap.ClearAllTiles();
        AnomaliesTilemap.ClearAllTiles();

        //Destroy(GameObject.Find("Tilemap (3)(Clone)"));

        while (GameObject.FindWithTag("Tree") != null)
            DestroyImmediate(GameObject.FindWithTag("Tree"));

        while (GameObject.FindWithTag("Boar") != null)
        {
            GameObject.FindWithTag("Boar").GetComponent<BoarMove>().enabled = false;
            DestroyImmediate(GameObject.FindWithTag("Boar"));
        }

        while (GameObject.FindWithTag("Barrel") != null)
            DestroyImmediate(GameObject.FindWithTag("Barrel"));
    }

    public void Gen()
    {
        ClearGen();

        var MapPos = new Vector3Int(0, 0, 0);

        GroundTilemapGen(MapPos);

        RoadGen(MapPos);
        FenceGen(MapPos);

        RockGen(MapPos);

        GrassNoise = Random.Range(0, 10000);
        float[,] noiseMap = GenerateNoiseMap(mapWidth, mapHeight, noiseScale, GrassNoise);
        GrassGen(MapPos, noiseMap);

        ElectraNoise = Random.Range(0, 10000);
        noiseMap = GenerateNoiseMap(mapWidth, mapHeight, noiseScale, ElectraNoise);
        ElectraGen(MapPos, noiseMap);

        //NOTE: использовать после всех генераций tilemap
        //после внедрения нового tilemap с объектами, находящимися НА земле, добавить в LayoutGen в if дополнительное условие && !*.HasTile(MapPos)
        LayoutGen(MapPos);

        BoarGen(MapPos);


        /*Tilemap tilemap4 = Instantiate(GrassTilemap, GrassTilemap.transform.localPosition, Quaternion.identity);
        tilemap4.transform.SetParent(grid.transform);
        tilemap4.transform.position = tilemap4.transform.position + new Vector3(0, 1, 0);
        TilemapCollider2D tcollider = tilemap4.GetComponent<TilemapCollider2D>();
        tcollider.isTrigger = true;
        TilemapRenderer trenderer3 = GrassTilemap.GetComponent<TilemapRenderer>();
        TilemapRenderer trenderer4 = tilemap4.GetComponent<TilemapRenderer>();
        trenderer3.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        trenderer4.enabled = false;*/
    }
}