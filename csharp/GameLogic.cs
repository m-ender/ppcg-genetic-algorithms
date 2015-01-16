/**
 * Loosley based off this: https://github.com/mbuettner/ppcg-genetic-algorithms/tree/master/c%2B%2B
 * 
 * If it breaks, shout at VisualMelon
 */

using System;
using BitArr = System.Collections.BitArray;
using System.Collections.Generic;
using System.Linq;

// sorry about the naming, never written code for someone else to use before
// TODO: find OBOEs

namespace ppcggacscontroller
{
	public class GameLogic
	{
		public class GameConstants
		{	
			// board
			public int boardWidth = 50;
			public int boardHeight = 15;
		
			public int safeColCount = 10;
			public int trapColCount = 1;
			public int teleColCount = 1;
			public int wallColCount = 1;
			
			// game
			public int turnCount = 10000;
			public int repeatCount = 20;
			
			public int initialSpecimenCount = 15;
			public int turnsPerBreeding = 1;
			
			// genome
			public int genomeSize = 50;
			
			public double genomeSwapRate = 0.01;
			public double genomeMutateRate = 0.01;
			
			// specimens
			public int maxAge = 100;
			public int reproductionRate = 10;
			
			// views
			public int viewDimX = 2;
			public int viewDimY = 2;
		}
		
		private enum SpecimenState
		{
			Alive,
			Dead,
			Win,
		}
		
		private enum ColorType
		{
			Safe,
			Tele,
			Trap,
			Wall,
		}
		
		// sue me
		private class Color
		{
			public static Color OutOfBoundsDead = new Color(-1, ColorType.Trap);
			public static Color OutOfBoundsAlive = new Color(-1, ColorType.Safe);
			
			public int n {get; private set;}
			public ColorType type {get; private set;}
			public int tox, toy; // target offsets
			
			public Color(int nN, ColorType typeN)
			{
				n = nN;
				type = typeN;
			}
			
			public Color(int nN, ColorType typeN, int toxN, int toyN) : this(nN, typeN)
			{
				tox = toxN;
				toy = toyN;
			}
		}
		
		public interface IView
		{
			int this[int ox, int oy] {get;}
		}
		
		// this one actually needs to be public
		private class View : IView
		{
			private int[,] colors;
			private int xd;
			private int yd;
			
			public View(GameConstants consts)
			{
				xd = consts.viewDimX;
				yd = consts.viewDimY;
				
				colors = new int[xd * 2 + 1, yd * 2 + 1];
			}
			
			// clone
			private View(View org)
			{
				xd = org.xd;
				yd = org.yd;
				colors = (int[,])org.colors.Clone();
			}
			
			public void see(Board.Position pos)
			{
				for (int i = -xd; i <= xd; i++)
				{
					for (int j = -yd; j <= yd; j++)
					{
						colors[i + xd, j + yd] = pos.brd.getColor(i + pos.x, j + pos.y).n;
					}
				}
			}
			
			public View clone()
			{
				return new View(this);
			}
			
			public int this[int ox, int oy]
			{
				get
				{
					return colors[ox + xd, oy + yd];
				}
			}
		}
		
		public interface IGenome
		{
			bool this[int idx] {get;}
			BitArr getBitArray();
		}
		
		private class Genome : IGenome
		{
			private BitArr barr;
			
			private Genome(GameConstants consts)
			{
				barr = new BitArr(consts.genomeSize);
			}
			
			public Genome(GameConstants consts, Random rnd, Genome a, Genome b) : this(consts)
			{
				Genome cur = a;
				Genome oth = b;
				
				Action swap = () =>
				{
					var t = cur;
					cur = oth;
					oth = t;
				};
				
				if (rnd.NextDouble() < 0.5)
					swap();
				
				for (int i = 0; i < barr.Length; i++)
				{
					barr[i] = cur[i];
					
					if (rnd.NextDouble() < consts.genomeMutateRate)
						barr[i] = !barr[i];
					
					if (rnd.NextDouble() < consts.genomeSwapRate)
						swap();
				}
			}
			
			public Genome(GameConstants consts, Random rnd) : this(consts)
			{
				for (int i = 0; i < barr.Length; i++)
					barr[i] = rnd.NextDouble() < 0.5;
			}

			public BitArr getBitArray()
			{
				return (BitArr)barr.Clone();
			}
			
			public bool this[int idx]
			{
				get
				{
					return barr[idx];
				}
			}
		}
		
		private class Specimen
		{
			public Board.Position pos;
			public int age;
			public int score;
			
			public Genome g {get; private set;}
			
