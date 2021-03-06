﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
//using Serilog;
//using BitMEX.Model;
//using BitMEX.Client;
//using PStrategies.ZoneRecovery;
//using System.Data.SqlClient;
//using log4net;
//using IO.Swagger.Api;
//using IO.Swagger.Client;
//using IO.Swagger.Model;
using Serilog;

using BitMEXRest.Authorization;
using BitMEXRest.Client;
using BitMEXRest.Dto;
using BitMEXRest.Model;

namespace BitMEX.TestForm
{
    public partial class Form1 : Form
    {
        //private MordoR mconn;
        //private MordoR connLong;
        //private MordoR connShort;
        //private Calculator calc;
        //private Dictionary<long, MordoR> Connections;
        //ILog log;
        //string guid;

        //ApiClient ClientA;
        //ApiClient ClientB;

        private List<int> lijst;
        private TestThreadSafePassedList test;
        private int Counter = 0;

        private readonly IBitmexAuthorization _bitmexAuthorizationA;
        private readonly IBitmexAuthorization _bitmexAuthorizationB;

        private IBitmexApiService bitmexApiServiceA;
        private IBitmexApiService bitmexApiServiceB;

        private BindingSource bs;
        //private long ID = 0;

        // TESTING

        //private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);
        //private static readonly string API_KEY = "QbpGewiOyIYMbyQ-ieaTKfOJ";
        //private static readonly string API_SECRET = "";

        // TESTLONG  [51091]    : "QbpGewiOyIYMbyQ-ieaTKfOJ"
        // TESTSHORT [170591]   : "xEuMT-y7ffwxrvHA2yDwL1bZ"

        public Form1()
        {
            InitializeComponent();
            InitForm();

            _bitmexAuthorizationA = new BitmexAuthorization { BitmexEnvironment = BitmexEnvironment.Test };
            _bitmexAuthorizationB = new BitmexAuthorization { BitmexEnvironment = BitmexEnvironment.Test };
            PrepareConnections(/*(double)NUDMaxExp.Value, (double)NUDLeverage.Value, (int)NUDDepth.Value, (int)NUDZonesize.Value, (double)NUDMinProfit.Value*/);

            bs = new BindingSource();
            dGV.AutoGenerateColumns = false;
            dGV.AutoSize = true;
            dGV.AllowUserToAddRows = false;
            dGV.AllowUserToDeleteRows = false;
            dGV.AllowUserToResizeColumns = false;
            dGV.AllowUserToResizeRows = false;
            dGV.DataSource = bs;

            DataGridViewColumn column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "Account";
            column.Name = "Account";
            dGV.Columns.Add(column);
            
            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "OrderQty";
            column.Name = "OrderQty";
            dGV.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "ClOrdId";
            column.Name = "ClOrdId";
            column.Visible = false;                 //this.dataGridView1.Columns["CustomerID"].Visible = false;
            dGV.Columns.Add(column);

        }

        private string GetExpiresString()
        {
            return (ToUnixTimeSeconds(DateTimeOffset.UtcNow) + 3600).ToString();
        }

        public void PrepareConnections(/*double maxExp, double leverage,int maxDepth, int zoneSize, double minProfit*/)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string apiA, secA, apiB, secB;
            
            apiA = appSettings["API_KEY_BITMEX_A_TEST"] ?? string.Empty;
            secA = appSettings["API_SECRET_BITMEX_A_TEST"] ?? string.Empty;
            apiB = appSettings["API_KEY_BITMEX_B_TEST"] ?? string.Empty;
            secB = appSettings["API_SECRET_BITMEX_B_TEST"] ?? string.Empty;
            
            if (!string.IsNullOrEmpty(apiA) && !string.IsNullOrEmpty(secA) && !string.IsNullOrEmpty(apiB) && !string.IsNullOrEmpty(secB))
            {
                _bitmexAuthorizationA.Key = apiA;
                _bitmexAuthorizationA.Secret = secA;
                _bitmexAuthorizationB.Key = apiB;
                _bitmexAuthorizationB.Secret = secB;
                
            }

        }

