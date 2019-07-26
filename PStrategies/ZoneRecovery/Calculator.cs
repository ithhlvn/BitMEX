﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitMEX.Model;

namespace PStrategies.ZoneRecovery
{
    public class Calculator
    {
        #region Variables
        private Object _Lock = new Object();
        private static double[] FactorArray = new double[] { 1, 2, 3, 6, 12, 24, 48, 96 };
        private enum ZoneRecoveryStatus { Winding, Unwinding }
        private ZoneRecoveryStatus CurrentStatus;
        private List<ZoneRecoveryPosition> OpenPositions;
        private double MaximumUnitSize;
        private double MinimumProfitPercentage;

        /// <summary>
        /// CurrentZRPosition reflects the position withing the Zone Recovery strategy.
        /// When 0 > Strategy has been initialized or is completely unwound. There should be no open positions.
        /// When 1 > First Winding / last Unwinding
        /// When CurrentZRPosition = MaxDepthIndex > ZoneRecoveryStatus is switched and winding process reversed
        /// </summary>
        private int? CurrentZRPosition;

        public double InitialPrice { get; set; }
        public double MaxExposure { get; set; }
        public double TotalBalance { get; set; }
        public double Leverage { get; set; }
        public double PipSize { get; set; }
        public int ZoneSize { get; set; }
        public int MaxDepthIndex { get; set; }
        #endregion Variables

        /// <summary>
        /// Initializes the Zone Recovery Calculator.
        /// TODO: InitialPrice and its derived calculations should be recalculated when the first order is filled. Zone is 
        ///       determined relative to the price of the first position.
        /// </summary>
        /// <param name="initialPrice">The price at the time the class is initialized</param>
        /// <param name="maxExposure">Percentage of the maximum market exposure when completely wound</param>
        /// <param name="totalBalance">Most recent total balance</param>
        /// <param name="leverage">Leverage used to calculate other parameters</param>
        /// <param name="pipSize">The minimum pip size possible on the exchange</param>
        /// <param name="maxDepthIndex">Maximum dept allowed in the Zone Recovery system</param>
        /// <param name="zoneSize">The size of the zone in nr of pips</param>
        /// <param name="minPrftPerc">Minimum required profit margin</param>
        public Calculator(double initialPrice, double maxExposure, double totalBalance, double leverage, double pipSize, int maxDepthIndex, int zoneSize, double minPrftPerc)
        {
            CurrentStatus = ZoneRecoveryStatus.Winding;
            OpenPositions = new List<ZoneRecoveryPosition>();
            InitialPrice = initialPrice;
            MaxExposure = maxExposure;
            TotalBalance = totalBalance;
            Leverage = leverage;
            PipSize = pipSize;
            CurrentZRPosition = 0;
            MaxDepthIndex = maxDepthIndex;
            ZoneSize = zoneSize;
            MinimumProfitPercentage = minPrftPerc;
            MaximumUnitSize = GetRelativeUnitSize();
        }

        /// <summary>
        /// Calculates the total maximum exposure (depth) possible, according to the depths defines in FactorArray. Basically
        /// this is the sum of all the factors defined in FactorArray until the defined MaxDepth. It represents the 
        /// maximum exposure possible.
        /// Example: 
        ///     FactorArray = new double[] { 1, 2, 3, 6, 12, 24, 48, 96 };
        ///     MaxDepth = 4
        ///     GetTotalDepthMaxExposure() returns 12 (1 + 2 + 3 + 6) 
        /// </summary>
        private double GetTotalDepthMaxExposure()
        {
            double sum = 0;

            for (int i = 0; i < MaxDepthIndex; i++)
            {
                sum = sum + FactorArray[i];
            }
            return sum;
        }

        /// <summary>
        /// For a given InitialPrice, Balance, MaxExposure and Leverage, this function returns the calculated maximum unit size. 
        /// This unit size multiplied with the factors in FactorArray will give the unit size of each order.
        /// </summary>
        private double GetRelativeUnitSize()
        {
            double totalDepthMaxExposure = (double)this.GetTotalDepthMaxExposure();
            return Math.Round((double)(InitialPrice * Leverage * TotalBalance * MaxExposure / totalDepthMaxExposure) * (1 / (double)PipSize), MidpointRounding.AwayFromZero) / (1 / (double)PipSize);
        }

