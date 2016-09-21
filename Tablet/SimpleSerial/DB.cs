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


		public static void CreateProductItem( Table productCatalog ) {
			Console.WriteLine( "\n*** Executing CreateProductItem() ***" );
			var product = new Document();
			product["ProductId"] = currentProductId;
			product["UpTimestamp"] = upTime;
            product["DownTimestamp"] = downTime;
            product["ShelfRokrID"] = shelfID;
            
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
    }
}