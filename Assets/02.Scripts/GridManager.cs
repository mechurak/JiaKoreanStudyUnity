using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Vector2Int gridSize = new Vector2Int(5, 5);
    public GameObject blockPrefab;
    public Transform bottomLeft;
    public Transform topRight;

    private GameObject[] blocks;

    // Start is called before the first frame update
    void Start()
    {
        
        blocks = new GameObject[gridSize.y * gridSize.x];        

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                float posX = bottomLeft.position.x + ((topRight.position.x - bottomLeft.position.x) / (gridSize.x - 1)) * x;
                float posY = bottomLeft.position.y + ((topRight.position.y - bottomLeft.position.y) / (gridSize.y - 1)) * y;
                blocks[y * gridSize.y + x] = Instantiate(blockPrefab, new Vector3(posX, posY, 0), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
