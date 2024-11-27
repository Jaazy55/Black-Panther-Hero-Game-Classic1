using System;
using System.Collections;
using Game.Character;
using Game.Managers;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.GlobalComponent
{
	public class AmbientSoundPoint : MonoBehaviour
	{
		private void Start()
		{
			AmbientSoundPoint.ambiantSoundLayer = LayerMask.NameToLayer("AmbiantSound");
			base.Invoke("Init", 0.5f);
			this.currentVolume = SoundManager.instance.GetSoundValue();
			SoundManager.ValueChanged b = delegate(float value)
			{
				this.currentVolume = SoundManager.instance.GetSoundValue();
				this.currentVolume *= value;
			};
			SoundManager instance = SoundManager.instance;
			instance.GameSoundChanged = (SoundManager.ValueChanged)Delegate.Combine(instance.GameSoundChanged, b);
		}

		private void Init()
		{
			this.player = PlayerInteractionsManager.Instance.Player.transform;
			this.pointsParent = base.transform.root.Find("AudioSorses");
			this.ambientSoundPoints = base.GetComponentsInChildren<AmbientSoundPoint>();
			base.gameObject.layer = AmbientSoundPoint.ambiantSoundLayer;
			for (int i = 0; i < this.AmbientPoints.Length; i++)
			{
				GameObject fromPool = PoolManager.Instance.GetFromPool(this.AmbiantPointPrefab);
				fromPool.transform.parent = this.pointsParent;
				fromPool.transform.position = base.transform.position;
				fromPool.name = base.name + "_" + i.ToString();
				this.AmbientPoints[i].AudioSource = fromPool.GetComponent<AudioSource>();
				this.AmbientPoints[i].AudioSource.clip = this.AmbientPoints[i].AmbientSound;
				this.AmbientPoints[i].CurrentVolume = this.AmbientPoints[i].MinVolume;
				this.AmbientPoints[i].AudioSource.volume = this.AmbientPoints[i].CurrentVolume * this.currentVolume;
				this.AmbientPoints[i].AudioSource.maxDistance = this.SwitchingDistance;
				this.AmbientPoints[i].AudioSource.outputAudioMixerGroup =
					Resources.Load<AudioMixerGroup>("Workaround/NewAudioMixer");
				this.AmbientPoints[i].AudioMixer = this.AmbientPoints[i].AudioSource.outputAudioMixerGroup.audioMixer;
			}
			if (this.CurrZoneType.Equals(AmbientSoundPoint.ZoneType.Box))
			{
				this.BoxTrigger = base.gameObject.AddComponent<BoxCollider>();
				base.gameObject.AddComponent<ActivateOnTriggerStayHack>();
				this.BoxTrigger.size = this.BoxRect;
				this.BoxTrigger.isTrigger = true;
				this.SwitchingDistance = (this.BoxRect.x + this.BoxRect.y + this.BoxRect.z) / 3f;
			}
			base.StartCoroutine(this.Detect());
		}

		private bool CheckChildsPoints()
		{
			if (this.player != null)
			{
				if (Vector3.Distance(base.transform.position, this.player.position) > this.SwitchingDistance * 0.9f)
				{
					return true;
				}
				for (int i = 0; i < this.ambientSoundPoints.Length; i++)
				{
					if (this.ambientSoundPoints[i].IsPlaing && !this.ambientSoundPoints[i].Equals(this))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void PlayStopSounds(bool play = true)
		{
			for (int i = 0; i < this.AmbientPoints.Length; i++)
			{
				if (play)
				{
					this.AmbientPoints[i].AudioSource.Play();
				}
				else
				{
					this.AmbientPoints[i].AudioSource.Stop();
				}
			}
		}

		private bool Mute(bool mute)
		{
			if (mute)
			{
				for (int i = 0; i < this.AmbientPoints.Length; i++)
				{
					this.AmbientPoints[i].CurrentVolume = Mathf.Lerp(this.AmbientPoints[i].CurrentVolume, this.AmbientPoints[i].MinVolume, Time.deltaTime * 0.5f);
					this.AmbientPoints[i].AudioSource.volume = this.AmbientPoints[i].CurrentVolume * this.currentVolume;
					if (this.AmbientPoints[i].AudioSource.volume <= this.AmbientPoints[i].MinVolume + 0.001f)
					{
						return false;
					}
				}
			}
			else
			{
				for (int j = 0; j < this.AmbientPoints.Length; j++)
				{
					this.AmbientPoints[j].CurrentVolume = Mathf.Lerp(this.AmbientPoints[j].CurrentVolume, this.AmbientPoints[j].MaxVolume, Time.deltaTime * 0.5f);
					this.AmbientPoints[j].AudioSource.volume = this.AmbientPoints[j].CurrentVolume * this.currentVolume;
					if (this.AmbientPoints[j].AudioSource.volume >= this.AmbientPoints[j].MaxVolume - 0.001f)
					{
						return false;
					}
				}
			}
			return true;
		}

		private void Update()
		{
			if (this.IsPlaing)
			{
				this.Mute(this.muting);
			}
		}

		private IEnumerator Detect()
		{
			WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);
			for (;;)
			{
				if (this.player == null)
				{
					yield return waitForSeconds;
				}
				if (this.player != null)
				{
					if (this.CurrZoneType.Equals(AmbientSoundPoint.ZoneType.Sphere))
					{
						if (Vector3.Distance(base.transform.position, this.player.position) < this.SwitchingDistance && !this.IsPlaing)
						{
							this.PlayStopSounds(true);
							this.IsPlaing = true;
						}
						if (Vector3.Distance(base.transform.position, this.player.position) > this.SwitchingDistance && this.IsPlaing)
						{
							this.PlayStopSounds(false);
							this.IsPlaing = false;
						}
					}
					if (this.CurrZoneType.Equals(AmbientSoundPoint.ZoneType.Box))
					{
						if (Vector3.Distance(base.transform.position, this.player.position) < this.SwitchingDistance + 20f)
						{
							if (this.BoxTrigger)
							{
								this.BoxTrigger.enabled = true;
							}
						}
						else if (this.BoxTrigger)
						{
							this.BoxTrigger.enabled = false;
						}
					}
				}
				this.muting = this.CheckChildsPoints();
				yield return waitForSeconds;
			}
			yield break;
		}

		public void OnDrawGizmosSelected()
		{
			if (!AmbientSoundPoint.AllGizmos)
			{
				if (this.CurrZoneType.Equals(AmbientSoundPoint.ZoneType.Box))
				{
					Gizmos.matrix = base.transform.localToWorldMatrix;
					Gizmos.color = this.GizmosColor;
					Gizmos.DrawCube(Vector3.zero, this.BoxRect);
				}
				if (this.CurrZoneType.Equals(AmbientSoundPoint.ZoneType.Sphere))
				{
					Gizmos.DrawSphere(base.transform.position, this.SwitchingDistance);
				}
			}
		}

		private void OnDrawGizmos()
		{
			if (AmbientSoundPoint.AllGizmos)
			{
				Gizmos.color = this.GizmosColor;
				if (this.CurrZoneType.Equals(AmbientSoundPoint.ZoneType.Box))
				{
					Gizmos.matrix = base.transform.localToWorldMatrix;
					Gizmos.color = this.GizmosColor;
					Gizmos.DrawCube(Vector3.zero, this.BoxRect);
				}
				if (this.CurrZoneType.Equals(AmbientSoundPoint.ZoneType.Sphere))
				{
					Gizmos.DrawSphere(base.transform.position, this.SwitchingDistance);
				}
			}
		}

		private IEnumerator SoftStop()
		{
			while (this.Mute(true))
			{
				yield return new WaitForSeconds(Time.deltaTime);
			}
			this.PlayStopSounds(false);
			yield break;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!this.IsPlaing)
			{
				this.PlayStopSounds(true);
				this.IsPlaing = true;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (this.IsPlaing)
			{
				this.IsPlaing = false;
				base.StartCoroutine(this.SoftStop());
			}
		}

		private const string AmbiantSoundLayerName = "AmbiantSound";

		private const string PointsParetName = "AudioSorses";

		private const float MuteLerpSpeed = 0.5f;

		private const float DetectTimeOut = 0.5f;

		private const float MuteLerpSensitivity = 0.9f;

		private const float SwithOfset = 20f;

		private static int ambiantSoundLayer = -1;

		public static bool AllGizmos;

		[Tooltip("квадратные жрут больше чем круглые")]
		public AmbientSoundPoint.ZoneType CurrZoneType;

		private BoxCollider BoxTrigger;

		[HideInInspector]
		[Range(0f, 1000f)]
		public float SwitchingDistance = 10f;

		[HideInInspector]
		public Vector3 BoxRect;

		public GameObject AmbiantPointPrefab;

		public AmbientSoundPoint.AmbientPoint[] AmbientPoints;

		[HideInInspector]
		public bool IsPlaing;

		public Color GizmosColor = Color.cyan;

		private Transform player;

		private Transform pointsParent;

		private bool muting;

		private AmbientSoundPoint[] ambientSoundPoints;

		private float currentVolume;

		public enum ZoneType
		{
			Sphere,
			Box
		}

		[Serializable]
		public class AmbientPoint
		{
			public string Name;

			public AudioSource AudioSource;

			public AudioMixer AudioMixer;

			public float CurrentVolume;

			public AudioClip AmbientSound;

			[Range(0.1f, 1f)]
			public float MaxVolume;

			[Range(0f, 1f)]
			public float MinVolume;
		}
	}
}
