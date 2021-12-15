using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackSegment : MonoBehaviour
{
    public int segmentLength = 1;
    public Transform StartPivot;
    public Transform EndPivot;
    public float horizontalFlipChance = 0;
    [Space]
    public Transform[] overlapCheckOrigins;
    public static float RAY_LENGTH = 15f;
    public static float TILE_UNIT_SIZE = 10f;
    public static float SPHERE_CAST_RADIUS = 4.3f;



    // Transform this object so that the pose of it's start pivot matches the targetPivot
    //   Returns true if snap was successful, and false otherwise
    public bool SnapStartPivotToTransform(Transform targetPivot)
    {
        if (Random.Range(0f,1f) < horizontalFlipChance)
        {
            transform.localScale -= transform.localScale.x * Vector3.right * 2;
        }

        // This is the pivot of this sgement that we want to use for snapping
        Transform myPivot = StartPivot;


        // Calculate and apply transformation needed
        Quaternion rotationDifference = targetPivot.rotation * Quaternion.Inverse(myPivot.rotation);
        transform.rotation = rotationDifference * transform.rotation;
        transform.position += targetPivot.position - myPivot.transform.position;
        //Vector3 positionDifference = targetPivot.position - myPivot.TransformPoint(rotationDifference * myPivot.localPosition);


        // Validate that there isn't already track segment here
        foreach (Transform currOrigin in overlapCheckOrigins)
        {
            Vector3 origin = currOrigin.position + Vector3.up * RAY_LENGTH / 2;

            if (Physics.SphereCast(origin, SPHERE_CAST_RADIUS, Vector3.down, out _, RAY_LENGTH))
            {
                Debug.DrawRay(origin, Vector3.down * RAY_LENGTH, Color.red, 10);
                return false;
            } else
            {
                //Debug.DrawRay(origin, Vector3.down * RAY_LENGTH, Color.green, 10);
            }
        }



        // Make sure that another piece can be placed given that this one is placed
        Vector3 lookaheadCastOffset = EndPivot.forward * TILE_UNIT_SIZE / 2 + Vector3.up * RAY_LENGTH / 2;
        if (Physics.SphereCast(EndPivot.position + lookaheadCastOffset, SPHERE_CAST_RADIUS, Vector3.down, out _, RAY_LENGTH))
        {
            Debug.DrawRay(EndPivot.position, lookaheadCastOffset, Color.magenta, 10);
            Debug.DrawRay(EndPivot.position + lookaheadCastOffset, Vector3.down * RAY_LENGTH, Color.black, 10);
            return false;
        }
        else
        {
            //Debug.DrawRay(EndPivot.position, lookaheadCastOffset, Color.blue, 10);
            //Debug.DrawRay(EndPivot.position + lookaheadCastOffset, Vector3.down * RAY_LENGTH, Color.white, 10);
        }


        return true;
    }
}
