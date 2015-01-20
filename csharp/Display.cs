using System;
using System.Drawing;
using System.Windows.Forms;

namespace ppcggacscontroller
{
	public interface IDisplay
	{
		void tick();
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
		public bool gone; /* {get; private set;} //pfcr? performance!!*/
		private int tickCount = 0;
		private int scale = 12;
		
		private int ticksPerFrame = 1;
		private bool showTeleArrows = false;
		
		public Display(GameLogic.Game gN)
		{
			InitializeComponent();
			
			g = gN;
		}
		
		void ViewFPaint(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(Color.White);
			g.draw(e.Graphics, scale, showTeleArrows);
		}
		
		private void invalidateMe()
		{
			this.Invoke(new Action(() => viewF.Refresh()));
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
		
		void TpfFScroll(object sender, EventArgs e)
		{
			ticksPerFrame = tpfF.Value;
		}
		
		void StFCheckedChanged(object sender, EventArgs e)
		{
			showTeleArrows = stF.Checked;
		}
	}
}