        /// <summary>
        /// Returns the price at which profit should be taken to be mathematically "Break Even". Commissions are not yet taken into account.
        /// TODO: Extend the calculation to take into account commissions and real profit using the MinimumProfitPercentage variable.
        /// </summary>
        /// <returns></returns>
        private double CalculateTPPrice()
        {
            double v_numerator = 0.0;
            double v_denominator = 0.0;
            double o = 0.0;

            foreach(ZoneRecoveryPosition zp in OpenPositions)
            {
                v_numerator = v_numerator + (zp.TotalVolume * zp.AVGPrice);
                v_denominator = v_denominator + (zp.TotalVolume);
            }

            o = Math.Round(v_numerator / v_denominator * (1 / PipSize), MidpointRounding.AwayFromZero) / (1 / PipSize);

            return o;
        }

        /// <summary>
        /// SetNewPosition should be called when a new position is taken on the exchange. The Zone Recovery
        /// strategy proceeds one step in its logic. The List of OrderResponses passed should be the 
        /// OrderResponses returned for one specific order.
        /// Example:
        ///     MaxDepthIndex = 4
        ///     CurrentZRPosition >  0   1   2   3   4   3   2   1   0   X
        ///     Position L/S      >  -   L   S   L   S   L4  S3  L2  S1  X
        ///     (Un)Winding       >  W   W   W   W   U   U   U   U   U   X
        /// 
        /// TODO: Check how WebSocket returns updates on resting orders. This function makes the assumption 
        /// that a list of OrderResponses is returned.
        /// </summary>
        /// <param name="orderResp">The List object with all the OrderResponses for one order returned by the Exchange.</param>
        public void SetNewPosition(List<BitMEX.Model.OrderResponse> orderResp)
        {
            // Create a new position object for the calculation of the average position size
            ZoneRecoveryPosition newPos = new ZoneRecoveryPosition((double)PipSize, OpenPositions.Count + 1);

            // Loop all the OrderResponse objects related to a specific filled previously resting or market order.
            foreach (BitMEX.Model.OrderResponse resp in orderResp)
            {
                // TODO: Check if assumption is correct that AvgPx is the average price for the filled OrderQty...
                newPos.AddToPosition((double)resp.AvgPx, (double)resp.OrderQty);
            }

            // Add the new averaged position to the positions collection
            this.OpenPositions.Add(newPos);

            if(CurrentStatus == ZoneRecoveryStatus.Winding)
            {
                //if(CurrentZRPosition == 0)
                //{
                //    // Initialize stuff
                //}

                CurrentZRPosition++;

                // Zone Recovery logic is reversed
                if (CurrentZRPosition == MaxDepthIndex)
                    CurrentStatus = ZoneRecoveryStatus.Unwinding;
            }
            else // Unwinding...
                CurrentZRPosition--;
        }

        /// <summary>
        /// Calculates the parameters for the next orders, TP & Reverse > InitialPrice & Volume and returns it as a ZoneRecoveryAction.
        /// Zone calculation strategy = "Stick with initial zone". This means when a new position diverts from the planned order price,
        /// the next order price is still set at a price calculated relative to the initial price. 
        /// Example for PipSize = 1 and ZoneSize = 50:
        ///     Buy     1   at  1000$
        ///     Sell    2   at   950$
        ///     Buy     3   at  1025$   > DISCREPANCY between planned vs actual price
        ///     
        /// According to the "plan", the Buy price here should have been 1000$ but due to extreme market volatility, the first price 
        /// at which the volume (3) could get filled was 1025$. Rather than average calculating the next reverse price this step 
        /// calculator will stick to the original plan and try to keep buying at 1000$ and keep selling at 950$.
        /// OpenPositions.Single(s => s.PositionIndex == 1).AVGPrice returns the initial reference price (= price of first position)
        /// </summary>
        /// <returns>ZoneRecoveryAction</returns>
        public ZoneRecoveryAction GetNextStep()
        {
            lock (_Lock)
            {
                ZoneRecoveryAction zra = new ZoneRecoveryAction(OpenPositions.Count + 1);

                if (CurrentStatus == ZoneRecoveryStatus.Winding)
                {
                    if (CurrentZRPosition > 0)
                    {
                        // Calculate the next take profit price
                        zra.TPPrice = this.CalculateTPPrice();
                        //zra.TPVolume = 

                        // Determine the direction of next step relative to the first position
                        if (OpenPositions.Single(s => s.PositionIndex == 1).TotalVolume > 0) // First position = LONG position
                        {
                            // Trade direction
                            int dir = (OpenPositions.Count % 2 == 1) ? -1 : 1;
                            
                            //TODO: fill ZoneRecoveryAction zra

                        }
                        else                                                                // First position = SHORT position
                        {
                            // Trade direction
                            int dir = (OpenPositions.Count % 2 == 1) ? 1 : -1;


                        }

                    }
                    else
                        //Should never happen because GetNextStep is called only after the first position is taken...
                        return null;
                }
                else // Unwinding
                {

                }
            }
            
            return null;
        }
    }

