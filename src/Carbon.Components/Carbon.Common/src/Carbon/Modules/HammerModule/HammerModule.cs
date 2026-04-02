using Facepunch;
using Newtonsoft.Json;
using Oxide.Game.Rust.Cui;
using Timer = Oxide.Plugins.Timer;

namespace Carbon.Modules;

public partial class HammerModule : CarbonModule<HammerModule.HammerConfig, HammerModule.HammerData>
{
	public const string cuiName = "hammereditor.cui";

	public override string Name => "Hammer";
	public override VersionNumber Version => new(1, 0, 0);
	public override bool EnabledByDefault => false;
	public override Type Type => typeof(HammerModule);

	public ListHashSet<Func<BaseEntity, bool, (string name, object value, bool shouldShow)>> CustomFields = new();
	public ListHashSet<Func<BaseEntity, bool, (string name, string color, string command, bool shouldShow)>> CustomButons = new();

	private static readonly Translate.Phrase destroyingBuildingCancelledPhrase = new("destroyedbuildingCancelled", "Destroying building: <color=white>cancelled</color>");
	private static readonly Translate.Phrase destroyingBuildingPhrase = new("destroyedbuilding", "Destroying building: <color=white>{0}</color>/{1} entities ({2} dead)");
	private static readonly Translate.Phrase repairedCancelledPhrase = new("repairedCancelled", "Repairing: <color=white>cancelled</color>");
	private static readonly Translate.Phrase repairedPhrase = new("repaired", "Repairing: <color=white>{0}</color>/{1} entities ({2} needed repair, {3} dead)");

	private static readonly Dictionary<ulong, BaseEntity> lastCreativeModePlayers = new();
	private static readonly Dictionary<ulong, BaseEntity> lastLastCreativeModePlayers = new();
	private static readonly Dictionary<string, ModalModule.Modal.Field> temp = new();
	private static CuiDraggableComponent cachedDraggable = new();
	private static HammerModule ins;
	private static bool isSubscribedToOnPlayerInput = true;
	private static bool forcefullySubscribeToOnPlayerInput;

	private static readonly string[] blacklistedMovingPrefabs =
	[
		"crudeoutput",
		"hopperoutput",
		"fuelstorage",
		"excavator_output_pile",
		"static",
		"caboose",
		"elevator",
		"mission",
	];

	public ModalModule Modal;

	private Timer timer;

	public override void Init()
	{
		base.Init();
		ins = this;
	}

	public override void OnPostServerInit(bool initial)
	{
		base.OnPostServerInit(initial);
		Modal = BaseModule.GetModule<ModalModule>();
	}

	public override void OnEnabled(bool initialized)
	{
		base.OnEnabled(initialized);
		timer?.Destroy();
		timer = Community.Runtime.Core.timer.Every(UIRefreshRate, TickCheck);
		ValidatePermanentPlayerInputHook();
	}

	public override void OnDisabled(bool initialized)
	{
		base.OnDisabled(initialized);
		timer?.Destroy();
		timer = null;

		lastCreativeModePlayers.Clear();
		lastLastCreativeModePlayers.Clear();
		for (int i = 0; i < BasePlayer.activePlayerList.Count; i++)
		{
			var player = BasePlayer.activePlayerList[i];
			ClearGUI(player);
		}
		foreach (var hammer in DataInstance.Hammers)
		{
			var hammerVal = hammer.Value;
			hammerVal.Reset();
		}
	}

	public override void Load()
	{
		base.Load();
		ValidatePermanentPlayerInputHook();
	}

	public bool ShouldSubscribeToOnPlayerInput()
	{
		if (forcefullySubscribeToOnPlayerInput)
		{
			return true;
		}
		for(int i = 0; i < BasePlayer.activePlayerList.Count; i++)
		{
			var player = BasePlayer.activePlayerList[i];
			if (!player.IsValid())
			{
				continue;
			}
			if (player.IsInCreativeMode)
			{
				return true;
			}
		}
		return false;
	}

	public void ValidatePermanentPlayerInputHook()
	{
		foreach(var hammer in DataInstance.Hammers)
		{
			if (hammer.Value.bypassHammer || hammer.Value.bypassCreativeMode)
			{
				forcefullySubscribeToOnPlayerInput = true;
				return;
			}
		}
		forcefullySubscribeToOnPlayerInput = false;
	}

	public bool CanBeMoved(BasePlayer player, BaseEntity entity)
	{
		if (!entity.IsValid())
		{
			return false;
		}

		if (player.net.ID.Equals(entity.net.ID))
		{
			return false;
		}

		switch (entity)
		{
			case SimpleBuildingBlock:
			case BuildingBlock:
			{
				return false;
			}
			case IOEntity ioEntity:
			{
				if(ioEntity.GetConnectedInputCount() > 0 || ioEntity.GetConnectedOutputCount() > 0)
				{
					return false;
				}
				break;
			}
		}

		if (entity.GetRootParentEntity() is PlayerBoat || entity.PrefabName.Contains("boatbuilding", CompareOptions.OrdinalIgnoreCase))
		{
			return false;
		}

		if (MoveEverything)
		{
			return true;
		}

		if (entity is BasePlayer targetPlayer && targetPlayer.IsSleeping())
		{
			return true;
		}


		for (int i = 0; i < blacklistedMovingPrefabs.Length; i++)
		{
			// Exceptions
			if (entity is MiningQuarry)
			{
				continue;
			}

			if (entity.ShortPrefabName.Contains(blacklistedMovingPrefabs[i], StringComparison.CurrentCultureIgnoreCase))
			{
				return false;
			}
		}

		return entity switch
		{
			NPCVendingMachine => false,
			BigWheelBettingTerminal => false,
			SlotMachine => false,
			MarketTerminal => false,
			FuseBox => false,
			ANDSwitch or ORSwitch or XORSwitch or RANDSwitch or SmartSwitch or DummySwitch or ElectricSwitch or FluidSwitch or TimerSwitch or PressButton or RFBroadcaster or CardReader or DoorManipulator => false,
			Recycler => false,
			WheelSwitch => false,
			ProgressDoor => false,
			HackableLockedCrate => false,
			Door => false,
			Lift => false,
			ComputerStation => false,
			HarborCraneContainerPickup or HarborCraneStatic or MagnetCrane => false,
			Barricade => false,

			BaseSubmarine => true,
			Candle => true,
			DroppedItemContainer => true,
			CinematicEntity => true,
			HotAirBalloon => true,
			BaseHelicopter => true,
			BaseChair => true,
			BaseBoat => true,
			BaseCorpse => true,
			BaseLadder => true,
			RidableHorse => true,
			MiningQuarry => true,
			TreeEntity => true,
			Snowmobile or Bike or Minicopter or ScrapTransportHelicopter => true,
			ModularCar or BasicCar => true,
			DecayEntity => true,
			_ => entity.ShortPrefabName switch
			{
				_ when entity.ShortPrefabName.Contains("deploy", CompareOptions.IgnoreCase) => true,
				_ when entity.ShortPrefabName.Contains("cliff", CompareOptions.IgnoreCase) => true,
				_ when entity.ShortPrefabName.Contains("rock", CompareOptions.IgnoreCase) => true,
				_ when entity.ShortPrefabName.Contains("admin_invis", CompareOptions.IgnoreCase) => true,
				_ when entity.ShortPrefabName.Contains("grass_displace", CompareOptions.IgnoreCase) => true,
				_ => false
			}
		};
	}

