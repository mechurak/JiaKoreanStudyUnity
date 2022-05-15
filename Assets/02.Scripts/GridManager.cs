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
    private int startIndex = -1;
    private int curIndex = -1;
    private int curDirection = 0;

    // Start is called before the first frame update
    void Start()
    {
        
        blocks = new GameObject[gridSize.y * gridSize.x];        

        for (int y = 0; y < gridSize.y; y++)
        {
            float posY = bottomLeft.position.y + ((topRight.position.y - bottomLeft.position.y) / (gridSize.y - 1)) * y;
            for (int x = 0; x < gridSize.x; x++)
            {
                float posX = bottomLeft.position.x + ((topRight.position.x - bottomLeft.position.x) / (gridSize.x - 1)) * x;                
                GameObject tempBlock = Instantiate(blockPrefab, new Vector3(posX, posY, 0), Quaternion.identity);
                blocks[y * gridSize.y + x] = tempBlock;
                tempBlock.GetComponent<Block>().SetBlockId(y * gridSize.y + x);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool input = false;
        Vector3 touchPosition = new Vector3();
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            touchPosition = Input.mousePosition;
            input = true;
        }
#else
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            input = true;
        }
#endif

        if (input)
        {
            RaycastHit hitObj;
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            if (Physics.Raycast(ray, out hitObj, Mathf.Infinity))
            {
                if (hitObj.transform.tag == "block")
                {                    
                    int blockId = hitObj.transform.GetComponent<Block>().GetBlockId();
                    if (startIndex == -1)
                    {
                        startIndex = blockId;
                        Debug.Log($"Start! blockId={blockId}");
                        blocks[blockId].GetComponent<Block>().Select();
                    } else if (curIndex != blockId && blockId != startIndex)
                    {
                        // TODO: Cache already checked block

                        SetCurrentBlock(blockId);
                    }
                }
            }
        } else
        {
            // Touch End
            if (startIndex != -1)
            {
                Debug.Log($"End!! startIndex={startIndex}, curIndex={curIndex}");
                Evaluate();                
            }
        }
    }

    private int GetDirection(int candidateIndex)
    {
        int startRow = startIndex / gridSize.y;
        int candidateRow = candidateIndex / gridSize.y;
        if (startRow == candidateRow)
        {
            Debug.Log($"startRow: {startRow}, candidateRow: {candidateRow}");
            return candidateIndex > startIndex ? 1 : -1;
        }

        int startCol = startIndex % gridSize.x;
        int candidateCol = candidateIndex % gridSize.x;
        if (startCol == candidateCol)
        {
            Debug.Log($"startCol: {startCol}, candidateCol: {candidateCol}");
            return candidateIndex > startIndex ? gridSize.x : -gridSize.x;
        }

        int dx = candidateRow - startRow;
        int dy = candidateCol - startCol;
        if (Mathf.Abs(dx) == Mathf.Abs(dy))
        {
            if (dx * dy > 0)
            {
                int largeCandidate = gridSize.x + 1; // up + right
                Debug.Log($"diagonal! /");
                return candidateIndex > startIndex ? largeCandidate : -largeCandidate;
            } else
            {
                int smallCandidate = -gridSize.x + 1; // down + right
                Debug.Log($"diagonal! \\");
                return candidateIndex < startIndex ? smallCandidate : -smallCandidate;
            }
        }

        return 0;  // ignore
    }

    private void SetCurrentBlock(int candidateIndex)
    {
        int direction = GetDirection(candidateIndex);
        if (direction != 0)
        {
            curIndex = candidateIndex;
            curDirection = direction;
            Debug.Log($"Update curIndex! blockId={curIndex}");
            foreach (GameObject block in blocks)
            {
                block.GetComponent<Block>().Prepare();
            }

            int temp = startIndex;
            while (temp != curIndex)
            {
                blocks[temp].GetComponent<Block>().Select();
                temp += direction;
            }
            blocks[temp].GetComponent<Block>().Select();
        }
    }

    private void Evaluate()
    {
        int temp = startIndex;
        while (curIndex != -1 && temp != curIndex)
        {
            blocks[temp].GetComponent<Block>().Clear();
            temp += curDirection;
        }
        blocks[temp].GetComponent<Block>().Clear();

        startIndex = -1;
        curIndex = -1;
        curDirection = 0;
    }
}
