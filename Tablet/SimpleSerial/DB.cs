using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
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

		private static string tableName = "ProductCatalog";

        //Creates a new item in the database.
		public static void CreateProductItem( Table productCatalog, int currentProdLocation, int currentTime, string eventType ) {
			Console.WriteLine( "\n*** Executing CreateProductItem() ***" );
			var product = new Document();
            var currentShelfMAC =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            product["ProductName"] = currentProductName;
            product["ProductLocation"] = currentProdLocation;
			product["ShelfMAC"] = currentShelfMAC;
            product["DeviceName"] = deviceName;
            product["Timestamp"] = currentTime;
            product["EventType"] = eventType;
            
			productCatalog.PutItem( product );
		}
        private static void RetrieveProduct(Table productCatalog, int currentProductID)
        {
            Console.WriteLine("\n*** Executing RetrieveProduct() ***");
            // Optional configuration.
            GetItemOperationConfig config = new GetItemOperationConfig
            {
                AttributesToGet = new List<string> { "ProductId", "UpTimestamp", "DownTimestamp", "ShelfRokrID" },
                ConsistentRead = true
            };
            Document document = productCatalog.GetItem(currentProductID, config);
            Console.WriteLine("Retrieveproduct: Printing product retrieved...");
            PrintDocument(document);
        }
        private static void AddShelf(Table shelfList, )
    }
}