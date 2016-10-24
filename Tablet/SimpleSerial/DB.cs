﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

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

		private static string tableName = "Events";

		public DB() {
			//var currentShelfMAC =
			//(
			//    from nic in NetworkInterface.GetAllNetworkInterfaces()
			//    where nic.OperationalStatus == OperationalStatus.Up
			//    select nic.GetPhysicalAddress().ToString()
			//).FirstOrDefault();
			//tableName = currentShelfMAC;
			//DescribeTableRequest request = new DescribeTableRequest
			//{
			//    TableName = tableName
			//};
			//try
			//{
			//    TableDescription tabledescription = client.DescribeTable(request).Table;
			//} catch(ResourceNotFoundException)
			//{
			//    CreateTable();
			//}
			LogEvent( ShelfInventory.Instance.ProductList()[0], "Pick Up" );
		}

		//Creates a new item in the database.
		public void LogEvent( Product currentProduct, string eventType ) {
			//Util.Log("\n*** Executing LogEvent() ***");
			Table productCatalog = Table.LoadTable( client, tableName );
			var product = new Document();
			var currentShelfMAC =
			(
				from nic in NetworkInterface.GetAllNetworkInterfaces()
				where nic.OperationalStatus == OperationalStatus.Up
				select nic.GetPhysicalAddress().ToString()
			).FirstOrDefault();
			var deviceName = Environment.MachineName;
			product["ProductID"] = currentProduct.productID;
			product["ProductName"] = currentProduct.name;
			product["ProductLocation"] = currentProduct.slotID;
			product["ShelfMAC"] = currentShelfMAC;
			product["DeviceName"] = deviceName;
			product["Timestamp"] = DateTime.Now.ToString( new CultureInfo( "en-US" ) );
			product["EventType"] = eventType;

			productCatalog.PutItem( product );
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
				TableName = tableName
			};

			var response = client.CreateTable( request );

			var tableDescription = response.TableDescription;
			WaitTillTableCreated( client, tableName, response );
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

		private static void WaitTillTableCreated( AmazonDynamoDBClient client, string tableName,
													CreateTableResponse response ) {
			var tableDescription = response.TableDescription;

			string status = tableDescription.TableStatus;

			Util.Log( tableName + " - " + status );

			// Let us wait until table is created. Call DescribeTable.
			while ( status != "ACTIVE" ) {
				System.Threading.Thread.Sleep( 5000 ); // Wait 5 seconds.
				try {
					var res = client.DescribeTable( new DescribeTableRequest {
						TableName = tableName
					} );
					Util.Log( "Table name: {0}, status: {1}", res.Table.TableName,
																	  res.Table.TableStatus );
					status = res.Table.TableStatus;
				}
				// Try-catch to handle potential eventual-consistency issue.
				catch ( ResourceNotFoundException ) { }
			}
		}
	}
}