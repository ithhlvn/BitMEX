﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Bitmex.Client.Websocket.Client;
using Bitmex.Client.Websocket.Requests;
using Bitmex.Client.Websocket.Websockets;
using Serilog;
using BitMEX.Model;
using OpenAPI.Api;
using OpenAPI.Client;
using OpenAPI.Model;
using System.Data.SqlClient;
using Bitmex.Client.Websocket;

namespace BitMEX.TestForm
{
    public partial class Form1 : Form
    {
        private MordoR mconn;

        // Required for WebSocket
        private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);
        private static readonly string API_KEY = "rTAFXRKn2dLARuG_t1dDOtgI";
        private static readonly string API_SECRET = "K2LmL6aTbj8eW_LVj7OLa7iA6eZa8TJMllh3sjCynV4fpnMr";

        public Form1()
        {
            InitializeComponent();
            mconn = new MordoR();
            TBMarketOrder.Text = "XBTUSD";
        }

        private void btnMarketOrder_Click(object sender, EventArgs e)
        {
            // Local environment checks...
            if (NUDMarketOrderQuantity.Value >= 1 || NUDMarketOrderQuantity.Value <= -1)
            {
                // Catch API and connection errors
                try
                {
                    string clOrdID;
                    object outcome = mconn.MarketOrder(
                        TBMarketOrder.Text.ToString(), 
                        out clOrdID, 
                        Decimal.ToInt32((decimal)NUDMarketOrderQuantity.Value));

                    if (outcome != null)
                    {
                        if (outcome.GetType().ToString() == "BitMEX.Model.OrderResponse")
                        {
                            // Successful API call with successful result...
                            MessageBox.Show("Order success: " + clOrdID + "=" + ((OrderResponse)outcome).ClOrdId.ToString());
                        }
                        else
                        {
                            // Successful API call with error as result...
                            MessageBox.Show("Order error: " + ((BaseError)outcome).Error.Message.ToString());
                        }
                    }
                    else 
                    {
                        MessageBox.Show("Outcome is null");
                    }
                }
                catch (Exception exc)
                {
                    // Catch all external exceptions like connection issues etc.
                    MessageBox.Show("Exception[" + exc.Message.ToString() + "]");
                }
            }
        }
        
        private void btnLimitOrder_Click(object sender, EventArgs e)
        {
            // Local environment checks...
            if ((NUDMarketOrderQuantity.Value >= 1 || NUDMarketOrderQuantity.Value <= -1))
            {
                // Catch API and connection errors
                try
                {
                    OrderResponse orderResp = new OrderResponse();
                    BaseError orderErr = new BaseError();
                    string clOrdID;
                    object outcome = mconn.LimitOrder(
                        TBMarketOrder.Text.ToString(), 
                        Decimal.ToInt32((decimal)NUDMarketOrderQuantity.Value), 
                        Decimal.ToInt32((decimal)NUDPrice.Value), 
                        out clOrdID);

                    if (outcome.GetType() == orderResp.GetType())
                    {
                        // Successful API call with successful result...
                        orderResp = (OrderResponse)outcome;
                        MessageBox.Show("Order success: " + clOrdID + "=" + orderResp.ClOrdId.ToString());
                    }
                    else if (outcome.GetType() == orderErr.GetType())
                    {
                        // Successful API call with error as result...
                        orderErr = (BaseError)outcome;
                        MessageBox.Show("BitMEX API Error [" + orderErr.Error.Message.ToString() + "]");
                    }
                    else
                    {
                        // Should never happen...
                        MessageBox.Show("Unknown return type [" + outcome.GetType().ToString() + "]");
                    }
                }
                catch (Exception exc)
                {
                    // Catch all external exceptions like connection issues etc.
                    MessageBox.Show("Exception[" + exc.Message.ToString() + "]");
                }
            }
        }

