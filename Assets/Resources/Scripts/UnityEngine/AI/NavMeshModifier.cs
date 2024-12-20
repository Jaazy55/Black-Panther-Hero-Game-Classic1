using System;
using System.Collections.Generic;

namespace UnityEngine.AI
{
	[ExecuteInEditMode]
	[AddComponentMenu("Navigation/NavMeshModifier", 32)]
	[HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
	public class NavMeshModifier : MonoBehaviour
	{
		public bool overrideArea
		{
			get
			{
				return this.m_OverrideArea;
			}
			set
			{
				this.m_OverrideArea = value;
			}
		}

		public int area
		{
			get
			{
				return this.m_Area;
			}
			set
			{
				this.m_Area = value;
			}
		}

		public bool ignoreFromBuild
		{
			get
			{
				return this.m_IgnoreFromBuild;
			}
			set
			{
				this.m_IgnoreFromBuild = value;
			}
		}

		public static List<NavMeshModifier> activeModifiers
		{
			get
			{
				return NavMeshModifier.s_NavMeshModifiers;
			}
		}

		public bool AffectsAgentType(int agentTypeID)
		{
			return this.m_AffectedAgents.Count != 0 && (this.m_AffectedAgents[0] == -1 || this.m_AffectedAgents.IndexOf(agentTypeID) != -1);
		}

		[SerializeField]
		private bool m_OverrideArea;

		[SerializeField]
		private int m_Area;

		[SerializeField]
		private bool m_IgnoreFromBuild;

		[SerializeField]
		private List<int> m_AffectedAgents = new List<int>(new int[]
		{
			-1
		});

		private static readonly List<NavMeshModifier> s_NavMeshModifiers = new List<NavMeshModifier>();
	}
}
