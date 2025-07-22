using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Necesara pentru .OrderBy si .Any

public class MazeGenerator : MonoBehaviour
{
    public static MazeGenerator Instance; // Singleton

    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject coinPrefab;
    public GameObject bunnyPrefab;
    public GameObject hyenaPrefab;
    private float cellSize = 1f;
    private float wallThickness = 0.3f;
    private float overlapOffset = 0.05f;
    private int _maxCoins = 76;
    private int remainingCoins;
    private int _lastRecordedCoins = 0;
    private int _remainingCoins = -1; //  adăugăm o variabilă fixă
    private List<int> _coinsBeforeJumpList = new List<int>();
    // monede colectate înainte de fiecare salt

    public float tileSize = 1f;

    public int[,] maze = new int[,] {
     {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
     {1,4,2,2,1,2,2,2,1,2,2,2,2,2,1},
     {1,2,1,2,1,2,1,2,1,2,1,1,1,2,1},
     {1,2,1,2,0,0,1,2,2,2,0,0,1,2,1},
     {1,2,1,1,1,1,1,1,1,1,1,2,1,2,1},
     {1,2,2,2,2,2,2,2,2,2,1,2,1,2,1},
     {1,2,1,1,1,1,1,1,1,0,1,2,1,2,1},
     {1,2,1,0,0,0,0,2,1,0,1,2,1,2,1},
     {1,2,1,0,1,1,0,2,1,0,1,2,1,2,1},
     {1,2,1,0,1,3,0,2,0,0,1,2,1,2,1},
     {1,2,1,0,1,1,1,1,1,1,1,2,1,2,1},
     {1,2,2,0,0,0,2,2,2,2,2,2,1,4,1}, // a doua hiena
     {1,1,1,1,1,1,1,1,1,1,1,1,1,2,1},
     {1,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
     {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
    };

    private int _targetCoinsPerMaze;
    private int _targetHyenasPerMaze; // Noua variabila pentru numarul de hiene
    public int totalCoinsCollectedByPlayer = 0;


    void Start()
    {
        Instance = this;

        _targetCoinsPerMaze = 0;
        _targetHyenasPerMaze = 0;
        _maxCoins = 0;

        for (int y = 0; y < maze.GetLength(0); y++)
        {
            for (int x = 0; x < maze.GetLength(1); x++)
            {
                if (maze[y, x] == 2)
                {
                    _targetCoinsPerMaze++;
                    _maxCoins++;
                }
                if (maze[y, x] == 4)
                {
                    _targetHyenasPerMaze++;
                }
            }
        }

        if (_remainingCoins == -1) 
            _remainingCoins = _maxCoins;

        GenerateMaze();
    }



    public void CoinCollected()
    {
        totalCoinsCollectedByPlayer++;
        _remainingCoins--;

        Debug.Log($"[COIN] Ai colectat o monedă! Rămase: {_remainingCoins}");

        if (_remainingCoins <= 0)
        {
            Debug.Log("[RESET] Toate monedele au fost colectate!");
        }
    }





    void GenerateMaze()
    {
        GameObject bunny = GameObject.FindGameObjectWithTag("Player");
        if (bunny != null)
        {
            BunnyController bc = bunny.GetComponent<BunnyController>();
            if (bc != null)
            {
                bc.enabled = false;
            }
        }

        // Distruge toate obiectele din labirint (excluzând jucătorul care e în afara acestui transform)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int y = 0; y < maze.GetLength(0); y++)
        {
            for (int x = 0; x < maze.GetLength(1); x++)
            {
                Vector3 position = new Vector3(x * tileSize, 0, -y * tileSize);
                int cell = maze[y, x];

                Instantiate(floorPrefab, position, Quaternion.identity, transform);

                if (cell == 1)
                {
                    GameObject wall = Instantiate(wallPrefab, position + Vector3.up * 0.5f, Quaternion.identity, transform);
                    wall.transform.localScale = new Vector3(1f, 0.4f, 1f);
                }
                else if (cell == 2)
                {
                    Instantiate(coinPrefab, position + Vector3.up * 0.5f, Quaternion.identity, transform);
                }
                else if (cell == 3) // Iepurașul (Player)
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                    {
                        player.transform.position = position + Vector3.up * 0.5f;
                        player.transform.rotation = Quaternion.identity;

                        Rigidbody rb = player.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.linearVelocity = Vector3.zero;
                            rb.angularVelocity = Vector3.zero;
                        }
                    }
                    else
                    {
                        Instantiate(bunnyPrefab, position + Vector3.up * 0.5f, Quaternion.identity);
                    }
                }
                else if (cell == 4) // Hiena
                {
                    Instantiate(hyenaPrefab, position + Vector3.up * 0.5f, Quaternion.identity, transform);
                }
            }
        }

        // Reactivăm controlul iepurașului doar dacă e în scenă
        bunny = GameObject.FindGameObjectWithTag("Player");
        if (bunny != null)
        {
            BunnyController bc = bunny.GetComponent<BunnyController>();
            if (bc != null)
            {
                bc.enabled = true;
            }
        }
    }


    public bool IsWalkable(int x, int y)
    {
        if (y < 0 || y >= maze.GetLength(0) || x < 0 || x >= maze.GetLength(1))
            return false;

        return maze[y, x] != 1;
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / tileSize);
        int y = Mathf.RoundToInt(-worldPos.z / tileSize);
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        float x = gridPos.x * tileSize;
        float z = -gridPos.y * tileSize;
        return new Vector3(x, 0, z);
    }

    public void RegisterCoinsBeforeJump()
    {
        _coinsBeforeJumpList.Add(totalCoinsCollectedByPlayer);
        Debug.Log($"[Salt] Monede colectate înainte de salt: {totalCoinsCollectedByPlayer}");
    }

}

