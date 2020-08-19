using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[System.Serializable]
public class PipePiece : MonoBehaviour
{
    private static bool _checkNeighborsOnStart;
    public bool isPad;
    // todo: update render on change

    [SerializeField]
    public PipePiece pipeN;
    [SerializeField]
    public PipePiece pipeE;
    [SerializeField]
    public PipePiece pipeS;
    [SerializeField]
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

    public enum PipeType
    {
        ERROR,
        END,
        CORNER,
        STRAIGHT,
        // ReSharper disable once InconsistentNaming
        T_INTERSECT,
        CROSS,
    }

    public PipeType GetPipeType
    {
        get 
        {
            int i = RenderIndex % 16;
            switch (i)
            {
                default:
                case 0:
                    return PipeType.ERROR;
                case 1:
                case 2:
                case 4:
                case 8:
                    return PipeType.END;
                case 3:
                case 6:
                case 9:
                case 12:
                    return PipeType.CORNER;
                case 5:
                case 10:
                    return PipeType.STRAIGHT;
                case 7:
                case 11:
                case 13:
                case 14:
                    return PipeType.T_INTERSECT;
                case 15:
                    return PipeType.CROSS;
            }
        }
    }

    public static void CalculateNeighborsOfAllPipes()
    {
        PipePiece[] pipes = FindObjectsOfType<PipePiece>();
        foreach (PipePiece pipe in pipes)
        {
            for (int i = 0; i < 4; i++)
            {
                // Check to see if there is a connection
                PipePiece check = pipe.GetPipe(i);
                if (check == null)
                    continue;

                // Check to see if the connection is both ways
                int invertDirection = (i + 2 + 4) % 4;
                if (check.GetPipe(invertDirection) != pipe)
                {
                    // Fix the connection
                    if (i == 0)
                    {
                        pipe.pipeN = check;
                        check.pipeS = pipe;
                    }
                    else if (i == 1)
                    {
                        pipe.pipeE = check;
                        check.pipeW = pipe;
                    }
                    else if (i == 2)
                    {
                        pipe.pipeS = check;
                        check.pipeN = pipe;
                    }
                    else if (i == 3)
                    {
                        pipe.pipeW = check;
                        check.pipeE = pipe;
                    }
                }

                // else, see if we can find a pipe in the right location

                //Vector3 pos = pipe.transform.position;
                //Vector3 checkLocation = pos;

                //if (i == 0)
                //    checkLocation += Vector3.up;
                //else if (i == 1)
                //    checkLocation += Vector3.right;
                //else if (i == 2)
                //    checkLocation += Vector3.down;
                //else if (i == 3)
                //    checkLocation += Vector3.left;

                //foreach (PipePiece comparePipe in pipes)
                //{
                //    if (comparePipe == pipe)
                //        continue;

                //    Vector3 displacement = comparePipe.transform.position - checkLocation;
                //    if (displacement.sqrMagnitude < 0.01f) // close enough to be the at the new location
                //    {
                //        // Join the nextPipe with this pipe
                //        if (i == 0)
                //        {
                //            pipe.pipeN = comparePipe;
                //            comparePipe.pipeS = pipe;
                //        }
                //        else if (i == 1)
                //        {
                //            pipe.pipeE = comparePipe;
                //            comparePipe.pipeW = pipe;
                //        }
                //        else if (i == 2)
                //        {
                //            pipe.pipeS = comparePipe;
                //            comparePipe.pipeN = pipe;
                //        }
                //        else if (i == 3)
                //        {
                //            pipe.pipeW = comparePipe;
                //            comparePipe.pipeE = pipe;
                //        }

                //        break;
                //    }
                //}
            }
        }
    }

    void Start()
    {
        if (!_checkNeighborsOnStart)
        {
            _checkNeighborsOnStart = true;
            CalculateNeighborsOfAllPipes();
        }
    }

    public void UpdateRender()
    {
        int index = RenderIndex;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == index);
        }
    }

    public PipePiece AddConnection(int direction)
    {
        // Find the location of the next pipe
        Vector3 pos = transform.position;
        Vector3 nextPos = pos;

        if (direction == 0)
            nextPos += Vector3.up;
        else if (direction == 1)
            nextPos += Vector3.right;
        else if (direction == 2)
            nextPos += Vector3.down;
        else if (direction == 3)
            nextPos += Vector3.left;
        else
        {
            Debug.LogError("Invalid Direction");
            return null;
        }

        // Determine if a pipe exists at this position;
        PipePiece connection = null;
        foreach (PipePiece p in FindObjectsOfType<PipePiece>())
        {
            if (p == this)
                continue;

            Vector3 displacement = p.transform.position - nextPos;
            if (displacement.sqrMagnitude < 0.01f) // close enough to be the at the new location
            {
                connection = p;
                break;
            }
        }

        // If there is no pipe at the next location, return a null reference
        if (connection == null)
        {
            return null;
        }

        // Join the connection with this pipe
        if (direction == 0)
        {
            pipeN = connection;
            connection.pipeS = this;
        }
        else if (direction == 1)
        {
            pipeE = connection;
            connection.pipeW = this;
        }
        else if (direction == 2)
        {
            pipeS = connection;
            connection.pipeN = this;
        }
        else if (direction == 3)
        {
            pipeW = connection;
            connection.pipeE = this;
        }

        return connection;
    }

    public PipePiece RemoveConnection(int direction)
    {
        PipePiece connection = GetPipe(direction);

        if (connection == null)
            return null;

        if (direction == 0)
            pipeN = null;
        else if (direction == 1)
            pipeE = null;
        else if (direction == 2)
            pipeS = null;
        else if (direction == 3)
            pipeW = null;

        int invertDirection = (direction + 2 + 4) % 4;
        if (invertDirection == 0)
            connection.pipeN = null;
        else if (invertDirection == 1)
            connection.pipeE = null;
        else if (invertDirection == 2)
            connection.pipeS = null;
        else if (invertDirection == 3)
            connection.pipeW = null;

        UpdateRender();
        connection.UpdateRender();

        return connection;
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

    void OnDestroy()
    {
        for (int i = 0; i < 4; i++)
        {
            RemoveConnection(i)?.UpdateRender();
        }
    }
}


