using AxWMPLib;

namespace SimpleSerial {

	/// <summary>
	/// Extension methods are used to add new behavior to existing classes.
	/// </summary>
	internal static class ExtensionMethods {

		public static void Play( this AxWindowsMediaPlayer player, string filePath ) {
			Util.LogSuccess( "Playing video: " + filePath );
			player.URL = filePath;
			player.Ctlcontrols.stop();
			player.Ctlcontrols.play();
		}

		public static void Play( this AxWindowsMediaPlayer player, Product product ) {
			player.Play( LocalStorage.Instance.GetFilePathForProduct( product ) );
		}
	}
}