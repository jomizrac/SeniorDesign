using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;

namespace SimpleSerial {

	/// <summary>
	/// This class is responsible for storing the global AmazonDynamoDBClient connection.
	/// </summary>
	internal class Database {

		#region Singleton

		private static object LOCK = new object();

		private static Database m_instance;

		public static Database Instance {
			get {
				lock ( LOCK ) {
					return m_instance ?? ( m_instance = new Database() );
				}
			}
		}

		#endregion Singleton

		private const string EventsTableName = "Events";
		private const string ShelvesTableName = "Shelves";
		private const string ProductsTableName = "ProductCatalog";
		private static AmazonDynamoDBClient client;
		private static string shelfMAC;

		private Database() {
			shelfMAC = ( from nic in NetworkInterface.GetAllNetworkInterfaces()
						 where nic.OperationalStatus == OperationalStatus.Up
						 select nic.GetPhysicalAddress().ToString()
						 ).FirstOrDefault();
			Util.LogSuccess( "Detected system MAC address: " + shelfMAC );

			client = new AmazonDynamoDBClient();
			Util.LogSuccess( "Connected to DynamoDB" );

			//    DescribeTableRequest request = new DescribeTableRequest
			//     {
			//        TableName = ShelfTableName
			//                    };
			//                try
			//     {
			//        TableDescription tabledescription = client.DescribeTable(request).Table;
			//                    } catch (ResourceNotFoundException e)
			//     {
			//        CreateTable();
			//                    }
			//    }
		}

		//Creates a new item in the database.
		public void LogEvent( Product product, string eventType ) {
			try {
				var eventsTable = Table.LoadTable( client, EventsTableName );
				var eventDoc = new Document();
				eventDoc["ProductID"] = product.productID;
				eventDoc["ProductName"] = product.name;
				eventDoc["ProductLocation"] = product.slotID;
				eventDoc["ShelfMAC"] = shelfMAC;
				eventDoc["DeviceName"] = Environment.MachineName;
				eventDoc["Timestamp"] = DateTime.Now.Ticks;
				eventDoc["DateTime"] = DateTime.Now.ToString();
				eventDoc["EventType"] = eventType;
				eventsTable.BeginPutItem( eventDoc, PutItemCallback, eventDoc );
			}
			catch ( Exception ) {
				Util.LogError( "Failed to BeginPutItem \"" + eventType + "\" event for: " + product.name );
			}
		}

		public void UpdateShelfInventory( List<Product> products ) {
			List<string> productStrings = new List<string>();
			foreach ( var product in products ) {
				productStrings.Add( product.productID );
			}

			var shelvesTable = Table.LoadTable( client, ShelvesTableName );
			var shelfDoc = new Document();
			shelfDoc["ShelfMAC"] = shelfMAC;
			shelfDoc["Products"] = productStrings;
			shelvesTable.PutItem( shelfDoc );
		}

		public List<string> getProductList() {
			var shelvesTable = Table.LoadTable( client, ShelvesTableName );
			var doc = shelvesTable.GetItem( shelfMAC );
			List<string> products = doc["Products"].AsListOfString();
			return products;
		}

		public string getProductName( string currentProductID ) {
			var productsTable = Table.LoadTable( client, ProductsTableName );
			var doc = productsTable.GetItem( currentProductID );
			return doc["Name"];
		}

		private static void PutItemCallback( IAsyncResult asyncResult ) {
			var eventDoc = asyncResult.AsyncState as Document;
			try {
				var eventsTable = Table.LoadTable( client, EventsTableName );
				eventsTable.EndPutItem( asyncResult );
				Util.LogSuccess( "Logged \"" + eventDoc["EventType"] + "\" event for: " + eventDoc["ProductName"] );
			}
			catch ( Exception ) {
				Util.LogError( "Failed to EndPutItem \"" + eventDoc["EventType"] + "\" event for: " + eventDoc["ProductName"] );
			}
		}

		private static void CreateTable() {
			Util.LogInfo( "\n*** Creating table ***" );
			var request = new CreateTableRequest {
				AttributeDefinitions = new List<AttributeDefinition>()
		{
		  new AttributeDefinition
		  {
			AttributeName = "ProductID",
			AttributeType = "N"
		  },
		  new AttributeDefinition
		  {
			AttributeName = "Timestamp",
			AttributeType = "N"
		  }
		},
				KeySchema = new List<KeySchemaElement>
		{
		  new KeySchemaElement
		  {
			AttributeName = "ProductID",
			KeyType = "HASH"  //Partition key
		  },
		  new KeySchemaElement
		  {
			AttributeName = "Timestamp",
			KeyType = "RANGE"  //Sort key
		  }
		},
				ProvisionedThroughput = new ProvisionedThroughput {
					ReadCapacityUnits = 5,
					WriteCapacityUnits = 5
				},
				TableName = EventsTableName
			};

			var response = client.CreateTable( request );
			var tableDescription = response.TableDescription;
			WaitTilTableCreated( EventsTableName, response );
		}

		private static void AddShelf( List<Product> productList ) {
			//			Table shelfList = new Table();
			//			List<string> nameList = new List<Product>();
			//			foreach ( int element in productList ) {
			//				nameList.Add( productList[element].name );
			//			}
			//			var shelf = new Document();
			//			shelf["ShelfMAC"] = shelfMAC;
			//			shelf["ProductList"] = nameList;
			//			shelfList.PutItem( shelf );
		}

		private static void WaitTilTableCreated( string tableName, CreateTableResponse response ) {
			var tableDescription = response.TableDescription;

			string status = tableDescription.TableStatus;

			Util.LogInfo( tableName + " - " + status );

			// Let us wait until table is created. Call DescribeTable.
			while ( status != "ACTIVE" ) {
				Thread.Sleep( 5000 ); // Wait 5 seconds.
				try {
					var res = client.DescribeTable( new DescribeTableRequest {
						TableName = tableName
					} );
					Util.LogInfo( "Table name: " + res.Table.TableName + ", status: " + res.Table.TableStatus );
					status = res.Table.TableStatus;
				}
				// Try-catch to handle potential eventual-consistency issue.
				catch ( ResourceNotFoundException ) { }
			}
		}
	}
}