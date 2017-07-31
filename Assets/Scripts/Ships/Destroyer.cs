using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Mime;

public class Destroyer : Ship
{
    /// <summary>
    /// The torpedo cap.
    /// </summary>
    public int torpedoCap;
    /// <summary>
    /// The amount of torpedoes this destroyer has.
    /// </summary>
    public float torpedoes;
    /// <summary>
    /// The amount of torpedoes resupplied each turn.
    /// </summary>
    public float reloadRate;
    /// <summary>
    /// The firing angles.
    /// </summary>
    public float[] firingAngles;

    public override void OnBattleBegin ()
    {
        base.OnBattleBegin();
        //owner.destroyer = this;

    }

    public override void OnTurnBegin ()
    {
        base.OnTurnBegin();
        torpedoes = Mathf.Clamp( torpedoes + reloadRate, 0, torpedoCap );
    }

    public override void OnShipPlace ( Ship ship )
    {
        base.OnShipPlace( ship );
        if (ship.type == ShipType.AIRCRAFT_CARRIER)
        {
            CalculateReloadRate();
        }
    }

    public void CalculateReloadRate ()
    {
        float boardDimensions = owner.board.sideTileLength;
        float distance = Vector3.Distance( transform.position, owner.aircraftCarrier.transform.position );
        reloadRate = 1.2f / Mathf.Pow( distance, 0.8f );
    }

    public void UpdateFiringArc ()
    {
        if (ShipPositioner.currentPlayer == owner)
        {
            if (!ShipPositioner.shipsToPlace.Contains( this ))
            {
                List<float> calc = new List<float>() { -90, 90 };

                foreach (Ship ship in owner.ships)
                {
                    if (!ShipPositioner.shipsToPlace.Contains( ship ) && ship != this)
                    {
                        float lowerLimit = Mathf.Infinity;
                        float upperLimit = Mathf.NegativeInfinity;
                        Vector3[] positions = {
                            ship.transform.TransformPoint(Vector3.forward * ship.length / 2.0f),
                            ship.transform.TransformPoint(Vector3.back * ship.length / 2.0f)
                        };

                        foreach (Vector3 position in positions)
                        {
                            Vector3 relativePosition = position - transform.position;
                            Vector2 finalPosition = new Vector2( relativePosition.z, relativePosition.x );
                            float angle = Mathf.Atan2( finalPosition.y, finalPosition.x ) * Mathf.Rad2Deg;

                            if (angle < lowerLimit)
                            {
                                lowerLimit = angle;
                            }

                            if (angle > upperLimit)
                            {
                                upperLimit = angle;
                            }
                        }



                        for (int i = 0; i < calc.Count; i += 2)
                        {
                            float lowerAngle = calc[i];
                            float upperAngle = calc[i + 1];
                            if (lowerLimit > lowerAngle && lowerLimit < upperAngle && upperLimit > lowerAngle && upperLimit < upperAngle)
                            {
                                calc.Insert( i + 1, upperLimit );
                                calc.Insert( i + 1, lowerLimit );
                                break;
                            }

                            if (lowerLimit > lowerAngle && lowerLimit < upperAngle)
                            {
                                calc.RemoveAt( i + 1 );
                                calc.Insert( i + 1, lowerLimit );
                            }
                            else if (upperLimit > lowerAngle && upperLimit < upperAngle)
                            {
                                calc.RemoveAt( i );
                                calc.Insert( i, upperLimit );
                            }
                        }
                    }
                }

                firingAngles = calc.ToArray();
            }
        }
    }

    /// <summary>
    /// Checks the line of fire.
    /// </summary>
    /// <returns><c>true</c>, if line of fire is free, <c>false</c> otherwise.</returns>
    /// <param name="angle">Angle.</param>
    public bool CheckLineOfFire ( float angle )
    {
        for (int i = 0; i < firingAngles.Length; i += 2)
        {
            float lowerAngle = firingAngles[i];
            float upperAngle = firingAngles[i + 1];
            if (angle > lowerAngle && angle < upperAngle)
            {
                return true;
            }
        }

        return false;
    }
}
