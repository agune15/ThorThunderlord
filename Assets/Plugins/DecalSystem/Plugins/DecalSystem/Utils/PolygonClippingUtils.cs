#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Decal {

    public static class PolygonClippingUtils {

        private static readonly Plane right = new Plane( Vector3.right, 0.5f );
        private static readonly Plane left = new Plane( Vector3.left, 0.5f );

        private static readonly Plane top = new Plane( Vector3.up, 0.5f );
        private static readonly Plane bottom = new Plane( Vector3.down, 0.5f );

        private static readonly Plane front = new Plane( Vector3.forward, 0.5f );
        private static readonly Plane back = new Plane( Vector3.back, 0.5f );



        public static Vector3[] Clip(params Vector3[] poly) {
            poly = Clip( poly, right ).ToArray();
            poly = Clip( poly, left ).ToArray();
            poly = Clip( poly, top ).ToArray();
            poly = Clip( poly, bottom ).ToArray();
            poly = Clip( poly, front ).ToArray();
            poly = Clip( poly, back ).ToArray();
            return poly;
        }

        private static IEnumerable<Vector3> Clip(Vector3[] poly, Plane plane) {
            for (int i = 0; i < poly.Length; i++) {
                int next = (i + 1) % poly.Length;
                Vector3 v1 = poly[i];
                Vector3 v2 = poly[next];

                if (plane.GetSide( v1 )) {
                    yield return v1;
                }

                if (plane.GetSide( v1 ) != plane.GetSide( v2 )) {
                    yield return PlaneLineCast( plane, v1, v2 );
                }
            }
        }

        private static Vector3 PlaneLineCast(Plane plane, Vector3 a, Vector3 b) {
            float dis;
            Ray ray = new Ray( a, b - a );
            plane.Raycast( ray, out dis );
            return ray.GetPoint( dis );
        }

    }

}
#endif