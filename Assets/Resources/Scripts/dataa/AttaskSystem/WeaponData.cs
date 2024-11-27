using System;
using UnityEngine;

namespace Naxeex.AttaskSystem
{
	[CreateAssetMenu(fileName = "WeaponData", menuName = "Attack System/Weapon Data", order = 20)]
	public class WeaponData : ScriptableObject
	{
		public GameObject ModelPrefab
		{
			get
			{
				return this.m_ModelPrefab;
			}
		}

		public AudioClip SoundAttack
		{
			get
			{
				return this.m_SoundAttack;
			}
		}

		public GameObject Tracer
		{
			get
			{
				return this.m_Tracer;
			}
		}

		public float ScatterAngle
		{
			get
			{
				return this.m_ScatterAngle;
			}
		}

		private GameObject m_ModelPrefab;

		private AudioClip m_SoundAttack;

		private GameObject m_Tracer;

		[Tooltip("Тангенс угла разброса")]
		private float m_ScatterAngle;
	}
}