	public bool CanBeToggled(BaseEntity entity)
	{
		return entity switch
		{
			Door => true,
			IOEntity ioEntity when ioEntity.inputs.Length > 0 => true,
			StorageContainer => true,
			IAlwaysOn => true,
			MiningQuarry or EngineSwitch => true,
			BuildingBlock => true,
			VendingMachine => true,
			SteeringWheel => true,
			_ => false
		};
	}

	public void TickCheck()
	{
		using(TimeMeasure.New("Hammer.TickCheck", 100))
		{
			var shouldSubscribeToOPI = ShouldSubscribeToOnPlayerInput();
			if (shouldSubscribeToOPI && !isSubscribedToOnPlayerInput)
			{
				Subscribe(nameof(OnPlayerInput));
				isSubscribedToOnPlayerInput = true;
			}
			else if (!shouldSubscribeToOPI && isSubscribedToOnPlayerInput)
			{
				Unsubscribe(nameof(OnPlayerInput));
				isSubscribedToOnPlayerInput = false;
			}

			using var missingPlayers = Pool.Get<PooledList<HammerEditor>>();
			foreach (var playerId in lastCreativeModePlayers)
			{
				var editor = DataInstance.GetOrCreateEditor(playerId.Key);
				if (!ShouldShowUI(editor, out _))
				{
					missingPlayers.Add(editor);
				}
			}

			lastLastCreativeModePlayers.Clear();
			foreach (var element in lastCreativeModePlayers)
			{
				lastLastCreativeModePlayers[element.Key] = element.Value;
			}

			lastCreativeModePlayers.Clear();
			for (int i = 0; i < BasePlayer.activePlayerList.Count; i++)
			{
				var player = BasePlayer.activePlayerList[i];
				var editor = DataInstance.GetOrCreateEditor(player.userID);
				if (ShouldShowUI(editor, out var entity) && !editor.showExtra)
				{
					if (!lastLastCreativeModePlayers.TryGetValue(player.userID, out var lastEntity) || lastEntity != entity)
					{
						ApplyGUI(player, entity, false);
					}
					lastCreativeModePlayers[player.userID] = entity;
				}
			}
			for (int i = 0; i < missingPlayers.Count; i++)
			{
				var editor = missingPlayers[i];
				if (editor.showExtra)
				{
					continue;
				}
				ClearGUI(editor.GetPlayer());
			}
		}
	}

	public bool ShouldShowUI(HammerEditor editor, out BaseEntity entity)
	{
		entity = null;
		var player = editor.GetPlayer();
		var distance = editor.uiDistance;
		if (player.IsFlying)
		{
			distance *= UIDistanceFlyMultiplier;
		}
		var item = player.GetActiveItem();
		var holdsHammer = editor.bypassHammer || (item != null && item.info.itemid is 200773292 /* Hammer */ or 1803831286 /* Gmod Tool Gun */);
		if (!(player.IsInCreativeMode || editor.bypassCreativeMode) || !holdsHammer || !Physics.Raycast(player.eyes.HeadRay(), out var hit, distance, ~0, QueryTriggerInteraction.Ignore))
		{
			return false;
		}
		return (entity = hit.GetEntity()).IsValid() && !editor.isMovingEntity;
	}

