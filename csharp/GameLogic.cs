/**
 * Loosley based off this: https://github.com/mbuettner/ppcg-genetic-algorithms/tree/master/c%2B%2B
 * Should conform to the Spec as of 2015-01-19 23:00 UTC: http://codegolf.stackexchange.com/questions/44707/lab-rat-race-an-exercise-in-genetic-algorithms
 * 
 * If it breaks, shout at VisualMelon
 */

// testGenome means that we create TestGenomes, instead of ManGenomes, which run ManGenomes and BarrGenomes together to check the former works
//#define testGenome

using System;
using BitArr = System.Collections.BitArray;
using System.Collections.Generic;
using System.Linq;

// sorry about the naming, never written code for someone else to use before

namespace ppcggacscontroller
{
	public class GameLogic
	{
		public class GameConstants
		{
			// board
			public int boardWidth = 53;
			public int goalX = 49;
			public int boardHeight = 15;
		
			public int safeColCount = 8;
			public int trapColCount = 2;
			public int teleColCount = 4;
			public int wallColCount = 2;
			
			public int teleDist = 4;
			public int trapDist = 1;
			
			// game
			public int turnCount = 10000;
			public int repeatCount = 50;
			
			public int initialSpecimenCount = 15;
			public int turnsPerBreeding = 1;
			
			// genome
			public int genomeSize = 100;
			
			public double genomeSwapRate = 0.05;
			public double genomeMutateRate = 0.01;
			
			// specimens
			public int maxAge = 100;
			public int reproductionRate = 10;
			public int fitnessScoreCoef = 50;
			
			// views
			public int viewDimX = 2;
			public int viewDimY = 2;
			
#if! nogdi
			// display
			public System.Drawing.Brush emptyBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 255, 255, 255));
			public System.Drawing.Brush specimenBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 0, 0, 0));
			public System.Drawing.Brush deathBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 255, 128, 128));
			public System.Drawing.Brush teleBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 128, 128, 255));
			public System.Drawing.Brush wallBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 128, 128, 128));
			public System.Drawing.Brush goalBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 64, 128, 64));
