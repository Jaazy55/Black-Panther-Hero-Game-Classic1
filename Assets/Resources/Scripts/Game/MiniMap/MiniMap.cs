using System;
using System.Diagnostics;
using Game.Character.CharacterController;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnitySampleAssets.CrossPlatformInput;

namespace Game.MiniMap
{
	public class MiniMap : MonoBehaviour
	{
		public static bool HasInstance
		{
			get
			{
				return MiniMap.instance != null;
			}
		}

		public static MiniMap Instance
		{
			get
			{
				if (MiniMap.instance == null)
				{
					MiniMap.instance = UnityEngine.Object.FindObjectOfType<MiniMap>();
				}
				return MiniMap.instance;
			}
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event UnityAction<bool> OnChangeSignal;

		private GameObject firstTarget
		{
			get
			{
				return PlayerManager.Instance.Player.gameObject;
			}
		}

		public bool HasSignal
		{
			get
			{
				return this.m_hasSignal;
			}
			set
			{
				if (this.m_hasSignal != value)
				{
					this.m_hasSignal = value;
					if (!value)
					{
						float farClipPlane = this.MiniMapCamera.farClipPlane;
						float nearClipPlane = this.MiniMapCamera.nearClipPlane;
						this.MiniMapCamera.nearClipPlane = 0f;
						this.MiniMapCamera.farClipPlane = 0.01f;
						this.MiniMapCamera.Render();
						this.MiniMapCamera.farClipPlane = farClipPlane;
						this.MiniMapCamera.nearClipPlane = nearClipPlane;
					}
					this.MiniMapCamera.gameObject.SetActive(value);
					if (this.m_NoSignalPanel != null)
					{
						this.m_NoSignalPanel.gameObject.SetActive(!value);
					}
					if (this.OnChangeSignal != null)
					{
						this.OnChangeSignal(value);
					}
				}
			}
		}

		private void Awake()
		{
			if (MiniMap.miniMapLayerNumber == -1)
			{
				MiniMap.miniMapLayerNumber = LayerMask.NameToLayer("MiniMap");
			}
			this.Init();
		}

		private void Init()
		{
			if (MiniMap.instance == null)
			{
				MiniMap.instance = this;
			}
			this.MiniMapCamera.orthographicSize = this.MiniMapOrthographicSize;
			this.PlayerMark.SetActive(false);
			this.CreateMapPlane();
		}

		private void Update()
		{
			if (!this.Target || !this.m_hasSignal)
			{
				return;
			}
			if (this.IsFullScreen)
			{
				this.FullMapControll();
			}
			else
			{
				this.PositionControl();
				this.RotationControl(this.PlayerIcon.gameObject, this.Target);
				this.RotationControl(this.VisibleAreaImage.gameObject, this.GameCamera);
			}
		}

		public void SetTarget(GameObject newTarget)
		{
			this.Target = newTarget;
		}

		public void ResetTarget()
		{
			this.Target = this.firstTarget;
		}

		private void CreateMapPlane()
		{
			//this.MiniMapCamera.cullingMask = 1 << MiniMap.miniMapLayerNumber;
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
			gameObject.transform.Rotate(Vector3.up, 580f);
			Vector3 vector = this.WorldSpace.sizeDelta;
			gameObject.transform.localPosition = this.WorldSpace.localPosition;
			gameObject.transform.localScale = new Vector3(vector.x, 1f, vector.y) / 10f;
			gameObject.layer = MiniMap.miniMapLayerNumber;
			gameObject.GetComponent<Renderer>().material = this.CreateMaterial();
		}

		private Material CreateMaterial()
		{
			Material material = new Material(this.MapPlaneShader)
			{
				mainTexture = this.MapTexture,
				color = this.tintColor
			};
			material.SetColor("_SpecColor", this.specularColor);
			material.SetColor("_Emission", this.emessiveColor);
			return material;
		}

		private void PositionControl()
		{
			Vector3 position = this.Target.transform.position;
			position.y = 0f;
			base.transform.position = position;
		}

		private void RotationControl(GameObject rotatedObject, GameObject sourceObject)
		{
			RectTransform component = rotatedObject.GetComponent<RectTransform>();
			Vector3 zero = Vector3.zero;
			zero.z = -sourceObject.transform.eulerAngles.y;
			component.eulerAngles = zero;
		}

		public void ChangeMapSize(bool fullScreen)
		{
			this.IsFullScreen = fullScreen;
			if (this.IsFullScreen)
			{
				this.MiniMapCamera.orthographicSize = this.FullMapOrthographicSize;
				this.PlayerMark.transform.position = this.Target.transform.position;
				this.PlayerMark.GetComponent<MarkForMiniMap>().RotateIcon(this.Target.transform.eulerAngles);
			}
			else
			{
				this.MiniMapCamera.orthographicSize = this.MiniMapOrthographicSize;
			}
			this.PlayerMark.SetActive(this.IsFullScreen);
		}

		private void FullMapControll()
		{
			Vector3 a = new Vector3(-CrossPlatformInputManager.GetVirtualOnlyAxis("Horizontal_Map", false), 0f, -CrossPlatformInputManager.GetVirtualOnlyAxis("Vertical_Map", false));
			Vector3 b = this.ScrollSpeed * this.MiniMapCamera.orthographicSize * a;
			float num = CrossPlatformInputManager.GetAxis("MapZoom") * this.ZoomSpeed;
			Vector3 vector = base.transform.position + b;
			vector.y = 0f;
			Vector3 position = this.WorldSpace.transform.position;
			position.y = 0f;
			float num2 = this.MiniMapCamera.orthographicSize + num;
			float num3 = this.WorldSpace.rect.width / 2f + 1000f;
			num2 = Mathf.Clamp(num2, this.MinOrthographicSize, num3);
			if (Vector3.Distance(vector, position) > num3 - num2)
			{
				vector = base.transform.position;
				if (num > 0f && num2 != num3)
				{
					vector = Vector3.MoveTowards(vector, position, num);
				}
			}
			base.transform.position = vector;
			this.MiniMapCamera.orthographicSize = num2;
		}

		public void LocateUserMark(Vector3 pointerPosition)
		{
			Ray ray = new Ray(this.MiniMapCamera.transform.position + pointerPosition, Vector3.down);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, this.MiniMapCamera.farClipPlane, this.MiniMapCamera.cullingMask))
			{
				this.UserMark.transform.position = raycastHit.point;
				this.UserMark.SetActive(true);
			}
		}