	public void ApplyGUI(BasePlayer player, BaseEntity entity, bool showExtra)
	{
		const float width = 150f;
		const float optionHeight = 10f;
		const float optionSpacing = 12.5f;
		const string defaultButtonColor = ".9 .2 .3 .9";
		const string optionColor = ".1 .1 .1 .3";
		const string optionTitleColor = "1 1 1 .5";
		const string noticeColor = "1 1 1 .4";

		if (!entity.IsValid())
		{
			ClearGUI(player);
			return;
		}

		using var cui = new CUI(Community.Runtime.Core.CuiHandler);
		var editor = DataInstance.GetOrCreateEditor(player.userID);
		var heightOffset = 0f;
		var coordinates = editor.GetCoordinates();
		var container = cui.CreateContainer(cuiName, Cache.CUI.BlackColor, xMin: coordinates.x, xMax: coordinates.x, yMin: coordinates.y, yMax: coordinates.y,
			OxMin: -width, OxMax: width, destroyUi: cuiName, parent: CUI.ClientPanels.Hud, needsCursor: showExtra, needsKeyboard: showExtra);

		var primaryPanel = container[0];
		primaryPanel.Components.Add(cachedDraggable);
		cachedDraggable.LimitToParent = true;
		cachedDraggable.ParentLimitIndex = 1;
		cachedDraggable.DragAlpha = .5f;
		cachedDraggable.PositionRPC = CommunityEntity.DraggablePositionSendType.NormalizedScreen;

		var entityId = entity.IsValid() ? entity.net.ID : default;

		CreateText(cui, container, container.Name, ref heightOffset, $"{(CanBeMoved(player, entity) ? "<color=green><b>✓</b></color>" : "<color=red><b>✘</b></color>")} Use <color=white>RIGHT-CLICK</color> to move the entity (hold <color=white>SPRINT</color> to skip auto-snapping)\n{(CanBeToggled(entity) ? "<color=green><b>✓</b></color>" : "<color=red><b>✘</b></color>")} Use <color=white>MIDDLE-CLICK</color> to toggle the entity (hold <color=white>SPRINT</color> to lock/unlock)");
		if (showExtra || editor.destructionMode)
		{
			CreateButton(cui, container, container.Name, ref heightOffset, entityId, "Destruction Mode", 3, editor.destructionMode ? "#8bb52a" : ".9 .2 .3 .4");
		}
		if (entity is not BasePlayer playerEntity || !playerEntity.userID.IsSteamId())
		{
			CreateButton(cui, container, container.Name, ref heightOffset, entityId, "Destroy Entity", 1);
		}

		for (int i = 0; i < CustomButons.Count; i++)
		{
			var buttons = CustomButons[i](entity, showExtra);
			if (!buttons.shouldShow)
			{
				continue;
			}
			CreateCustomButton(cui, container, container.Name, ref heightOffset, buttons.name, buttons.command, buttons.color ?? defaultButtonColor);
		}

		for (int i = 0; i < CustomFields.Count; i++)
		{
			var fields = CustomFields[i](entity, showExtra);
			if (!fields.shouldShow)
			{
				continue;
			}
			CreateOption(cui, container, container.Name, ref heightOffset, entityId, fields.name, fields.value);
		}

		if (showExtra && entity.GetLock() is CodeLock codeLock)
		{
			if (codeLock.hasGuestCode)
			{
				CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Guest Code", codeLock.guestCode);
			}
			CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Code", codeLock.code);
		}

		ModularCar car = default;
		switch (entity)
		{
			case SleepingBag sleepingBag:
			{
				CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Assigned To", BasePlayer.FindAwakeOrSleepingByID(sleepingBag.deployerUserID)?.ToString() ?? sleepingBag.deployerUserID.ToString());
				break;
			}
			case MiningQuarry miningQuarry:
			{
				CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Static Type", miningQuarry.staticType);
				break;
			}
			case IOEntity ioEntity:
			{
				CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Power", ioEntity.currentEnergy.ToString("0"));
				break;
			}
			case SteeringWheel steeringWheel:
			{
				if (showExtra)
				{
					CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Code", steeringWheel.BoatLock?.Code);
				}
				break;
			}
			case PlanterBox:
			{
				CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Temperature", $"{entity.currentTemperature:0}°C / {CelsiusToFahrenheit(entity.currentTemperature):0}°F");
				break;
			}
			case VehicleModuleEngine vehicleModuleEngine:
			{
				car = vehicleModuleEngine.Car;
				break;
			}
			case ModularCar modularCar:
			{
				car = modularCar;
				break;
			}
		}

		if (showExtra && car.IsValid() && car.CarLock.HasALock)
		{
			if (car.CarLock.whitelistPlayers.Count > 0)
			{
				var lockCreator = car.CarLock.whitelistPlayers[0];
				CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Lock Owner ID", lockCreator);
				if (BasePlayer.FindAwakeOrSleepingByID(lockCreator) is BasePlayer owner && owner.IsValid())
				{
					CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Lock Owner", owner.displayName);
				}
			}
			CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Code", car.CarLock.Code);
		}

		if (entity?.flags != 0)
		{
			CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Flags", entity?.flags);
		}
		if (entity?.skinID != 0)
		{
			CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Skin ID", entity?.skinID);
		}
		if (entity?.transform.localScale != Vector3.one)
		{
			CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Scale", entity?.transform.localScale);
		}
		CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Rotation", entity?.transform.rotation.eulerAngles);
		CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Position", entity?.transform.position);
		if (entity.IsValid() && entity.OwnerID != 0)
		{
			CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Owner ID", entity.OwnerID);
			if (BasePlayer.FindAwakeOrSleepingByID(entity.OwnerID) is BasePlayer owner && owner.IsValid())
			{
				CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Owner", owner.displayName);
			}
		}
		if (entity is BasePlayer myPlayer)
		{
			CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Display Name", myPlayer.displayName);
		}
		if (entity is BuildingBlock buildingBlock)
		{
			CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Building ID", buildingBlock.buildingID);
			if (showExtra)
			{
				CreateButton(cui, container, container.Name, ref heightOffset, entityId, $"Destroy Building ({buildingBlock.GetBuilding().decayEntities.Count:n0} entities)", 2);
			}
		}
		CreateOption(cui, container, container.Name, ref heightOffset, entityId, "NetID", entityId);
		CreateOption(cui, container, container.Name, ref heightOffset, entityId, "Target", entity?.ShortPrefabName);
		if (!showExtra)
		{
			CreateButton(cui, container, container.Name, ref heightOffset, entityId, "Show extra settings", 0, "#8cbf1d");
		}
		else
		{
			CreateButton(cui, container, container.Name, ref heightOffset, entityId, "Show fewer settings", 0);
		}

		static void CreateOption(CUI cui, CuiElementContainer container, string panel, ref float offset, NetworkableId id, string name, object value)
		{
			var option = cui.CreatePanel(container, panel, optionColor, blur: true, OyMin: -optionHeight + offset, OyMax: optionHeight + offset);
			cui.CreateText(container, option, optionTitleColor, name, 10, xMax: .25f, align: TextAnchor.MiddleRight);
			var input = cui.CreatePanel(container, option, "0 0 0 .5", xMin: .28f);
			cui.CreateProtectedInputField(container, input, Cache.CUI.WhiteColor, value?.ToString() ?? "undefined", 10, 0, true, OxMin: 7.5f,
				align: TextAnchor.MiddleLeft);
			offset += optionHeight + optionSpacing;
		}

		static void CreateButton(CUI cui, CuiElementContainer container, string panel, ref float offset, NetworkableId id, string name, int optionId, string color = ".9 .2 .3 .9")
		{
			var option = cui.CreatePanel(container, panel, optionColor, blur: true, OyMin: -optionHeight + offset, OyMax: optionHeight + offset);
			cui.CreateProtectedButton(container, option, color, Cache.CUI.WhiteColor, name.ToUpperInvariant(), 9, font: CUI.Handler.FontTypes.RobotoCondensedBold,
				command: $"ezeditor.editoption {optionId} {id}");
			offset += optionHeight + optionSpacing;
		}

		static void CreateCustomButton(CUI cui, CuiElementContainer container, string panel, ref float offset, string name, string command, string color = ".9 .2 .3 .9")
		{
			var option = cui.CreatePanel(container, panel, optionColor, blur: true, OyMin: -optionHeight + offset, OyMax: optionHeight + offset);
			cui.CreateProtectedButton(container, option, color, Cache.CUI.WhiteColor, name.ToUpperInvariant(), 8, font: CUI.Handler.FontTypes.RobotoCondensedBold, command: command);
			offset += optionHeight + optionSpacing;
		}

		static void CreateText(CUI cui, CuiElementContainer container, string panel, ref float offset, string text)
		{
			const float height = optionHeight + 2.5f;
			var option = cui.CreatePanel(container, panel, optionColor, blur: true, OyMin: -height + offset, OyMax: height + offset);
			cui.CreateText(container, option, noticeColor, text, 8, OxMin: 10f, align: TextAnchor.MiddleLeft);
			offset += height + optionSpacing;
		}

		cui.Send(container, player);
	}

