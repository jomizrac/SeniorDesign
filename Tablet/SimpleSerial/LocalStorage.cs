﻿using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Configuration;
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

		public static string productVideoBucket = ConfigurationManager.AppSettings["productVideoBucket"];
		public static string videoDirectory = ConfigurationManager.AppSettings["videoDirectory"];
		public static string videoFileExtension = ConfigurationManager.AppSettings["videoFileExtension"];

		private static AmazonS3Client client;
		private static TransferUtility fileTransferUtility;

		public LocalStorage() {
			client = new AmazonS3Client( Amazon.RegionEndpoint.USEast1 );
			fileTransferUtility = new TransferUtility( client );
		}

		public void SyncVideos() {
			// Delete any videos for products that are no longer present
			if ( Directory.Exists( videoDirectory ) ) {
				string[] filePaths = Directory.GetFiles( videoDirectory );
				foreach ( string filePath in filePaths ) {
					string fileName = Path.GetFileNameWithoutExtension( filePath );
					bool productPresent = ShelfInventory.Instance.products.Exists( p => p.productID.ToString() == fileName );
					if ( !productPresent ) {
						File.Delete( filePath );
						Console.WriteLine( "Deleted unused " + filePath );
					}
				}
			}

			// Download any missing videos for the current product lineup
			foreach ( var product in ShelfInventory.Instance.products ) {
				string filePath = GetFilePathForProductID( product.productID );
				if ( !File.Exists( filePath ) ) {
					string key = product.productID + videoFileExtension;
					fileTransferUtility.Download( filePath, productVideoBucket, key );
					Console.WriteLine( "Successfully downloaded " + key );
				}
			}
		}

		public string GetFilePathForProductID( int id ) {
			return videoDirectory + id + videoFileExtension;
		}
	}
}