		public void MarkOnClick(Vector3 pointerPosition)
		{
			Ray ray = new Ray(this.MiniMapCamera.transform.position + pointerPosition, Vector3.down);
			MarkButton markButton = null;
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, this.MiniMapCamera.farClipPlane, this.MiniMapCamera.cullingMask))
			{
				markButton = raycastHit.collider.gameObject.GetComponent<MarkButton>();
			}
			if (markButton != null)
			{
				markButton.OnMarkClick();
			}
		}

		private static MiniMap instance;

		private static int miniMapLayerNumber = -1;

		private const int AdditionalBorderSize = 1000;

		private const string MiniMapLayerName = "MiniMap";

		private const string ShaderSpecularColorPropertyName = "_SpecColor";

		private const string ShaderEmissionPropertyName = "_Emission";

		private const string HorizontalScrollAxis = "Horizontal_Map";

		private const string VerticalScrollAxis = "Vertical_Map";

		private const string ZoomAxis = "MapZoom";

		public GameObject Target;

		public GameObject GameCamera;

		public Camera MiniMapCamera;

		public RectTransform WorldSpace;

		public Image PlayerIcon;

		public Image VisibleAreaImage;

		public Shader MapPlaneShader;

		public float MiniMapOrthographicSize = 120f;

		public float FullMapOrthographicSize = 550f;

		[SerializeField]
		private Image m_NoSignalPanel;

		[Separator("FullMapControlOptions")]
		public float ScrollSpeed = 2f;

		public float ZoomSpeed = 10f;

		public float MinOrthographicSize = 250f;

		public float MaxOrthographicSize = 1600f;

		public GameObject UserMark;

		public GameObject PlayerMark;

		[Separator]
		public Texture MapTexture;

		public Vector2 MapTextureResolution = new Vector2(1024f, 1024f);

		[HideInInspector]
		public bool IsFullScreen;

		private bool m_hasSignal = true;

		private readonly Color tintColor = new Color(1f, 1f, 1f, 0.9f);

		private readonly Color specularColor = new Color(1f, 1f, 1f, 0.9f);

		private readonly Color emessiveColor = new Color(0f, 0f, 0f, 0.9f);
	}
}
