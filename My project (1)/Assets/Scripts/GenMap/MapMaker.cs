using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapMaker : MonoBehaviour{
    public RuleTile tile;
    public Tilemap tilemap;
    public int mapWidth;
    public int mapHeight;
    public int[,] mapData;
    public CellularData cell;
    public PerlinData perlin;
    void Start(){
        this.mapData = this.cell.GenerateData(this.mapWidth, this.mapHeight);

        this.GenerateTiles();
    }

    void GenerateTiles(){
        for(int x = 0; x < this.mapWidth; x++){
            for(int y = 0; y < this.mapHeight; y++){
                if(this.mapData[x, y] == 1){
                    this.tilemap.SetTile(new Vector3Int(x, y, 0), this.tile);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
