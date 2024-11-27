using System;
using Game.Character;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	[AddComponentMenu("MiniMap/Map marker")]

	public class QwestStart : MonoBehaviour
	{
		
		private void OnTriggerEnter(Collider col)
		{
			if (GameEventManager.Instance.TimeQwestActive)
			{
				return;
				Debug.Log ("PrintReturn");
			}
		
			if (col.GetComponent<CharacterSensor>())
			{
				GameEventManager.Instance.StartQwest(this.Qwest);
				PoolManager.Instance.ReturnToPool(this);
				this.gameObject.GetComponent<MapMarker> ().isActive = false;
				this.gameObject.SetActive (true);




				//Destroy (this.gameObject);
			
			
			}
		
		}
	
		public Qwest Qwest;

	}

}
