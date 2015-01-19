/*
 * Created by SharpDevelop.
 * User: Freddie
 * Date: 19/01/2015
 * Time: 17:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ppcggacscontroller
{
	public interface IDisplay
	{
		void tick();
		int ticksPerFrame {get; set;}
	}
	
	/// <summary>
	/// Description of Display.
	/// </summary>
	public partial class Display : Form, IDisplay
	{
		public static IDisplay createAndRun(GameLogic.Game g)
		{
			Display d = new Display(g);
			Action a = () => Application.Run(d);
			a.BeginInvoke(null, null);
			return d;
		}
		
		private GameLogic.Game g;
		public bool gone {get; private set;}
		public int ticksPerFrame {get; set;}
		private int tickCount = 0;
		private int scale = 10;
		
		public Display(GameLogic.Game gN)
		{
			InitializeComponent();
			
			g = gN;
			ticksPerFrame = 1; // this is a sensible number
		}
		
		void ViewFPaint(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(Color.White);
			g.draw(e.Graphics, scale);
		}
		
		private void invalidateMe()
		{
			this.Invoke(new Action(() => this.Refresh()));
		}
		
		public void tick()
		{
			tickCount++;
			
			if (tickCount < ticksPerFrame)
				return;
			
			tickCount = 0;
			
			try
			{
				if (!gone)
					invalidateMe();
			}
			catch
			{
				gone = true;
			}
		}
		
		void DisplayFormClosing(object sender, FormClosingEventArgs e)
		{
			gone = true;
		}
		
		void TpfFValueChanged(object sender, EventArgs e)
		{
			ticksPerFrame = tpfF.Value;
		}
	}
}

