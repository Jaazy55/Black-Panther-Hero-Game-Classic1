using System;
using Game.Character;
using Game.Traffic;
using UnityEngine;

namespace Game.GlobalComponent.Quality
{
	public class QualityManager : MonoBehaviour
	{
		public static int CountVehicles
		{
			get
			{
				return BaseProfile.ResolveValue<int>("MaxCountVehicles", 5);
			}
			set
			{
				BaseProfile.StoreValue<int>(Mathf.Clamp(value, 3, 10), "MaxCountVehicles");
			}
		}

		public static int CountPedestrians
		{
			get
			{
				return BaseProfile.ResolveValue<int>("MaxCountPedestrians", 5);
			}
			set
			{
				BaseProfile.StoreValue<int>(Mathf.Clamp(value, 3, 10), "MaxCountPedestrians");
			}
		}

		public static QualityManager Instance
		{
			get
			{
				if (QualityManager.instance == null)
				{
					throw new Exception("QualityManager is not initialized");
				}
				return QualityManager.instance;
			}
		}

		public static int FarClipPlane
		{
			get
			{
				return BaseProfile.ResolveValue<int>("FarClipPlane", 150);
			}
			set
			{
				BaseProfile.StoreValue<int>(value, "FarClipPlane");
			}
		}

		private static int QualityLevel
		{
			get
			{
				return BaseProfile.ResolveValue<int>("QualityLevel", 6);
			}
			set
			{
				BaseProfile.StoreValue<int>(value, "QualityLevel");
			}
		}

		public static QualityLvls QualityLvl
		{
			get
			{
				switch (QualityManager.QualityLevel)
				{
				case 6:
					return QualityLvls.Low;
				case 7:
					return QualityLvls.Mid;
				case 8:
					return QualityLvls.High;
				case 9:
					return QualityLvls.Ultra;
				default:
					return QualityLvls.Low;
				}
			}
		}

		private void Awake()
		{
			QualityManager.instance = this;
		}

		private void Start()
		{
			QualityManager.SetQuality(QualityManager.QualityLevel, QualityManager.FarClipPlane, true);
		}

		public static void ChangeFog(bool disable = false)
		{
			if (QualityManager.QualityLevel == 6 || disable)
			{
				RenderSettings.fog = false;
			}
			else
			{
				RenderSettings.fog = true;
				RenderSettings.fogMode = FogMode.Linear;
				RenderSettings.fogEndDistance = (float)QualityManager.FarClipPlane;
				RenderSettings.fogStartDistance = (float)(QualityManager.FarClipPlane / 4);
			}
		}

		public static void SetCountPedestrians(int value)
		{
			QualityManager.CountPedestrians = value;
			TrafficSlider.UpdteValue();
			try
			{
				if (TrafficManager.Instance)
				{
					TrafficManager.Instance.MaxCountPedestrians = value;
				}
			}
			catch (Exception)
			{
			}
		}

		public static void SetCountVehicles(int value)
		{
			QualityManager.CountVehicles = value;
			TrafficSlider.UpdteValue();
			try
			{
				if (TrafficManager.Instance)
				{
					TrafficManager.Instance.MaxCountVehicles = value;
				}
			}
			catch (Exception)
			{
			}
		}

		public static void ChangeQuality(QualityLvls lvl, bool setNow = false)
		{
			switch (lvl)
			{
			case QualityLvls.Low:
				QualityManager.ChangeQuality(6, setNow);
				break;
			case QualityLvls.Mid:
				QualityManager.ChangeQuality(7, setNow);
				break;
			case QualityLvls.High:
				QualityManager.ChangeQuality(8, setNow);
				break;
			case QualityLvls.Ultra:
				QualityManager.ChangeQuality(9, setNow);
				break;
			}
		}

		public static void ChangeQuality(int level, bool setNow = false)
		{
			int level2;
			int farClip;
			switch (level)
			{
			case 7:
				level2 = 7;
				farClip = 250;
				break;
			case 8:
				level2 = 8;
				farClip = 450;
				break;
			case 9:
				level2 = 9;
				farClip = 650;
				break;
			default:
				level2 = 6;
				farClip = 150;
				break;
			}
			QualityManager.SetQuality(level2, farClip, setNow);
		}

		public void ChangeCameraCliping(float farClip)
		{
			CameraManager.Instance.UnityCamera.farClipPlane = (float)((int)farClip);
			QualityManager.FarClipPlane = (int)farClip;
			QualityManager.ChangeFog(false);
		}

		private static void ChangeTrafficDensity(int qualityLevel)
		{
			QualityManager.SetCountPedestrians((qualityLevel != 6) ? QualityManager.CountPedestrians : 3);
			QualityManager.SetCountVehicles((qualityLevel != 6) ? QualityManager.CountVehicles : 3);
		}

		private static void SetQuality(int level, int farClip, bool change)
		{
			if (change)
			{
				if (GameplayUtils.OnPause)
				{
					WaitingPanelController.Instance.StartWaiting(delegate()
					{
						QualitySettings.SetQualityLevel(level);
						QualityManager.ChangeFog(false);
						CameraManager.Instance.UnityCamera.farClipPlane = (float)farClip;
					}, 30);
				}
				else
				{
					QualitySettings.SetQualityLevel(level);
					QualityManager.ChangeFog(false);
					CameraManager.Instance.UnityCamera.farClipPlane = (float)farClip;
				}
			}
			QualityManager.ChangeTrafficDensity(level);
			QualityManager.QualityLevel = level;
			QualityManager.FarClipPlane = farClip;
			if (QualityManager.updateQuality != null)
			{
				QualityManager.updateQuality();
			}
		}

		public const int LowQualityLevelIndex = 6;

		public const int MidQualityLevelIndex = 7;

		public const int HighQualityLevelIndex = 8;

		public const int UltraQualityLevelIndex = 9;

		public const int LowQualityClipPlane = 150;

		public const int MidQualityClipPlane = 250;

		public const int HighQualityClipPlane = 450;

		public const int UltraQualityClipPlane = 650;

		public const int MinFarClipPlane = 100;

		public const int MaxFarClipPlane = 650;

		public const int MinCountVehicles = 3;

		public const int MaxCountVehicles = 10;

		public const int MinCountPedestrians = 3;

		public const int MaxCountPedestrians = 10;

		private static QualityManager instance;

		public static QualityManager.UpdateQuality updateQuality;

		public delegate void UpdateQuality();
	}
}