			public long fitness
			{
				get
				{
					return pos.x + 1 + pos.brd.width * score;
				}
			}
			
			public Specimen(Genome gN)
			{
				g = gN;
			}
			
			public Genome cross(GameConstants consts, Random rnd, Specimen other)
			{
				return new Genome(consts, rnd, g, other.g);
			}
		}
		
		public delegate void PlayerDel(IView v, IGenome g, out int ox, out int oy);
		
		private class Player
		{
			public int score;
			public PlayerDel pd;
			
			public Player(PlayerDel pdN)
			{
				pd = pdN;
				score = 0;
			}
		}
		
		private class Board
		{
			private class Cell
			{
				public Position truePos {get; private set;}
				public Position movePos {get; private set;}
				public bool lethal {get; private set;}
				public Color trueColor {get; private set;}
				
				public Cell(Position truePosN, Color trueColorN)
				{
					truePos = truePosN;
					movePos = truePos; // default
					trueColor = trueColorN;
					
					lethal = false;
				}
				
				public void apply(Color c)
				{
					switch (c.type)
					{
						case ColorType.Safe:
							// meh
							break;
						case ColorType.Trap:
							lethal = true;
							break;
						case ColorType.Wall:
							movePos = null;
							lethal = true;
							break;
						case ColorType.Tele:
							movePos = new Position(movePos.brd, movePos.x + c.tox, movePos.y + c.toy);
							break;
					}
				}
				
				public Position moveFrom(Position ipos)
				{
					if (movePos == null)
						return ipos;
					else
						return movePos;
				}
				
				// only does dead or alive
				public SpecimenState moveTo(Position pos)
				{
					if (lethal)
						return SpecimenState.Dead;
					else
						return SpecimenState.Alive;
				}
			}
			
			// this isn't wierd atall
			public class Position
			{
				public Board brd {get; private set;}
				public int x {get; private set;}
				public int y {get; private set;}
				
				public Position(Board brdN, int xN, int yN)
				{
					brd = brdN;
					x = xN;
					y = yN;
				}
				
				public Color getColor()
				{
					return brd.getColor(x, y);
				}
				
				// move from this position to somewhere else
				public SpecimenState move(int ox, int oy, out Position rpos)
				{
					return brd.move(this, ox, oy, out rpos);
				}
				
				#region Equals and GetHashCode implementation
				// generated by #D
				
				public override bool Equals(object obj)
				{
					GameLogic.Board.Position other = obj as GameLogic.Board.Position;
					if (other == null)
						return false;
					return object.Equals(this.brd, other.brd) && this.x == other.x && this.y == other.y;
				}
				
				public override int GetHashCode()
				{
					int hashCode = 0;
					unchecked {
						if (brd != null)
							hashCode += 1000000007 * brd.GetHashCode();
						hashCode += 1000000009 * x.GetHashCode();
						hashCode += 1000000021 * y.GetHashCode();
					}
					return hashCode;
				}
				
				public static bool operator ==(GameLogic.Board.Position lhs, GameLogic.Board.Position rhs)
				{
					if (ReferenceEquals(lhs, rhs))
						return true;
					if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
						return false;
					return lhs.Equals(rhs);
				}
				
				public static bool operator !=(GameLogic.Board.Position lhs, GameLogic.Board.Position rhs)
				{
					return !(lhs == rhs);
				}
				#endregion
			}
			
			public int width {get; private set;}
			public int height {get; private set;}
			
			private Cell[,] grid; // tidy rather than fast
			private Color[] colorTable; // Color.n -> ColourType
			
			private Random rnd;
			
			private GameConstants consts;
			
			public Board(GameConstants constsN, Random rndN)
			{
				consts = constsN;
				
				width = consts.boardWidth; // shh
				height = consts.boardHeight;
				rnd = rndN;
				
				grid = new Cell[width, height]; // allocate now
				
				generate();
			}
			
			private void generate()
			{
				// create colorTable
				var colorCounts = new []
				{
					new { type = ColorType.Safe, count = consts.safeColCount, toxb = 0, toyb = 0 },
					new { type = ColorType.Tele, count = consts.teleColCount, toxb = 2, toyb = 2 },
					new { type = ColorType.Trap, count = consts.trapColCount, toxb = 1, toyb = 1 },
					new { type = ColorType.Wall, count = consts.wallColCount, toxb = 0, toyb = 0 }
				};
				
				colorTable = new Color[colorCounts.Sum(cc => cc.count)];
				
				Action<Color> assignColor = (c) =>
				{
					int k;
					while (colorTable[k = rnd.Next(0, colorTable.Length)] != null); // best pracice
					colorTable[k] = c;
				};
				
				int ci = 0;
				foreach (var cc in colorCounts)
				{
					for (int i = 0; i < cc.count; i++)
					{
						Color c = new Color(ci++, cc.type, rnd.Next(-cc.toxb, cc.toxb+1), rnd.Next(-cc.toyb, cc.toyb+1));
						assignColor(c);
					}
				}
				
				// fill grid (don't reallocate)
			again:
				Func<Color> rndColor = () => colorTable[rnd.Next(colorTable.Length)];
				
				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						grid[i, j] = new Cell(new Position(this, i, j), rndColor());
					}
				}
				
