#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace _Decal {

    [RequireComponent( typeof( MeshFilter ) )]
    [RequireComponent( typeof( MeshRenderer ) )]
    [ExecuteInEditMode]
    public class Decal : MonoBehaviour {

        public Material material;
        public Sprite sprite;

        public float maxAngle = 90.0f;
        public float pushDistance = 0.009f;
        public LayerMask affectedLayers = -1;

        public Texture texture {
            get {
                return material ? material.mainTexture : null;
            }
        }


        void OnEnable() {
            if (Application.isPlaying) enabled = false;
        }

        void Start() {
            transform.hasChanged = false;
        }


        void Update() {
            if (transform.parent && transform.parent.hasChanged) { // when inspector is not allowed
                transform.parent.hasChanged = false;
                DecalBuilder.BuildAndSetDirty( this );
            }
        }


        void OnDrawGizmosSelected() {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube( Vector3.zero, Vector3.one );
        }

    }
}
#endif