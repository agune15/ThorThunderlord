using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace _Decal {

    [CustomEditor( typeof( Decal ) )]
    public class DecalEditor : Editor {
        
        private string[] materialPaths;
        private Vector3 oldScale;
        private GameObject[] affectedObjects;


        void OnEnable() {
            materialPaths = AssetDatabase.FindAssets( "decal t:Material" ).Select( GUID => AssetDatabase.GUIDToAssetPath( GUID ) ).ToArray();
        }


        public override void OnInspectorGUI() {
            Decal decal = (Decal) target;

            decal.material = GUIUtils.DrawMaterialChooser( decal.material, materialPaths );

            if (decal.texture != null) {
                EditorGUILayout.Separator();
                decal.sprite = GUIUtils.DrawSpriteList( decal.sprite, LoadSprites( decal.texture ) );
                decal.sprite = GUIUtils.AssetField( "Sprite", decal.sprite );
            }

            if (decal.sprite != null && decal.sprite.texture != decal.texture) decal.sprite = null;



            EditorGUILayout.Separator();
            decal.maxAngle = EditorGUILayout.FloatField( "Max Angle", decal.maxAngle );
            decal.maxAngle = Mathf.Clamp( decal.maxAngle, 1, 180 );
            decal.pushDistance = EditorGUILayout.FloatField( "Push Distance", decal.pushDistance );
            decal.pushDistance = Mathf.Clamp( decal.pushDistance, 0.01f, 0.1f );
            decal.affectedLayers = GUIUtils.LayerMaskField( "Affected Layers", decal.affectedLayers );

            EditorGUILayout.Separator();
            if (GUILayout.Button( "Build" )) GUI.changed = true;


            if (affectedObjects != null && affectedObjects.Length > 0) {
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                GUILayout.Label( "Affected Objects" );
                foreach (GameObject go in affectedObjects) {
                    EditorGUILayout.ObjectField( go, typeof( GameObject ), true );
                }
            }


            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox( "Left Ctrl + Left Mouse Button - put decal on surface", MessageType.Info );


            if (GUI.changed) {
                affectedObjects = DecalBuilder.BuildAndSetDirty( decal );
            }
        }


        private Texture cachedSpritesTexture;
        private Sprite[] cachedSprites;
        private Sprite[] LoadSprites(Texture texture) {
            if (cachedSpritesTexture == texture) return cachedSprites;

            cachedSpritesTexture = texture;
            string path = AssetDatabase.GetAssetPath( texture );
            return cachedSprites = AssetDatabase.LoadAllAssetsAtPath( path ).Select( i => i as Sprite ).Where( i => i != null ).ToArray();
        }
        
        
        void OnSceneGUI() {
            Decal decal = (Decal) target;

            if (Event.current.control) {
                HandleUtility.AddDefaultControl( GUIUtility.GetControlID( FocusType.Passive ) );
            }

            if (Event.current.control && Event.current.type == EventType.MouseDown) {
                Ray ray = HandleUtility.GUIPointToWorldRay( Event.current.mousePosition );
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast( ray, out hit, 50 )) {
                    decal.transform.position = hit.point;
                    decal.transform.forward = -hit.normal;
                }
            }


            Vector3 scale = decal.transform.localScale;
            Sprite sprite = decal.sprite;
            if (sprite != null) {
                float ratio = sprite.rect.width / sprite.rect.height;

                if (!Mathf.Approximately( oldScale.x, scale.x )) {
                    scale.y = scale.x / ratio;
                }
                if (!Mathf.Approximately( oldScale.y, scale.y )) {
                    scale.x = scale.y * ratio;
                }

                if (!Mathf.Approximately( scale.x / scale.y, ratio )) {
                    scale.x = scale.y * ratio;
                }
            }
            decal.transform.localScale = scale;
            oldScale = scale;


            if (decal.transform.hasChanged) {
                decal.transform.hasChanged = false;
                affectedObjects = DecalBuilder.BuildAndSetDirty( decal );
            }
        }



    }
}