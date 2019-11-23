﻿namespace PStrategies.ZoneRecovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using BitMEX.Model;
    using BitMEX.Client;

    /// <summary>
    /// An enumeration that represents the type of order within the Zone Recovery strategy.
    /// TP  = Take Profit in Profit
    /// TL  = Take Loss
    /// REV = Reverse
    /// </summary>
    public enum ZoneRecoveryOrderType { TP, TL, REV, Cancel }

    /// <summary>
    /// An enumeration that represents the status of an order as known in the application.
    /// New     = Resting order
    /// Filled  = Filled order
    /// Cancel  = Canceled order
    /// Error   = Error occurred at the time the order was being transferred to the server.
    /// </summary>
    public enum ZoneRecoveryOrderStatus { New, Filled, PartiallyFilled, Canceled, Error, Unknown }

    /// <summary>
    /// Class that represents an order sent to the exchange. 
    /// When Qty is negative, it is a Sell order.
    /// When Qty is position, it is a Long order.
    /// </summary>
    public class ZoneRecoveryOrder
    {
        /// <summary>
        /// Unique identifier of the position, as it is know on the exchange.
        /// </summary>
        public string ClOrdId { get; set; }

        /// <summary>
        /// The Order response coming from the Exchange. This response is used to compare the status of the order when it 
        /// was placed with a later status.
        /// </summary>
        public OrderResponse ServerResponseCompare { get; set; }

        /// <summary>
        /// The response coming from the server at the time the order is placed. Should be one of types:
        ///     - OrderResponse
        ///     - List<OrderResponse>
        ///     - BaseError
        /// </summary>
        public object ServerResponseInitial { get; set; }

        /// <summary>
        /// The symbol of the asset.
        /// </summary>
        public string Symbol { get; set; }
        
        /// <summary>
        /// The average price of the position.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// The total quantity of the position.
        /// </summary>
        public long Qty { get; set; }

        /// <summary>
        /// Shortcut to get the last known status of the order.
        /// </summary>
        public ZoneRecoveryStatus CurrentStatus { get; set; }

        /// <summary>
        /// The type of order.
        /// </summary>
        public ZoneRecoveryOrderType OrderType { get; set; }

        /// <summary>
        /// The status of the order as known by the application.
        /// </summary>
        public ZoneRecoveryOrderStatus OrderStatus { get; set; }

        /// <summary>
        /// Constructor of the class ZoneRecoveryOrder
        /// </summary>
        /// <param name="clOrdId"></param>
        /// <param name="account"></param>
        /// <param name="price"></param>
        /// <param name="qty"></param>
        /// <param name="orderType"></param>
        public ZoneRecoveryOrder(string clOrdId, string symbol, double price, long qty, ZoneRecoveryOrderType orderType, ZoneRecoveryStatus cStatus)
        {
            ClOrdId         = clOrdId;
            Symbol          = symbol;
            Price           = price;
            Qty             = qty;
            OrderType       = orderType;
            CurrentStatus   = cStatus;
        }

        /// <summary>
        /// Override for default ToString() method
        /// </summary>
        /// <returns>Concatenated values of this instance of ZoneRecoveryOrder.</returns>
        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("class ZoneRecoveryOrder {\n");
            sb.Append("  ClOrdId: ").Append(ClOrdId.ToString()).Append("\n");
            sb.Append("  Symbol: ").Append(Symbol.ToString()).Append("\n");
            sb.Append("  Price: ").Append(Price.ToString()).Append("\n");
            sb.Append("  Qty: ").Append(Qty.ToString()).Append("\n");
            sb.Append("  OrderType: ").Append(OrderType.ToString()).Append("\n");
            sb.Append("  CurrentStatus: ").Append(CurrentStatus.ToString()).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Send the current ZoneRecoveryOrder to the exchange.
        /// </summary>
        /// <param name="conn">The MordoR connection to use to send the ZoneRecoveryOrder.</param>
        /// <returns>The object returned by the server.</returns>
        public object SendOrderToServer(MordoR conn)
        {
            //if (Account != conn.Account)
            //    return null;

            switch(OrderType)
            {
                case ZoneRecoveryOrderType.TP: // Limit order > Tegenovergestelde van open position
                    ServerResponseInitial = conn.LimitOrder(Symbol, ClOrdId, Qty, Price, "TP");
                    break;
                case ZoneRecoveryOrderType.TL: // Speciaal order
                    ServerResponseInitial = conn.StopMarketOrder(Symbol, ClOrdId, Qty, Price, "TL");
                    break;
                case ZoneRecoveryOrderType.REV:// Speciaal order
                    if (CurrentStatus == ZoneRecoveryStatus.Winding)
                        ServerResponseInitial = conn.StopMarketOrder(Symbol, ClOrdId, Qty, Price, "REV");
                    else if (CurrentStatus == ZoneRecoveryStatus.Unwinding)
                        ServerResponseInitial = conn.LimitOrder(Symbol, ClOrdId, Qty, Price, "REV"); //TODO
                    else
                        return null;
                    break;
                case ZoneRecoveryOrderType.Cancel:
                    var o = UndoOrder(conn);                                                            
                    if (o is List<OrderResponse>)
                        ServerResponseInitial = ((List < OrderResponse > )o).First();
                    else
                        ServerResponseInitial = o;                
                    break;
                default:
                    OrderStatus = ZoneRecoveryOrderStatus.Error;
                    return null;
            }

            if(ServerResponseInitial is OrderResponse)
            {
                switch (((OrderResponse)ServerResponseInitial).OrdStatus)
                {
                    case "Filled":
                        OrderStatus = ZoneRecoveryOrderStatus.Filled;
                        break;
                    case "New":
                        OrderStatus = ZoneRecoveryOrderStatus.New;
                        break;
                    case "Partially filled":
                        OrderStatus = ZoneRecoveryOrderStatus.PartiallyFilled;
                        break;
                    case "Canceled":
                        OrderStatus = ZoneRecoveryOrderStatus.Canceled;
                        break;
                    default:
                        OrderStatus = ZoneRecoveryOrderStatus.Unknown;
                        break;
                }
            }
            else
                OrderStatus = ZoneRecoveryOrderStatus.Error;

            return ServerResponseInitial;
        }

        /// <summary>
        /// Cancel this ZoneRecoveryOrder on the Exchange.
        /// </summary>
        /// <param name="conn">The MordoR connection to use to send the ZoneRecoveryOrder.</param>
        /// <returns>The object returned by the server.</returns>
        public object UndoOrder(MordoR conn)
        {
            ServerResponseInitial = conn.CancelOrders(new string[] { ClOrdId });
            return ServerResponseInitial;
        }
    }
}
