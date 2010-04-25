using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace InstallHelper
{
    static class Program
    {

		private static Form1 _mainFrm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			if (!StudioControls.Controls.UACUtilities.HasAdminPrivileges())
			{
				if (!SingleInstanceApplication.ApplicationInstanceManager.CreateSingleInstance(Assembly.GetExecutingAssembly().GetName().Name, SingleInstanceCallback)) return;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			_mainFrm = new Form1();
			_mainFrm.AppendArgs(args);

			Application.Run(_mainFrm);
		}

		/// <summary>
		/// Single instance callback handler.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="SingleInstanceApplication.InstanceCallbackEventArgs"/> instance containing the event data.</param>
		private static void SingleInstanceCallback(object sender, SingleInstanceApplication.InstanceCallbackEventArgs args)
		{
			if (args == null || _mainFrm == null) return;
			Action<bool> d = (bool x) =>
			{
				_mainFrm.AppendArgs(args.CommandLineArgs);
				_mainFrm.Activate(x);
			};
			_mainFrm.Invoke(d, true);
		}

    }
}
