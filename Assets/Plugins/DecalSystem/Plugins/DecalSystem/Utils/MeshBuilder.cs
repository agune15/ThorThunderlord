#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Decal {

    public class MeshBuilder {

        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<Vector3> normals = new List<Vector3>();
        private readonly List<Vector2> texCoords = new List<Vector2>();
        private readonly List<int> indices = new List<int>();



        public void AddPolygon(Vector3[] poly, Vector3 normal, Rect uvRect) {
            int ind1 = AddVertex( poly[0], normal, uvRect );

            for (int i = 1; i < poly.Length - 1; i++) {
                int ind2 = AddVertex( poly[i], normal, uvRect );
                int ind3 = AddVertex( poly[i + 1], normal, uvRect );

                indices.Add( ind1 );
                indices.Add( ind2 );
                indices.Add( ind3 );
            }
        }

        private int AddVertex(Vector3 vertex, Vector3 normal, Rect uvRect) {
            int index = FindVertex( vertex );
            if (index == -1) {
                vertices.Add( vertex );
                normals.Add( normal );
                AddTexCoord( vertex, uvRect );
                return vertices.Count - 1;
            } else {
                normals[index] = (normals[index] + normal).normalized;
                return index;
            }
        }

        private int FindVertex(Vector3 vertex) {
            for (int i = 0; i < vertices.Count; i++) {
                if (Vector3.Distance( vertices[i], vertex ) < 0.01f) return i;
            }
            return -1;
        }


        private void AddTexCoord(Vector3 ver, Rect uvRect) {
            float u = Mathf.Lerp( uvRect.xMin, uvRect.xMax, ver.x + 0.5f );
            float v = Mathf.Lerp( uvRect.yMin, uvRect.yMax, ver.y + 0.5f );
            texCoords.Add( new Vector2( u, v ) );
        }





        public void Push(float distance) {
            for (int i = 0; i < vertices.Count; i++) {
                vertices[i] += normals[i] * distance;
            }
        }


        public void ToMesh(Mesh mesh) {
            mesh.Clear( true );
            if (indices.Count == 0) return;


            mesh.vertices = vertices.ToArray();
            mesh.normals = normals.ToArray();
            mesh.uv = texCoords.ToArray();
            mesh.uv2 = texCoords.ToArray();
            mesh.triangles = indices.ToArray();


            vertices.Clear();
            normals.Clear();
            texCoords.Clear();
            indices.Clear();
        }


    }

}
#endif