﻿using System;

public class NetworkManager {
	//TODO: create loading queue

	public void LoadPlayerData() {
		//TODO: load player resources, progress, heroes
		EventsAggregator.Network.Broadcast(ENetworkEvent.PlayerDataLoadSuccess);
	}

	public void SavePlayerData(PlayerData data) {
		//TODO: save player resources, progress, heroes
		EventsAggregator.Network.Broadcast(ENetworkEvent.PlayerDataSaveSuccess);
	}
}