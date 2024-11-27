using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.MiniMap
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class SnapShotForMiniMap : MonoBehaviour
	{
		public static void CreateNewMinimapSnapshot(Vector2 snapshotSize)
		{
			WorldSpaceForMiniMap worldSpaceForMiniMap = UnityEngine.Object.FindObjectOfType<WorldSpaceForMiniMap>();
			if (worldSpaceForMiniMap == null)
			{
				throw new Exception("Not find world space!");
			}
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("SnapshotCamera"));
			gameObject.transform.parent = worldSpaceForMiniMap.gameObject.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, gameObject.transform.position.z);
			gameObject.transform.localEulerAngles = Vector3.zero;
			gameObject.GetComponent<Camera>().orthographicSize = worldSpaceForMiniMap.GetComponent<RectTransform>().rect.width / 2f;
			SnapShotForMiniMap component = gameObject.GetComponent<SnapShotForMiniMap>();
			component.ResWidth = (int)snapshotSize.x;
			component.ResHeight = (int)snapshotSize.y;
			string path = component.TakeSnapshot();
			UnityEngine.Object.FindObjectOfType<MiniMap>().MapTexture = Resources.Load<Texture>(path);
			UnityEngine.Object.DestroyImmediate(gameObject);
		}

		[ContextMenu("Take Snap Shot")]
		public string TakeSnapshot()
		{
			return string.Empty;
		}

		private string SnapshotName(int width, int height, out string snapShotName)
		{
			string name = SceneManager.GetActiveScene().name;
			snapShotName = string.Format("MapSnapshot_{0}_{1}x{2}_{3}", new object[]
			{
				name,
				width,
				height,
				DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
			});
			return this.GetFullFolderPath() + snapShotName + ".png";
		}

		private string GetFullFolderPath()
		{
			return Application.dataPath + SnapShotForMiniMap.folderPath;
		}

		public int ResWidth = 1024;

		public int ResHeight = 1024;

		[Space(7f)]
		[InspectorButton("TakeSnapshot")]
		public string TakeSnapShot = string.Empty;

		private static int msaa = 1;

		private static string folderPath = "/Prefabs/Managers/MiniMap/Resources/";
	}
}
