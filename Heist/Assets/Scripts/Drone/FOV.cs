using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Drone
{
    public class FOV : MonoBehaviour
    {
        public float edgeDstThreshold;
        public int edgeResolveIterations;

        public float maskCutawayDst = .1f;

        public float meshResolution;
        public LayerMask obstacleMask;

        public LayerMask targetMask;

        [Range(0, 360)] public float viewAngle;

        public float viewRadius;

        public List<Transform> visibleTargets = new List<Transform>();

        //public MeshFilter viewMeshFilter;
        //Mesh viewMesh;

        private void Start()
        {
            //viewMesh = new Mesh();
            //viewMesh.name = "View Mesh";
            //viewMeshFilter.mesh = viewMesh;

            StartCoroutine("FindTargetsWithDelay", .2f);
        }


        private IEnumerator FindTargetsWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                FindVisibleTargets();
            }
        }

        private void LateUpdate()
        {
            DrawFieldOfView();
        }

        private void FindVisibleTargets()
        {
            visibleTargets.Clear();
            var targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

            for (var i = 0; i < targetsInViewRadius.Length; i++)
            {
                var target = targetsInViewRadius[i].transform;
                var dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    var dstToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                        visibleTargets.Add(target);
                }
            }
        }

        private void DrawFieldOfView()
        {
            var stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
            var stepAngleSize = viewAngle / stepCount;
            var viewPoints = new List<Vector3>();
            var oldViewCast = new ViewCastInfo();
            for (var i = 0; i <= stepCount; i++)
            {
                var angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
                var newViewCast = ViewCast(angle);

                if (i > 0)
                {
                    var edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                    if (oldViewCast.hit != newViewCast.hit ||
                        oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded)
                    {
                        var edge = FindEdge(oldViewCast, newViewCast);
                        if (edge.pointA != Vector3.zero) viewPoints.Add(edge.pointA);
                        if (edge.pointB != Vector3.zero) viewPoints.Add(edge.pointB);
                    }
                }


                viewPoints.Add(newViewCast.point);
                oldViewCast = newViewCast;
            }

            var vertexCount = viewPoints.Count + 1;
            var vertices = new Vector3[vertexCount];
            var triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;
            for (var i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + Vector3.forward * maskCutawayDst;

                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            //viewMesh.Clear();

            //viewMesh.vertices = vertices;
            //viewMesh.triangles = triangles;
            //viewMesh.RecalculateNormals();
        }


        private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            var minAngle = minViewCast.angle;
            var maxAngle = maxViewCast.angle;
            var minPoint = Vector3.zero;
            var maxPoint = Vector3.zero;

            for (var i = 0; i < edgeResolveIterations; i++)
            {
                var angle = (minAngle + maxAngle) / 2;
                var newViewCast = ViewCast(angle);

                var edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = newViewCast.point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCast.point;
                }
            }

            return new EdgeInfo(minPoint, maxPoint);
        }


        private ViewCastInfo ViewCast(float globalAngle)
        {
            var dir = DirFromAngle(globalAngle, true);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }

        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal) angleInDegrees += transform.eulerAngles.y;
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        public struct ViewCastInfo
        {
            public bool hit;
            public Vector3 point;
            public float dst;
            public float angle;

            public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
            {
                hit = _hit;
                point = _point;
                dst = _dst;
                angle = _angle;
            }
        }

        public struct EdgeInfo
        {
            public Vector3 pointA;
            public Vector3 pointB;

            public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
            {
                pointA = _pointA;
                pointB = _pointB;
            }
        }
    }
}