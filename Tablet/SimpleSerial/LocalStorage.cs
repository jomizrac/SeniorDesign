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

		private static LocalStorage m_instance;

		public static LocalStorage Instance {
			get { return m_instance ?? ( m_instance = new LocalStorage() ); }
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

			new Thread( () => Initialize() ).Start();
		}

		public void SyncVideos() {
			if ( !Directory.Exists( videoDirectory ) ) {
				Directory.CreateDirectory( videoDirectory );
				Util.Log( "Created new video directory: " + videoDirectory );
			}

			DeleteUnneededVideos();
			DownloadMissingOrOutdatedVideos();

			Util.Log( "Video cloud sync complete" );
		}

		public string GetFilePathForProduct( Product product ) {
			return GetFilePathForProduct( product.productID );
		}

		public string GetFilePathForProduct( string productID ) {
			return videoDirectory + productID + videoFileExtension;
		}

		private void DeleteUnneededVideos() {
			string[] filePaths = Directory.GetFiles( videoDirectory );
			foreach ( string filePath in filePaths ) {
				string fileName = Path.GetFileNameWithoutExtension( filePath );
				bool productPresent = ShelfInventory.Instance.ProductList().Exists( p => p.productID == fileName );
				if ( !productPresent ) {
					File.Delete( filePath );
					Util.Log( "Deleted unused " + filePath );
				}
			}
		}

		private void DownloadMissingOrOutdatedVideos() {
			foreach ( Product product in ShelfInventory.Instance.ProductList() ) {
				string filePath = GetFilePathForProduct( product );
				string key = product.productID + videoFileExtension;
				if ( !File.Exists( filePath ) || IsOutdated( filePath, key ) ) {
					try {
						Util.Log( "Downloading " + key + "... ", false );
						fileTransferUtility.Download( filePath, productVideoBucket, key );
						Util.Log( "Download complete" );
					}
					catch ( Exception ) {
						Util.Log( "Error retrieving object with key: " + key );
					}
				}
			}
		}

		private bool IsOutdated( string filePath, string key ) {
			DateTime local = File.GetLastWriteTime( filePath );
			GetObjectMetadataResponse metadata = client.GetObjectMetadata( productVideoBucket, key );
			DateTime remote = metadata.LastModified;
			return local < remote;
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