        private void InitForm()
        {
            lijst = new List<int>();
            lijst.Add(Counter);
            test = new TestThreadSafePassedList(lijst);
            TimerTest.Interval = 1000;
            TimerTest.Enabled = true;
            
            //btn1.Text = "Start/Stops";
            //btn8.Text = "calc.Evaluate()";

            LabelOnOff.Text = "OFF";
            Heartbeat.Interval = 2000;
            TimerTest.Interval = 250;

            // Default values
            NUDDepth.Value = 4;
            NUDLeverage.Value = 10;
            NUDMaxExp.Value = (decimal)0.1;
            NUDMinProfit.Value = (decimal)0.03;
            NUDZonesize.Value = 24;

            //btn2.Text = "Test timer";
            //btn6.Text = "Market";
            //btn5.Text = "TEST";
            //btn7.Text = "Connect";

            //PrepareConnections();

            //TBMarketOrder.Text = "XBTUSD";

            //log4net.Config.XmlConfigurator.Configure();
            //log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

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

        #region Button handlers

        private void btn1_Click(object sender, EventArgs e)
        {
            LabelOnOff.Text = (LabelOnOff.Text == "OFF") ? "ON" : "OFF";

            

            var posOrderParamsA = OrderPOSTRequestParams.CreateMarketStopOrder("XBTUSD", 150, 8000, OrderSide.Buy);
            bitmexApiServiceA = BitmexApiService.CreateDefaultApi(_bitmexAuthorizationA);
            bitmexApiServiceA.Execute(BitmexApiUrls.Order.PostOrder, posOrderParamsA).ContinueWith(ProcessPostOrderResult);

            var posOrderParamsB = OrderPOSTRequestParams.CreateMarketStopOrder("XBTUSD", 150, 6500, OrderSide.Sell);
            bitmexApiServiceB = BitmexApiService.CreateDefaultApi(_bitmexAuthorizationB);
            bitmexApiServiceB.Execute(BitmexApiUrls.Order.PostOrder, posOrderParamsB).ContinueWith(ProcessPostOrderResult);
            
        }

        #endregion Button handlers

        private async void ProcessPostOrderResult(Task<BitmexApiResult<OrderDto>> task)
        {
            if (task.Exception != null)
            {
                MessageBox.Show((task.Exception.InnerException ?? task.Exception).Message);
            }
            else
            {
                MessageBox.Show($"order has been placed with Id {task.Result.Result.OrderId}");
            }
        }

        private void DBLogOperation(string operation, object obj)
        {
            //try
            //{
            //    //// Build connection string
            //    //SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            //    //builder.DataSource = "localhost";   // update me
            //    //builder.UserID = "sa";              // update me
            //    //builder.Password = "your_password";      // update me
            //    //builder.InitialCatalog = "master";

            //    string connectionString = @"Data Source=.\MYSQLSDB;Initial Catalog=Trading;Integrated Security=true;";

            //    // Connect to SQL
            //    using (SqlConnection connection = new SqlConnection(connectionString))
            //    {
            //        connection.Open();
            //        // Connected !

            //        //String sql = "DROP DATABASE IF EXISTS [SampleDB]; CREATE DATABASE [SampleDB]";
            //        //using (SqlCommand command = new SqlCommand(sql, connection))
            //        //{
            //        //    command.ExecuteNonQuery();
            //        //    Console.WriteLine("Done.");
            //        //}

            //        // Create a Table and insert some sample data
            //        //StringBuilder sb = new StringBuilder();
            //        //sb.Append("USE SampleDB; ");
            //        //sb.Append("CREATE TABLE Employees ( ");
            //        //sb.Append(" Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY, ");
            //        //sb.Append(" Name NVARCHAR(50), ");
            //        //sb.Append(" Location NVARCHAR(50) ");
            //        //sb.Append("); ");
            //        //sb.Append("INSERT INTO Employees (Name, Location) VALUES ");
            //        //sb.Append("(N'Jared', N'Australia'), ");
            //        //sb.Append("(N'Nikita', N'India'), ");
            //        //sb.Append("(N'Tom', N'Germany'); ");
            //        //sql = sb.ToString();
            //        //using (SqlCommand command = new SqlCommand(sql, connection))
            //        //{
            //        //    command.ExecuteNonQuery();
            //        //    Console.WriteLine("Done.");
            //        //}

            //        //// INSERT demo
            //        //Console.Write("Inserting a new row into table, press any key to continue...");
            //        //Console.ReadKey(true);
            //        //sb.Clear();
            //        //sb.Append("INSERT Employees (Name, Location) ");
            //        //sb.Append("VALUES (@name, @location);");
            //        //sql = sb.ToString();
            //        //using (SqlCommand command = new SqlCommand(sql, connection))
            //        //{
            //        //    command.Parameters.AddWithValue("@name", "Jake");
            //        //    command.Parameters.AddWithValue("@location", "United States");
            //        //    int rowsAffected = command.ExecuteNonQuery();
            //        //    Console.WriteLine(rowsAffected + " row(s) inserted");
            //        //}

            //        //// UPDATE demo
            //        //String userToUpdate = "Nikita";
            //        //Console.Write("Updating 'Location' for user '" + userToUpdate + "', press any key to continue...");
            //        //Console.ReadKey(true);
            //        //sb.Clear();
            //        //sb.Append("UPDATE Employees SET Location = N'United States' WHERE Name = @name");
            //        //sql = sb.ToString();
            //        //using (SqlCommand command = new SqlCommand(sql, connection))
            //        //{
            //        //    command.Parameters.AddWithValue("@name", userToUpdate);
            //        //    int rowsAffected = command.ExecuteNonQuery();
            //        //    Console.WriteLine(rowsAffected + " row(s) updated");
            //        //}

            //        //// DELETE demo
            //        //String userToDelete = "Jared";
            //        //Console.Write("Deleting user '" + userToDelete + "', press any key to continue...");
            //        //Console.ReadKey(true);
            //        //sb.Clear();
            //        //sb.Append("DELETE FROM Employees WHERE Name = @name;");
            //        //sql = sb.ToString();
            //        //using (SqlCommand command = new SqlCommand(sql, connection))
            //        //{
            //        //    command.Parameters.AddWithValue("@name", userToDelete);
            //        //    int rowsAffected = command.ExecuteNonQuery();
            //        //    Console.WriteLine(rowsAffected + " row(s) deleted");
            //        //}

            //        //// READ demo
            //        //Console.WriteLine("Reading data from table, press any key to continue...");
            //        //Console.ReadKey(true);
            //        //sql = "SELECT Id, Name, Location FROM Employees;";
            //        //using (SqlCommand command = new SqlCommand(sql, connection))
            //        //{

            //        //    using (SqlDataReader reader = command.ExecuteReader())
            //        //    {
            //        //        while (reader.Read())
            //        //        {
            //        //            Console.WriteLine("{0} {1} {2}", reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
            //        //        }
            //        //    }
            //        //}
            //    }
            //}
            //catch (SqlException e)
            //{
            //    Console.WriteLine(e.ToString());
            //}

            Console.WriteLine("All done. Press any key to finish...");
            Console.ReadKey(true);
        }

        #region Timer handlers

        private void Heartbeat_Tick(object sender, EventArgs e)
        {
            //if (calc != null)
            //{
            //    long r = calc.Evaluate();
                
            //    RefreshLabels(r.ToString());

            //    //if (calc.GetStatus().ToString() == "Finish")
            //    //    btn
            //    //{
            //    //    calc.GetLastPrice();
            //    //    PrepareConnections((double)NUDMaxExp.Value, (double)NUDLeverage.Value, (int)NUDDepth.Value, (int)NUDZonesize.Value, (double)NUDMinProfit.Value);
            //    //}  
            //}
            //else
            //    lbl1.Text = "Status:Disconnected";

        }

        private void TimerTest_Tick(object sender, EventArgs e)
        {
            Counter++;
            lijst.Add(Counter);
        }

        #endregion Timer handlers

        private void btn6_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (calc == null)
            //    {
            //        PrepareConnections((double)NUDMaxExp.Value, (double)NUDLeverage.Value, (int)NUDDepth.Value, (int)NUDZonesize.Value, (double)NUDMinProfit.Value);
            //        lblConnectionStatus.Text = NUDMaxExp.Value.ToString() + "-" + NUDLeverage.Value.ToString() + "-" + NUDDepth.Value.ToString() + "-" + NUDZonesize.Value.ToString() + "-" + NUDMinProfit.Value.ToString() + "-";
            //    }
                
            //    double price;
            //    double usize;

            //    price = calc.GetPrevClosePrice();
                
            //    if (price == 0)
            //        price = (double)NUDDepth.Value;

            //    if (price > 0)
            //    {
            //        usize = calc.GetUnitSizeForPrice(price);
            //        connLong.MarketOrder("XBTUSD", MordoR.GenerateGUID(), long.Parse(usize.ToString()));
            //        lblLastPrice.Text = "Last price:" + price.ToString();
            //        lblUS.Text = "Unit size:" + usize.ToString();
            //    }
            //}
            //catch (Exception exc)
            //{
            //    MessageBox.Show(exc.Message);
            //}
            
        }

