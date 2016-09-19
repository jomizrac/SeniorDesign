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

		// The sample uses the following id PK value to add product item.
		private static int sampleproductId = 555;

		public static void CreateProductItem( Table productCatalog ) {
			Console.WriteLine( "\n*** Executing CreateProductItem() ***" );
			var product = new Document();
            //product["ShelfID"] = currentShelfID;
			product["Id"] = currentProductId;
			product["Timestamp"] = timeStamp;
            product["actionType"] = upOrDown;

            

			productCatalog.PutItem( product );
		}
        private static void Retrieveproduct(Table productCatalog)
        {
            Console.WriteLine("\n*** Executing Retrieveproduct() ***");
            // Optional configuration.
            GetItemOperationConfig config = new GetItemOperationConfig
            {
                AttributesToGet = new List<string> { "Id", "Timestamp", "actionType" },
                ConsistentRead = true
            };
            Document document = productCatalog.GetItem(sampleproductId, config);
            Console.WriteLine("Retrieveproduct: Printing product retrieved...");
            PrintDocument(document);
        }
    }
}