using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTrajectory : MonoBehaviour
{
    public LineRenderer lr;

    public int lrCount = 20;

    List<Vector3> lrPoints = new List<Vector3>();

    #region Singleton

    public static DrawTrajectory Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion
    public void UpdateTrajectory(Vector3 forceVector, Rigidbody rib, Vector3 startingPoint)
    {
        //Debug.Log("Draw");
        Vector3 velocity = (forceVector / rib.mass) * Time.fixedDeltaTime;

        float FlightDuration = (350 * velocity.y) / Physics.gravity.y;

        float stepTime = FlightDuration / lrCount;

        lrPoints.Clear();

        for (int i = 0; i < lrCount; i++)
        {
            float stepTimePassed = stepTime * i;

            Vector3 MovementVector = new Vector3(
                velocity.x * stepTimePassed,
                velocity.y * stepTimePassed - 0.5f,
                velocity.z * stepTimePassed);

            lrPoints.Add(-MovementVector + startingPoint);
        }

        lr.positionCount = lrPoints.Count;
        lr.SetPositions(lrPoints.ToArray());
    }

    public void HideLine()
    {
        lr.positionCount = 0;
    }
}
