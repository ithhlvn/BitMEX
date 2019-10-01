﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStrategies.ZoneRecovery
{
    /// <summary>
    /// Class that represents a taken position. Since one order could be filled by a number of OrderResponses, it is
    /// easier to have a calculated object with only the information needed rather than the collection of
    /// OrderResponses.
    /// When Volume is negative, the position is a Sell position.
    /// When Volume is position, the position is a Long position.
    /// </summary>
    public class ZoneRecoveryPosition
    {
        /// <summary>
        /// Unique identifier of the position, as it is know on the exchange.
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// The identifier of the account on which the position is known.
        /// </summary>
        public long? AccountID { get; set; }

        /// <summary>
        /// The average price of the position.
        /// </summary>
        public double AVGPrice { get; set; } = 0.0;

        /// <summary>
        /// The total quantity of the position.
        /// </summary>
        public double TotalQty { get; set; } = 0.0;

        /// <summary>
        /// The minimum pipsize of the exchange.
        /// </summary>
        public double PipSize { get; set; }

        /// <summary>
        /// PositionIndex keeps the sequence number of a position within its container. (followup number)
        /// Could be deleted later on if not used...
        /// </summary>
        public int PositionIndex { get; set; }

        public ZoneRecoveryPosition(string ordID, long? accountID, double pipSize, int posIndex, double? avgPrice = null, double? totalQty = null)
        {
            this.OrderID = ordID;
            this.PipSize = pipSize;
            this.PositionIndex = posIndex;
            this.AccountID = accountID;

            if (avgPrice != null && totalQty != null)
            {
                this.AVGPrice = (double)avgPrice;
                this.TotalQty = (double)totalQty;
            }
        }

        public void AddToPosition(double executionPrice, double executionVolume)
        {
            this.AVGPrice = CalculateAveragePrice(AVGPrice, executionPrice, TotalQty, executionVolume, PipSize);
            this.TotalQty = this.TotalQty + executionVolume;
        }
        
        public static double CalculateAveragePrice(double price1, double price2, double vol1, double vol2, double pipSize)
        {
            return Math.Round(((price1 * vol1) + (price2 * vol2)) / (vol1 + vol2) * (1 / pipSize), MidpointRounding.AwayFromZero) / (1 / pipSize);
        }
    }
}
