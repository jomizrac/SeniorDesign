using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Configuration;
using System.IO;
using System.Threading;

namespace SimpleSerial {

	/// <summary>
	/// This class is responsible for syncing the device's local storage contents with objects from the cloud.
	/// </summary>
	internal class LocalStorage {

		#region Singleton

		private static object LOCK = new object();

		private static LocalStorage m_instance;

		public static LocalStorage Instance {
			get {
				lock ( LOCK ) {
					return m_instance ?? ( m_instance = new LocalStorage() );
				}
			}
		}

		#endregion Singleton

		private string productVideoBucket = ConfigurationManager.AppSettings["productVideoBucket"];
		private string videoDirectory = ConfigurationManager.AppSettings["videoDirectory"];
		private string videoFileExtension = ConfigurationManager.AppSettings["videoFileExtension"];

		private AmazonS3Client client;
		private TransferUtility fileTransferUtility;

		private LocalStorage() {
			client = new AmazonS3Client( Amazon.RegionEndpoint.USEast1 );
			fileTransferUtility = new TransferUtility( client );
			Util.LogSuccess( "Connected to S3" );

			new Thread( () => Initialize() ).Start();
		}

		public void SyncVideos() {
			if ( !Directory.Exists( videoDirectory ) ) {
				Directory.CreateDirectory( videoDirectory );
				Util.LogWarning( "Created new video directory: " + videoDirectory );
			}

			DeleteUnusedVideos();
			DownloadMissingOrOutdatedVideos();

			Util.LogSuccess( "Video cloud sync complete" );
		}

		public string GetFilePathForProduct( Product product ) {
			return GetFilePathForProduct( product.productID );
		}

		public string GetFilePathForProduct( string productID ) {
			return videoDirectory + productID + videoFileExtension;
		}

		private void DeleteUnusedVideos() {
			string[] filePaths = Directory.GetFiles( videoDirectory );
			foreach ( string filePath in filePaths ) {
				string fileName = Path.GetFileNameWithoutExtension( filePath );
				bool productPresent = ShelfInventory.Instance.ProductList().Exists( p => p.productID == fileName );
				if ( !productPresent ) {
					File.Delete( filePath );
					Util.LogWarning( "Deleted unused product video: " + filePath );
				}
			}
		}

		private void DownloadMissingOrOutdatedVideos() {
			foreach ( Product product in ShelfInventory.Instance.ProductList() ) {
				string filePath = GetFilePathForProduct( product );
				string key = product.productID + videoFileExtension;
				if ( !File.Exists( filePath ) || IsOutdated( filePath, key ) ) {
					try {
						Util.LogSuccess( "Downloading " + key + "... ", false );
						fileTransferUtility.Download( filePath, productVideoBucket, key );
						Util.LogSuccess( "Download complete" );
					}
					catch ( Exception ) {
						Util.LogError( "Unable to retrieve object with key: " + key );
					}
				}
			}
		}

		private bool IsOutdated( string filePath, string key ) {
			var localTime = File.GetLastWriteTime( filePath );
			var localSize = new FileInfo( filePath ).Length;
			try {
				var metadata = client.GetObjectMetadata( productVideoBucket, key );
				var remoteTime = metadata.LastModified.ToLocalTime();
				var remoteSize = metadata.ContentLength;
				return localTime < remoteTime || localSize != remoteSize;
			}
			catch ( Exception ) {
				Util.LogError( "Unable to retrieve object with key: " + key );
				return false;
			}
		}

		private void Initialize() {
			ShelfInventory.Instance.SlotUpdatedEvent -= Instance.OnSlotUpdated;
			ShelfInventory.Instance.SlotUpdatedEvent += Instance.OnSlotUpdated;
		}

		private void OnSlotUpdated( int slotIdx, Product oldProduct, Product newProduct ) {
			Instance.SyncVideos();
		}
	}
}