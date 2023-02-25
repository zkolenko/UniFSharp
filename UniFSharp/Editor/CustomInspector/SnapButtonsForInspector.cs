using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Reflection;
using UnityEditorInternal;
using UnityEditor.SceneManagement;

namespace UniFSharp
{


	////////
	//////// ICONS ////////
	////////
	abstract class IconBehaviour
	{
		protected abstract string _mData { get; }

		public static implicit operator Texture2D( IconBehaviour ib )
		{

			var result = new Texture2D( 1, 1, TextureFormat.ARGB32, false, true );
#if !UNITY_2017_1_OR_NEWER
            result.LoadImage( Convert.FromBase64String( ib._mData ) );
#else
			ImageConversion.LoadImage( result, Convert.FromBase64String( ib._mData ), false );
#endif
			result.alphaIsTransparency = true;
			result.mipMapBias = 0;
			result.filterMode = FilterMode.Point;
			result.wrapMode = TextureWrapMode.Clamp;
			result.alphaIsTransparency = true;
			result.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontSaveInBuild;
			result.Apply();

			return result;
		}
	}

	class Icon_SnapButtonsForInspectorSurface : IconBehaviour
	{
		protected override string _mData { get { return "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAADAUlEQVQ4jT3QOW8cZQDG8f87OzO7nrV3djc+dr1JCEEQ2UvACRilMVIKOuhAogyIgh4kPgfwGZAQDVIKGkSDAFN5E0VObPARC4+vtbPX3PMeFEZUT/dIv79YXV2dq7eKr64tR/c6N5KsVrXNbMPlasdm2i8RjhRHgeRskDOMpAj2qs7h5tSPUWh9s/7rhrJX3o0+++jz8As1ehPHLVNxffypGVy3jOM4tGYM882QMAvJiwmDlQvSD/bWjJgcr6/xvdVXY1s2ary1fI/5ueu43jT9NMG260SZ4NlZwFBKyl6NVzq3uX/nPUSzzp45bwHYJy90chB43L1R5u/jQ1KZMYoHtKbrjNMJ2yeb1CpNGlWfWa/OVrDPo50JgUEB2EIgLHLS/IK/znYBTVqk5CpGqYxhkhDlfcJ8QHdxhj/2f+Nc7yD3Fg0cYSsFSR5ilzKWFhbRgFQSu6TwXM1yaw4scEsaYwbcbnc4tbd59l3VBrALYxHJiJLIea29AFhgBCUro1Qq6HauINAIIQDDrfYsfw59ikrmAthGgy0swnzIDxsbGKMI85gPV15nHKf8tPWUiu3S9Dze797i4eNtno5HlJ1ZDWBLZZHLCK1DUpUg0OQ6By1RRqGMRhuF1gZjFEobUgVa41wSMksU5CgdM0gjAAZJQqEluSo4jWKmXQ+rlKEoGGYJoxyM4bIBfpENownTZcOnby9hBAhj4TkufsXmy7V3EEIghEYIw8d3Xubk0T6/JMUl4flyoPS4RipTnhwPcLCQBlYWG2RKst0PEUDZNnRbPpunEw7H4ApxGTHLEFrEJKWQXrBPlBj+Gcd0mm8QvIj5dn2Hm77L1XqVV9sWfdnnLAP+J2RUH48kT6IeN1+CRMJCDHn1gikK7i9BrZpTmcrZkgOa8w7xLpynWfXy4JCt3yfQT+CaD+06NKvQCwIaHnTa8HwIgz78vAcHBwW7J3AttY8ARPdul0zkDw68808KbySpaIMHNIApIAaG/21qCSf27evJlYc1NfN1r9eT/wKb0ZEkj3mB4AAAAABJRU5ErkJggg=="; } }
	}

	class Icon_SnapButtonsForInspectorNormal : IconBehaviour
	{
		protected override string _mData { get { return "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAABkklEQVQ4jY2TMWtUQRSFvzu77rqJMRixECQEtBEs7dQu/0AbOxO0SJEqdQoFIVpaGBAtgoVgZ2PjD5CtDSimiGACEjQhu7Nx33sz71js8nDd9xIvnGbmnHPv3HsHSZQB/A3wAr8GHvClPBuQx8Os9xm4AgiYBX5Ik2M8VyFeAC4BXSAFXpdmqTIAbgMvgAA8AZIqg6r3A/4i+D3wc8f1oKqC/45Sg2GzNHo2UWUgRlFo7W/il82fC29ebY6NzDbWt2V1yHNwaqFah4ij02nx6PFZnj7sUmtmhaBhLe4uXjAb1u6O0t00yiFz9BqRM2lO+8F94soSN/efEZbv8W15hcwSpvunyBTYeLmv9sdfqwCWSdSTnDun53WNwBQ1mvymSYspGkyQMklOnwaRlPblW8yureL2djk/c7jIwdZX+wD6BHrLnL5zbrCgcCySMBy5onhnvN8BxRNEYxgohGKCQOFfgpm0/hylPRRELo+uXm8X9/3u0CBE1DsgK8T1o34WSjevQJIjcxr9jTUnYj4y/5PiD6S6SmBNvkhIAAAAAElFTkSuQmCC"; } }
	}

	class Icon_SnapButtonsForInspectorImage : IconBehaviour
	{
		protected override string _mData { get { return "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAACiklEQVQ4ja2TS2sUaRiFn+9S9+5UuqNtvMGo0Y0XxJFB8B8YHf0ByjhbQUaXrgXXIoiCq+wUXGhQXHhhRMHViMYrjjEq6hiT7rJNp7qrvqr+XDRkfoAeeLfPew6cAz8oAbDw9esR4thTr1966tAWV+elJ6vCQ1mPHJfNvzbb2/bcSW7eudU5dJTKf2+xs59Yf/IMGkC57lkHsK6D45UDrGcHLzygaBE/uXwijsz16fbs0VRVZiJjAJAAfhCgASeMoM/gBOAABki+QTgCq+vjGx6cmwq+TI+1128FGDh4NjWVs6xRVt+/MUHiF/3FniHVBYK45hQVTzWh9QVqK8ATlXUfbt945Nc2LgHuX5tc1wmjcmXWy0cXaqafZsbJXGN7ac0ZHto7Vs6dH3VnfISE5avAvBtbe+XMfuDqIIIQnzR2VtsycZK5jttOssB2+1WnaNaXDU987K453Jk2sDAPzXmoNBip2fElB9V6HRGERFlItHMn1pSoZA5fQBRHyEJeStvBRKU171G44AUg0pVLgI07tmPdACUEcvduRBQSnz5FOjlJvusAYq6j9a1/JSlgm6A16LiE7gAgLVhAOg5m9SrsUAXZaJC9nUfcvYfbt8dDkTmgsL0S8e493U2/vYHPA4Cx7MvLslStVh49/sco3ze91zN54YuqfvHq91rYPeaPKIpcomUJOSQfOxNLEYYbo5NZfQT/6RPcP/9A5BbRcAiXhwQiJwgVvVwidR8WC9qjv1xcGNv+mL+fDwDadXGrQwRRiI40EoNQGuUIrNDkBrS0qMWcThQ9bO4aP6ji+v9F6qVd7GIHm+WE2kfoEmk1RdciNUjZx0AvUfGFbn3NX8RVK135ozv8SfoOUdQDgPzMkmcAAAAASUVORK5CYII="; } }
	}










	class Root
	{

		const string MENU_PATH = "Tools/Snap Free/";
		[MenuItem( MENU_PATH + "Enable", true )]
		public static bool EnableV() { return !USE_SNAP_MOD; }
		[MenuItem( MENU_PATH + "Enable", false )]
		public static void Enable()
		{
			USE_SNAP_MOD.Set( !USE_SNAP_MOD );
			SnapMod.SET_ENABLE( USE_SNAP_MOD );
		}
		[MenuItem( MENU_PATH + "Disable", true )]
		public static bool DisableV() { return USE_SNAP_MOD; }
		[MenuItem( MENU_PATH + "Disable", false )]
		public static void Disable()
		{
			USE_SNAP_MOD.Set( !USE_SNAP_MOD );
			SnapMod.SET_ENABLE( USE_SNAP_MOD );
		}
		[MenuItem( MENU_PATH + "Settings", priority = 50 )]
		public static void Settings()
		{
			SnapButtonsForInspector.SELECT();
		}





		internal const string PREFS_KEY = "EM.SnapPlugin.";
		internal static CachedPref USE_SNAP_MOD = new CachedPref(PREFS_KEY + "USE_SNAP_MOD", true);

		internal static void RequestScriptReload()
		{
#if UNITY_2019_3_OR_NEWER
			EditorUtility.RequestScriptReload();
#else
			InternalEditorUtility.RequestScriptReload();
#endif
		}