        public void RefreshLabels(string evalReturnCode = "")
        {
            //lbl2.Text = "RLim [L:" + connLong.LastKnownRateLimit.ToString() + "] & [S:" + connShort.LastKnownRateLimit.ToString() + "]";
            //lbl1.Text = "Status:" + calc.GetStatus().ToString();
            //lbl4.Text = "PrevClosePrice:" + calc.GetPrevClosePrice().ToString();
            //lbl5.Text = "Return code:" + evalReturnCode;
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            
        }

        private void btn7_Click(object sender, EventArgs e)
        {
            
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(test.ToString());
            //if (Heartbeat.Enabled)
            //{
            //    Heartbeat.Stop();
            //    LabelOnOff.Text = "OFF";
            //}
            //else
            //{
            //    //PrepareConnections();
            //    Heartbeat.Start();
            //    LabelOnOff.Text = "ON";
            //}
        }

        // Market Order
        private void btn3_Click(object sender, EventArgs e)
        {
            //Log.Debug("Testing Button clicked 123");


            //var bitmexApiServiceA = BitmexApiService.CreateDefaultApi(new BitmexAuthorization
            //{
            //    BitmexEnvironment = BitmexEnvironment.Test,
            //    Key = "vUVW4tbj-wZG5UQgRkqHL4_z",
            //    Secret = "8HaCUEbx0qgJfQSo4EiX6RUGzDsjiY-uOsEsCBstIpYu-J7Q"
            //});
            var bitmexApiServiceA = BitmexApiService_Test_POS_Outcome.CreateDefaultApi("111");
            var OrderParamsA = OrderPOSTRequestParams.CreateSimpleLimit("XBTUSD", "1234HoedjeVanPapier-1234", 150, (decimal)10150.0, OrderSide.Buy);
            bitmexApiServiceA.Execute(BitmexApiUrls.Order.PostOrder, OrderParamsA).ContinueWith(HandleOrderResponse, TaskContinuationOptions.AttachedToParent);
            
            var OrderParams = new OrderDELETERequestParams() { ClOrdID = "bladieblakakkahahhaha" };
            var result = bitmexApiServiceA.Execute(BitmexApiUrls.Order.DeleteOrder, OrderParams);

            MessageBox.Show(result.Result.Result[0].ToString());
        }

