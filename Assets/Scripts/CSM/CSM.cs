using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CSM : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // don't need start game
    void OnDrawGizmos()
    {
        var mainCam = Camera.main;
        Gizmos.color = Color.green;
        // https://discussions.unity.com/t/drawfrustum-is-drawing-incorrectly/518760/3
        // Gizmos.DrawFrustum(mainCam.transform.position, mainCam.fieldOfView, mainCam.farClipPlane, mainCam.nearClipPlane, mainCam.aspect); // incorrect
        GizmosUtilities.DrawFrustum(mainCam);

        var bounds = FrustumBoundingBox(mainCam, transform);
        Gizmos.color = Color.red;
        GizmosUtilities.DrawWireCube(bounds);
    }



    Vector3[] FrustumBoundingBox(Camera camera, Transform light)
    {
        // near plane
        Vector3[] nearCorners = new Vector3[4];
        // Local space
        camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, nearCorners);
        // To world space
        for (int i = 0; i < 4; i++)
        {
            nearCorners[i] = camera.transform.TransformPoint(nearCorners[i]);
        }

        // far plane
        Vector3[] farCorners = new Vector3[4];
        // Local space
        camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, farCorners);
        // To world space
        for (int i = 0; i < 4; i++)
        {
            farCorners[i] = camera.transform.TransformPoint(farCorners[i]);
        }

        // calculate bounding box
        Vector3 minPoint = new();
        Vector3 maxPoint = new();
        // world to light space
        for (int i = 0; i < 4; i++)
        {
            nearCorners[i] = light.InverseTransformPoint(nearCorners[i]);
            farCorners[i] = light.InverseTransformPoint(farCorners[i]);
        }
        minPoint.x = Mathf.Min(nearCorners.Min((p) => p.x), farCorners.Min((p) => p.x));
        minPoint.y = Mathf.Min(nearCorners.Min((p) => p.y), farCorners.Min((p) => p.y));
        minPoint.z = Mathf.Min(nearCorners.Min((p) => p.z), farCorners.Min((p) => p.z));

        maxPoint.x = Mathf.Max(nearCorners.Max((p) => p.x), farCorners.Max((p) => p.x));
        maxPoint.y = Mathf.Max(nearCorners.Max((p) => p.y), farCorners.Max((p) => p.y));
        maxPoint.z = Mathf.Max(nearCorners.Max((p) => p.z), farCorners.Max((p) => p.z));

        // HINT: two points can only describe a AABB, so we can't just transform this two points
        // minPoint = light.TransformPoint(minPoint);
        // maxPoint = light.TransformPoint(maxPoint);

        var res = new Vector3[8];
        res[0] = new Vector3(minPoint.x, minPoint.y, minPoint.z);
        res[1] = new Vector3(minPoint.x, maxPoint.y, minPoint.z);
        res[2] = new Vector3(maxPoint.x, maxPoint.y, minPoint.z);
        res[3] = new Vector3(maxPoint.x, minPoint.y, minPoint.z);
        res[4] = new Vector3(minPoint.x, minPoint.y, maxPoint.z);
        res[5] = new Vector3(minPoint.x, maxPoint.y, maxPoint.z);
        res[6] = new Vector3(maxPoint.x, maxPoint.y, maxPoint.z);
        res[7] = new Vector3(maxPoint.x, minPoint.y, maxPoint.z);

        // back to world space after get 8 points
        for (int i = 0; i < 8; i++)
        {
            res[i] = light.TransformPoint(res[i]);
        }

        return res;
    }

}