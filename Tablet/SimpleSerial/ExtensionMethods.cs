using AxWMPLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleSerial {

	/// <summary>
	/// Extension methods are used to add new behavior to existing classes.
	/// </summary>
	internal static class ExtensionMethods {

		public static void Play( this AxWindowsMediaPlayer player, string filePath ) {
			player.URL = filePath;
			player.Ctlcontrols.play();
		}

		public static void Play( this AxWindowsMediaPlayer player, Product product ) {
			player.Play( LocalStorage.Instance.GetFilePathForProduct( product ) );
		}
	}
}