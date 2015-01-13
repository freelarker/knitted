public class EventsAggregator {
	private static EventManager<EItemEvent> _items = new EventManager<EItemEvent>();
	public static EventManager<EItemEvent> Items { get { return _items; } }

	private static EventManager<ENetworkEvent> _network = new EventManager<ENetworkEvent>();
	public static EventManager<ENetworkEvent> Network { get { return _network; } }
}