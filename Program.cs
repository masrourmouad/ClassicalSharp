﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace ClassicalSharp {
	
	class Program {
		
		[STAThread]
		public static void Main( string[] args ) {
			if( !Debugger.IsAttached ) {
				AppDomain.CurrentDomain.UnhandledException += UnhandledException;
			}
			
			Console.WriteLine( "Starting " + Utils.AppName + ".." );
			if( !AllResourcesExist( "terrain.png", "char.png", "clouds.png" ) ) {
				return;
			}
			if( args.Length < 4 ) {
				Fail( "ClassicalSharp.exe is only the raw client. You must either use the launcher or"
				     + " provide command line arguments to start the client." );
				return;
			}

            IPAddress ip = null;
            if( !IPAddress.TryParse( args[2], out ip ) ) {
                Fail( "Invalid IP \"" + args[2] + '"' );
            }

            int port = 0;
            if( !Int32.TryParse( args[3], out port ) ) {
                Fail( "Invalid port \"" + args[3] + '"' );
                return;
            } else if( port < ushort.MinValue || port > ushort.MaxValue ) {
                Fail( "Specified port " + port + " is out of valid range." );
            }

            string skinServer = args.Length >= 5 ? args[4] : "http://s3.amazonaws.com/MinecraftSkins/";
			using( Game game = new Game() ) {
				game.Username = args[0];
				game.Mppass = args[1];
				game.IPAddress = ip;
                game.Port = port;
				game.skinServer = skinServer;
				game.Run();
			}
		}
		
		static bool AllResourcesExist( params string[] resources ) {
			foreach( string resource in resources ) {
				if( !File.Exists( resource ) ) {
					Fail( resource + " not found. Cannot start." );
					return false;
				}
			}
			return true;
		}
		
		static void Fail( string text ) {
			Utils.LogWarning( text );
			Console.WriteLine( "Press any key to exit.." );
			Console.ReadKey( true );
		}

		static void UnhandledException( object sender, UnhandledExceptionEventArgs e ) {
			// So we don't get the normal unhelpful crash dialog on Windows.
			Exception ex = (Exception)e.ExceptionObject;
			string error = ex.GetType().FullName + ": " + ex.Message + Environment.NewLine + ex.StackTrace;
			try {
				using( StreamWriter writer = new StreamWriter( "crash.log", true ) ) {
					writer.WriteLine( "Crash time: " + DateTime.Now.ToString() );
					writer.WriteLine( error );
					writer.WriteLine();
				}
			} catch( Exception ) {
			}
			
			MessageBox.Show(
				"Oh dear. ClassicalSharp has crashed." + Environment.NewLine + Environment.NewLine + 
				" The cause of the crash has been logged to \"crash.log\". Please consider reporting the " +
				"crash (and the circumstances that caused it) to github.com/UnknownShadow200/ClassicalSharp/issues " +
				Environment.NewLine + Environment.NewLine +			
				"(the cause of the crash is reproduced below)" + Environment.NewLine + error, 
				"ClassicalSharp has crashed" );
			
			Environment.Exit( 1 );
		}
	}
}