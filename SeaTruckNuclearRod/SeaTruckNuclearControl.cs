using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UWE;

namespace SeaTruckNuclearRod
{
	class SeaTruckNuclearControl: MonoBehaviour, IPowerInterface
	{

		public SeaTruckUpgrades seatruckUpgr;

		public int slotID = -1;

		private SeaTruckSegment _mainSts = null;

		private PowerRelay _powerRelay = null;

		private Battery _battery = null;

		private IPowerInterface batteryIpo = null;

		private List<IPowerInterface> vehicleBatteries = null;

		private float timeLastPowerTransfered = 0f;

		public bool isActive = false;
		public bool isUpdate = false;

		private Event<PowerRelay>.HandleFunction relayPowerDownEvent = null;
		private Event<PowerRelay>.HandleFunction relayPowerUpEvent = null;
		private Event<PowerRelay>.HandleFunction relayPowerStatusEvent = null;

		public void setBattery(Battery battery) {
			_battery = battery;
		}

		~SeaTruckNuclearControl()
        {
			isActive = false;
		}

		private void OnDestroy()
		{
			if (GameApplication.isQuitting)
				return;

			if (_powerRelay != null) {
				if (this.relayPowerDownEvent == null)
					_powerRelay.powerDownEvent.RemoveHandler(this, this.relayPowerDownEvent);

				if (this.relayPowerUpEvent == null)
					_powerRelay.powerUpEvent.RemoveHandler(this, this.relayPowerUpEvent);

				if (this.relayPowerStatusEvent == null)
					_powerRelay.powerStatusEvent.RemoveHandler(this, this.relayPowerStatusEvent);
			}

			if (vehicleBatteries == null)
				vehicleBatteries = new List<IPowerInterface>();

			if ((_battery != null) && (vehicleBatteries.Count > 0))
			{
				foreach (IPowerInterface ipo in vehicleBatteries)
					if (ipo as EnergyMixin)
					{
						StorageSlot s = ((EnergyMixin)ipo).batterySlot;
						s.onRemoveItem -= batterySlotChangedEvent;
						s.onAddItem -= batterySlotChangedEvent;
					}
			}

		}

		private void OnPowerStatusChanged(bool isRelayPowered)
		{
			isUpdate = true;

			isActive = isRelayPowered;

			if (isActive) {
				_mainSts.FindMainCabBattery(out _mainSts, out vehicleBatteries);

				if (vehicleBatteries == null)
					vehicleBatteries = new List<IPowerInterface>();

				if ((_battery != null) && (vehicleBatteries.Count > 0))
					isActive = true;

			}

			isUpdate = false;
		}

        private void batterySlotChangedEvent(InventoryItem item)
        {
			OnPowerChanged(_powerRelay);
		}

		public void Start()
		{

			if ((_mainSts != null) && _powerRelay != null)
			{

				if (this.relayPowerDownEvent == null) {
					this.relayPowerDownEvent = new Event<PowerRelay>.HandleFunction(OnPowerChanged);
					_powerRelay.powerDownEvent.AddHandler(this, this.relayPowerDownEvent);
				}

				if (this.relayPowerUpEvent == null) {
					this.relayPowerUpEvent = new Event<PowerRelay>.HandleFunction(OnPowerChanged);
					_powerRelay.powerUpEvent.AddHandler(this, this.relayPowerUpEvent);
				}

				if (this.relayPowerStatusEvent == null) {
					this.relayPowerStatusEvent = new Event<PowerRelay>.HandleFunction(OnPowerChanged);
					_powerRelay.powerStatusEvent.AddHandler(this, this.relayPowerStatusEvent);
				}

				_mainSts.FindMainCabBattery(out _mainSts, out vehicleBatteries);
				if (vehicleBatteries == null)
					vehicleBatteries = new List<IPowerInterface>();

				if ((_battery != null) && (vehicleBatteries.Count > 0))
				{
					foreach (IPowerInterface ipo in vehicleBatteries)
					{
						if (ipo as EnergyMixin)
						{
							StorageSlot s = ((EnergyMixin)ipo).batterySlot;
							s.onRemoveItem += batterySlotChangedEvent;
							s.onAddItem += batterySlotChangedEvent;
						}
					}

					isActive = true;
				}

			}
		}

		void Awake()
		{
			batteryIpo = this as IPowerInterface;

			_mainSts = base.GetComponentInParent<SeaTruckSegment>();

			if (_mainSts != null)
				_powerRelay = _mainSts.transform.GetComponentInParent<PowerRelay>();

		}

		void Update()
		{
			bool isReactorRodEmpty = false;

			if (isUpdate || !isActive || (timeLastPowerTransfered + 1f >= Time.time))
				return;

			timeLastPowerTransfered = Time.time;

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
						if (ap >= 0)
						{
							power = ap;
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

		private void OnPowerChanged(PowerRelay powerRelay)
		{
			OnPowerStatusChanged(powerRelay.IsPowered());
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
