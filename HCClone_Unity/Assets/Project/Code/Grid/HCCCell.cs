using UnityEngine;

public class HCCCell {
	public HCCGridPoint GridPosition { get; private set; }

	public HCCGridObject ObjectData { get; set; }

	public HCCGridObject ObjectPathReservation { get; set; }

	public bool IsFree {
		get { return ObjectData == null; }
	}

	public bool IsBusyByStaticObject {
		get { return ObjectData != null && ObjectData.IsStatic; }
	}

	public HCCCell(int x, int z) {
		GridPosition = new HCCGridPoint(x, z);
	}
}