        private void HandleOrderResponse(Task<BitmexApiResult<OrderDto>> task)
        {
            MessageBox.Show($"Yay, Order {task.Result.Result.ClOrdId} successfully placed, zogezegd!");
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void btn8_Click(object sender, EventArgs e)
        {
            //ID++;
            //bs.Add(new OrderResponse { Account = 1234, OrderQty = 1, ClOrdId = "A-" + ID.ToString() });
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            //var obj = bs.List.OfType<OrderResponse>().ToList().Find(f => f.ClOrdId == textBox1.Text);
            //var foundIndex = bs.IndexOf(obj);
            
            //if (foundIndex > -1)
            //    bs.List.RemoveAt(foundIndex);
            //else
            //    MessageBox.Show("Not found.");
        }
    }
}



//if (o is List<ZoneRecoveryOrder>)
//{
//    foreach (ZoneRecoveryOrder zo in (List<ZoneRecoveryOrder>)o)
//    {
//        //MessageBox.Show(zo.ToString());

//        if (zo.ServerResponseInitial is OrderResponse)
//        {
//            if(zo.ServerResponseInitial != null)
//                MessageBox.Show(((OrderResponse)zo.ServerResponseInitial).ClOrdId.ToString());
//            else
//                MessageBox.Show("ServerResponseInitial NULL > " + zo.ToString());
//        }
//        else if (zo.ServerResponseInitial is BaseError)
//        {
//            MessageBox.Show(((BaseError)zo.ServerResponseInitial).Error.Message);
//        }
//        else if (zo.ServerResponseInitial == null)
//        {
//            MessageBox.Show("NULL = " + zo.ToString());
//            //MessageBox.Show("NULL");
//        }
//        else
//        {
//            MessageBox.Show(zo.ServerResponseInitial.GetType().ToString());
//        }
//    }
//}
//else
//    MessageBox.Show("Dikke Sheiss");