    /// <summary>
    /// ZoneRecoveryAction class serves merely the purpose of transporting all the parameters needed for creating the orders in the
    /// application that uses this library.
    /// TODO: Make ZoneRecoveryAction class dual account friendly by extending it with TPVolumeSellAccount and TPVolumeBuyAccount.
    /// </summary>
    public class ZoneRecoveryAction
    {
        /// <summary>
        /// PositionIndex represents the number of positions taken in the past within the current strategy instance. It can be used 
        /// as a unique identifier by the application that uses this library to make sure the same action is not taken twice...
        /// </summary>
        public int PositionIndex { set; get; }

        /// <summary>
        /// The price where a profit is taken. TP = Take Profit
        /// </summary>
        public double TPPrice { set; get; }
        public double TPVolume { set; get; }

        /// <summary>
        /// The price at which the position should be reversed
        /// </summary>
        public double ReversePrice { set; get; }
        public double ReverseVolume { set; get; }

        public ZoneRecoveryAction(int posIndex)
        {
            PositionIndex = posIndex;
        }
    }

    /// <summary>
    /// Class that represents a taken position. Since one order could be filled by a number of OrderResponses, it is
    /// easier to have a calculated object with only the information needed rather than the collection of
    /// OrderResponses.
    /// When Volume is negative, the position is a Sell position.
    /// When Volume is position, the position is a Long position.
    /// </summary>
    public class ZoneRecoveryPosition
    {
        public double AVGPrice { get; set; }
        public double TotalVolume { get; set; }
        public double PipSize { get; set; }

        /// <summary>
        /// PositionIndex keeps the sequence number of a position within its container. (followup number)
        /// Could be deleted later on if not used...
        /// </summary>
        public int PositionIndex { get; set; }

        public void AddToPosition(double executionPrice, double executionVolume)
        {
            this.AVGPrice = CalculateAveragePrice(AVGPrice, executionPrice, TotalVolume, executionVolume, PipSize);
            this.TotalVolume = this.TotalVolume + executionVolume;
        }

        public ZoneRecoveryPosition(double pipSize, int posIndex)
        {
            this.AVGPrice = 0.0;
            this.TotalVolume = 0.0;
            this.PipSize = pipSize;
            this.PositionIndex = posIndex;
        }

        public static double CalculateAveragePrice(double price1, double price2, double vol1, double vol2, double pipSize)
        {
            return Math.Round(((price1 * vol1) + (price2 * vol2)) / (vol1 + vol2) * (1 / pipSize), MidpointRounding.AwayFromZero) / (1 / pipSize);
        }
    }
}

/*
 * BreakEvenPrice
 * i    A[i]    V[i]   A[i]*V[i]
 * --------------------------------
 * 1    1.13    -1000    -1130
 * 2    1.125    2000     2250
 * 3    1.129   -3000    -3387   +
 * --------------------------------
 * SOM:         -2000    -2267
 * BREAK_EVEN_PRICE = SOM(A[i]*V[i]) / SOM(V[i]) = -2267 / -2000 = 1.1335

private static double CalculateBEP(double[] orders, double[] vol, int tradeindex, double dir) {
	// BEP = Sum(Vol R P) / Sum(Vol R)
	double v_numerator 		= 0.0;
	double v_denominator 	= 0.0;
	double o 				= 0.0;
	double cDir 				= dir;
			
	for (int i = tradeindex-1; i >= 0; i--) {
		v_numerator = Math.Round(v_numerator + (vol[i] * orders[i] * cDir), 5);
		v_denominator = Math.Round(v_denominator + (vol[i] * cDir), 5);
		// Change direction for next calculation
		cDir = (cDir < 0)? 1 : -1;
	}
			
	o = v_numerator / v_denominator;
			
	return Math.Round(o, 4);
}
 */