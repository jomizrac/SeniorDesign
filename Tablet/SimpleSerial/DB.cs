using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace SimpleSerial {

	/// <summary>
	/// This class is responsible for storing the global AmazonDynamoDBClient connection.
	/// </summary>
	internal class DB {

		#region Singleton

		private static DB m_instance;

		public static DB Instance {
			get { return m_instance ?? ( m_instance = new DB() ); }
		}

		#endregion Singleton

		private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();

		private static string tableName = "EventCatalog";

        public DB()
        {
            LogEvent("ExampleID", "ExampleName", 1, 999, "pick up");
        }

		//Creates a new item in the database.
		public static void LogEvent( string currentProductID, string currentProductName, int currentProdLocation, string eventType ) {
            Console.WriteLine("\n*** Executing LogEvent() ***");
            Table productCatalog = Table.LoadTable(client, tableName);
            var product = new Document();
            var currentShelfMAC =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            var deviceName = Environment.MachineName;
            product["ProductID"] = currentProductID;
            product["ProductName"] = currentProductName;
            product["ProductLocation"] = currentProdLocation;
            product["ShelfMAC"] = currentShelfMAC;
            product["DeviceName"] = deviceName;
            product["Timestamp"] = LocalDate.ToString(new CultureInfo("en-US"));
            product["EventType"] = eventType;

            productCatalog.PutItem(product);
        }

		private static void RetrieveProduct( int currentProductID ) {
            //Console.WriteLine("\n*** Executing RetrieveProduct() ***");
            //Table productCatalog = new Table();
            // Optional configuration.
            //GetItemOperationConfig config = new GetItemOperationConfig
            //{
            //    AttributesToGet = new List<string> { "ProductID", "ProductName", "ProductLocation", "ShelfMAC", "DeviceName", "Timestamp", "EventType" },
            //    ConsistentRead = true
            //};
            //Document document = productCatalog.GetItem(currentProductID, config);
            //Console.WriteLine("Retrieveproduct: Printing product retrieved...");
            //PrintDocument(document);
        }

		private static void AddShelf( List<Product> productList ) {
			//			Table shelfList = new Table();
			//			List<string> nameList = new List<Product>();
			//			foreach ( int element in productList ) {
			//				nameList.Add( productList[element].name );
			//			}
			//			var shelf = new Document();
			//			var currentShelfMAC =
			//			(
			//				from nic in NetworkInterface.GetAllNetworkInterfaces()
			//				where nic.OperationalStatus == OperationalStatus.Up
			//				select nic.GetPhysicalAddress().ToString()
			//			).FirstOrDefault();
			//			shelf["ShelfMAC"] = currentShelfMAC;
			//			shelf["ProductList"] = nameList;
			//			shelfList.PutItem( shelf );
		}
	}
}