using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerEnter : MonoBehaviour
{
    public PipePiece pipe;
    public ChangePipeEvent changePipeEvents;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            {
                pipe.isPad = changePipeEvents.makePad;

                if (changePipeEvents.addOrRemoveConnection == ChangePipeEvent.AddRemove.ADD)
                {

                    pipe.AddConnection((int) changePipeEvents.direction);
                }
                else
                {
                    pipe.RemoveConnection((int) changePipeEvents.direction);
                }
            }

            Destroy(gameObject);
        }
    }
}