#endif
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
			public static Color OutOfBounds = new Color(-1, ColorType.Trap);
			
			public int n {get; private set;} //pfcr?
			public ColorType type {get; private set;} //pfcr?
			public int tox {get; private set;} //pfcr? // target offsets
			public int toy {get; private set;} //pfcr?
			
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
			/// <summary>
			/// Look up the color at the given offset from my position
			/// </summary>
			int this[int ox, int oy] {get;}
			
			/// <summary>
			/// The x-dimension of the view (the view spans from -xd to xd)
			/// </summary>
			int xd {get;}
			
			/// <summary>
			/// The y-dimension of the view (the view spans from -yd to yd)
			/// </summary>
			int yd {get;}
		}
		
		// this one actually needs to be public
		private class View : IView
		{
			private int[,] colors;
			public int xd {get; private set;}
			public int yd {get; private set;}
			
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
			
			public void see(Board.Position pos, Board brd)
			{
				for (int i = -xd; i <= xd; i++)
				{
					for (int j = -yd; j <= yd; j++)
					{
						colors[i + xd, j + yd] = brd.getColor(i + pos.x, j + pos.y).n;
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
		
		public interface IGenome : IEnumerable<bool>
		{
			/// <summary>
			/// Look up the bit at the given index
			/// </summary>
			bool this[int idx] {get;}
			
			/// <summary>
			/// The length of the Genome
			/// </summary>
			int length {get;}
			
			uint cutOutInt(int idx, int len);
		}
		
#if testGenome
		private class TestGenome : IGenome
		{
			private ManGenome g;
			
			public TestGenome(GameConstants consts, Random rnd, IGenome a, IGenome b)
			{
				//g = new ManGenome(consts, rnd, a, b);
				
				int seed = rnd.Next();
				Random mrnd = new Random(seed);
				Random brnd = new Random(seed);
				
				ManGenome mg = new ManGenome(consts, mrnd, ((TestGenome)a).g, ((TestGenome)b).g);
				BarrGenome bg = new BarrGenome(consts, brnd, a, b);
				
				for (int i = 0; i < mg.length; i++)
				{
					if (bg[i] != mg[i])
						throw new Exception("err");
					
					for (int j = 0; j <= 32 && i + j < mg.length; j++)
					{
						if (bg.cutOutInt(i, j) != mg.cutOutInt(i, j))
							throw new Exception("err " + i + " " + j);
					}
				}
				
				foreach (bool bbb in mg)
				{
					g = mg;
				}
				
				g = mg;
			}
			
			public TestGenome(GameConstants consts, Random rnd)
			{
				g = new ManGenome(consts, rnd);
			}
			
			public bool this[int idx]
			{
				get
				{
					return g[idx];
				}
			}
			
			public int length
			{
				get
				{
					return g.length;
				}
			}
			
			public uint cutOutInt(int idx, int len)
			{
				return g.cutOutInt(idx, len);
			}
			
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
			
			public IEnumerator<bool> GetEnumerator()
			{
				return g.GetEnumerator();
			}
		}
#endif

		private class ManGenome : IGenome
		{
			private uint[] ints;
			private int glen;
			readonly int k = sizeof(uint) * 8;
			
			private ManGenome(GameConstants consts)
			{
				glen = consts.genomeSize;
				
				ints = new uint[(glen - 1) / k + 1];
			}
			
			// I hate this method
			public ManGenome(GameConstants consts, Random rnd, ManGenome a, ManGenome b) : this(consts)
			{
				
				ManGenome cur = a;
				ManGenome oth = b;
				uint curi = cur.ints[0];
				uint othi = oth.ints[0];
				
				Action swap = () =>
				{
					var t = cur;
					cur = oth;
					oth = t;
					
					var ti = curi;
					curi = othi;
					othi = ti;
				};
				
				if (rnd.NextDouble() < 0.5)
					swap();
				
				int ii = 0;
				int ij = 0;
				uint ib = 1;
				
				uint c = 0;
				
				for (int i = 0; i < glen; i++)
				{
					if (ib == 0)
					{
						ints[ii++] = c;
						
						c = 0;
						
						ij = 0;
						ib = 1;
						
						curi = cur.ints[ii];
						othi = oth.ints[ii];
					}
					
					uint v = (curi & ib);
					
					if (rnd.NextDouble() < consts.genomeMutateRate)
						v ^= ib;
					
					c |= v;
					
					if (rnd.NextDouble() < consts.genomeSwapRate)
						swap();
					
					ij++;
					ib = ib << 1;
				}
				
				ints[ii] = c;
			}
			
			public ManGenome(GameConstants consts, Random rnd) : this(consts)
			{
				byte[] bytes = new byte[4];
				
				for (int i = 0; i < ints.Length; i++)
				{
					rnd.NextBytes(bytes);
					ints[i] = (uint)(((uint)bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3]);
				}
			}
			
			public uint cutOutInt(int idx, int len)
			{
				if (idx < 0 || idx + len > glen)
					throw new Exception("Range out of bounds in Genome, looked up range at " + idx + " of length " + len + " in Genome of length " + glen);
				
				if (len > k || len < 0)
					throw new Exception("Invalid length provided, Len of " + len + " asked for, " + k + " is max len, cannot be negative");
				
				if (len == 0)
					return 0;
				
				int i = idx / k;
				int s = idx % k;
				int f = s + len;
				
				uint n = 0;
				
				if (f > k)
					n = ((uint)ints[i] >> s) + (((uint)ints[i + 1] << (k-(f-k))) >> (k-len));
				else
					n = ((uint)ints[i] << (k-f)) >> (k-len);
				
				return n;
			}
			
			public bool this[int idx]
			{
				get
				{
					if (idx >= glen)
						throw new Exception("Index out of bounds in Genome, looked up " + idx + " in Genome of length " + glen);
				
					int i = idx / k;
					uint b = (uint)1 << (idx % k);
					return (ints[i] & b) != 0;
				}
				private set
				{
					int i = idx / k;
					uint b = (uint)1 << (idx % k);
					if (value)
						ints[i] |= b;
					else
						ints[i] &= ~b;
				}
			}
			
			public int length
			{
				get
				{
					return glen;
				}
			}
			
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
			
			// TODO: make this faster
			public IEnumerator<bool> GetEnumerator()
			{
				int ii = 0;
				int ij = 0;
				
				uint c = ints[0];
				
				for (int i = 0; i < glen; i++)
				{
					bool r = (c & 1) == 1;
#if testGenome
					if (r != this[i])
						throw new Exception("Grah!");
#endif
					yield return r;
					c = c >> 1;
					
					ij++;
					if (ij >= k)
					{
						ij = 0;
						c = ints[++ii];
					}
				}
			}
		}
		
#if testGenome
		private class BarrGenome : IGenome
		{
			private BitArr barr;
			
			private BarrGenome(GameConstants consts)
			{
				barr = new BitArr(consts.genomeSize);
			}
			
			public BarrGenome(GameConstants consts, Random rnd, IGenome a, IGenome b) : this(consts)
			{
				IGenome cur = a;
				IGenome oth = b;
				
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
					bool t = cur[i];
					
					if (rnd.NextDouble() < consts.genomeMutateRate)
						t = !t;
					
					barr[i] = t;
					
					if (rnd.NextDouble() < consts.genomeSwapRate)
						swap();
				}
			}
			
			public BarrGenome(GameConstants consts, Random rnd) : this(consts)
			{
				for (int i = 0; i < barr.Length; i++)
					barr[i] = rnd.NextDouble() < 0.5;
			}

			public uint cutOutInt(int idx, int len)
			{
				uint n = 0;
				for (int i = len - 1; i >= 0; i--)
				{
					n = n * 2 + (uint)(barr[i + idx] ? 1 : 0);
				}
				
				return n;
			}
			
			public bool this[int idx]
			{
				get
				{
					return barr[idx];
				}
			}
			
			public int length
			{
				get
				{
					return barr.Length;
				}
			}
			
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
			
			public IEnumerator<bool> GetEnumerator()
			{
				foreach (bool b in barr)
					yield return b;
			}
		}
#endif
		
		private class Specimen
		{
			public Board.Position pos;
			public int age;
			public int score;
			private int fitnessScoreCoef;
			
#if testGenome
			public IGenome g {get; private set;} //pfcr?
#else
			public ManGenome g {get; private set;} //pfcr?
#endif
			
			public long fitness {get; private set;} //pfcr?
			
			public void computeFitness()
			{
				fitness = pos.x + 1 + fitnessScoreCoef * score;
			}
			
#if testGenome
			public Specimen(IGenome gN, GameConstants consts)		
#else
			public Specimen(ManGenome gN, GameConstants consts)		
#endif
			{
				g = gN;
				fitnessScoreCoef = consts.fitnessScoreCoef;
			}
			
#if testGenome
			public IGenome cross(GameConstants consts, Random rnd, Specimen other)
#else
			public ManGenome cross(GameConstants consts, Random rnd, Specimen other)
#endif
			{
#if testGenome
				return new TestGenome(consts, rnd, g, other.g);
#else
				return new ManGenome(consts, rnd, g, other.g);
#endif
			}
		}
		
		public delegate void PlayerDel(IView v, IGenome g, Random r, out int ox, out int oy);
		
		private class Player
		{
			public int score;
			public PlayerDel pd;
			
			public Player(PlayerDel pdN)
			{
				pd = pdN;
			}
		}
		
		private class Board
		{
			private class Cell
			{
				// location of this Cell
				public Position truePos {get; private set;} //pfcr?
				// where a Specimen moving onto this Cell should be moved to
				public Position movePos {get; private set;} //pfcr?
				// whether a Specimen is not allowed to move to this Cell
				public bool wall {get; private set;} //pfcr?
				// whether a Specimen that ends up on this Cell is a dead Specimen
				public bool lethal {get; private set;} //pfcr?
				// the Color of the Cell
				public Color trueColor {get; private set;} //pfcr?
				
				public View view {get; private set;} //pfcr?
				
				public Cell(Position truePosN, Color trueColorN)
				{
					truePos = truePosN;
					movePos = truePos; // default
					trueColor = trueColorN;
					
					wall = false;
					lethal = false;
				}
				
				public void createView(GameConstants consts, Board brd)
				{
					view = new View(consts);
					view.see(truePos, brd);
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
							wall = true;
							lethal = true;
							break;
						case ColorType.Tele:
							movePos = new Position(movePos.x + c.tox, movePos.y + c.toy);
							break;
					}
				}
				
				public Position moveFrom(Position ipos)
				{
					if (wall) // we are a wall, or the like
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
			
			public class Position
			{
				public int x {get; private set;} //pfcr?
				public int y {get; private set;} //pfcr?
				
				public Position(int xN, int yN)
				{
					x = xN;
					y = yN;
				}
				
				#region Equals and GetHashCode implementation
				// generated by #D
				
				public override bool Equals(object obj)
				{
					GameLogic.Board.Position other = obj as GameLogic.Board.Position;
					if (other == null)
						return false;
					return this.x == other.x && this.y == other.y;
				}
				
				public override int GetHashCode()
				{
					int hashCode = 0;
					unchecked {
						hashCode += 1000000007 * x.GetHashCode();
						hashCode += 1000000009 * y.GetHashCode();
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
			
			public int width {get; private set;} //pfcr?
			public int height {get; private set;} //pfcr?
			
			private Cell[,] grid; // tidy rather than fast
			private Color[] colorTable; // Color.n -> ColourType
			private int[] startCellYs;
			
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
					new { type = ColorType.Safe, count = consts.safeColCount, manual = false, toxb = 0, toyb = 0 },
					new { type = ColorType.Tele, count = consts.teleColCount, manual = true,  toxb = consts.teleDist, toyb = consts.teleDist }, // we do these separately
					new { type = ColorType.Trap, count = consts.trapColCount, manual = false, toxb = consts.trapDist, toyb = consts.trapDist },
					new { type = ColorType.Wall, count = consts.wallColCount, manual = false, toxb = 0, toyb = 0 }
				};
				
				colorTable = new Color[colorCounts.Sum(cc => cc.count)];
				
				Func<int> nextColorId = () =>
				{
					int k;
					while (colorTable[k = rnd.Next(0, colorTable.Length)] != null); // best practise
					return k;
				};
				
				// add tele colors
				for (int i = 0; i < consts.teleColCount; i += 2)
				{
					// I am lazy
				reo:
					int ox = rnd.Next(-consts.teleDist, consts.teleDist + 1);
					int oy = rnd.Next(-consts.teleDist, consts.teleDist + 1);
					
					if (ox == 0 && oy == 0) // no thanks
						goto reo;
					
					Color c;
					
					c = new Color(nextColorId(), ColorType.Tele, ox, oy);
					colorTable[c.n] = c;
					
					c = new Color(nextColorId(), ColorType.Tele, -ox, -oy);
					colorTable[c.n] = c;
				}
				
				// add less picky colors
				foreach (var cc in colorCounts)
				{
					if (cc.manual)
						continue;
					
					for (int i = 0; i < cc.count; i++)
					{
						Color c = new Color(nextColorId(), cc.type, rnd.Next(-cc.toxb, cc.toxb+1), rnd.Next(-cc.toyb, cc.toyb+1));
						colorTable[c.n] = c;
					}
				}
				
				// fill grid (don't reallocate)
			again:
				Func<Color> rndColor = () => colorTable[rnd.Next(colorTable.Length)];
				
				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						Color rndc = rndColor();
						
						while (i >= consts.goalX && rndc.type != ColorType.Safe)
						{ // goal column must be safe
							rndc = rndColor();
						}
						
						grid[i, j] = new Cell(new Position(i, j), rndc);
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
					
					if (boundsCheck(tx, ty) == SpecimenState.Alive) // only apply to non-goal cells on the track
					{
						grid[tx, ty].apply(c);
					}
				}
				
				foreach (Cell cl in grid)
				{
					cl.createView(consts, this);
				}
				
				// verify
				if (!verify())
					goto again;
			}
			
			
			private bool verify()
			{
				int inadmissibleCount = 0;
				
				List<int> startCellYlist = new List<int>();
				
				for (int j = 0; j < height; j++)
				{
					if (admissibleStartingCellCheck(new Position(0, j)))
					{
						startCellYlist.Add(j);
					}
					else
					{
						inadmissibleCount++;
						if (inadmissibleCount >= 10)
							return false;
					}
				}
				
				startCellYs = startCellYlist.ToArray();
				
				return true;
			}
			
			private bool admissibleStartingCellCheck(Position sp)
			{
				int maxTurns = consts.maxAge;
				
				HashSet<Position> seen = new HashSet<Position>(); // only set things seen when we move onto them
				List<Position> due = new List<Position>();
				List<Position> cur = new List<Position>();
				
				cur.Add(sp);
				
				int turnCount = 0;
				
				while (cur.Count > 0 && turnCount < maxTurns)
				{
					foreach (Position next in cur)
					{
						for (int i = -1; i <= 1; i++)
						{
							for (int j = -1; j <= 1; j++)
							{
								Position np;
								SpecimenState ss = this.move(next, i, j, out np);
								
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
			
			public Position rndStartCell()
			{
				int y = startCellYs[rnd.Next(startCellYs.Length)];
				return new Position(0, y);
			}
			
			public int numStartCells
			{
				get
				{
					return startCellYs.Length;
				}
			}
			
			public View getView(int x, int y)
			{
				return grid[x, y].view;
			}
			
			// evaluate a move onto this position
			public SpecimenState move(Position ipos, int ox, int oy, out Position rpos)
			{
				if (ox > 1 || ox < -1 || oy > 1 || oy < -1)
				{
					throw new Exception("Invalid move");
				}
				
				int x = ipos.x + ox;
				int y = ipos.y + oy;
				
				SpecimenState premss = boundsCheck(x, y);
				if (premss == SpecimenState.Dead)
				{
					rpos = new Position(x, y);
					return SpecimenState.Dead;
				}
				
				rpos = grid[x, y].moveFrom(ipos);
				
				SpecimenState postmss = boundsCheck(rpos.x, rpos.y);
				if (postmss != SpecimenState.Alive)
					return postmss;
				
				if (grid[rpos.x, rpos.y].moveTo(rpos) == SpecimenState.Dead)
					return SpecimenState.Dead;
				
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
				
				if (x >= consts.goalX)
					return SpecimenState.Win;
				
				return SpecimenState.Alive;
			}
			
			public Position position(int x, int y)
			{
				return new Position(x, y);
			}
			
			public Color getColor(int x, int y)
			{
				if (x < 0 || y < 0 || y >= height || x >= width)
					return Color.OutOfBounds;
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
						
						if (c.wall)
							writer.Write("W");
						else if (c.trueColor.type == ColorType.Tele)
							writer.Write("T");
						else
							writer.Write(" ");
						
						if (c.lethal)
							writer.Write("!");
						else
							writer.Write(" ");
					}
					
					writer.WriteLine();
				}
			}
			
#if! nogdi
			private Cell[,] bgGrid = null;
			private int bgScale = -1;
			private bool bgDrawTeleArrows = false;
			private System.Drawing.Bitmap bgBmp;
			
			private void drawBG(int scale, bool drawTeleArrows)
			{
				int w = scale * width;
				int h = scale * height;
				
				if (bgBmp != null)
					bgBmp.Dispose();
				
				bgGrid = grid;
				bgScale = scale;
				bgDrawTeleArrows = drawTeleArrows;
				
				bgBmp = new System.Drawing.Bitmap(w, h);
				
				System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bgBmp);
				
				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						System.Drawing.Brush brsh = consts.emptyBrush;
						
						if (boundsCheck(i, j) == SpecimenState.Win)
							brsh = consts.goalBrush;
						
						Cell cl = grid[i, j];
						
						if (cl.trueColor.type == ColorType.Wall)
							brsh = consts.wallBrush;
						else if (cl.trueColor.type == ColorType.Tele)
							brsh = consts.teleBrush;
						else if (cl.lethal)
							brsh = consts.deathBrush;
						
						g.FillRectangle(brsh, i * scale, j * scale, scale, scale);
					}
				}
				
				if (drawTeleArrows)
				{
					System.Drawing.Pen telePen = new System.Drawing.Pen(System.Drawing.Color.DarkGray, 1);
					telePen.StartCap = System.Drawing.Drawing2D.LineCap.NoAnchor;
					telePen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
					
					foreach (Cell cl in grid)
					{
						if (cl.trueColor.type == ColorType.Tele)
						{
							g.DrawLine(telePen, cl.truePos.x * scale + scale / 2, cl.truePos.y * scale + scale / 2, cl.movePos.x * scale + scale / 2, cl.movePos.y * scale + scale / 2);
						}
					}
				}
			}
			
			public void draw(System.Drawing.Graphics g, int scale, bool drawTeleArrows)
			{
				if (grid != bgGrid || scale != bgScale || drawTeleArrows != bgDrawTeleArrows || bgBmp == null)
				{
					drawBG(scale, drawTeleArrows);
				}
				
				g.DrawImage(bgBmp, 0, 0);
			}
#endif
		}
		
		public class Game
		{
			private Object drawLock = new Object();
			public IDisplay displayer {get; set;}
			
			private Board b;
			
			private Player plyr;
			private List<Specimen> specimens;
			
			private Random rnd = new Random();
			
			private GameConstants consts = new GameConstants();
			
			public Game(PlayerDel pd)
			{
				plyr = new Player(pd);
				specimens = new List<Specimen>();
			}
			
#if testGenome
			private void addSpecimen(IGenome g)
#else
			private void addSpecimen(ManGenome g)
#endif
			{
				Specimen s = new Specimen(g, consts);
				resetSpecimen(s);
				s.computeFitness();
				specimens.Add(s);
			}
			
			private void resetSpecimen(Specimen s)
			{
				s.pos = b.rndStartCell();
				s.age = 0;
			}
			
			// returns the score (mean score)
			public double runSession()
			{
				List<int> scores = new List<int>();
				
				for (int i = 0; i++ < consts.repeatCount;)
				{
					Console.WriteLine("Running game {0}", i);
					
					runGame();
					
					scores.Add(plyr.score);
					
					Console.WriteLine("Score: {0}", plyr.score);
					Console.WriteLine();
				}
				
				Console.WriteLine("Scores: " + string.Join(", ", scores.Select(s => s.ToString()).ToArray()));
				
				double finalScore = geoMean(scores.Select(s => (double)s));
				Console.WriteLine("Final score is " + finalScore);
				
				return finalScore;
			}
			
			private static double geoMean(IEnumerable<double> values)
			{
				double pac = 0;
				int n = 0;
				
				foreach (double v in values)
				{
					pac += Math.Log(v);
					n++;
				}
				
				return Math.Exp(pac / (double)n);
			}
			
			private void runGame()
			{
				// init
				
				lock (drawLock)
				{
					
					b = new Board(consts, rnd);
					
					Console.WriteLine(b.numStartCells + " start cells");
					
					plyr.score = 1;
					specimens.Clear();
					
					for (int i = 0; i < consts.initialSpecimenCount; i++)
					{
	#if testGenome
						addSpecimen(new TestGenome(consts, rnd));
	#else
						addSpecimen(new ManGenome(consts, rnd));
	#endif
					}
				
				}
				
				// run
				System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
				sw.Reset();
				sw.Start();
				
				int turn = 0;
				int breedTicker = consts.turnsPerBreeding;
				long spc = 0;
				
				long bestFitness = 0;
				
				Console.WriteLine("\ttime\tscore\tcount\tavg F\tmax F\ttop F");
				
				Action printDiags = () =>
				{
					Console.WriteLine(
						(100 * turn / consts.turnCount) + "%"
						+ "\t" +
						((decimal)sw.ElapsedMilliseconds / 1000M).ToString("0.0") + "s"
						+ "\t" +
						plyr.score
						+ "\t" +
						specimens.Count
						+ "\t" +
						(specimens.Count == 0 ? "-" : specimens.Average(s => s.fitness).ToString("0.00"))
						+ "\t" +
						(specimens.Count == 0 ? "-" : specimens.Max(s => s.fitness).ToString())
						+ "\t" +
						bestFitness
					); 
				};
				
				int diagInterval = 200;
				int diagTicker = diagInterval;
				
				while (turn < consts.turnCount)
				{
					turn++;
					
					if (specimens.Count < 2)
						break; // end game
					
					spc += specimens.Count;
					
					lock (drawLock)
					{
						
						move();
						
						long fitnessSum = 0L;
						foreach (Specimen s in specimens)
						{
							s.computeFitness();
							
							if (s.fitness > bestFitness)
								bestFitness = s.fitness;
							fitnessSum += s.fitness;
						}
						
						if (--diagTicker == 0)
						{
							printDiags();
							diagTicker = diagInterval;
						}
						
						// breed
						if (--breedTicker == 0)
						{
							breed(fitnessSum);
							breedTicker = consts.turnsPerBreeding;
						}
						
					}
					
					if (displayer != null)
						displayer.tick();
				}
				
				sw.Stop();
				Console.WriteLine();
				printDiags();
				Console.WriteLine(turn + " last turn");
				Console.WriteLine(spc + " specimen turns");
				if (sw.ElapsedMilliseconds > 0)
					Console.WriteLine(((decimal)spc / (decimal)sw.ElapsedMilliseconds * 1000M) + " specimen turns per second");
			}
			
			private void move()
			{
				for (int i = specimens.Count - 1; i >= 0; i--)
				{
					Specimen s = specimens[i];
					
					if (s.age == consts.maxAge)
					{
						specimens.RemoveAt(i);
						continue;
					}
					
					s.age++;
					
					int ox, oy;
					
					plyr.pd.Invoke(b.getView(s.pos.x, s.pos.y), s.g, rnd, out ox, out oy);
					
					Board.Position npos;
					SpecimenState sstate = b.move(s.pos, ox, oy, out npos);
					s.pos = npos;
					
					if (sstate == SpecimenState.Win)
					{
						s.score++;
						plyr.score++;
						resetSpecimen(s);
					}
					else if (sstate == SpecimenState.Dead)
					{
						specimens.RemoveAt(i);
					}
				}
			}
			
			private void breed(long fitnessSum)
			{
				if (specimens.Count >= 2)
				{
					for (int i = 0; i < consts.reproductionRate; i++)
					{
						Specimen a, b;
						grabDistinctPair(fitnessSum, out a, out b);
						addSpecimen(a.cross(consts, rnd, b));
					}
				}
			}
			
			private void grabDistinctPair(long maxRnd, out Specimen a, out Specimen b)
			{
				a = b = null;
				
				long ai = rndLong(rnd, maxRnd);
				long bi = rndLong(rnd, maxRnd);
				if (ai > bi)
				{
					long t = ai;
					ai = bi;
					bi = t;
				}
				
				bi = bi - ai;
				
			again:
				for (int i = 0; i < specimens.Count; i++)
				{
					Specimen s = specimens[i];
					long f = s.fitness;
					
					if (ai < f)
					{
						if (bi < 0)
						{
							if (a == s)
								goto nope;
							
							b = s;
							return;
						}
						
						a = s;
						ai += bi;
						bi = -1;
						
						if (ai < f)
							goto nope;
					}
					
					ai -= f;
				}
				
			nope:
				ai = rndLong(rnd, maxRnd);
				goto again;
			}
			
#if! nogdi
			public void draw(System.Drawing.Graphics g, int scale, bool drawTeleArrows)
			{
				if (b == null)
					return;
				
				lock (drawLock)
				{
				
					b.draw(g, scale, drawTeleArrows);
					
					HashSet<Board.Position> pps = new HashSet<Board.Position>();
					
					foreach (Specimen s in specimens)
					{
						Board.Position p = s.pos;
						if (pps.Contains(p))
							continue;
						pps.Add(p);
						
						System.Drawing.Brush brsh = consts.specimenBrush;
						
						// too much fun
						//brsh = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 0, 255 - (int)(255.0 / (1.0 + (double)s.fitness / 50.0)), 100));
						
						g.FillRectangle(brsh, p.x * scale + 1, p.y * scale + 1, scale - 2, scale - 2);
					}
					
				}
			}
#endif
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