using System;
using System.Collections.Generic;

public static class SystemExtends
{
	private static Random rng = new Random();

	public static void SafeInvoke(this Action action)
	{
		action?.Invoke();
	}

	public static void SafeInvoke<T>(this Action<T> action, T par)
	{
		action?.Invoke(par);
	}

	public static void SafeInvoke<T1, T2>(this Action<T1, T2> action, T1 par1, T2 par2)
	{
		action?.Invoke(par1, par2);
	}

	public static void SafeInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 par1, T2 par2, T3 par3)
	{
		action?.Invoke(par1, par2, par3);
	}

	public static void SafeInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 par1, T2 par2, T3 par3, T4 par4)
	{
		action?.Invoke(par1, par2, par3, par4);
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = rng.Next(num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}
}
