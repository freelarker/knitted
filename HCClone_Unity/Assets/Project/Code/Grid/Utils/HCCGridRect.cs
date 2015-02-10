public struct HCCGridRect {
	public int XMin;
	public int XMax;
	public int ZMin;
	public int ZMax;
	//public int XMin { get; set; }
	//public int XMax { get; set; }
	//public int ZMin { get; set; }
	//public int ZMax { get; set; }

	public HCCGridPoint Min {
		get { return new HCCGridPoint(XMin, ZMin); }
	}

	public HCCGridPoint Max {
		get { return new HCCGridPoint(XMax, ZMax); }
	}

	public int XSize {
		get { return XMax - XMin; }
	}
	public int ZSize {
		get { return ZMax - ZMin; }
	}

	public HCCGridRect(int xMin, int xMax, int zMin, int zMax) {
		XMin = xMin;
		XMax = xMax;
		ZMin = zMin;
		ZMax = zMax;
	}

	public bool Equals(HCCGridRect gridRect) {
		return XMin == gridRect.XMin && ZMin == gridRect.ZMin && XMax == gridRect.XMax && ZMax == gridRect.ZMax;
	}

	#region static
	public HCCGridRect Zero {
		get { return new HCCGridRect(0, 0, 0, 0); }
	}
	#endregion
}