        private void btnGetOrders_Click(object sender, EventArgs e)
        {
            try
            {
                List<OrderResponse> orderResp = new List<OrderResponse>();
                BaseError orderErr = new BaseError();
                object outcome = mconn.GetOpenOrdersForSymbol(TBMarketOrder.Text.ToString());

                if(outcome.GetType() == orderResp.GetType())
                {
                    // Successful API call with successful result...
                    string orderAccumulation = "";
                    orderResp = (List<OrderResponse>)outcome;
                    foreach(var resp in orderResp.Where(x=> x.OrdStatus == "New").Select(n => n.OrderId))
                    {
                        orderAccumulation = orderAccumulation + "¦" + resp.ToString();
                    }
                    MessageBox.Show(orderAccumulation);
                }
                else if(outcome.GetType() == orderErr.GetType())
                {
                    // Successful API call with error as result...
                    orderErr = (BaseError)outcome;
                    MessageBox.Show("BitMEX API Error [" + orderErr.Error.Message.ToString() + "]");
                }
                else
                {
                    // Should never happen...
                    MessageBox.Show("Unknown return type [" + outcome.GetType().ToString() + "]");
                }
            }
            catch (Exception exc)
            {
                // Unsuccessful API call
                MessageBox.Show("Exception [" + exc.Message.ToString() + "]");
            }
        }

        private void btnTestMe_ClickAsync(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            var url = BitmexValues.ApiWebsocketUrl;
            using (var communicator = new BitmexWebsocketCommunicator(url))
            {
                communicator.Name = "Bitmex-1";
                communicator.ReconnectTimeoutMs = (int)TimeSpan.FromMinutes(10).TotalMilliseconds;
                communicator.ReconnectionHappened.Subscribe(type =>
                    Log.Information($"Reconnection happened, type: {type}")
                );

                using (var client = new BitmexWebsocketClient(communicator))
                {

                    client.Streams.InfoStream.Subscribe(info =>
                    {
                        Log.Information($"Reconnection happened, Message: {info.Info}, Version: {info.Version:D}");
                        SendSubscriptionRequests(client).Wait();
                    });

                    SubscribeToStreams(client);

                    communicator.Start();

                    ExitEvent.WaitOne();
                }
            }
        }

        #region Bitmex.Websocket

        private static async Task SendSubscriptionRequests(BitmexWebsocketClient client)
        {
            await client.Send(new PingRequest());
            //await client.Send(new BookSubscribeRequest("XBTUSD"));
            await client.Send(new TradesSubscribeRequest("XBTUSD"));
            //await client.Send(new TradeBinSubscribeRequest("1m", "XBTUSD"));
            //await client.Send(new TradeBinSubscribeRequest("5m", "XBTUSD"));
            //await client.Send(new QuoteSubscribeRequest("XBTUSD"));
            //await client.Send(new LiquidationSubscribeRequest());
            //await client.Send(new InstrumentSubscribeRequest("XBTUSD"));

            if (!string.IsNullOrWhiteSpace(API_SECRET))
                await client.Send(new AuthenticationRequest(API_KEY, API_SECRET));
        }