				foreach (Cell cl in grid)
				{
					int tx = cl.truePos.x;
					int ty = cl.truePos.y;
					
					Color c = cl.trueColor;
					
					if (c.type == ColorType.Trap)
					{
						tx += c.tox;
						ty += c.toy;
					}
					
					if (indicesInBounds(tx + c.tox, ty + c.toy))
					{
						grid[tx, ty].apply(c);
					}
				}
				
				// verify
				if (!verify())
					goto again;
			}
			
			private bool verify()
			{
				int maxTurns = consts.maxAge;
				
				HashSet<Position> seen = new HashSet<Position>(); // only set things seen when we move onto them
				List<Position> due = new List<Position>();
				List<Position> cur = new List<Position>();
				
				int turnCount = 0;
				
				for (int j = 0; j < height; j++)
				{
					cur.Add(new Position(this, 0, j));
				}
				
				while (cur.Count > 0 && turnCount < maxTurns)
				{
					foreach (Position next in cur)
					{
						for (int i = -1; i <= 1; i++)
						{
							for (int j = -1; j <= 1; j++)
							{
								Position np;
								SpecimenState ss = next.move(i, j, out np);
								
								if (ss == SpecimenState.Win)
								{
									// glorious victory
									return true;
								}
								
								if (ss == SpecimenState.Alive)
								{
									if (!seen.Contains(np))
									{
										seen.Add(np);
										due.Add(np);
									}
								}
							}
						}
					}
					
					var t = cur;
					cur = due;
					due = t;
					due.Clear();
					
					turnCount++;
				}
				
				return false;
			}
			
			// evaluate a move onto this position (i.e. end of turn)
			public SpecimenState move(Position ipos, int ox, int oy, out Position rpos)
			{
				if (Math.Abs(ox) > 1 || Math.Abs(oy) > 1)
				{
					throw new Exception("Invalid move");
				}
				
				int x = ipos.x + ox;
				int y = ipos.y + oy;
				
				rpos = new Position(this, x, y);
				
				if (boundsCheck(rpos.x, rpos.y) == SpecimenState.Dead)
					return SpecimenState.Dead;
				
				if (rpos.x >= width)
					return SpecimenState.Win;
				
				rpos = grid[rpos.x, rpos.y].moveFrom(ipos);
				
				if (boundsCheck(rpos.x, rpos.y) == SpecimenState.Dead)
					return SpecimenState.Dead;
				
				if (rpos.x >= width)
					return SpecimenState.Win;
				
				if (grid[rpos.x, rpos.y].moveTo(rpos) == SpecimenState.Dead)
					return SpecimenState.Dead;
				
				if (rpos.x >= width - 1)
					return SpecimenState.Win;
				
				return SpecimenState.Alive;
			}
			
			public bool indicesInBounds(int x, int y)
			{
				if (x < 0 || y < 0 || y >= height || x >= width)
					return false;
				return true;
			}
			
			public SpecimenState boundsCheck(int x, int y)
			{
				if (x < 0 || y < 0 || y >= height)
					return SpecimenState.Dead;
				
				return SpecimenState.Alive;
			}
			
			public Position position(int x, int y)
			{
				return new Position(this, x, y);
			}
			
			public Color getColor(int x, int y)
			{
				if (x < 0 || y < 0 || y >= height)
					return Color.OutOfBoundsDead;
				else if (x >= width)
					return Color.OutOfBoundsAlive;
				else
					return grid[x, y].trueColor;
			}
			