		internal static void SET( string m_registryKey, int value )
		{
			EditorPrefs.SetInt( PREFS_KEY + m_registryKey, value );
		}
		internal static void SET( string m_registryKey, bool value )
		{
			EditorPrefs.SetBool( PREFS_KEY + m_registryKey, value );
		}
		internal static int? GET( string m_registryKey, int m_boolDefaultValue )
		{
			return EditorPrefs.GetInt( PREFS_KEY + m_registryKey, m_boolDefaultValue );
		}
		internal static bool? GET( string m_registryKey, bool m_boolDefaultValue )
		{
			return EditorPrefs.GetBool( PREFS_KEY + m_registryKey, m_boolDefaultValue );
		}
	}








	class SnapMod
	{



		static string SnapModSwitcherFile = "SnapModSwitcher.cs";
		static string SwitcherContent = @"#define USE
#if USE
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace EM.SnapPlugin.Editor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Transform), true)]
    public partial class SnapButtonsForInspectorGUI : UnityEditor.Editor
    {
    }
}
#endif";

		internal static bool SET_ENABLE( bool value )
		{

			if ( !Directory.Exists( SnapButtonsForInspector.FOLDER ) ) Directory.CreateDirectory( SnapButtonsForInspector.FOLDER );
			var path = SnapButtonsForInspector.FOLDER + SnapModSwitcherFile;
			var ex = File.Exists(path);
			if ( !ex && !value ) return false;
			MonoScript mono = ex ? AssetDatabase.LoadAssetAtPath<MonoScript>(path) : null;
			if ( !mono && !value ) return false;

			if ( !mono )
			{
				File.WriteAllText( path, SwitcherContent );
				AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
			}

			var l = File.ReadAllLines(path);
			var oldVal = !l[0].Trim(new[] { ' ', '\r', '\n' }).StartsWith("//");
			if ( oldVal == value )
			{
				return false;
			}
			if ( value ) l[ 0 ] = l[ 0 ].Replace( "/", "" );
			else l[ 0 ] = "//" + l[ 0 ];
			File.WriteAllLines( path, l );
			AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport );
			Root.RequestScriptReload();
			return true;
		}





		////////
		//////// SETTINGS PARAMS ////////
		////////

		public static CachedPref SNAP_SNAP_USEHOTKEYS = new CachedPref("SNAP_SNAP_USE", true) { name = "Press key to activate snap" };
		public static CachedPref SNAP_SNAP_HOTKEYS = new CachedPref("SNAP_SNAP_HOTKEYS", (int)KeyCode.K) { name = "Ctrl+shift+..." };

		public static CachedPref SNAP_SURFACE_USEHOTKEYS = new CachedPref("SNAP_SURFACE_USE", true) { name = "Press key to activate surface" };
		public static CachedPref SNAP_SURFACE_HOTKEYS = new CachedPref("SNAP_SURFACE_HOTKEYS", (int)KeyCode.L) { name = "Ctrl+shift+..." };

		public static CachedPref SNAP_FASTINVERT_USEHOTKEYS = new CachedPref("SNAP_INVERT_USE", false) { name = "Use key holding to activate snap" };
		public static CachedPref SNAP_FASTINVERT_HOTKEYS = new CachedPref("SNAP_INVERT_HOTKEYS", (int)KeyCode.C) { name = "Hold:" };

		public static VectorPref SNAP_POS = new VectorPref("SNAP_TOOGLES_POS", enableName: "Enable position snapping %#k");
		public static VectorPref SNAP_ROT = new VectorPref("SNAP_TOOGLES_ROT", enableName: "Enable rotation snapping");
		public static VectorPref SNAP_SCALE = new VectorPref("SNAP_TOOGLES_SCALE", true, enableName: "Enable scale snapping");
		public static CachedPref SNAP_AUTOAPPLY = new CachedPref("SNAP_TOOGLES_SNAP_AUTOAPPLY", false) { name = "Auto-apply snap (for new selected object)" };

		public static CachedPref PLACE_ON_SURFACE_ENABLE = new CachedPref("SNAP_TOOGLES_PLACE_ON_SURFACE_ENABLE", false) { name = "Enable surface placement %#l" };
		public static CachedPref PLACE_ON_SURFACE_ALIGNBYMOUSE = new CachedPref("SNAP_TOOGLES_PLACE_ON_SURFACE_ALIGNBYMOUSE", true) { name = "" };
		// public static CachedPref PLACE_ON_SURFACE_BOUNDS = new CachedPref( "SNAP_TOOGLES_PLACE_ON_SURFACE_BOUNDS", true ) { name = "Calc bounds offset" };
		//public static CachedPref PLACE_ON_SURFACE_BOUNDS = new CachedPref("SNAP_TOOGLES_PLACE_ON_SURFACE_BOUNDS", true) { name = "Enable object's bound calculation (if disabled)" };
		public static CachedPref CALCULATE_OBJECT_BOUNDS = new CachedPref("CALCULATE_OBJECT_BOUNDS", false) { name = "Enable object's bound calculation (if disabled object will snap to pivot)" };

		public static CachedPref ALIGN_BY_NORMAL = new CachedPref("SNAP_TOOGLES_ALIGN_BY_NORMAL", false) { name = "Enable align by surface normal" };
		public static CachedPref ALIGN_UP_VECTOR = new CachedPref("SNAP_TOOGLES_ALIGN_UP_VECTOR", 0) { name = "Surface snapping direction" };





		////////
		//////// UPDATING ////////
		////////

		// vars
		static Vector3 p, v;
		static bool b;
		static System.Reflection.PropertyInfo dragActive;
		// strings
		struct PRF { public string key; public float def; }
		static PRF _PRF( string s, float v ) { return new PRF() { key = s, def = v }; }
		static PRF[] POS_PREFKEYS = { _PRF("MoveSnapX", 1), _PRF("MoveSnapY", 1), _PRF("MoveSnapZ", 1) };
		static PRF[] ROT_PREFKEYS = { _PRF("RotationSnap", 15), _PRF("RotationSnap", 15), _PRF("RotationSnap", 15) };
		static PRF[] SCALE_PREFKEYS = { _PRF("ScaleSnap", 0.1f), _PRF("ScaleSnap", 0.1f), _PRF("ScaleSnap", 0.1f) };
		static string[] UNDO_TEXT = { "Move", "Rotate", "Scale" };
		public static Vector3[] VECTORS = { Vector3.up, Vector3.forward, Vector3.down, Vector3.right, Vector3.left, Vector3.back };
		public static string[] VECTORS_STRING = { "Look up", "Look forward", "Look down", "Look right", "Look left", "Look back" };
		public static string[] ALIGN_BY = { "Alignment by mouse position", "Alignment by camera projection" };

		// initialization
		[InitializeOnLoadMethod]
		internal static void Subscribe()
		{

			EditorApplication.update += EditorApplication_UPDATESNAPPING;

#if !UNITY_2017_1_OR_NEWER
			SceneView.onSceneGUIDelegate += SceneView_PLACEONSURFACE;
			SceneView.onSceneGUIDelegate += modifierKeysChanged_SCENE;
#else
			SceneView.duringSceneGui += SceneView_PLACEONSURFACE;
			SceneView.duringSceneGui += modifierKeysChanged_SCENE;
#endif
			EditorApplication.modifierKeysChanged += modifierKeysChanged_KEYS;
			//sbs.OnGlobalKeyPressed += globalKeyPressed;

			if ( !wasInit )
			{
				dragActive = typeof( UnityEditor.Editor ).Assembly.GetType( "UnityEditor.TransformManipulator" ).GetProperty( "active" );
				var snapSettings = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.SnapSettings");
				var initMethod = snapSettings.GetMethod("Initialize", (System.Reflection.BindingFlags)int.MaxValue);
				if ( initMethod != null ) initMethod.Invoke( null, null );
				wasInit = true;
			}
		}

		/* available only with the Hierarchy Pro or Hierarchy Pro Extended
		static void globalKeyPressed( bool used )
		{
			if ( used ) return;
			if ( Event.current.type == EventType.KeyDown && Event.current.shift && Event.current.control )
			{
				bool has = false;
				if ( SNAP_SNAP_USEHOTKEYS && (int)Event.current.keyCode == (int)SNAP_SNAP_HOTKEYS ) { SNAP_POS.ENABLE.Set( !SNAP_POS.ENABLE ); has = true; }
				if ( SNAP_SURFACE_USEHOTKEYS && (int)Event.current.keyCode == (int)SNAP_SURFACE_HOTKEYS ) { PLACE_ON_SURFACE_ENABLE.Set( !PLACE_ON_SURFACE_ENABLE ); has = true; }
				if ( has ) UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
			}
		}
		*/


		static bool wasInit;
		// save object params
		static void SET_DIRTY( Transform t )
		{
			if ( !Application.isPlaying ) EditorUtility.SetDirty( t );
			//  if (!t.gameObject.scene.isDirty) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
			if ( !Application.isPlaying )
			{
				var s=  t.gameObject.scene;
				if ( s.IsValid() && !s.isDirty ) EditorSceneManager.MarkSceneDirty( s );
			}
		}

		public struct Keys
		{
			public KeyCode keyCode; public bool ctrl; public bool shift; public bool alt;
			public Keys( KeyCode keyCode, bool control, bool shift, bool alt ) : this()
			{
				this.keyCode = keyCode;
				this.ctrl = control;
				this.shift = shift;
				this.alt = alt;
			}
		}
		internal static Dictionary<int, Keys> sceneKeyCode = new Dictionary<int, Keys>();
		static void modifierKeysChanged_SCENE( SceneView sv )
		{
			if ( !SNAP_FASTINVERT_USEHOTKEYS ) return;
			if ( !sceneKeyCode.ContainsKey( GUIUtility.keyboardControl ) ) sceneKeyCode.Add( GUIUtility.keyboardControl, new Keys() );
			if ( Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None )
			{
				sceneKeyCode[ GUIUtility.keyboardControl ] = new Keys( Event.current.keyCode, Event.current.control, Event.current.shift, Event.current.alt );
			}
			if ( Event.current.type == EventType.KeyUp )
			{
				sceneKeyCode[ GUIUtility.keyboardControl ] = new Keys();
			}
		}
		static System.Reflection.FieldInfo ins;
		internal static void modifierKeysChanged_KEYS()
		{
			if ( SNAP_FASTINVERT_HOTKEYS == 0 || !SNAP_FASTINVERT_USEHOTKEYS ) return;

			if ( ins == null )
			{
				var wa = Resources.FindObjectsOfTypeAll<EditorWindow>().FirstOrDefault(w => w.GetType().FullName == "UnityEditor.InspectorWindow");
				if ( !wa ) return;
				ins = wa.GetType().GetField( "m_AllInspectors", (System.Reflection.BindingFlags)(-1) );
			}
			if ( EditorWindow.focusedWindow ) EditorWindow.focusedWindow.Repaint();

			var wrapp = ins.GetValue(null);
			//  var i = new System.Collections.ArrayList().ToArray();
			//var i = ins.GetValue(null) as IReadOnlyList<EditorWindow>;
			foreach ( var item in wrapp as System.Collections.IList ) if ( (EditorWindow)item ) ((EditorWindow)item).Repaint();
		}



		static void EditorApplication_UPDATESNAPPING()
		{
			was = false;
			//sceneKeyCode = KeyCode.None;
			var any = SNAP_POS.ENABLE || SNAP_ROT.ENABLE || SNAP_SCALE.ENABLE || IsSnapInverted() && !SNAP_POS.ENABLE && !SNAP_ROT.ENABLE && !SNAP_SCALE.ENABLE;
			if ( !any || !SNAP_AUTOAPPLY && !(bool)dragActive.GetValue( null, null ) ) return;

			foreach ( var t in Selection.GetTransforms( flags ) )
			{
				PosSnappingUpdater( t );
				RotSnappingUpdater( t );
				ScaleSnappingUpdater( t );
			}
		}

		internal static bool IsSnapInverted()
		{
			if ( !SnapMod.SNAP_FASTINVERT_USEHOTKEYS ) return false;
			var inv = (int)SnapMod.SNAP_FASTINVERT_HOTKEYS;
			var I_KEY = inv & 0xFFFF;
			var ICTRL = (inv & (1 << 16)) != 0;
			var ISHIFT = (inv & (1 << 17)) != 0;
			var IALT = (inv & (1 << 18)) != 0;
			// var res = (KeyCode)I_KEY == Event.current.keyCode;
			var res = false;
			foreach ( var item in SnapMod.sceneKeyCode )
			{
				if ( !res )
				{
					res = (KeyCode)I_KEY == item.Value.keyCode;
					res &= ICTRL == item.Value.ctrl;
					res &= ISHIFT == item.Value.shift;
					res &= IALT == item.Value.alt;
				}
			}

			return res;
			// Debug.Log((KeyCode)Event.current.keyCode  + " " + Event.current.control + " " + Event.current.shift + " " + Event.current.alt);
			//  return SnapMod.IsSnapInverted = Event.current.shift;
		}
		static void PosSnappingUpdater( Transform t )
		{
			var en = (bool)SNAP_POS.ENABLE;
			if ( IsSnapInverted() ) en = !en;
			if ( !en ) return;
			v = SNAP_POS.USE_LOCAL ? t.localPosition : t.position;
			if ( DO_SNAPACTION( t, SNAP_POS, ref POS_PREFKEYS, 1 ) )
			{
				if ( SNAP_POS.USE_LOCAL ) t.localPosition = v;
				else t.position = v;
				SET_DIRTY( t );
			}
		}
		static void RotSnappingUpdater( Transform t )
		{
			var en = (bool)SNAP_ROT.ENABLE;
			if ( IsSnapInverted() ) en = !en;
			if ( !en ) return;

			v = SNAP_ROT.USE_LOCAL ? t.localRotation.eulerAngles : t.rotation.eulerAngles;
			if ( DO_SNAPACTION( t, SNAP_ROT, ref ROT_PREFKEYS, 1 ) )
			{
				if ( SNAP_ROT.USE_LOCAL ) t.localRotation = Quaternion.Euler( v );
				else t.rotation = Quaternion.Euler( v );
				SET_DIRTY( t );
			}
		}
		static void ScaleSnappingUpdater( Transform t )
		{
			var en = (bool)SNAP_SCALE.ENABLE;
			if ( IsSnapInverted() ) en = !en;
			if ( !en ) return;
			v = t.localScale;
			if ( DO_SNAPACTION( t, SNAP_SCALE, ref SCALE_PREFKEYS, 1 ) )
			{
				t.localScale = v;
				SET_DIRTY( t );
			}
		}


		static bool DO_SNAPACTION( Transform t, VectorPref pref, ref PRF[] prefKeys, int undoTextIndex )
		{
			p = v;

			if ( pref.X ) v.x = (float)Math.Round( v.x / EditorPrefs.GetFloat( prefKeys[ 0 ].key, prefKeys[ 0 ].def ), 0 ) * EditorPrefs.GetFloat( prefKeys[ 0 ].key, prefKeys[ 0 ].def );
			if ( pref.Y ) v.y = (float)Math.Round( v.y / EditorPrefs.GetFloat( prefKeys[ 1 ].key, prefKeys[ 1 ].def ), 0 ) * EditorPrefs.GetFloat( prefKeys[ 1 ].key, prefKeys[ 1 ].def );
			if ( pref.Z ) v.z = (float)Math.Round( v.z / EditorPrefs.GetFloat( prefKeys[ 2 ].key, prefKeys[ 2 ].def ), 0 ) * EditorPrefs.GetFloat( prefKeys[ 2 ].key, prefKeys[ 2 ].def );

			if ( float.IsNaN( v.x ) ) v.x = p.x;
			if ( float.IsNaN( v.y ) ) v.y = p.y;
			if ( float.IsNaN( v.z ) ) v.z = p.z;

			b = p != v;
			if ( b ) Undo.RecordObject( t, UNDO_TEXT[ undoTextIndex ] );
			return b;
		}



		static SelectionMode flags {
			get {
				return
					SelectionMode.TopLevel |
#if UNITY_2019_1_OR_NEWER
SelectionMode.Editable
#else
SelectionMode.OnlyUserModifiable
#endif
;
			}
		}


		static bool was = false;
		// static Vector2 mouseTunning;
		static Vector2[] mouseTunning;
		private static void SceneView_PLACEONSURFACE( SceneView sceneView )
		{
			if ( !sceneView ) return;
			/*
            if (Event.current.type == EventType.KeyDown && Event.current.control && Event.current.shift)
            {
                if (Event.current.keyCode == KeyCode.L) PLACE_ON_SURFACE_ENABLE.Set(!PLACE_ON_SURFACE_ENABLE);
                if (Event.current.keyCode == KeyCode.K) SNAP_POS.ENABLE(w).Set(!SNAP_POS.ENABLE);
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
            */
			if ( !PLACE_ON_SURFACE_ENABLE || UnityEditor.Tools.current == Tool.Rotate ||
#if UNITY_2017_3_OR_NEWER
				UnityEditor.Tools.current == Tool.Transform ||
#endif
				UnityEditor.Tools.current == Tool.Rect ) return;

			if ( Event.current.type == EventType.MouseDown && (Selection.GetTransforms( flags ).Length > 0) )
			{   /* if ( PLACE_ON_SURFACE_ALIGNBYMOUSE )
                         mouseTunning = new[] { (HandleUtility.WorldToGUIPoint( UnityEditor.Tools.handlePosition ) - Event.current.mousePosition) };
                     else*/
				mouseTunning = Selection.GetTransforms( flags ).Select( t => HandleUtility.WorldToGUIPoint( t.position ) - Event.current.mousePosition ).ToArray();
			}

			if ( Event.current.rawType == EventType.Repaint ) was = false;


			if ( !(bool)dragActive.GetValue( null, null ) && !was ) return;

			was = true;
			var selection = Selection.GetTransforms(flags);
			var excludeList = CreateExcludeList(selection);


			for ( int i = 0; i < selection.Length; i++ )
			{
				var t = selection[i];

				PosSnappingUpdater( t );

				p = t.position;
				/* var ray = PLACE_ON_SURFACE_ALIGNBYMOUSE ?
                               HandleUtility.GUIPointToWorldRay( Event.current.mousePosition + mouseTunning[Math.Min( mouseTunning.Length - 1, i )] ) :
                               HandleUtility.GUIPointToWorldRay( HandleUtility.WorldToGUIPoint( p ) );*/
				var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition + mouseTunning[Math.Min(mouseTunning.Length - 1, i)]);

				SNAP_VARIANT_2( t, ray, excludeList );

				t.position = v;
				SET_DIRTY( t );


			}

			EditorApplication_UPDATESNAPPING();

		}


		static public Vector3 ClosestPointOnPlane( Plane plane, Vector3 point )
		{
			float num = Vector3.Dot(plane.normal, point) + plane.distance;
			return point - plane.normal * num;
		}

		static void SNAP_VARIANT_2( Transform t, Ray ray, Dictionary<int, int> excludeList )
		{
			object obj = null;
			float mindist = float.MaxValue;
			foreach ( var hit in Physics.RaycastAll( ray ) )
			{
				if ( hit.collider == null || hit.collider.isTrigger || excludeList.ContainsKey( hit.collider.transform.GetInstanceID() ) ) continue;
				if ( Vector3.SqrMagnitude( hit.point - ray.origin ) > mindist && Vector3.SqrMagnitude( hit.point - ray.origin ) != float.PositiveInfinity ) continue;
				mindist = Vector3.SqrMagnitude( hit.point - ray.origin );
				obj = hit;
			}

			if ( obj != null )
			{
				Undo.RecordObject( t, UNDO_TEXT[ 0 ] );
				RaycastHit raycastHit = (RaycastHit)obj;
				v = raycastHit.point;
				//Debug.Log(raycastHit.collider.gameObject.name);

				t.position = v;


				if ( CALCULATE_OBJECT_BOUNDS && CalcBoundsForGameObjectHierarchy( t.gameObject ) )
				{
					var plane = new Plane(raycastHit.normal, raycastHit.point);
					var offset = 0f;

					for ( int i = 0; i < boundsVertices.Length; i++ )
					{

#if !UNITY_2017_1_OR_NEWER
                        var projection = ClosestPointOnPlane (plane, boundsVertices[i] );
#else
						var projection = plane.ClosestPointOnPlane(boundsVertices[i]);
#endif
						var difference = projection - boundsVertices[i];

						if ( Vector2.Dot( difference.normalized, raycastHit.normal ) > 0 )
						{
							if ( difference.sqrMagnitude > offset ) offset = difference.sqrMagnitude;
						}
					}

					if ( offset != 0 )
					{
						offset = (float)Math.Sqrt( offset );
						//Debug.Log(offset);
						v += raycastHit.normal * offset;
					}
				}

				if ( ALIGN_BY_NORMAL )
				{
					Undo.RecordObject( t, UNDO_TEXT[ 1 ] );
					switch ( ALIGN_UP_VECTOR )
					{
						case 0: t.up = raycastHit.normal; break;
						case 1: t.forward = raycastHit.normal; break;
						case 2: t.up = -raycastHit.normal; break;
						case 3: t.right = raycastHit.normal; break;
						case 4: t.right = -raycastHit.normal; break;
						case 5: t.forward = -raycastHit.normal; break;
					}
					SET_DIRTY( t );
				}
			}
			else
			{
				v = t.position;
			}
		}


		static Dictionary<int, int> CreateExcludeList( Transform[] t )
		{
			var result = new Dictionary<int, int>();
			foreach ( var transform in t )
			{
				foreach ( var componentsInChild in transform.GetComponentsInChildren<Transform>() )
					if ( !result.ContainsKey( componentsInChild.GetInstanceID() ) ) result.Add( componentsInChild.GetInstanceID(), 0 );
				if ( !result.ContainsKey( transform.GetInstanceID() ) ) result.Add( transform.GetInstanceID(), 0 );
			}
			return result;
		}

		static Vector3[] MIN_MAX = new Vector3[2];
		static Vector3[] boundsVertices = new Vector3[8];

		static Vector3 min_static = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		static Vector3 max_static = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		// BASED ON MESH VERTICES
		static bool CalcBoundsForGameObjectHierarchy( GameObject go )
		{

			MIN_MAX[ 0 ] = min_static;
			MIN_MAX[ 1 ] = max_static;
			bool skip = true;
			//var t = go.transform;
			var m = go.GetComponentsInChildren<MeshFilter>().ToList();
			m.Add( go.GetComponent<MeshFilter>() );
			foreach ( var mf in m )
			{
				if ( !mf ) continue;
				if ( !mf.sharedMesh || !mf.GetComponent<MeshRenderer>() || !mf.GetComponent<MeshRenderer>().enabled ) continue;
				var vertices = mf.sharedMesh.vertices;
				for ( int i = 0; i < vertices.Length; i++ )
				{
					if ( skip ) skip = false;
					var v = mf.transform.TransformPoint(vertices[i]);
					for ( int DIR = 0; DIR < 3; DIR++ )
					{
						if ( v[ DIR ] < MIN_MAX[ 0 ][ DIR ] ) MIN_MAX[ 0 ][ DIR ] = v[ DIR ];
						if ( v[ DIR ] > MIN_MAX[ 1 ][ DIR ] ) MIN_MAX[ 1 ][ DIR ] = v[ DIR ];
					}
				}
			}
			if ( skip ) MIN_MAX[ 0 ] = MIN_MAX[ 1 ] = Vector3.zero;

			var result = MIN_MAX[0] != Vector3.zero || MIN_MAX[1] != Vector3.zero;

			//MIN_MAX[0] = .(MIN_MAX[0]);
			//MIN_MAX[1] = go.transform.TransformPoint(MIN_MAX[1]);

			for ( int i = 0; i < 8; i++ ) boundsVertices[ i ].Set( MIN_MAX[ (i % 2) / 1 ].x, MIN_MAX[ (i % 4) / 2 ].y, MIN_MAX[ (i % 8) / 4 ].z );

			return result;

		}

		// BASED ON GAMEOBJECT BOUNDS
		/*    static bool CalcBoundsForGameObjectHierarchy( GameObject go )
                {

                    Bounds bounds = new Bounds( );
                    bounds.center = go.transform.position;
                    bounds.extents = Vector3.zero;

                    if ( go.GetComponent<Renderer>( ) )
                    {
                        bounds = go.GetComponent<Renderer>( ).bounds;
                    }

                    foreach ( Renderer renderer in go.GetComponentsInChildren<Renderer>( ) )
                    {
                        if ( !renderer ) continue;
                        bounds.Encapsulate( renderer.bounds );
                    }

                    MIN_MAX[0] = (bounds.min);
                    MIN_MAX[1] = (bounds.max);
                    for ( int i = 0; i < 8; i++ ) boundsVertices[i].Set( MIN_MAX[(i % 2) / 1].x, MIN_MAX[(i % 4) / 2].y, MIN_MAX[(i % 8) / 4].z );

                    return bounds.extents != Vector3.zero;

                }*/



	}









	public partial class SnapButtonsForInspectorGUI : UnityEditor.Editor
	{
		//params
		bool DRAW_SURFACE_BUTTON = true;


		////////
		//////// INSPECTOR DECORATOR ////////
		////////

		// inspector vars
		static Type decoratedEditorType;
		UnityEditor.Editor EDITOR_INSTANCE;
		internal void OnDisable() { if ( EDITOR_INSTANCE ) DestroyImmediate( EDITOR_INSTANCE ); }


		////////
		//////// GUI BUTTONS DRAWING ////////
		////////

		// buttons vars
		const float BUT_SIZE = 16;

		float buttonRectWidth { get { return (DRAW_SURFACE_BUTTON ? BUT_SIZE * 2 : BUT_SIZE) - 3; } }
		Color enableColor = new Color(0.9f, 0.6f, 0.3f, 1f);
		GUIStyle buttonStyle;
		Rect r = new Rect();
		GUIContent snapContent = new GUIContent("", "- Click to enable/disable snap\n- Right-Click to select axises\n- Use CTRL to use Unity internal snapping");
		GUIContent raycastContent = new GUIContent("", "- Click to enable/disable surface placement raycast\n- Right-Click to select align mode\n- Use CTRL+SHIFT to use Unity surface snapping");
		GUIContent normalContent = new GUIContent("", "- Align Object by surface normal if used surface placement raycast\n- Right-Click to choose up Vector");

		void InitStyles()
		{

			snapContent.image = new Icon_SnapButtonsForInspectorImage();
			raycastContent.image = new Icon_SnapButtonsForInspectorSurface();
			normalContent.image = new Icon_SnapButtonsForInspectorNormal();


			buttonStyle = new GUIStyle( GUI.skin.button );
			buttonStyle.padding = new RectOffset( 3, 3, 3, 3 );
		}

		bool IsSnapInverted()
		{
			if ( !SnapMod.SNAP_FASTINVERT_USEHOTKEYS ) return false;
			if ( !SnapMod.sceneKeyCode.ContainsKey( GUIUtility.keyboardControl ) ) SnapMod.sceneKeyCode.Add( GUIUtility.keyboardControl, new SnapMod.Keys() );
			if ( Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None )
			{
				SnapMod.sceneKeyCode[ GUIUtility.keyboardControl ] = new SnapMod.Keys( Event.current.keyCode, Event.current.control, Event.current.shift, Event.current.alt );
			}
			if ( Event.current.type == EventType.KeyUp )
			{
				SnapMod.sceneKeyCode[ GUIUtility.keyboardControl ] = new SnapMod.Keys();
			}
			return SnapMod.IsSnapInverted();
		}

		void CUSTOM_GUI( float start_y, float window_width )
		{
			r.x = window_width - buttonRectWidth;
			r.y = start_y + 1;
			r.width = BUT_SIZE;
			r.height = BUT_SIZE;



			DO_SNAP_BUTTON( r, SnapMod.SNAP_POS );
			if ( DRAW_SURFACE_BUTTON ) DO_SURFACE_BUTTON( new Rect( r.x + BUT_SIZE, r.y, r.width, r.height ) );
			r.y += r.height + 2;
			DO_SNAP_BUTTON( r, SnapMod.SNAP_ROT );
			if ( DRAW_SURFACE_BUTTON ) DO_NORMAL_BUTTON( new Rect( r.x + BUT_SIZE, r.y, r.width, r.height ) );
			r.y += r.height + 2;
			DO_SNAP_BUTTON( r, SnapMod.SNAP_SCALE );

		}

		void DO_SNAP_BUTTON( Rect r, VectorPref pref )
		{
			if ( GUI.Button( r, snapContent, buttonStyle ) )
			{
				if ( Event.current.button == 0 ) pref.ENABLE.Set( !pref.ENABLE );
				else
				{
					var menu = new GenericMenu();
					menu.AddItem( new GUIContent( pref.ENABLE.name ), pref.ENABLE, () => pref.X.Set( !pref.X ) );
					menu.AddSeparator( "" );
					menu.AddItem( new GUIContent( SnapMod.SNAP_FASTINVERT_USEHOTKEYS.name ), SnapMod.SNAP_FASTINVERT_USEHOTKEYS, () => SnapMod.SNAP_FASTINVERT_USEHOTKEYS.Set( !SnapMod.SNAP_FASTINVERT_USEHOTKEYS ) );
					menu.AddSeparator( "" );
					menu.AddItem( new GUIContent( SnapMod.SNAP_AUTOAPPLY.name ), SnapMod.SNAP_AUTOAPPLY, () => SnapMod.SNAP_AUTOAPPLY.Set( !SnapMod.SNAP_AUTOAPPLY ) );
					menu.AddSeparator( "" );
					menu.AddItem( new GUIContent( pref.X.name ), pref.X, () => pref.X.Set( !pref.X ) );
					menu.AddItem( new GUIContent( pref.Y.name ), pref.Y, () => pref.Y.Set( !pref.Y ) );
					menu.AddItem( new GUIContent( pref.Z.name ), pref.Z, () => pref.Z.Set( !pref.Z ) );
					menu.AddSeparator( "" );
					menu.AddItem( new GUIContent( pref.USE_LOCAL.name ), pref.USE_LOCAL, () => pref.USE_LOCAL.Set( !pref.USE_LOCAL ) );
					menu.AddSeparator( "" );
					menu.AddItem( new GUIContent( "Open unity snap settings" ), false, () =>
#if UNITY_2019_3_OR_NEWER
					EditorApplication.ExecuteMenuItem( "Edit/Grid and Snap Settings..." )
#else
					EditorApplication.ExecuteMenuItem("Edit/Snap Settings...")
#endif
					);
					menu.AddItem( new GUIContent( "Open plugin snap settings" ), false, () => { SnapButtonsForInspector.SELECT(); } );
					menu.ShowAsContext();
				}
			}
			var en = (bool)pref.ENABLE;
			if ( IsSnapInverted() ) en = !en;
			if ( Event.current.type == EventType.Repaint && en )
			{
				var oldColor = GUI.color;
				GUI.color *= enableColor;
				buttonStyle.Draw( r, snapContent, true, true, false, true );
				GUI.color = oldColor;
			}
		}

		void DO_SURFACE_BUTTON( Rect r )
		{
			var on = GUI.enabled;
			GUI.enabled &= UnityEditor.Tools.current != Tool.Rotate &&
#if UNITY_2017_3_OR_NEWER
					   UnityEditor.Tools.current != Tool.Transform &&
#endif
					   UnityEditor.Tools.current != Tool.Rect;
			if ( GUI.Button( r, raycastContent, buttonStyle ) )
			{
				if ( Event.current.button == 0 )
				{
					SnapMod.PLACE_ON_SURFACE_ENABLE.Set( !SnapMod.PLACE_ON_SURFACE_ENABLE );
				}
				else
				{
					var menu = new GenericMenu();
					menu.AddItem( new GUIContent( SnapMod.PLACE_ON_SURFACE_ENABLE.name ), SnapMod.PLACE_ON_SURFACE_ENABLE, () => SnapMod.PLACE_ON_SURFACE_ENABLE.Set( !SnapMod.PLACE_ON_SURFACE_ENABLE ) );
					menu.AddSeparator( "" );
					menu.AddItem( new GUIContent( SnapMod.ALIGN_BY[ 0 ] ), SnapMod.PLACE_ON_SURFACE_ALIGNBYMOUSE, () => SnapMod.PLACE_ON_SURFACE_ALIGNBYMOUSE.Set( true ) );
					// menu.AddItem( new GUIContent( SnapMod.ALIGN_BY[1] ), !SnapMod.PLACE_ON_SURFACE_ALIGNBYMOUSE, () => SnapMod.PLACE_ON_SURFACE_ALIGNBYMOUSE.Set( false ) );
					menu.AddDisabledItem( new GUIContent( SnapMod.ALIGN_BY[ 1 ] ) );
					menu.AddSeparator( "" );
					menu.AddItem( new GUIContent( SnapMod.CALCULATE_OBJECT_BOUNDS.name ), SnapMod.CALCULATE_OBJECT_BOUNDS, () => SnapMod.CALCULATE_OBJECT_BOUNDS.Set( !SnapMod.CALCULATE_OBJECT_BOUNDS ) );
					menu.AddSeparator( "" );
					menu.AddItem( new GUIContent( "Open plugin snap settings" ), false, () => { SnapButtonsForInspector.SELECT(); } );
					menu.ShowAsContext();
				}
			}
			if ( GUI.enabled && Event.current.type == EventType.Repaint && SnapMod.PLACE_ON_SURFACE_ENABLE )
			{
				var oldColor = GUI.color;
				GUI.color *= enableColor;
				buttonStyle.Draw( r, raycastContent, true, true, false, true );
				GUI.color = oldColor;
			}
			GUI.enabled = on;
		}

		void DO_NORMAL_BUTTON( Rect r )
		{
			var en = GUI.enabled;
			var on = GUI.enabled;
			GUI.enabled = SnapMod.PLACE_ON_SURFACE_ENABLE && UnityEditor.Tools.current != Tool.Rotate &&
#if UNITY_2017_3_OR_NEWER
					  UnityEditor.Tools.current != Tool.Transform &&
#endif
					  UnityEditor.Tools.current != Tool.Rect;
			if ( GUI.Button( r, normalContent, buttonStyle ) )
			{
				if ( Event.current.button == 0 )
				{
					SnapMod.ALIGN_BY_NORMAL.Set( !SnapMod.ALIGN_BY_NORMAL );
				}
				else
				{
					var menu = new GenericMenu();
					menu.AddItem( new GUIContent( SnapMod.ALIGN_BY_NORMAL.name ), SnapMod.ALIGN_BY_NORMAL, () => SnapMod.ALIGN_BY_NORMAL.Set( !SnapMod.ALIGN_BY_NORMAL ) );
					menu.AddSeparator( "" );
					for ( int i = 0; i < SnapMod.VECTORS.Length; i++ )
					{
						var captureI = i;
						menu.AddItem( new GUIContent( SnapMod.VECTORS_STRING[ i ] ), SnapMod.ALIGN_UP_VECTOR == i, () => SnapMod.ALIGN_UP_VECTOR.Set( captureI ) );
					}
					menu.AddSeparator( "" );
					menu.AddItem( new GUIContent( "Open plugin snap settings" ), false, () => { SnapButtonsForInspector.SELECT(); } );
					menu.ShowAsContext();
				}
			}
			if ( GUI.enabled && Event.current.type == EventType.Repaint && SnapMod.ALIGN_BY_NORMAL )
			{
				var oldColor = GUI.color;
				GUI.color *= enableColor;
				buttonStyle.Draw( r, normalContent, true, true, false, true );
				GUI.color = oldColor;
			}
			GUI.enabled = en;
		}


	}








	////////
	//////// PREFS CLASSES ////////
	////////

	class VectorPref
	{
		public CachedPref ENABLE;
		public CachedPref X;
		public CachedPref Y;
		public CachedPref Z;
		public CachedPref USE_LOCAL;

		public VectorPref( string keyPrefix, bool defaultUseLocalValue = false, bool lockUseLocalValue = false, string enableName = null )
		{
			ENABLE = new CachedPref( keyPrefix + "_ENABLE", false ) { name = enableName ?? "Enable" };
			X = new CachedPref( keyPrefix + "_X", true ) { name = "Snap x-axis" };
			Y = new CachedPref( keyPrefix + "_Y", true ) { name = "Snap y-axis" };
			Z = new CachedPref( keyPrefix + "_Z", true ) { name = "Snap z-axis" };

			USE_LOCAL = new CachedPref( keyPrefix + "_USE_LOCAL", defaultUseLocalValue, lockUseLocalValue ) { name = "Use local space" };
		}

		internal void DRAW( Rect r )
		{
			var ov=  (bool)ENABLE;
			var nv = EditorGUI.ToggleLeft(r, ENABLE.name, ov);
			if ( nv != ov ) ENABLE.Set( nv );
		}
	}

	class CachedPref
	{

		internal void DRAW( Rect r )
		{
			var ov=  (bool)this;
			var nv = EditorGUI.ToggleLeft(r, name, ov);
			if ( nv != ov ) Set( nv );
		}

		public CachedPref( string registryKey, bool defaulValue, bool lockValue = false )
		{
			this.m_registryKey = registryKey;
			this.m_boolDefaultValue = defaulValue;
			this.m_lock = lockValue;
		}

		public CachedPref( string registryKey, int defaulValue, bool lockValue = false )
		{
			this.m_registryKey = registryKey;
			this.m_intDefaultValue = defaulValue;
			this.m_lock = lockValue;
		}

		public static implicit operator bool( CachedPref d ) { return d.m_boolValue; }
		public void Set( bool value )
		{
			if ( m_lock ) return;
			m_boolValue = value;
		}

		public static implicit operator int( CachedPref d ) { return d.m_intValue; }
		public void Set( int value )
		{
			if ( m_lock ) return;
			m_intValue = value;
		}


		public string name;

		string m_registryKey;
		bool m_lock;

		bool m_boolDefaultValue;
		bool? m_boolCache;
		bool m_boolValue {
			get { return m_boolCache ?? (m_boolCache = Root.GET( m_registryKey, m_boolDefaultValue )).Value; }
			set {
				if ( m_boolValue == value ) return;
				Root.SET( m_registryKey, (m_boolCache = value).Value );
			}
		}

		int m_intDefaultValue;
		int? m_intCache;
		int m_intValue {
			get { return m_intCache ?? (m_intCache = Root.GET( m_registryKey, m_intDefaultValue )).Value; }
			set {
				if ( m_intValue == value ) return;
				Root.SET( m_registryKey, (m_intCache = value).Value );
			}
		}
	}























	class Draw
	{


		[NonSerialized]
		internal static int groupIndex = 0;
		[NonSerialized]
		internal static float padding = 20;

		//static EventType? _lastResetEvent = null;
		internal static int CurrentId;
		internal static void RESET()
		{
			currentViewWidth = (EditorGUIUtility.currentViewWidth - 16);
			CurrentId = EditorGUIUtility.GetControlID( FocusType.Passive );
			padding = 20;
			//Debug.Log(  );
			// if ( _lastResetEvent == Event.current.type ) return;
			//_lastResetEvent = Event.current.type;
			groupIndex = 0;
		}
		static  float currentViewWidth;
		static Rect _getRerct( GUILayoutOption gUILayoutOption = null )
		{
			var res = gUILayoutOption != null ? EditorGUILayout.GetControlRect(gUILayoutOption) : EditorGUILayout.GetControlRect();
			res.x = 0;
			res.width = currentViewWidth;
			res.x += padding;
			res.width -= Math.Min( 20, padding ) + padding;
			return last = res;
		}
		static Rect PEEK_NEW_WIDHT()
		{
			Rect res = Rect.zero;
			res.x = 0;
			res.width = currentViewWidth;
			res.x += padding;
			res.width -= Math.Min( 20, padding ) + padding;
			return res;
		}
		static float CALC_PADDING { get { return padding + Math.Min( 20, padding ); } }
		static GUIContent ec = new GUIContent();
		internal static Rect R05 { get { return _getRerct( GUILayout.Height( Mathf.RoundToInt( EditorGUIUtility.singleLineHeight * 0.2f ) ) ); } }
		internal static Rect R15 { get { return _getRerct( GUILayout.Height( Mathf.RoundToInt( EditorGUIUtility.singleLineHeight * 1.5f ) ) ); } }
		internal static Rect R { get { return _getRerct(); } }
		internal static Rect R2 { get { return _getRerct( GUILayout.Height( EditorGUIUtility.singleLineHeight * 2 ) ); } }
		internal static Rect RH( float h ) { return _getRerct( GUILayout.Height( h ) ); }
		internal static Rect RH( float h, int shrink, int shrink2 )
		{
			var r = _getRerct( GUILayout.Height( h ) );
			r.x += shrink;
			r.width -= shrink2;
			return r;
		}
		internal static Rect last;
		internal static Rect lastPlus {
			get {
				var r = last;
				r.x += 16;
				return r;
			}
		}

		static bool hover { get { return last.Contains( Event.current.mousePosition ); } }
		static bool press { get { return Event.current.button == 0 && Event.current.isMouse; } }
		internal static Rect Sp( float sp )
		{
			//  GUILayout.Space( sp );
			return _getRerct( GUILayout.Height( sp ) );
			//GUILayout.Space( sp );
		}

		internal static void HRx4RED()
		{
			Sp( 4 );

			Draw.HRx2();
			//EditorGUI.DrawRect(Draw.R2, Color.red);
			var c = GUI.color;
			GUI.color = Color.red;
			Draw.HRx2();
			GUI.color = c;
			/*	var r = R05;
                if (Event.current.type == EventType.Repaint)
                    s("dragHandle").Draw(r, ec, false, false, false, false);*/
			//Sp(12);
			Draw.HRx2();
			/*   r = R05;
              if ( Event.current.type == EventType.Repaint )
                  s( "dragHandle" ).Draw( r, ec, false, false, false, false );*/
			Sp( 4 );
		}

		internal static void HRx1( float resize = 0 )
		{
			Sp( 4 );
			var r = R05;
			r.width -= resize;
			if ( Event.current.type == EventType.Repaint )
				s( "dragHandle" ).Draw( r, ec, false, false, false, false );
			/*   r = R05;
              if ( Event.current.type == EventType.Repaint )
                  s( "dragHandle" ).Draw( r, ec, false, false, false, false );*/
			Sp( 4 );
		}
		internal static void HRx1( Rect r )
		{
			// r.y += r.height / 4;
			r.height /= 2;
			if ( Event.current.type == EventType.Repaint )
				s( "dragHandle" ).Draw( r, ec, false, false, false, false );
		}
		internal static void HRx05( Rect r )
		{
			r.y += (r.height - 3) / 2;
			r.height = 2f;
			if ( Event.current.type == EventType.Repaint )
				s( "preToolbar" ).Draw( r, ec, false, false, false, false );
		}
		internal static void HRx2()
		{
			Sp( 4 );
			var r = R05;
			if ( Event.current.type == EventType.Repaint )
				s( "dragHandle" ).Draw( r, ec, false, false, false, false );
			Sp( 12 );
			/*   r = R05;
              if ( Event.current.type == EventType.Repaint )
                  s( "dragHandle" ).Draw( r, ec, false, false, false, false );*/
			Sp( 4 );
		}
		internal static void EXPAND( string text )
		{
			GUI.Button( R, text, s( "preDropDown" ) );
		}

		internal static Rect Grow( Rect p, int v )
		{
			v = -v;
			p.x += v;
			p.y += v;
			p.width -= v * 2;
			p.height -= v * 2;
			return p;
		}


		static Dictionary<string, GUIStyle> _styles = new Dictionary<string, GUIStyle>();
		static Type t;
		internal static GUIStyle s( string style )
		{
			if ( _styles.ContainsKey( style ) ) return _styles[ style ];
			if ( t == null )
			{
				t = typeof( EditorWindow ).Assembly.GetType( "UnityEditor.InspectorWindow+Styles" );
				if ( t == null )
				{
					t = typeof( EditorWindow ).Assembly.GetType( "UnityEditor.PropertyEditor+Styles" );
					//if (t == null) throw new Exception("ASD");
				}
			}
			if ( t == null )
				_styles.Add( style, EditorStyles.toggle );
			else
			{
				var l = new GUIStyle(t.GetField(style, ~BindingFlags.Instance).GetValue(null) as GUIStyle);
				if ( style == "addComponentArea" )
				{
					l.fixedHeight = 0;
					l.stretchHeight = true;
					l.padding.left = 16 + 32;
				}
				else if ( style == "addComponentButtonStyle" )
				{
					l = new GUIStyle( EditorStyles.toolbarButton );
					l.fixedWidth = 0;
					l.stretchWidth = true;
					l.fixedHeight = 0;
					l.stretchHeight = true;
				}
				else if ( style == "preToolbar" )
				{
					l.fixedWidth = 0;
					l.stretchWidth = true;
					l.fixedHeight = 0;
					l.stretchHeight = true;
				}
				else
				{
					l.padding.left = 16;
				}
				l.fixedWidth = 0;
				l.stretchWidth = true;
				l.fixedHeight = 0;
				l.stretchHeight = true;
				l.alignment = TextAnchor.MiddleLeft;
				_styles.Add( style, l );
			}
			return _styles[ style ];
		}


		static Color oldc;
		internal static void HELP( string text, Color? c = null, bool drawTog = false )
		{
			//  EditorGUI.LabelField( R, text, s( "previewMiniLabel" ) );
			var _s = s("previewMiniLabel");
			_s.wordWrap = true;

			if ( c.HasValue )
			{
				oldc = GUI.color;
				GUI.color *= c.Value;
			}
			if ( drawTog ) text = "· " + text;
			var ca = _s.normal.textColor;
			if ( !EditorGUIUtility.isProSkin ) _s.normal.textColor = new Color32( 20, 20, 20, 255 );
			EditorGUI.TextArea( CALC_R( _s, text ), text, _s );
			_s.normal.textColor = ca;
			if ( c.HasValue ) GUI.color = oldc;
			// GUI.Label( CALC_R( _s, text ), text, _s );

		}
		static GUIContent _calcContent = new GUIContent();
		static Rect CALC_R( GUIStyle s, string t )
		{
			_calcContent.text = t;
			var h = s.CalcHeight(_calcContent, ( EditorGUIUtility.currentViewWidth- 16) - CALC_PADDING );
			var r = _getRerct(GUILayout.Height(h));
			return r;
		}

	}







	class SnapButtonsForInspector : ScriptableObject
	{

		static string __FOLDER;
		static internal string FOLDER {
			get {
				if ( __FOLDER != null ) return __FOLDER;
				var o = ScriptableObject.CreateInstance( typeof( SnapButtonsForInspector ) );
				if ( !o ) return null;
				var s = MonoScript.FromScriptableObject(o);
				if ( !s ) return null;
				var candidate = AssetDatabase.GetAssetPath( s );
				if ( string.IsNullOrEmpty( candidate ) ) return null;

				try
				{
					candidate = candidate.Replace( '\\', '/' );
					candidate = candidate.Remove( candidate.LastIndexOf( '/' ) );
					candidate = candidate.Trim( new[] { '/' } ) + '/';
				}
				catch
				{
					return null;
				}
				//PluginInternalFolder = candidate;
				//_PluginExternalFolder = UNITY_SYSTEM_PATH + _PluginInternalFolder;

				return __FOLDER = candidate;
			}
		}


		static SnapButtonsForInspector __GetInstance = null;
		internal static SnapButtonsForInspector GetInstance()
		{
			if ( __GetInstance ) return __GetInstance;
			var oldId = SessionState.GetInt(Root.PREFS_KEY + "setid" , -1);
			var load = InternalEditorUtility.GetObjectFromInstanceID( oldId ) as SnapButtonsForInspector;
			if ( !load )
			{
				if ( !Directory.Exists( FOLDER ) ) Directory.CreateDirectory( FOLDER );
				var path = FOLDER + "SnapModSettings.asset";
				load = AssetDatabase.LoadAssetAtPath<SnapButtonsForInspector>( path );
				if ( !load )
				{
					load = ScriptableObject.CreateInstance( typeof( SnapButtonsForInspector ) ) as SnapButtonsForInspector;
					SessionState.SetInt( Root.PREFS_KEY + "setid", load.GetInstanceID() );
					AssetDatabase.CreateAsset( load, path );
					AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate );
				}
			
			}
			__GetInstance = load;
			return load;
		}


		internal static void SELECT()
		{
			Selection.objects = new[] { GetInstance() };
		}

	}

	[CustomEditor( typeof( SnapButtonsForInspector ) )]
	class SETGUI_Snap : UnityEditor.Editor
	{



		internal static string set_text =  "Snap Transform Mod (Inspector Window)";
		internal static string WIKI_2_SNAP = "https://emem.store/wiki?Snap%20Free&%5B%20Getting%20Started&Usage";
		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();
		}

		static List<KeyCode> keys = Enumerable.Repeat(0, 122 - 97 + 1).Select((v, i) => (KeyCode)(i + 97)).ToList();


		static void DRAW_HOTKEY( CachedPref sNAP_SNAP_USEHOTKEYS, CachedPref sNAP_SNAP_HOTKEYS )
		{
			DrawToogle( sNAP_SNAP_USEHOTKEYS );
			GUI.enabled = sNAP_SNAP_USEHOTKEYS;
			GUILayout.BeginHorizontal();
			GUILayout.Space( 80 );

			var I_KEY = (int)sNAP_SNAP_HOTKEYS;
			var oldIndex = Mathf.Max(keys.FindIndex(k => (int)k == I_KEY), 0);
			//var ni = EditorGUI.Popup(Draw.R, sNAP_SNAP_HOTKEYS.name, oldIndex, keys.Select(k => k.ToString()).ToArray());
			var ni = EditorGUILayout.Popup( sNAP_SNAP_HOTKEYS.name, oldIndex, keys.Select(k => k.ToString()).ToArray());
			if ( ni != oldIndex ) I_KEY = (int)keys[ ni ];
			if ( sNAP_SNAP_HOTKEYS != I_KEY ) sNAP_SNAP_HOTKEYS.Set( I_KEY );

			GUILayout.Space( 40 );
			GUILayout.EndHorizontal();
			GUI.enabled = true;
		}

		public override void OnInspectorGUI()
		{
			_GUI();
		}

		static GUIContent __simple_wiki = new GUIContent();
		const string WIKI_LINK = "\n - Open online documentation page in browser\nLink also will be copied to text os buffer";
		static void simple_wiki( ref Rect r, string WIKI )
		{

			var wiki_rect = r;
			if ( WIKI != null )
			{
				var S = r.height * 4;

				if ( Event.current.type != EventType.Repaint ) r.width -= S;
				wiki_rect.x = wiki_rect.x + wiki_rect.width;
				if ( Event.current.type == EventType.Repaint ) wiki_rect.x -= S;
				wiki_rect.width = S;
				//Root.SetMouseTooltip( "[ " + text + " ]" + WIKI, wiki_rect );
			}
			if ( WIKI != null )
			{
				__simple_wiki.text = "-->wiki?";
				__simple_wiki.tooltip = "[ " + WIKI + " ]" + WIKI_LINK;

				//var cont = CONT( ,  );
				//Root.SetMouseTooltip( cont.tooltip, wiki_rect );
				if ( GUI.Button( (Rect)wiki_rect, __simple_wiki, EditorStyles.toolbarButton ) )
				{
					Application.OpenURL( WIKI );
					EditorGUIUtility.systemCopyBuffer = WIKI;
				}
			}
		}

		public static void _GUI()
		{


			Draw.RESET();




			/*var newT = EditorGUI.ToggleLeft(Draw.R2, set_text, Root.USE_SNAP_MOD );
			if ( newT != Root.USE_SNAP_MOD )
			{
				SnapMod.SET_ENABLE( newT );
				Root.USE_SNAP_MOD.Set( newT );
			}*/


			var ov=  (bool)Root.USE_SNAP_MOD;
			var qweqwe = Draw.R2;
			simple_wiki( ref qweqwe, WIKI_2_SNAP );
			var nv = EditorGUI.ToggleLeft(qweqwe, set_text, ov);
			if ( nv != ov )
			{
				if ( EditorUtility.DisplayDialog( "Compilation!", "Scripts compilation will start now. Are you sure?", "Yes", "No" ) )
				{
					Root.USE_SNAP_MOD.Set( nv );
					SnapMod.SET_ENABLE( nv );
				}

			}

			Draw.HELP( "Snap buttons available in the Inspector next to the position and rotation fields" );
			Draw.Sp( 10 );


			GUILayout.Space( 10 );


			//  GUILayout.Label("HotKeys to Invert Snapping States");
			DrawToogle( SnapMod.SNAP_FASTINVERT_USEHOTKEYS );
			GUI.enabled = SnapMod.SNAP_FASTINVERT_USEHOTKEYS;
			var inv = (int)SnapMod.SNAP_FASTINVERT_HOTKEYS;
			var I_KEY = inv & 0xFFFF;
			GUILayout.BeginHorizontal();
			GUILayout.Space( 80 );
			EditorGUILayout.GetControlRect( GUILayout.Height( EditorGUIUtility.singleLineHeight ) );
			var r = GUILayoutUtility.GetLastRect(); r.width /= 3;
			DrawButton( r, "Ctrl", ref inv, 16, EditorStyles.miniButtonLeft ); r.x += r.width;
			DrawButton( r, "Shift", ref inv, 17, EditorStyles.miniButtonMid ); r.x += r.width;
			DrawButton( r, "Alt", ref inv, 18, EditorStyles.miniButtonRight );

			var oldIndex = Mathf.Max(keys.FindIndex(k => (int)k == I_KEY), 0);
			//var ni = EditorGUI.Popup(Draw.R, oldIndex, keys.Select(k => k.ToString()).ToArray());
			var ni = EditorGUILayout.Popup( oldIndex, keys.Select(k => k.ToString()).ToArray());
			GUILayout.Space( 40 );
			GUILayout.EndHorizontal();
			if ( ni != oldIndex )
			{
				inv &= ~0xFFFF;
				inv |= (int)keys[ ni ];
			}
			if ( SnapMod.SNAP_FASTINVERT_HOTKEYS != inv ) SnapMod.SNAP_FASTINVERT_HOTKEYS.Set( inv );
			GUI.enabled = true;
			Draw.HRx2();

			DRAW_HOTKEY( SnapMod.SNAP_SNAP_USEHOTKEYS, SnapMod.SNAP_SNAP_HOTKEYS );
			Draw.HRx2();
			DRAW_HOTKEY( SnapMod.SNAP_SURFACE_USEHOTKEYS, SnapMod.SNAP_SURFACE_HOTKEYS );
			Draw.HRx2();


			DrawToogle( SnapMod.SNAP_AUTOAPPLY );


			GUILayout.Space( 10 );
			GUILayout.Label( "Grid Snapping" );
			DrawVector( SnapMod.SNAP_POS );
			GUILayout.Space( 5 );
			DrawVector( SnapMod.SNAP_ROT );
			GUILayout.Space( 5 );
			DrawVector( SnapMod.SNAP_SCALE );
			Draw.HELP( "You can also you right-click on icon to change snap axis" );
			GUILayout.Space( 10 );



			GUILayout.Label( "Surface Placement" );
			DrawToogle( SnapMod.PLACE_ON_SURFACE_ENABLE );
			var e = GUI.enabled;
			GUI.enabled = false;
			GUILayout.BeginHorizontal();
			var i = EditorGUILayout.Popup(SnapMod.PLACE_ON_SURFACE_ALIGNBYMOUSE ? 0 : 1, SnapMod.ALIGN_BY);
			GUILayout.Space( 40 );
			GUILayout.EndHorizontal();
			if ( i != (SnapMod.PLACE_ON_SURFACE_ALIGNBYMOUSE ? 0 : 1) ) SnapMod.PLACE_ON_SURFACE_ALIGNBYMOUSE.Set( i == 1 );
			GUI.enabled = e;
			DrawToogle( SnapMod.CALCULATE_OBJECT_BOUNDS );
			GUILayout.Space( 10 );
			DrawToogle( SnapMod.ALIGN_BY_NORMAL );
			GUILayout.BeginHorizontal();
			i = EditorGUILayout.Popup( SnapMod.ALIGN_UP_VECTOR.name, SnapMod.ALIGN_UP_VECTOR, SnapMod.VECTORS_STRING );
			GUILayout.Space( 40 );
			GUILayout.EndHorizontal();
			if ( i != SnapMod.ALIGN_UP_VECTOR ) SnapMod.ALIGN_UP_VECTOR.Set( i );

			if ( GUI.changed ) UnityEditorInternal.InternalEditorUtility.RepaintAllViews();


			//Draw.HRx4RED();
			Draw.Sp( 10 );

			//Draw.TOG_TIT( "Quick tips:" );
			GUILayout.Label( "Quick tips:" );
			//Draw.HELP_TEXTURE( w, "HELP_SNAP", 0 );
			Draw.HELP( "RMB to open a menu.", drawTog: true );
			Draw.HELP( "You can setup key hold to enable quick switching between snap states.", drawTog: true );
			//Draw.HELP( w, "Use direction and offset if you use surface snapping and align by normal.", drawTog: true );

		}





		static void DrawVector( VectorPref pref )
		{
			// if (GUILayout.Button(pref.ENABLE.name)) pref.ENABLE.Set(!pref.ENABLE);
			//  if (Event.current.type == EventType.Repaint && pref.ENABLE) GUI.skin.button.Draw(GUILayoutUtility.GetLastRect(), pref.ENABLE.name, true, true, false, true);
			//pref.ENABLE.Set(EditorGUI.ToggleLeft());
			//Draw.TOG_TIT( Draw.R15, pref.ENABLE.name,  );
			pref.DRAW( Draw.R15 );



			GUILayout.BeginHorizontal();
			var R = Draw.R2;
			R.width /= 5;
			R.x += R.width;
			pref.X.Set( EditorGUI.ToggleLeft( R, "X", (bool)pref.X ) );
			R.x += R.width;
			pref.Y.Set( EditorGUI.ToggleLeft( R, "Y", (bool)pref.Y ) );
			R.x += R.width;
			pref.Z.Set( EditorGUI.ToggleLeft( R, "Z", (bool)pref.Z ) );
			/* if (GUILayout.Button("X", EditorStyles.miniButtonLeft, new GUILayoutOption[0])) pref.X.Set(!pref.X);
			 if (Event.current.type == EventType.Repaint && pref.X) GUI.skin.button.Draw(GUILayoutUtility.GetLastRect(), pref.X.name, true, true, false, true);
			 if (GUILayout.Button("Y", EditorStyles.miniButtonMid, new GUILayoutOption[0])) pref.Y.Set(!pref.Y);
			 if (Event.current.type == EventType.Repaint && pref.Y) GUI.skin.button.Draw(GUILayoutUtility.GetLastRect(), pref.Y.name, true, true, false, true);
			 if (GUILayout.Button("Z", EditorStyles.miniButtonRight, new GUILayoutOption[0])) pref.Z.Set(!pref.Z);
			 if (Event.current.type == EventType.Repaint && pref.Z) GUI.skin.button.Draw(GUILayoutUtility.GetLastRect(), pref.Z.name, true, true, false, true);*/
			GUILayout.EndHorizontal();
		}

		static void DrawToogle( CachedPref pref )
		{
			//var b = Draw.TOG(Draw.R, pref.name, pref);
			//if ( b != pref ) pref.Set( b );
			pref.DRAW( Draw.R );
		}
		static void DrawButton( Rect r, string c, ref int val, int offset, GUIStyle s )
		{

			var oldEn = (val & (1 << offset)) != 0;

			var newEn = EditorGUI.ToggleLeft(r, c, oldEn);
			//	var newEn = Draw.TOG(r, c, oldEn);


			if ( newEn != oldEn )
			{
				if ( oldEn ) val &= ~(1 << offset);
				else val |= (1 << offset);
				oldEn = !oldEn;
			}
			// if (Event.current.type == EventType.Repaint && oldEn) GUI.skin.button.Draw(r, c, true, true, false, true);
		}



	}












}