        private static void SubscribeToStreams(BitmexWebsocketClient client)
        {
            client.Streams.ErrorStream.Subscribe(x =>
                Log.Warning($"Error received, message: {x.Error}, status: {x.Status}")
            );

            client.Streams.AuthenticationStream.Subscribe(x =>
            {
                // Log.Information($"Authentication happened, success: {x.Success}");
                //client.Send(new WalletSubscribeRequest()).Wait();
                client.Send(new OrderSubscribeRequest()).Wait();
                client.Send(new PositionSubscribeRequest()).Wait();
            });
            
            //client.Streams.SubscribeStream.Subscribe(x =>
            //{
            //    Log.Information(x.IsSubscription
            //        ? $"Subscribed ({x.Success}) to {x.Subscribe}"
            //        : $"Unsubscribed ({x.Success}) from {x.Unsubscribe}");
            //});

            //client.Streams.PongStream.Subscribe(x =>
            //    Log.Information($"Pong received ({x.Message})"));


            //client.Streams.WalletStream.Subscribe(y =>
            //    y.Data.ToList().ForEach(x =>
            //        Log.Information($"Wallet {x.Account}, {x.Currency} amount: {x.BalanceBtc}"))
            //);

            client.Streams.OrderStream.Subscribe(y =>
                y.Data.ToList().ForEach(x =>
                    Log.Information(
                        $"Order {x.Symbol} updated. Time: {x.Timestamp:HH:mm:ss.fff}, Amount: {x.OrderQty}, " +
                        $"Price: {x.Price}, Direction: {x.Side}, Working: {x.WorkingIndicator}, Status: {x.OrdStatus}"))
            );

            client.Streams.PositionStream.Subscribe(y =>
                y.Data.ToList().ForEach(x =>
                    Log.Information(
                        $"Position {x.Symbol}, {x.Currency} updated. Time: {x.Timestamp:HH:mm:ss.fff}, Amount: {x.CurrentQty}, " +
                        $"Price: {x.LastPrice}, PNL: {x.UnrealisedPnl}"))
            );

            //client.Streams.TradesStream.Subscribe(y =>
            //    y.Data.ToList().ForEach(x =>
            //        Log.Information($"Trade {x.Symbol} executed. Time: {x.Timestamp:mm:ss.fff}, [{x.Side}] Amount: {x.Size}, " +
            //                        $"Price: {x.Price}"))
            //);

            //client.Streams.BookStream.Subscribe(book =>
            //    book.Data.Take(100).ToList().ForEach(x => Log.Information(
            //        $"Book | {book.Action} pair: {x.Symbol}, price: {x.Price}, amount {x.Size}, side: {x.Side}"))
            //);

            //client.Streams.QuoteStream.Subscribe(y =>
            //    y.Data.ToList().ForEach(x =>
            //        Log.Information($"Quote {x.Symbol}. Bid: {x.BidPrice} - {x.BidSize} Ask: {x.AskPrice} - {x.AskSize}"))
            //);

            //client.Streams.LiquidationStream.Subscribe(y =>
            //    y.Data.ToList().ForEach(x =>
            //        Log.Information(
            //            $"Liquadation Action: {y.Action}, OrderID: {x.OrderID}, Symbol: {x.Symbol}, Side: {x.Side}, Price: {x.Price}, LeavesQty: {x.leavesQty}"))
            //);

            //client.Streams.TradeBinStream.Subscribe(y =>
            //    y.Data.ToList().ForEach(x =>
            //    Log.Information($"TradeBin table:{y.Table} {x.Symbol} executed. Time: {x.Timestamp:mm:ss.fff}, Open: {x.Open}, " +
            //            $"Close: {x.Close}, Volume: {x.Volume}, Trades: {x.Trades}"))
            //);

            //client.Streams.InstrumentStream.Subscribe(x =>
            //{
            //    x.Data.ToList().ForEach(y =>
            //    {
            //        Log.Information($"Instrument, {y.Symbol}, " +
            //                        $"price: {y.MarkPrice}, last: {y.LastPrice}, " +
            //                        $"mark: {y.MarkMethod}, fair: {y.FairMethod}, direction: {y.LastTickDirection}, " +
            //                        $"funding: {y.FundingRate} i: {y.IndicativeFundingRate} s: {y.FundingQuoteSymbol}");
            //    });
            //});


            // example of unsubscribe requests
            //Task.Run(async () =>
            //{
            //    await Task.Delay(5000);
            //    await client.Send(new BookSubscribeRequest("XBTUSD") {IsUnsubscribe = true});
            //    await Task.Delay(5000);
            //    await client.Send(new TradesSubscribeRequest() {IsUnsubscribe = true});
            //});
        }

        private static void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
        {
            Log.Warning("Exiting process");
            ExitEvent.Set();
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Log.Warning("Canceling process");
            e.Cancel = true;
            ExitEvent.Set();
        }

        #endregion Bitmex.Websocket

