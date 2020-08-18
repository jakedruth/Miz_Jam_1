using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PipePiece : MonoBehaviour
{
    public bool isPad;

    public PipePiece pipeN;
    public PipePiece pipeE;
    public PipePiece pipeS;
    public PipePiece pipeW;

    public int RenderIndex
    {
        get
        {
            int index = 0;

            if (pipeN) index += 1;
            if (pipeE) index += 2;
            if (pipeS) index += 4;
            if (pipeW) index += 8;
            if (isPad)    index += 16;

            return index;
        }
    }

    void Update()
    {
        int index = RenderIndex;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == index);
        }
    }

    public List<PipePiece> GetPath(int direction, List<PipePiece> list = null)
    {
        if (list == null)
            list = new List<PipePiece>();

        PipePiece nextInPath = GetPipe(direction);
        if (nextInPath != null)
        {
            list.Add(nextInPath);
            return nextInPath.GetPath(direction, list);
        }
        else
        {
            // TODO: finish this algorithm
        }

        return list;
    }

    public PipePiece GetPipe(int direction)
    {
        switch (direction)
        {
            case 0:
                return pipeN;
            case 1:
                return pipeE;
            case 2:
                return pipeS;
            case 3:
                return pipeW;
            default:
                return null;
        }
    }


}


