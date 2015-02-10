public struct HCCGridPoint {
	public int X;
	public int Z;
	//public int X { get; set; }
	//public int Z { get; set; }

	public HCCGridPoint(int x, int z) {
		X = x;
		Z = z;
	}

	#region static
	public static HCCGridPoint Zero {
		get { return new HCCGridPoint(0, 0); }
	}
	#endregion
}
