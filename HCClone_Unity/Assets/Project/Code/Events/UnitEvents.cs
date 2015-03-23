public enum EUnitEvent {
	Idle = 0,
	HitTarget,
	HitReceived,

	HealTarget,
	HealReceived,

	DeathTarget,
	DeathCame,

	ReviveTarget,
	ReviveCame,

	EquipmentUpdate,
	RecalculateParams,
	AggroCrystalsUpdate,

	SkillUsage,
}