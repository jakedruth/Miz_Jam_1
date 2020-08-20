using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PipePiece))]
public class OnSwitchListener : MonoBehaviour
{
    public ChangePipeEvent[] changePipeEvents;

    public void HandleOnSwitchChanged(bool value)
    {
        PipePiece pipe = GetComponent<PipePiece>();

        foreach (ChangePipeEvent e in changePipeEvents)
        {
            if (value && e.addOrRemoveConnection == ChangePipeEvent.AddRemove.ADD ||
                !value && e.addOrRemoveConnection == ChangePipeEvent.AddRemove.REMOVE)
            {
                if (e.makePad)
                    pipe.isPad = true;

                pipe.AddConnection((int) e.direction);
                
            }
            else
            {
                if (e.makePad)
                    pipe.isPad = false;

                pipe.RemoveConnection((int) e.direction);
            }
        }
    }
}

[System.Serializable]
public struct ChangePipeEvent
{ 
    public enum AddRemove
    {
        ADD,
        REMOVE
    }

    public AddRemove addOrRemoveConnection;
    public Direction direction;
    public bool makePad;
}