			public void printIsh(System.IO.TextWriter writer)
			{
				for (int j = 0; j < height; j++)
				{
					for (int i = 0; i < width; i++)
					{
						Cell c = grid[i, j];
						
						if (c.movePos == c.truePos)
							writer.Write(" ");
						else if (c.movePos == null)
							writer.Write("W");
						else
							writer.Write("T");
						
						if (c.lethal)
							writer.Write("!");
						else
							writer.Write(" ");
					}
					
					writer.WriteLine();
				}
			}
		}
		
		public class Game
		{
			private Board b;
			
			private Player plyr;
			private List<Specimen> specimens;
			
			private Random rnd = new Random();
			
			private GameConstants consts = new GameConstants();
			
			public Game(PlayerDel pd)
			{
				b = new Board(consts, rnd);
				
				plyr = new Player(pd);
				specimens = new List<Specimen>();
			}
			
			private void addSpecimen(Genome g)
			{
				Specimen s = new Specimen(g);
				resetSpecimen(s);
				specimens.Add(s);
			}
			
			private void resetSpecimen(Specimen s)
			{
				s.pos = new Board.Position(b, 0, rnd.Next(b.height));
				s.age = 0;
			}
			
			// returns the score (mean score)
			public decimal runSession()
			{
				int oldScore = 0;
				
				List<int> scores = new List<int>();
				
				for (int i = 0; i < consts.repeatCount; i++)
				{
					Console.WriteLine("Running game {0}", i);
					b.printIsh(Console.Out);
					runGame();
					
					int thisScore = plyr.score - oldScore;
					scores.Add(thisScore);
					oldScore = plyr.score;
					
					Console.WriteLine("Score: {0}", thisScore);
					Console.WriteLine();
				}
				
				Console.WriteLine("Scores: " + string.Join(", ", scores));
				
				decimal mean = (decimal)plyr.score / (decimal)consts.repeatCount;
				Console.WriteLine("Average score is " + mean);
				
				return mean;
			}
			
			private void runGame()
			{
				// init
				b = new Board(consts, rnd);
				
				specimens.Clear();
				for (int i = 0; i < consts.initialSpecimenCount; i++)
				{
					addSpecimen(new Genome(consts, rnd));
				}
				
				// run
				int turn = 0;
				int breedTicker = consts.turnsPerBreeding;
				
				while (turn++ < consts.turnCount)
				{
					move();
					
					if (--breedTicker == 0)
					{
						breed();
						breedTicker = consts.turnsPerBreeding;
					}
				}
			}
			
			static int bestX = 1;
			
			private void move()
			{
				View v = new View(consts); // reusable
				
				for (int i = specimens.Count - 1; i >= 0; i--)
				{
					Specimen s = specimens[i];
					
					int ox, oy;
					
					v.see(s.pos);
					plyr.pd.Invoke(v, s.g, out ox, out oy);
					
					Board.Position npos;
					SpecimenState sstate = s.pos.move(ox, oy, out npos);
					s.pos = npos;
					
					if (s.pos.x > bestX)
					{
						bestX = s.pos.x;
						Console.WriteLine(bestX);
					}
					
					if (sstate == SpecimenState.Win)
					{
						s.score++;
						plyr.score++;
						resetSpecimen(s);
					}
					else if (sstate == SpecimenState.Dead || ++s.age > consts.maxAge)
					{
						specimens.RemoveAt(i);
					}
				}
			}
				
			private void breed()
			{
				if (specimens.Count > 1)
				{
					int n = consts.reproductionRate;
					
					for (int i = 0; i < n; i++)
					{
						Specimen[] breeders = grabDistinct(2);
						addSpecimen(breeders[0].cross(consts, rnd, breeders[1]));
					}
				}
			}
			
			private Specimen[] grabDistinct(int count)
			{
				if (specimens.Count < count)
					return null;
				
				Specimen[] res = new Specimen[count];
				
				long maxRnd = specimens.Sum(s => s.fitness);
				
				while (count > 0)
				{
					long idx = rndLong(rnd, maxRnd);
					
					for (int i = 0; i < specimens.Count; i++)
					{
						Specimen s = specimens[i];
						long f = s.fitness;
						
						if (idx < f)
						{
							if (Array.IndexOf(res, s) != -1)
								break; // go again
							
							res[--count] = s;
							break;
						}
						
						idx -= f;
					}
				}
				
				return res;
			}
		}
		
		// dies if max <= 0
		private static long rndLong(Random rnd, long max)
		{
			int bmask = 0;
			
			long dm = max;
			while (dm > 0)
			{
				dm /= 2;
				bmask = (bmask << 1) + 1;
			}
			
			byte[] bytes = new byte[8];
			
		again:
			rnd.NextBytes(bytes);
			
			long cur = 0;
			for (int i = 0; i < 8; i++)
				cur = (cur << 8) + bytes[i];
			
			cur &= bmask;
			
			if (cur >= max)
				goto again;
			
			return cur;
		}
	}
}    