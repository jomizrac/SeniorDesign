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

		// The sample uses the following id PK value to add book item.
		private static int sampleBookId = 555;

		public static void CreateBookItem( Table productCatalog ) {
			Console.WriteLine( "\n*** Executing CreateBookItem() ***" );
			var book = new Document();
			book["Id"] = sampleBookId;
			book["Title"] = "Book " + sampleBookId;
			book["Price"] = 19.99;
			book["ISBN"] = "111-1111111111";
			book["Authors"] = new List<string> { "Author 1", "Author 2", "Author 3" };
			book["PageCount"] = 500;
			book["Dimensions"] = "8.5x11x.5";
			book["InPublication"] = new DynamoDBBool( true );
			book["InStock"] = new DynamoDBBool( false );
			book["QuantityOnHand"] = 0;

			productCatalog.PutItem( book );
		}
	}
}