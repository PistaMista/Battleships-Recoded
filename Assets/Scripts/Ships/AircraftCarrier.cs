using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using System.ComponentModel;

public class AircraftCarrier : Ship
{
    public Player aircraftTarget;
    public int flightTime;
    public string sweepDirection;
    public float sweepPosition;
    public float sweepStep;
    public int sweepIndication;

    public override void OnShipPlace ( Ship ship )
    {
        base.OnShipPlace( ship );
        if (ship.type == ShipType.DESTROYER)
        {
            owner.destroyer.CalculateReloadRate();
        }
    }

    public bool SendAircraftToPlayer ( Player player, int time, string sweepDirection )
    {
        if (flightTime > 0 || destroyed)
        {
            return false;
        }

        aircraftTarget = player;
        if (player != null)
        {
            flightTime = time;
            this.sweepDirection = sweepDirection;
            sweepStep = player.board.sideTileLength / (float)time;
            sweepPosition = -player.board.sideTileLength / 2.0f;
        }
        else
        {
            flightTime = 0;
        }


        return true;
    }

    public override void OnTurnEnd ()
    {
        base.OnTurnEnd();
        if (!destroyed)
        {
            if (flightTime > 0)
            {
                sweepPosition += sweepStep;
                float minimumDistance = Mathf.Infinity;
                int finalDirection = 0;

                foreach (Ship ship in aircraftTarget.ships)
                {
                    if (!ship.destroyed)
                    {
                        Vector3 relativePos = ship.transform.localPosition - Vector3.one * sweepPosition;
                        float difference = sweepDirection == "HORIZONTAL" ? relativePos.x : relativePos.z;
                        float distance = Mathf.Abs( difference );
                        if (distance < minimumDistance)
                        {
                            finalDirection = (int)Mathf.Sign( difference );
                            minimumDistance = distance;
                        }
                    }
                }

                sweepIndication = finalDirection;
                flightTime--;
            }
            else
            {
                SendAircraftToPlayer( null, 0, "HORIZONTAL" );
            }
        }
    }
}