	public void ClearGUI(BasePlayer player)
	{
		using var cui = new CUI(Community.Runtime.Core.CuiHandler);
		cui.Destroy(cuiName, player);
	}

	#region Hooks

	private void OnPlayerInput(BasePlayer player, InputState state)
	{
		if (lastCreativeModePlayers.TryGetValue(player.userID, out var entity) && state.WasJustPressed(BUTTON.FIRE_THIRD) && CanBeToggled(entity))
		{
			var wantsLock = state.IsDown(BUTTON.SPRINT);
			var openFlag = wantsLock ? BaseEntity.Flags.Locked : BaseEntity.Flags.Open;
			var onFlag = wantsLock ? BaseEntity.Flags.Locked : BaseEntity.Flags.On;
			if (wantsLock && entity.GetLock() is BaseLock @lock)
			{
				@lock.SetFlag(BaseEntity.Flags.Locked, !@lock.IsLocked());
				return;
			}
			switch (entity)
			{
				case VendingMachine vm:
				{
					if (vm.CanRotate())
					{
						entity.transform.rotation = Quaternion.LookRotation(-entity.transform.forward, entity.transform.up);
						entity.SendNetworkUpdate();
					}
					break;
				}
				case BuildingBlock block:
				{
					if (block.blockDefinition != null && block.blockDefinition.canRotateAfterPlacement)
					{
						block.transform.localRotation *= Quaternion.Euler(block.blockDefinition.rotationAmount);
						block.RefreshEntityLinks();
						block.UpdateSurroundingEntities();
						block.UpdateSkin(force: true);
						block.RefreshNeighbours(linkToNeighbours: false);
						block.SendNetworkUpdateImmediate();
						block.ClientRPC(RpcTarget.NetworkGroup("RefreshSkin"));
						if (!block.globalNetworkCooldown)
						{
							block.globalNetworkCooldown = true;
							GlobalNetworkHandler.server.TrySendNetworkUpdate(block);
							block.CancelInvoke(block.ResetGlobalNetworkCooldown);
							block.Invoke(block.ResetGlobalNetworkCooldown, 15f);
						}
					}
					break;
				}
				case Candle:
				case SteeringWheel:
				case Door:
				{
					entity.SetFlag(openFlag, !entity.HasFlag(openFlag));
					break;
				}
				case IOEntity:
				{
					var isOn = entity.HasFlag(onFlag);
					entity.SetFlag(onFlag, !isOn);
					entity.SetFlag(BaseEntity.Flags.Reserved8, !isOn);
					break;
				}
				case StorageContainer:
				{
					entity.SetFlag(onFlag, !entity.HasFlag(onFlag));
					break;
				}
				case EngineSwitch:
				{
					if (entity.GetParentEntity() is MiningQuarry quarry)
					{
						quarry.EngineSwitch(!quarry.IsOn());
					}
					break;
				}
				case MiningQuarry quarry:
				{
					quarry.staticType++;
					if ((int)quarry.staticType > 3)
					{
						quarry.staticType = 0;
					}
					quarry.UpdateStaticDeposit();
					break;
				}
			}
			lastCreativeModePlayers.Remove(player.userID);
		}

		if (state.WasJustPressed(BUTTON.FIRE_SECONDARY) && player.Connection.authLevel >= MinimumAuthLevel)
		{
			var editor = DataInstance.GetOrCreateEditor(player.userID);
			if (editor.isMovingEntity)
			{
				editor.isMovingEntity = false;
				lastCreativeModePlayers.Remove(player.userID);
			}
			else if(CanBeMoved(player, entity))
			{
				editor.isMovingEntity = true;
				lastCreativeModePlayers.Remove(player.userID);
				player.StartCoroutine(MoveEntityRoutine(DataInstance.GetOrCreateEditor(player.userID), entity));
			}
		}
	}

