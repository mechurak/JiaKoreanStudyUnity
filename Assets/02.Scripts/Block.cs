using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Material idle;
    public Material selected;

    enum State
    {
        Idle,
        Selected,
        Fixed,
    }


    private int blockId;
    private State state = State.Idle;

    public void SetBlockId(int id)
    {
        blockId = id;
    }

    public int GetBlockId()
    {
        return blockId;
    }

    public void Prepare()
    {
        if (state != State.Fixed)
        {
            state = State.Idle;
            transform.GetComponent<Renderer>().material = idle;
        }
    }

    public void Clear()
    {
        state = State.Idle;
        transform.GetComponent<Renderer>().material = idle;
    }

    public void Select()
    {
        state = State.Selected;
        transform.GetComponent<Renderer>().material = selected;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<Renderer>().material = idle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
