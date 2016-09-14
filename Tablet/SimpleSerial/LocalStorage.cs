using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

		private static string bucketName = "*** Provide bucket name ***";
		private static string keyName = "*** Provide your object key ***";
		private static string filePath = "*** Provide file name ***";

		private static AmazonS3Client client;
		private static TransferUtility fileTransferUtility;

		private static string tmpDir = "C:\tmp";

		private static string fileExtension = ".wmv";

		public LocalStorage() {
			client = new AmazonS3Client( Amazon.RegionEndpoint.USEast1 );
			fileTransferUtility = new TransferUtility( client );
		}

		public void SyncVideos() {
			// Delete any videos for products that are no longer present
			if ( Directory.Exists( tmpDir ) ) {
				string[] fileEntries = Directory.GetFiles( tmpDir );
				foreach ( string filePath in fileEntries ) {
					string fileName = Path.GetFileNameWithoutExtension( filePath );
					bool contains = ShelfInventory.Instance.products.Any( p => p.productID.ToString() == fileName );
					if ( !contains ) {
						File.Delete( fileName );
					}
				}
			}

			// Download any missing videos for the current product lineup
			foreach ( var product in ShelfInventory.Instance.products ) {
				string filePath = tmpDir + product + fileExtension;
				if ( !File.Exists( filePath ) ) {
					string key = product + fileExtension;
					fileTransferUtility.Download( filePath, bucketName, key );
				}
			}
		}
	}
}