	private object OnHammerHit(BasePlayer player, HitInfo info)
	{
		if (player.Connection.authLevel < MinimumAuthLevel)
		{
			return null;
		}
		var editor = DataInstance.GetOrCreateEditor(player.userID);
		if ((!player.IsInCreativeMode && !editor.bypassCreativeMode) || info == null)
		{
			return null;
		}
		if (editor.destructionMode && info.HitEntity is BaseEntity entity && (CanBeMoved(player, entity) || CanBeToggled(entity)))
		{
			entity.Kill(BaseNetworkable.DestroyMode.Gib);
			return Cache.False;
		}
		if (info.HitEntity?.GetParentEntity() is PlayerBoat boat)
		{
			boat.Heal(float.MaxValue);
			return Cache.False;
		}
		if (info.HitEntity is BuildingBlock block && block.GetBuilding() is BuildingManager.Building building && !editor.isRepairingOrDestroyingBuilding)
		{
			var entityPool = Pool.Get<PooledList<BaseCombatEntity>>();
			entityPool.AddRange(building.decayEntities);
			player.StartCoroutine(RepairEntitiesOverTime(editor, entityPool));
			return Cache.False;
		}
		return null;
	}

	private void OnActiveItemChanged(BasePlayer player, Item oldItem)
	{
		if (!player.IsValid() || !player.IsConnected || player.Connection.authLevel < MinimumAuthLevel)
		{
			return;
		}
		var editor = DataInstance.GetOrCreateEditor(player.userID);
		if (editor.bypassHammer || (oldItem != null && oldItem.info.itemid is 200773292 /* Hammer */ or 1803831286 /* Gmod Tool Gun */))
		{
			editor.Reset();
			ClearGUI(player);
		}
	}

	private void OnPlayerSleep(BasePlayer player)
	{
		if (!player.IsValid() || !player.IsConnected || player.Connection.authLevel < MinimumAuthLevel)
		{
			return;
		}
		var editor = DataInstance.GetOrCreateEditor(player.userID);
		editor.Reset();
		ClearGUI(player);
	}

	private void OnPlayerDeath(BasePlayer player)
	{
		if (!player.IsConnected || player.Connection.authLevel < MinimumAuthLevel)
		{
			return;
		}
		var editor = DataInstance.GetOrCreateEditor(player.userID);
		editor.Reset();
		ClearGUI(player);
	}

	private void OnPlayerDisconnected(BasePlayer player)
	{
		if (!player.IsValid() || player.Connection.authLevel < MinimumAuthLevel)
		{
			return;
		}
		var editor = DataInstance.GetOrCreateEditor(player.userID);
		editor.Reset();
	}

	private void OnCuiDraggableDrag(BasePlayer player, string name, Vector3 position, CommunityEntity.DraggablePositionSendType type)
	{
		if (!name.Equals(cuiName) || type != CommunityEntity.DraggablePositionSendType.NormalizedParent)
		{
			return;
		}
		DataInstance.GetOrCreateEditor(player.userID).SetCoordinates(position);
	}

	#endregion

	public static float CelsiusToFahrenheit(float celsius)
	{
		return (celsius * 9f / 5f) + 32f;
	}

	[ProtectedCommand("ezeditor.editoption")]
	private void EditOption(ConsoleSystem.Arg arg)
	{
		var player = arg.Player();
		var editor = DataInstance.GetOrCreateEditor(player.userID);
		var option = arg.GetInt(0);
		var entity = BaseNetworkable.serverEntities.Find(arg.GetEntityID(1)) as BaseEntity;

		if (option != 0 && !entity.IsValid())
		{
			player.ChatMessage("Entity is now invalid");
			return;
		}

		switch (option)
		{
			case 0:
			{
				editor.showExtra = !editor.showExtra;

				if (ShouldShowUI(editor, out _))
				{
					ApplyGUI(player, entity, true);
				}
				else
				{
					ClearGUI(player);
				}
				break;
			}
			case 1:
			{
				if (CanBeMoved(player, entity) || entity is BuildingBlock)
				{
					entity.Kill(BaseNetworkable.DestroyMode.Gib);
					editor.showExtra = false;
					ClearGUI(player);
				}
				else
				{
					if (editor.bypassImmovableEntityDestroyConfirmations)
					{
						entity.Kill(BaseNetworkable.DestroyMode.Gib);
						editor.showExtra = false;
						ClearGUI(player);
					}
					else
					{
						Modal.Open(player, "Are you sure you wanna destroy that entity?", temp, (player, modal) =>
						{
							entity.Kill(BaseNetworkable.DestroyMode.Gib);
							editor.showExtra = false;
							ClearGUI(player);
						});
					}
				}

				break;
			}
			case 2:
			{
				if (entity is not BuildingBlock block || editor.isRepairingOrDestroyingBuilding)
				{
					return;
				}
				Modal.Open(player, "Are you sure you wanna destroy that building?", temp, (player, modal) =>
				{
					var entityPool = Pool.Get<PooledList<BaseEntity>>();
					entityPool.AddRange(block.GetBuilding().decayEntities);
					player.StartCoroutine(DestroyEntitiesOverTime(editor, entityPool));
					editor.showExtra = false;
					ClearGUI(player);
				});
				break;
			}
			case 3:
			{
				editor.destructionMode = !editor.destructionMode;
				editor.showExtra = false;
				ClearGUI(player);
				break;
			}
		}
	}

	private IEnumerator DestroyEntitiesOverTime(HammerEditor editor, List<BaseEntity> entities)
	{
		var player = editor.GetPlayer();
		editor.isRepairingOrDestroyingBuilding = true;
		var currentBatch = 0;
		var completedEntities = 0;
		var completedEntitiesDead = 0;
		var wasCancelled = false;
		player.ShowToast(GameTip.Styles.Red_Normal, destroyingBuildingPhrase, false, "1", entities.Count.ToString("n0"), completedEntitiesDead.ToString("n0"));
		for (int i = 0; i < entities.Count; i++)
		{
			if (!editor.isRepairingOrDestroyingBuilding)
			{
				wasCancelled = true;
				break;
			}
			if (currentBatch > BuildingDestroyBatch)
			{
				yield return null;
				yield return null;
				yield return CoroutineEx.waitForSeconds(BuildingBatchRefreshRate);
				player.ShowToast(GameTip.Styles.Red_Normal, destroyingBuildingPhrase, false, (i + 1).ToString("n0"), entities.Count.ToString("n0"), completedEntitiesDead.ToString("n0"));
				currentBatch = 0;
			}

			var entity = entities[i];
			if (entity.IsValid())
			{
				entity.Kill();
				currentBatch++;
				completedEntities++;
			}
			else
			{
				completedEntitiesDead++;
			}
		}
		if (wasCancelled)
		{
			player.ShowToast(GameTip.Styles.Red_Normal, destroyingBuildingCancelledPhrase, false);
		}
		else
		{
			player.ShowToast(GameTip.Styles.Red_Normal, destroyingBuildingPhrase, false, completedEntities.ToString("n0"), entities.Count.ToString("n0"), completedEntitiesDead.ToString("n0"));
		}
		Pool.FreeUnmanaged(ref entities);
		editor.isRepairingOrDestroyingBuilding = false;
	}

