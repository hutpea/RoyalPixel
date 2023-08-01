using System;

[Serializable]
public class ShortVector2
{
	public short X
	{
		get;
		private set;
	}

	public short Y
	{
		get;
		private set;
	}

	public ShortVector2(short x, short y)
	{
		X = x;
		Y = y;
	}

	public ShortVector2(int x, int y)
	{
		X = (short)x;
		Y = (short)y;
	}

	public override string ToString()
	{
		return $"{X}:{Y}";
	}
}
