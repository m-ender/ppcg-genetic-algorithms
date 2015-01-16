using System;
using System.Linq;

namespace ppcggacscontroller
{
	class Program
	{
		public struct Coord
		{
			public int x, y;
			
			public Coord(int xN, int yN)
			{
				x = xN;
				y = yN;
			}
		}
		
		public static void Main(string[] args)
		{
			GameLogic.Game g = new GameLogic.Game(fgrr);
			g.runSession();
			
			Console.ReadKey(true);
		}
		
		private static int cutOutInt(GameLogic.IGenome g, int idx, int len)
		{
			// do this the slow way
			int n = 0;
			for (int i = len - 1; i >= 0; i--)
			{
				n = n * 2 + (g[i + idx] ? 1 : 0);
			}
			
			return n;
		}
		
		// fgrr likes to go forward
		public static Coord[] fgrrcoords = new [] {
			new Coord(1, 0),
			new Coord(1, -1),
			new Coord(1, 1),
			new Coord(0, -1),
			new Coord(0, 1),
//			new Coord(-1, -1),
//			new Coord(-1, 1),
//			new Coord(0, 0),
//			new Coord(-1, 0),
		};
		
		public static void fgrr(GameLogic.IView v, GameLogic.IGenome g, out int ox, out int oy)
		{
			// defaults
			ox = 1; // into the unknown
			oy = 0;
			
			int k = 2;
			
			for (int j = 0; j < 3; j++)
			{
				foreach (Coord c in fgrrcoords.Where(c => c.x > 0))
				{
					for (int i = j * k; i < j * k + k; i++)
					{
						int ci = cutOutInt(g, i * 4, 4);
					
						if (v[c.x, c.y] == ci)
						{
							ox = c.x;
							oy = c.y;
							return;
						}
					}
				}
			}
			
			for (int j = 0; j < 3; j++)
			{
				foreach (Coord c in fgrrcoords.Where(c => c.x == 0))
				{
					for (int i = j * k; i < j * k + k; i++)
					{
						int ci = cutOutInt(g, i * 4, 4);
					
						if (v[c.x, c.y] == ci)
						{
							ox = c.x;
							oy = c.y;
							return;
						}
					}
				}
			}
		}
		
		// Linear Combination Player is a port from the Python one here https://github.com/mbuettner/ppcg-genetic-algorithms/tree/master/python/player.py
		public static Coord[] lcpcoords = new [] {
//			new Coord(-1, -1),
//			new Coord(0, -1),
			new Coord(1, 0),
			new Coord(1, -1),
//			new Coord(-1, 0),
//			new Coord(0, 0),
//			new Coord(-1, 1),
//			new Coord(0, -1),
			new Coord(1, 1),
		};
		
		// ported from python, no idea what it does
		public static void LinearCombinationPlayer(GameLogic.IView v, GameLogic.IGenome g, out int ox, out int oy)
		{
			ox = 0;
			oy = 0;
			
			var restrictedCoords = lcpcoords.Where(c => v[c.x, c.y] > -1).ToArray();
			var rcCount = restrictedCoords.Length;
			
			int s = 0;
			for (int i = 0; i < 25; i++)
				s += cutOutInt(g, 2 * i, 2) * v[i / 5 - 2, i % 5 - 2];
			g.getBitArray();
			
			Coord res = restrictedCoords[(s + rcCount * 1000/*bignumber*/) % rcCount];
			
			ox = res.x;
			oy = res.y;	
		}
	}
}