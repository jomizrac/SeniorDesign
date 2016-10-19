using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
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
				Console.WriteLine( "Created new video directory: " + videoDirectory );
			}

			// Delete any videos for products that are no longer present
			string[] filePaths = Directory.GetFiles( videoDirectory );
			foreach ( string filePath in filePaths ) {
				string fileName = Path.GetFileNameWithoutExtension( filePath );
				bool productPresent = ShelfInventory.Instance.ProductList().Exists( p => p.productID == fileName );
				if ( !productPresent ) {
					File.Delete( filePath );
					Console.WriteLine( "Deleted unused " + filePath );
				}
			}

			// Download any missing videos for the current product lineup
			foreach ( Product product in ShelfInventory.Instance.ProductList() ) {
				string filePath = GetFilePathForProduct( product );
				if ( !File.Exists( filePath ) ) {
					string key = product.productID + videoFileExtension;
					try {
						Console.Write( "Downloading " + key + "... " );
						fileTransferUtility.Download( filePath, productVideoBucket, key );
						Console.WriteLine( "Download complete" );
					}
					catch ( Exception ) {
						Console.WriteLine( "Error retrieving object with key: " + key );
					}
				}
			}

			Console.WriteLine( "Video cloud sync complete" );
		}

		public string GetFilePathForProduct( Product product ) {
			return GetFilePathForProduct( product.productID );
		}

		public string GetFilePathForProduct( string productID ) {
			return videoDirectory + productID + videoFileExtension;
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