//calc.SetUnitSize(1000);

//double breakEvenPrice = calc.CalculateBreakEvenPrice();
//double direction = -calc.GetNextDirection();
//double totalExposure = calc.CalculateTotalOpenExposure();
//double MinimumProfitPercentage = Convert.ToDouble(TBClOrdId.Text);

//double gewoon = breakEvenPrice + (direction * (totalExposure * MinimumProfitPercentage));
//double result = Math.Round(breakEvenPrice + (direction * (totalExposure * MinimumProfitPercentage)));

//OutputLabel.Text = OutputLabel.Text + MinimumProfitPercentage.ToString() + "=>" + gewoon.ToString() + "||"; //+ Environment.NewLine

//MessageBox.Show(breakEvenPrice.ToString());
//MessageBox.Show(direction.ToString());
//MessageBox.Show(totalExposure.ToString());
//MessageBox.Show(MinimumProfitPercentage.ToString());
//MessageBox.Show("gewoon=" + gewoon.ToString());
//MessageBox.Show("result=" + result.ToString());

//string s = "TP-Price=" + calc.CalculatePriceForOrderType(ZoneRecoveryOrderType.TP).ToString();
//s = s + Environment.NewLine + "TP-Qty=" + calc.CalculateQtyForOrderType(ZoneRecoveryOrderType.TP).ToString();
//s = s + Environment.NewLine + "BE-Price=" + calc.CalculateBreakEvenPrice().ToString();
//s = s + Environment.NewLine + "TE=" + calc.CalculateTotalOpenExposure().ToString();
//s = s + Environment.NewLine + "DIR=" + calc.GetNextDirection().ToString();
//s = s + Environment.NewLine + "-----------------";
//s = s + Environment.NewLine + "TP-Price=" + calc.CalculatePriceForOrderType(ZoneRecoveryOrderType.REV).ToString();
//s = s + Environment.NewLine + "TP-Qty=" + calc.CalculateQtyForOrderType(ZoneRecoveryOrderType.REV).ToString();
//MessageBox.Show(s);


// TODO Error: Price must be a number
// TODO Error: stopPx must be a number

//MessageBox.Show(o.ToString());
//if (o is string)
//    MessageBox.Show(o.ToString());
//else if(o is List<ZoneRecoveryOrder>)
//    MessageBox.Show(o.ToString());