	private IEnumerator RepairEntitiesOverTime(HammerEditor editor, List<BaseCombatEntity> entities)
	{
		var player = editor.GetPlayer();
		editor.isRepairingOrDestroyingBuilding = true;
		var currentBatch = 0;
		var completedEntities = 0;
		var completedEntitiesDead = 0;
		var completedEntitiesNeededRepair = 0;
		var wasCancelled = false;
		player.ShowToast(GameTip.Styles.Blue_Normal, repairedPhrase, false, "1", entities.Count.ToString("n0"), completedEntitiesNeededRepair.ToString("n0"), completedEntitiesDead.ToString("n0"));
		for (int i = 0; i < entities.Count; i++)
		{
			if (!editor.isRepairingOrDestroyingBuilding)
			{
				wasCancelled = true;
				break;
			}
			if (currentBatch > BuildingRepairBatch)
			{
				currentBatch = 0;
				player.ShowToast(GameTip.Styles.Blue_Normal, repairedPhrase, false, (i + 1).ToString("n0"), entities.Count.ToString("n0"), completedEntitiesNeededRepair.ToString("n0"), completedEntitiesDead.ToString("n0"));
				yield return CoroutineEx.waitForSeconds(BuildingBatchRefreshRate);
			}

			var entity = entities[i];
			if (entity.IsValid())
			{
				if (!Mathf.Approximately(entity.healthFraction, 1))
				{
					completedEntitiesNeededRepair++;
					entity.Heal(float.MaxValue);
				}
				currentBatch++;
				completedEntities++;
				yield return null;
			}
			else
			{
				completedEntitiesDead++;
			}
		}
		if (wasCancelled)
		{
			player.ShowToast(GameTip.Styles.Red_Normal, repairedCancelledPhrase, false);
		}
		else
		{
			player.ShowToast(GameTip.Styles.Blue_Normal, repairedPhrase, false, entities.Count.ToString("n0"), entities.Count.ToString("n0"), completedEntitiesNeededRepair.ToString("n0"), completedEntitiesDead.ToString("n0"));
		}
		Pool.FreeUnmanaged(ref entities);
		editor.isRepairingOrDestroyingBuilding = false;
	}

	private IEnumerator MoveEntityRoutine(HammerEditor editor, BaseEntity entity)
	{
		int layer = Rust.Layers.Solid;
		if (editor.waterLayer)
		{
			layer += Rust.Layers.Water;
		}
		var player = editor.GetPlayer();
		var rotation = Vector3.up * 180f;
		var hits = Pool.Get<List<RaycastHit>>(); 
		var hasContact = true;
		var rigidbody = entity.GetComponent<Rigidbody>() ?? entity.GetComponentInChildren<Rigidbody>() ?? entity.GetComponentInParent<Rigidbody>();
		var wasKinematic = rigidbody?.isKinematic;
		rigidbody?.isKinematic = true;
		if (entity is BaseHelicopter)
		{
			entity.SetFlag(BaseEntity.Flags.Protected, true);
		}
		if (entity is RidableHorse)
		{
			entity.SetFlag(BaseEntity.Flags.Reserved12, true);
		}
		ClearGUI(player);
		RaycastHit hit = default;
		entity?.SetParent(null, true);
		var transform = entity?.transform;
		while (player.IsValid() && entity.IsValid() && editor.isMovingEntity && !player.IsSleeping())
		{
			hits.Clear();
			hit = default;
			GamePhysics.TraceAll(player.eyes.HeadRay(), 0f, hits, editor.moveDistance, layer, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < hits.Count; i++)
			{
				var currentHit = hits[i];
				var currentEntity = currentHit.GetEntity();
				if (currentEntity == entity || currentEntity.HasEntityInParents(entity))
				{
					continue;
				}
				hit = currentHit;
				break;
			}

			hasContact = hit.point != Vector3.zero;

			if (!hasContact)
			{
				hit.point = player.eyes.position + (player.eyes.HeadForward() * editor.moveDistance);
			}

			if (player.serverInput.WasJustPressed(BUTTON.RELOAD))
			{
				rotation += Vector3.up * 90f;
				player.serverInput.SwallowButton(BUTTON.RELOAD);
			}

			var delta = UnityEngine.Time.deltaTime * MoveLerp;
			transform.position = Vector3.Lerp(transform.position, hit.point, delta);
			transform.localRotation = Quaternion.Slerp(transform.localRotation, (Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(rotation)) * Quaternion.Euler(player.eyes.GetLookRotation().eulerAngles.WithX(0)), delta);
			entity.SendNetworkUpdate_Position();
			yield return null;
		}

		if (!hasContact && entity.IsValid() && !player.serverInput.IsDown(BUTTON.SPRINT))
		{
			const float transitionTime = .75f;
			GamePhysics.Trace(new Ray(transform.position, Vector3.down), 0, out var hit2, float.MaxValue, layer, QueryTriggerInteraction.Ignore);
			var targetPosition = hit2.point;
			var targetRotation = (Quaternion.FromToRotation(Vector3.up, hit2.normal) * Quaternion.Euler(rotation)) *
								 Quaternion.Euler(player.eyes.GetLookRotation().eulerAngles.WithX(0));

			if (targetPosition != Vector3.zero)
			{
				var currentTime = 0f;
				while (entity.IsValid() && currentTime <= transitionTime)
				{
					currentTime += UnityEngine.Time.deltaTime;
					var delta = currentTime.Scale(0f, transitionTime, 0f, 1f);
					transform.position = Vector3.Lerp(transform.position, targetPosition, delta);
					transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, delta);
					entity.SendNetworkUpdate_Position();
					yield return null;
				}
				if (hit2.GetEntity() is BaseEntity parentEntity && parentEntity != entity && parentEntity is not BasePlayer && entity is not BasePlayer)
				{
					entity?.SetParent(parentEntity, true);
				}
			}
		}
		else if (entity.IsValid() && hit.GetEntity() is BaseEntity subParentEntity && subParentEntity != entity && entity is not BasePlayer && subParentEntity is not BasePlayer)
		{
			entity.SetParent(subParentEntity, true);
		}

