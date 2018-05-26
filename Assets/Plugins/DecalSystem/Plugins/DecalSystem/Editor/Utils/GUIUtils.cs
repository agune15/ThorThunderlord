using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace _Decal {

    public static class GUIUtils {


        public static T AssetField<T>(string label, T obj) where T : Object {
            using (new GUILayout.HorizontalScope()) {
                EditorGUILayout.PrefixLabel( label );
                return (T) EditorGUILayout.ObjectField( obj, typeof( T ), false );
            }
        }

        public static LayerMask LayerMaskField(string label, LayerMask mask) {
            var names = Enumerable.Range( 0, 32 ).Select( i => LayerMask.LayerToName( i ) )/*.Where( i => !string.IsNullOrEmpty( i ) )*/.ToArray(); // TODO: fix bug
            return EditorGUILayout.MaskField( label, mask.value, names );
        }


        public static Material DrawMaterialChooser(Material material, string[] paths) {
            return DrawAssetChooser( "Material", material, paths );
        }

        public static T DrawAssetChooser<T>(string label, T asset, string[] paths) where T : Object {
            string[] names = paths.Select( path => Path.GetFileNameWithoutExtension( path ) ).ToArray();
            int index = asset != null ? ArrayUtility.IndexOf( names, asset.name ) : -1;
            int oldIndex = index;

            using (new GUILayout.HorizontalScope()) {
                EditorGUILayout.PrefixLabel( label );
                asset = (T) EditorGUILayout.ObjectField( asset, typeof( T ), false );
                GUILayout.Space( 5 );
                index = EditorGUILayout.Popup( index, names );
            }

            if (index != oldIndex) return AssetDatabase.LoadAssetAtPath<T>( paths.ElementAtOrDefault( index ) );
            return asset;
        }



        public static Sprite DrawSpriteList(Sprite sprite, Sprite[] list) {
            foreach (var item in DrawGrid( list )) {
                bool selected = DrawSprite( item.Value, item.Key, item.Key == sprite );
                if (selected) sprite = item.Key;
            }

            return sprite;
        }

        private static bool DrawSprite(Rect rect, Sprite sprite, bool selected) {
            Texture texture = sprite.texture;
            Rect uvRect = sprite.rect;
            uvRect.x /= texture.width;
            uvRect.y /= texture.height;
            uvRect.width /= texture.width;
            uvRect.height /= texture.height;

            if (selected) EditorGUI.DrawRect( rect, Color.blue );
            GUI.DrawTextureWithTexCoords( rect, texture, uvRect );
            return GUI.Button( rect, GUIContent.none, GUI.skin.label );
        }



        private static IEnumerable<KeyValuePair<T, Rect>> DrawGrid<T>(T[] list) {
            int xCount = 5;
            int yCount = Mathf.CeilToInt( (float) list.Length / xCount );
            int i = 0;
            foreach (var rect in DrawGrid( xCount, yCount )) {
                if (i < list.Length) {
                    yield return new KeyValuePair<T, Rect>( list[i], rect );
                }
                i++;
            }
        }

        private static IEnumerable<Rect> DrawGrid(int xCount, int yCount) {
            int id = GUIUtility.GetControlID( "Grid".GetHashCode(), FocusType.Keyboard );

            using (new GUILayout.VerticalScope( GUI.skin.box )) {
                for (int y = 0; y < yCount; y++) {

                    using (new GUILayout.HorizontalScope()) {
                        for (int x = 0; x < xCount; x++) {
                            Rect rect = GUILayoutUtility.GetAspectRect( 1 );

                            if (Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition )) {
                                GUIUtility.hotControl = GUIUtility.keyboardControl = id;
                            }

                            yield return rect;
                        }
                    }

                }
            }
        }

        

    }

}