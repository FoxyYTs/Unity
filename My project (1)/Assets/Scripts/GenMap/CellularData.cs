using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularData : MonoBehaviour{
    public float fillPercent = 0.5f;
    public int iterations = 1;

    public int[,] GenerateData(int w, int h){
        int[,] mapData = new int[w, h];
        int y = 3;
        while(y < (h/1.5)){
            for(int x = 0; x < w; x++){
                float rand = Random.Range(0f, 1f);
                int miss = Random.Range(2, 16);
                Debug.Log(rand);

                if(rand < fillPercent && x == 17){
                    Platform(0, mapData, x, y);
                }else if(rand > fillPercent && x == 0){
                    Platform(1, mapData, x, y);
                }else if(x != 0 && x != 17){
                    Platform(2, mapData, miss, y);
                }
                y+=3;
                
            }
        }        
        return mapData;
    }
    private void Platform(int tipo, int[,] mapData, int x, int y){
        if(tipo == 0){
            mapData[x-2, y] = 1;
            mapData[x-1, y] = 1;
            mapData[x, y] = 1;
        }else if(tipo == 1){
            mapData[x, y] = 1;
            mapData[x+1, y] = 1;
            mapData[x+2, y] = 1;
        }else if(tipo == 2){
            mapData[x-1, y] = 1;
            mapData[x, y] = 1;
            mapData[x+1, y] = 1;
        }
        
    }  

    public int[,] GeneratseData(int w, int h){
        int[,] mapData = new int[w, h];

        for(int x = 0; x < w; x++){
            for(int y = 0; y < h; y++){
                float chance = Random.Range(0f, 1f);
                mapData[x, y] = chance < this.fillPercent ? 1 : 0;
            }
        }

        int[,] buffer = new int[w, h];

        for(int x = 0; x < this.iterations; x++){
            for(int i = 0; i < w; i++){
                for(int j = 0; j < h; j++){
                    if(((i == 0) || (i== w - 1)) || ((j == 0) || (j == h - 1))){
                        buffer[i, j] = 0;
                        continue;
                    }

                    var count = mapData[i, j];

                    //vertical
                    count += mapData[i, j - 1];
                    count += mapData[i, j + 1];
                    //horizontal
                    count += mapData[i - 1, j];
                    count += mapData[i + 1, j];
                    //diagonal
                    count += mapData[i - 1, j - 1];
                    count += mapData[i - 1, j + 1];
                    count += mapData[i + 1, j - 1];
                    count += mapData[i + 1, j + 1];

                    if(count < 4){
                        buffer[i, j] = 0;
                    }else if(count > 4){
                        buffer[i, j] = 1;
                    }
                }
            }
            for (int i = 0; i < w; i++){
                for (int j = 0; j < h; j++){
                    mapData[i, j] = buffer[i, j];
                }
            }
        }
        return mapData;
    }
    // Start is called before the first frame update
}