		ClearGUI(player);
		editor.isMovingEntity = false;
		if (rigidbody != null)
		{
			rigidbody.isKinematic = wasKinematic ?? false;
			rigidbody.transform.hasChanged = true;
			rigidbody.WakeUp();
		}
		if (entity is BaseBoat boat)
		{
			boat.OnServerWake();
		}
		Pool.FreeUnmanaged(ref hits);

		if (entity.IsValid() && entity is BaseHelicopter)
		{
			entity.SetFlag(BaseEntity.Flags.Protected, false);
		}
		if(entity.IsValid() && entity is RidableHorse)
		{
			entity.SetFlag(BaseEntity.Flags.Reserved12, false);
		}
		if (entity.IsValid() && entity is not BaseCorpse && !entity.HasEntityInParents(player) && !player.HasEntityInParents(entity))
		{
			ReconstructEntity(entity);
		}
	}

	private void ReconstructEntity(BaseEntity entity)
	{
		if (!entity.IsValid())
		{
			return;
		}

		entity.networkEntityScale = entity.transform.localScale != Vector3.one;

		for (int i = 0; i < entity.net.group.subscribers.Count; i++)
		{
			entity.DestroyOnClient(entity.net.group.subscribers[i]);
		}
		if (entity.children != null)
		{
			for (int i = 0; i < entity.children.Count; i++)
			{
				ReconstructEntity(entity.children[i]);
			}
		}
		entity.SendNetworkUpdateImmediate();
	}

	[ConsoleCommand("hammer", "Player-specific configuration editing for the Hammer UI and its behaviour")]
	public void Hammer(ConsoleSystem.Arg arg)
	{
		var player = arg.Player();
		if (player == null)
		{
			arg.ReplyWith("Command must be called from a client");
			return;
		}
		if (player.Connection.authLevel < MinimumAuthLevel)
		{
			arg.ReplyWith("Low auth level");
			return;
		}
		var editor = DataInstance.GetOrCreateEditor(player.userID);
		var setting = arg.GetString(0);
		var hasChanges = true;
		var value = (object)null;
		switch (setting)
		{
			case "uidistance":
			{
				value = editor.uiDistance = arg.GetFloat(1, editor.uiDistance);
				break;
			}
			case "movedistance":
			{
				value = editor.moveDistance = arg.GetFloat(1, editor.moveDistance);
				break;
			}
			case "waterlayer":
			{
				value = editor.waterLayer = arg.GetBool(1, editor.waterLayer);
				break;
			}
			case "creativebypass":
			{
				value = editor.bypassCreativeMode = arg.GetBool(1, editor.bypassCreativeMode);
				break;
			}
			case "hammerbypass":
			{
				value = editor.bypassHammer = arg.GetBool(1, editor.bypassHammer);
				ValidatePermanentPlayerInputHook();
				break;
			}
			case "bypassimmovableentitydestroyconfirmations":
			{
				value = editor.bypassImmovableEntityDestroyConfirmations = arg.GetBool(1, editor.bypassImmovableEntityDestroyConfirmations);
				break;
			}
			case "resetui":
			{
				editor.x = ConfigInstance.UIDefaultX;
				editor.y = ConfigInstance.UIDefaultY;
				editor.Reset();

				value = "Hammer UI has been reset";
				break;
			}
			default:
			{
				using var table = new StringTable("option", "value", "help");
				{
					table.AddRow("resetui", null, "Resets the position of the UI, in case it's stuck somehow, even though it shouldn't");
					table.AddRow("uidistance", editor.uiDistance, "Minimum distance from the player to the entity to show the Hammer UI");
					table.AddRow("movedistance", editor.moveDistance, "Distance the entity will float in front of the player if not connecting to a surface");
					table.AddRow("waterlayer", editor.waterLayer, "Should the water layer of the ocean be considered?");
					table.AddRow("creativebypass", editor.bypassCreativeMode, "Allow players to use the Hammer UI regardless if they're in creative mode or not");
					table.AddRow("hammerbypass", editor.bypassHammer, "Allow players to use the Hammer UI regardless if they're holding a hammer item");
					table.AddRow("bypassimmovableentitydestroyconfirmations", editor.waterLayer, "Should the confirmation popup happen when attempting to destroy an entity that can't be moved?");
				}
				arg.ReplyWith($"Invalid syntax!\n{table.Write(StringTable.FormatTypes.None)}");
				hasChanges = false;
				break;
			}
		}

		if (hasChanges)
		{
			arg.ReplyWith($"Hammer config - {setting}: {value}");
			Save();
		}
	}

	[CommandVar("hammer.uidistanceflymultiplier", "The multiplication value of the distance needed for an entity to be picked up by the Hammer UI when flying"), AuthLevel(1)]
	public float UIDistanceFlyMultiplier
	{
		get => ConfigInstance.UIDistanceFlyMultiplier;
		set
		{
			ConfigInstance.UIDistanceFlyMultiplier = value.Clamp(1, 10);
			Save();
		}
	}

	[CommandVar("hammer.uidistance", "The minimum distance from an entity you're looking at to be picked up by the Hammer UI"), AuthLevel(1)]
	public float UIDefaultDistance
	{
		get => ConfigInstance.UIDefaultDistance;
		set
		{
			ConfigInstance.UIDefaultDistance = value.Clamp(.5f, 50f);
			Save();
		}
	}

	[CommandVar("hammer.uirefreshrate", "The responsiveness of how fast the Hammer UI updates (lower is more accurate, but could be affecting performance)"), AuthLevel(1)]
	public float UIRefreshRate
	{
		get => ConfigInstance.UIRefreshRate;
		set
		{
			ConfigInstance.UIRefreshRate = value.Clamp(0f, 2.5f);
			timer?.Destroy();
			timer = Community.Runtime.Core.timer.Every(ConfigInstance.UIRefreshRate, TickCheck);
			Save();
		}
	}

	[CommandVar("hammer.movedistance", "The maximum distance away of the moved entity from the player's face"), AuthLevel(1)]
	public float DefaultMoveDistance
	{
		get => ConfigInstance.DefaultMoveDistance;
		set
		{
			ConfigInstance.DefaultMoveDistance = value.Clamp(.5f, 50f);
			Save();
		}
	}

	[CommandVar("hammer.movelerp", "Smoothing value of the moved entity (lesser is smoother)"), AuthLevel(1)]
	public float MoveLerp
	{
		get => ConfigInstance.MoveLerp;
		set
		{
			ConfigInstance.MoveLerp = value.Clamp(1, 20f);
			Save();
		}
	}

	[CommandVar("hammer.moveeverything", "Bypass all logical checks for important entities when moving entities (use cautiously!)"), AuthLevel(2)]
	public bool MoveEverything
	{
		get => ConfigInstance.MoveEverything;
		set
		{
			ConfigInstance.MoveEverything = value;
			Save();
		}
	}

	[CommandVar("hammer.uix", "Default UI X-axis position"), AuthLevel(1)]
	public float DefaultX
	{
		get => ConfigInstance.UIDefaultX;
		set
		{
			ConfigInstance.UIDefaultX = value.Clamp(0f, 1f);
			Save();
		}
	}

	[CommandVar("hammer.uiy", "Default UI Y-axis position"), AuthLevel(1)]
	public float DefaultY
	{
		get => ConfigInstance.UIDefaultY;
		set
		{
			ConfigInstance.UIDefaultY = value.Clamp(0f, 1f);
			Save();
		}
	}

	[CommandVar("hammer.brepairbatch", "Building entity repair count per batch"), AuthLevel(1)]
	public int BuildingRepairBatch
	{
		get => ConfigInstance.BuildingRepairBatchCount;
		set
		{
			ConfigInstance.BuildingRepairBatchCount = value.Clamp(1, 100);
			Save();
		}
	}

	[CommandVar("hammer.bdestroybatch", "Building entity destruction count per batch"), AuthLevel(1)]
	public int BuildingDestroyBatch
	{
		get => ConfigInstance.BuildingDestroyBatchCount;
		set
		{
			ConfigInstance.BuildingDestroyBatchCount = value.Clamp(1, 100);
			Save();
		}
	}

	[CommandVar("hammer.bbatchrefreshrate", "Speed of how fast batch iterations happen for building repairing and destroying"), AuthLevel(1)]
	public float BuildingBatchRefreshRate
	{
		get => ConfigInstance.BuildingBatchRefreshRate;
		set
		{
			ConfigInstance.BuildingBatchRefreshRate = value.Clamp(0, 2);
			Save();
		}
	}

	[CommandVar("hammer.minauthlevel", "Minimum auth level for certain Hammer UI checks"), AuthLevel(2)]
	public int MinimumAuthLevel
	{
		get => ConfigInstance.MinimumAuthLevel;
		set
		{
			ConfigInstance.MinimumAuthLevel = value.Clamp(0, 4);
			Save();
		}
	}

	public class HammerEditor
	{
		[JsonIgnore] public ulong playerId;
		public float x = ins.DefaultX;
		public float y = ins.DefaultY;
		public float uiDistance = ins.UIDefaultDistance;
		public float moveDistance = ins.DefaultMoveDistance;
		public bool bypassCreativeMode;
		public bool waterLayer = true;
		public bool bypassImmovableEntityDestroyConfirmations = false;
		public bool bypassHammer = false;
		[JsonIgnore] public bool showExtra;
		[JsonIgnore] public bool destructionMode;
		[JsonIgnore] public bool isMovingEntity;
		[JsonIgnore] public bool isRepairingOrDestroyingBuilding;

		private BasePlayer player;

		public BasePlayer GetPlayer()
		{
			if (!player.IsValid())
			{
				player = BasePlayer.FindAwakeOrSleepingByID(playerId);
			}
			return player;
		}

		public void Reset()
		{
			showExtra = false;
			isMovingEntity = false;
			isRepairingOrDestroyingBuilding = false;
			destructionMode = false;
			lastCreativeModePlayers.Remove(playerId);
		}

		public Vector2 GetCoordinates() => new(x, y);

		public void SetCoordinates(Vector2 value)
		{
			x = value.x;
			y = value.y;
			ins.Save();
		}
	}

	public class HammerConfig
	{
		public int MinimumAuthLevel = 1;
		public float UIRefreshRate = .1f;
		public float UIDefaultDistance = 10f;
		public float UIDistanceFlyMultiplier = 2.5f;
		public float UIDefaultX = .75f;
		public float UIDefaultY = .25f;
		public float DefaultMoveDistance = 5f;
		public float MoveLerp = 10f;
		public bool MoveEverything = false;
		public float BuildingBatchRefreshRate = .25f;
		public int BuildingRepairBatchCount = 50;
		public int BuildingDestroyBatchCount = 15;
	}

	public class HammerData
	{
		public Dictionary<ulong, HammerEditor> Hammers = new();

		public HammerEditor GetOrCreateEditor(ulong playerId)
		{
			if (!Hammers.TryGetValue(playerId, out var editor))
			{
				Hammers[playerId] = editor = new();
			}
			if (editor.playerId == 0)
			{
				editor.playerId = playerId;
			}
			return editor;
		}
	}
}
