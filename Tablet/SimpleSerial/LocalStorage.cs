using Amazon.S3;
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

		private static string existingBucketName = "*** Provide bucket name ***";
		private static string keyName = "*** Provide your object key ***";
		private static string filePath = "*** Provide file name ***";

		public void SyncVideos() {
			// Download all missing videos (detect if files have diff modified date?)
			// Delete unused local videos
		}

		private static void Main( string[] args ) {
			try {
				TransferUtility fileTransferUtility = new TransferUtility( new AmazonS3Client( Amazon.RegionEndpoint.USEast1 ) );

				// 1. Upload a file, file name is used as the object key name.
				fileTransferUtility.Upload( filePath, existingBucketName );
				Console.WriteLine( "Upload 1 completed" );

				// 2. Specify object key name explicitly.
				fileTransferUtility.Upload( filePath, existingBucketName, keyName );
				Console.WriteLine( "Upload 2 completed" );

				// 3. Upload data from a type of System.IO.Stream.
				using ( FileStream fileToUpload = new FileStream( filePath, FileMode.Open, FileAccess.Read ) ) {
					fileTransferUtility.Upload( fileToUpload, existingBucketName, keyName );
				}
				Console.WriteLine( "Upload 3 completed" );

				// 4.Specify advanced settings/options.
				TransferUtilityUploadRequest fileTransferUtilityRequest = new TransferUtilityUploadRequest {
					BucketName = existingBucketName,
					FilePath = filePath,
					StorageClass = S3StorageClass.ReducedRedundancy,
					PartSize = 6291456, // 6 MB.
					Key = keyName,
					CannedACL = S3CannedACL.PublicRead
				};
				fileTransferUtilityRequest.Metadata.Add( "param1", "Value1" );
				fileTransferUtilityRequest.Metadata.Add( "param2", "Value2" );
				fileTransferUtility.Upload( fileTransferUtilityRequest );
				Console.WriteLine( "Upload 4 completed" );
			}
			catch ( AmazonS3Exception s3Exception ) {
				Console.WriteLine( s3Exception.Message, s3Exception.InnerException );
			}
		}
	}
}