        private void DBLogOperation(string operation, object obj)
        {
            try
            {
                //// Build connection string
                //SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                //builder.DataSource = "localhost";   // update me
                //builder.UserID = "sa";              // update me
                //builder.Password = "your_password";      // update me
                //builder.InitialCatalog = "master";

                string connectionString = @"Data Source=.\MYSQLSDB;Initial Catalog=Trading;Integrated Security=true;";

                // Connect to SQL
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Connected !

                    //String sql = "DROP DATABASE IF EXISTS [SampleDB]; CREATE DATABASE [SampleDB]";
                    //using (SqlCommand command = new SqlCommand(sql, connection))
                    //{
                    //    command.ExecuteNonQuery();
                    //    Console.WriteLine("Done.");
                    //}

                    // Create a Table and insert some sample data
                    //StringBuilder sb = new StringBuilder();
                    //sb.Append("USE SampleDB; ");
                    //sb.Append("CREATE TABLE Employees ( ");
                    //sb.Append(" Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY, ");
                    //sb.Append(" Name NVARCHAR(50), ");
                    //sb.Append(" Location NVARCHAR(50) ");
                    //sb.Append("); ");
                    //sb.Append("INSERT INTO Employees (Name, Location) VALUES ");
                    //sb.Append("(N'Jared', N'Australia'), ");
                    //sb.Append("(N'Nikita', N'India'), ");
                    //sb.Append("(N'Tom', N'Germany'); ");
                    //sql = sb.ToString();
                    //using (SqlCommand command = new SqlCommand(sql, connection))
                    //{
                    //    command.ExecuteNonQuery();
                    //    Console.WriteLine("Done.");
                    //}

                    //// INSERT demo
                    //Console.Write("Inserting a new row into table, press any key to continue...");
                    //Console.ReadKey(true);
                    //sb.Clear();
                    //sb.Append("INSERT Employees (Name, Location) ");
                    //sb.Append("VALUES (@name, @location);");
                    //sql = sb.ToString();
                    //using (SqlCommand command = new SqlCommand(sql, connection))
                    //{
                    //    command.Parameters.AddWithValue("@name", "Jake");
                    //    command.Parameters.AddWithValue("@location", "United States");
                    //    int rowsAffected = command.ExecuteNonQuery();
                    //    Console.WriteLine(rowsAffected + " row(s) inserted");
                    //}

                    //// UPDATE demo
                    //String userToUpdate = "Nikita";
                    //Console.Write("Updating 'Location' for user '" + userToUpdate + "', press any key to continue...");
                    //Console.ReadKey(true);
                    //sb.Clear();
                    //sb.Append("UPDATE Employees SET Location = N'United States' WHERE Name = @name");
                    //sql = sb.ToString();
                    //using (SqlCommand command = new SqlCommand(sql, connection))
                    //{
                    //    command.Parameters.AddWithValue("@name", userToUpdate);
                    //    int rowsAffected = command.ExecuteNonQuery();
                    //    Console.WriteLine(rowsAffected + " row(s) updated");
                    //}

                    //// DELETE demo
                    //String userToDelete = "Jared";
                    //Console.Write("Deleting user '" + userToDelete + "', press any key to continue...");
                    //Console.ReadKey(true);
                    //sb.Clear();
                    //sb.Append("DELETE FROM Employees WHERE Name = @name;");
                    //sql = sb.ToString();
                    //using (SqlCommand command = new SqlCommand(sql, connection))
                    //{
                    //    command.Parameters.AddWithValue("@name", userToDelete);
                    //    int rowsAffected = command.ExecuteNonQuery();
                    //    Console.WriteLine(rowsAffected + " row(s) deleted");
                    //}

                    //// READ demo
                    //Console.WriteLine("Reading data from table, press any key to continue...");
                    //Console.ReadKey(true);
                    //sql = "SELECT Id, Name, Location FROM Employees;";
                    //using (SqlCommand command = new SqlCommand(sql, connection))
                    //{

                    //    using (SqlDataReader reader = command.ExecuteReader())
                    //    {
                    //        while (reader.Read())
                    //        {
                    //            Console.WriteLine("{0} {1} {2}", reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
                    //        }
                    //    }
                    //}
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("All done. Press any key to finish...");
            Console.ReadKey(true);
        }

        #region HELPERS

        private static long ToUnixTimeSeconds(DateTimeOffset dateTimeOffset)
        {
            var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var unixTimeStampInTicks = (dateTimeOffset.ToUniversalTime() - unixStart).Ticks;
            return unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }

        private long GetExpires()
        {
            return ToUnixTimeSeconds(DateTimeOffset.UtcNow) + 3600; // set expires one hour in the future
        }

        #endregion HELPERS

        private void btnTest_Click(object sender, EventArgs e)
        {
            // Examples: https://github.com/BitMEX/api-connectors/blob/master/auto-generated/csharp/docs/OrderApi.md#ordernew

            // Configure API key authorization: apiKey
            Configuration.Default.AddApiKey("api-key", "rTAFXRKn2dLARuG_t1dDOtgI");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("api-key", "Bearer");
            // Configure API key authorization: apiNonce
            //Configuration.Default.AddApiKey("api-nonce", "rTAFXRKn2dLARuG_t1dDOtgI");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("api-nonce", "Bearer");
            // Configure API key authorization: apiSignature
            Configuration.Default.AddApiKey("api-signature", "K2LmL6aTbj8eW_LVj7OLa7iA6eZa8TJMllh3sjCynV4fpnMr");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("api-signature", "Bearer");
            // Configure API key authorization: apiExpires
            Configuration.Default.AddDefaultHeader("api-expires", GetExpires().ToString());
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.ApiKeyPrefix.Add("api-expires", "Bearer");

            var apiInstance = new OrderApi();
            var symbol = TBMarketOrder.Text.ToString();  // string | Instrument symbol. e.g. 'XBTUSD'.
            var side = (Decimal.ToInt32((decimal)NUDMarketOrderQuantity.Value) >= 0) ? "Buy" : "Sell";  // string | Order side. Valid options: Buy, Sell. Defaults to 'Buy' unless `orderQty` or `simpleOrderQty` is negative. (optional) 
            //var simpleOrderQty = 0;  // double? | Order quantity in units of the underlying instrument (i.e. Bitcoin). (optional,Deprecated) 
            var orderQty = Decimal.ToInt32((decimal)NUDMarketOrderQuantity.Value);  // decimal? | Order quantity in units of the instrument (i.e. contracts). (optional) 
            //var price = 1.2;  // double? | Optional limit price for 'Limit', 'StopLimit', and 'LimitIfTouched' orders. (optional) 
            //var displayQty = 8.14;  // decimal? | Optional quantity to display in the book. Use 0 for a fully hidden order. (optional) 
            //var stopPx = 1.2;  // double? | Optional trigger price for 'Stop', 'StopLimit', 'MarketIfTouched', and 'LimitIfTouched' orders. Use a price below the current price for stop-sell orders and buy-if-touched orders. Use `execInst` of 'MarkPrice' or 'LastPrice' to define the current price used for triggering. (optional) 
            var clOrdID = "123456789";  // string | Optional Client Order ID. This clOrdID will come back on the order and any related executions. (optional) 
            //var clOrdLinkID = clOrdLinkID_example;  // string | Optional Client Order Link ID for contingent orders. (optional,Deprecated) 
            //var pegOffsetValue = 1.2;  // double? | Optional trailing offset from the current price for 'Stop', 'StopLimit', 'MarketIfTouched', and 'LimitIfTouched' orders; use a negative offset for stop-sell orders and buy-if-touched orders. Optional offset from the peg price for 'Pegged' orders. (optional) 
            //var pegPriceType = pegPriceType_example;  // string | Optional peg price type. Valid options: LastPeg, MidPricePeg, MarketPeg, PrimaryPeg, TrailingStopPeg. (optional) 
            var ordType = "Market";  // string | Order type. Valid options: Market, Limit, Stop, StopLimit, MarketIfTouched, LimitIfTouched, MarketWithLeftOverAsLimit, Pegged. Defaults to 'Limit' when `price` is specified. Defaults to 'Stop' when `stopPx` is specified. Defaults to 'StopLimit' when `price` and `stopPx` are specified. (optional)  (default to Limit)
            //var timeInForce = timeInForce_example;  // string | Time in force. Valid options: Day, GoodTillCancel, ImmediateOrCancel, FillOrKill. Defaults to 'GoodTillCancel' for 'Limit', 'StopLimit', 'LimitIfTouched', and 'MarketWithLeftOverAsLimit' orders. (optional) 
            //var execInst = execInst_example;  // string | Optional execution instructions. Valid options: ParticipateDoNotInitiate, AllOrNone, MarkPrice, IndexPrice, LastPrice, Close, ReduceOnly, Fixed. 'AllOrNone' instruction requires `displayQty` to be 0. 'MarkPrice', 'IndexPrice' or 'LastPrice' instruction valid for 'Stop', 'StopLimit', 'MarketIfTouched', and 'LimitIfTouched' orders. (optional) 
            //var contingencyType = contingencyType_example;  // string | Optional contingency type for use with `clOrdLinkID`. Valid options: OneCancelsTheOther, OneTriggersTheOther, OneUpdatesTheOtherAbsolute, OneUpdatesTheOtherProportional. (optional,Deprecated) 
            //var text = text_example;  // string | Optional order annotation. e.g. 'Take profit'. (optional) 

            try
            {
                // Create a new order.
                Order result = apiInstance.OrderNew(symbol, side, null, orderQty, null, null, null, clOrdID, null, null, null, ordType, null, null, null, null);
                MessageBox.Show(result.ClOrdID.ToString());
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception when calling OrderApi.OrderNew: " + exc.Message);
            }
        }

        
    }
}
