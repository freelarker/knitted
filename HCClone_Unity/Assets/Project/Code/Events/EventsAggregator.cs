public class EventsAggregator {
	private static EventManager<ENetworkEvent> _network = new EventManager<ENetworkEvent>();
	public static EventManager<ENetworkEvent> Network { get { return _network; } }

	private static EventManager<EFightEvent> _fight = new EventManager<EFightEvent>();
	public static EventManager<EFightEvent> Fight { get { return _fight; } }

	private static EventManager<EItemEvent> _items = new EventManager<EItemEvent>();
	public static EventManager<EItemEvent> Items { get { return _items; } }

	private static EventManager<EUnitEvent> _units = new EventManager<EUnitEvent>();
	public static EventManager<EUnitEvent> Units { get { return _units; } }
}