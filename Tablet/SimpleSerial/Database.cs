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

		private static Database m_instance;

		public static Database Instance {
			get { return m_instance ?? ( m_instance = new Database() ); }
		}

		#endregion Singleton

		private const string EventsTable = "Events";
		private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();

		public Database() {
			//var currentShelfMAC =
			//(
			//    from nic in NetworkInterface.GetAllNetworkInterfaces()
			//    where nic.OperationalStatus == OperationalStatus.Up
			//    select nic.GetPhysicalAddress().ToString()
			//).FirstOrDefault();
			//EventsTable = currentShelfMAC;
			//DescribeTableRequest request = new DescribeTableRequest
			//{
			//    TableName = EventsTable
			//};
			//try
			//{
			//    TableDescription tabledescription = client.DescribeTable(request).Table;
			//} catch(ResourceNotFoundException)
			//{
			//    CreateTable();
			//}

			//			LogEvent( ShelfInventory.Instance.ProductList()[0], "Pick Up" );
		}

		//Creates a new item in the database.
		public void LogEvent( Product currentProduct, string eventType ) {
			//Util.Log("\n*** Executing LogEvent() ***");
			Table eventTable = Table.LoadTable( client, EventsTable );
			var eventDoc = new Document();
			var currentShelfMAC =
			(
				from nic in NetworkInterface.GetAllNetworkInterfaces()
				where nic.OperationalStatus == OperationalStatus.Up
				select nic.GetPhysicalAddress().ToString()
			).FirstOrDefault();
			var deviceName = Environment.MachineName;

			eventDoc["ProductID"] = currentProduct.productID;
			eventDoc["ProductName"] = currentProduct.name;
			eventDoc["ProductLocation"] = currentProduct.slotID;
			eventDoc["ShelfMAC"] = currentShelfMAC;
			eventDoc["DeviceName"] = deviceName;
			eventDoc["Timestamp"] = DateTime.Now.ToString( new CultureInfo( "en-US" ) );
			eventDoc["EventType"] = eventType;

			eventTable.PutItem( eventDoc );
		}

		private static void CreateTable() {
			Util.Log( "\n*** Creating table ***" );
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
				TableName = EventsTable
			};

			var response = client.CreateTable( request );

			var tableDescription = response.TableDescription;
			WaitTilTableCreated( client, EventsTable, response );
		}

		private static void RetrieveProduct( int currentProductID ) {
			//Util.Log("\n*** Executing RetrieveProduct() ***");
			//Table productCatalog = new Table();
			// Optional configuration.
			//GetItemOperationConfig config = new GetItemOperationConfig
			//{
			//    AttributesToGet = new List<string> { "ProductID", "ProductName", "ProductLocation", "ShelfMAC", "DeviceName", "Timestamp", "EventType" },
			//    ConsistentRead = true
			//};
			//Document document = productCatalog.GetItem(currentProductID, config);
			//Util.Log("Retrieveproduct: Printing product retrieved...");
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

		private static void WaitTilTableCreated( AmazonDynamoDBClient client, string tableName, CreateTableResponse response ) {
			var tableDescription = response.TableDescription;

			string status = tableDescription.TableStatus;

			Util.Log( tableName + " - " + status );

			// Let us wait until table is created. Call DescribeTable.
			while ( status != "ACTIVE" ) {
				Thread.Sleep( 5000 ); // Wait 5 seconds.
				try {
					var res = client.DescribeTable( new DescribeTableRequest {
						TableName = tableName
					} );
					Util.Log( "Table name: " + res.Table.TableName + ", status: " + res.Table.TableStatus );
					status = res.Table.TableStatus;
				}
				// Try-catch to handle potential eventual-consistency issue.
				catch ( ResourceNotFoundException ) { }
			}
		}
	}
}