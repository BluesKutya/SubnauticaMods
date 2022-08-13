using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace SeaTruckNuclearRod
{
	class SeaTruckNuclearControl: MonoBehaviour, IPowerInterface
	{

		private static float powerPerCycle = 5f;

		public SeaTruckUpgrades seatruckUpgr;

		public int slotID = -1;

		public Vehicle vehicle = null;

		private SeaTruckSegment _mainSts = null;

		private Battery _battery = null;

		private IPowerInterface batteryIpo = null;

		private List<IPowerInterface> vehicleBatteries = null;

		public bool isActive = false;

		~SeaTruckNuclearControl()
        {
			isActive = false;
		}

		void Awake()
		{
			vehicle = base.gameObject.GetComponent<Vehicle>();

			foreach (Battery battery in base.GetComponentsInChildren<Battery>(true))
				if (battery.name == "SeaTruckNuclearRod(Clone)") {
					_battery = battery;
					break;
				}

			_mainSts = base.GetComponentInParent<SeaTruckSegment>();
			if (_mainSts != null)
				_mainSts.FindMainCabBattery(out _mainSts, out vehicleBatteries);

			if (vehicleBatteries == null)
				vehicleBatteries = new List<IPowerInterface>();

			if ((_battery != null) && (vehicleBatteries.Count > 0))
				isActive = true;

			batteryIpo = this as IPowerInterface;
		}


		void Update()
		{
			bool isReactorRodEmpty = false;

			if (!isActive)
				return;

			if (batteryIpo.GetPower() == 0)
			{
				isReactorRodEmpty = true;
			}

			if (!isReactorRodEmpty)
			{
				foreach (IPowerInterface ipo in vehicleBatteries)
				{
					float power;
					float ap = 0f;
					if (batteryIpo.GetPower() > 0)
					{
						ap = ipo.GetMaxPower() - ipo.GetPower();
						if (ap > 0)
						{
							power = (ap > powerPerCycle) ? powerPerCycle : ap;
							power = (batteryIpo.GetPower() < power) ? batteryIpo.GetPower() : power;

							ipo.AddEnergy(power, out ap);
							batteryIpo.ConsumeEnergy(power, out ap);
						}
					} else {
						isReactorRodEmpty = true;
						break;
					}
				}
			}

			if (isReactorRodEmpty) { //Depleting empty rod
				isActive = false;

				if (this.slotID > -1) {
					string slotID = SeaTruckUpgrades.slotIDs[this.slotID]; ;
					Object.Destroy(seatruckUpgr.modules.RemoveItem(slotID, true, false).item.gameObject);
					seatruckUpgr.StartCoroutine(AddDepletedRodToEquipmentAsync(slotID));
				}
			}

		}

		private IEnumerator AddDepletedRodToEquipmentAsync(string slotID)
		{
			CoroutineTask<GameObject> request = CraftData.GetPrefabForTechTypeAsync(TechType.DepletedReactorRod, true);
			yield return request;
			Pickupable component = Object.Instantiate<GameObject>(request.GetResult()).GetComponent<Pickupable>();
			component.Pickup(false);
			InventoryItem newItem = new InventoryItem(component);
			seatruckUpgr.modules.AddItem(slotID, newItem, true);
			yield break;
		}

		//IPowerInterface - BEGIN

		float IPowerInterface.GetPower()
		{
			return _battery._charge;
		}

		float IPowerInterface.GetMaxPower()
		{
			return _battery._capacity;
		}

		bool IPowerInterface.ModifyPower(float amount, out float modified)
		{
			modified = _battery._charge;
			if (amount > 0)
				return false;

			if ((_battery._charge + amount) < 0)
			{
				return false;
			}

			_battery._charge += amount;
			modified = _battery._charge;

			return true;
		}

		bool IPowerInterface.HasInboundPower(IPowerInterface powerInterface)
		{
			return false;
		}

		bool IPowerInterface.GetInboundHasSource(IPowerInterface powerInterface)
		{
			return false;
		}

		void IPowerInterface.PollPowerRate(out float consumed, out float created)
		{
			consumed = 0f;
			created = _battery._capacity - _battery._charge;
		}

        GameObject IPowerInterface.GetGameObject()
        {
			return seatruckUpgr.gameObject;
        }

		//IPowerInterface - END

	}
}
