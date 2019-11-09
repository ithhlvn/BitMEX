﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using BitMEX.Model;
//
//    var walletResponse = WalletResponse.FromJson(jsonString);

namespace BitMEX.Model
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class WalletResponse
    {
        [JsonProperty("account")]
        public long Account { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("prevDeposited")]
        public long PrevDeposited { get; set; }

        [JsonProperty("prevWithdrawn")]
        public long PrevWithdrawn { get; set; }

        [JsonProperty("prevTransferIn")]
        public long PrevTransferIn { get; set; }

        [JsonProperty("prevTransferOut")]
        public long PrevTransferOut { get; set; }

        [JsonProperty("prevAmount")]
        public long PrevAmount { get; set; }

        [JsonProperty("prevTimestamp")]
        public DateTimeOffset PrevTimestamp { get; set; }

        [JsonProperty("deltaDeposited")]
        public long DeltaDeposited { get; set; }

        [JsonProperty("deltaWithdrawn")]
        public long DeltaWithdrawn { get; set; }

        [JsonProperty("deltaTransferIn")]
        public long DeltaTransferIn { get; set; }

        [JsonProperty("deltaTransferOut")]
        public long DeltaTransferOut { get; set; }

        [JsonProperty("deltaAmount")]
        public long DeltaAmount { get; set; }

        [JsonProperty("deposited")]
        public long Deposited { get; set; }

        [JsonProperty("withdrawn")]
        public long Withdrawn { get; set; }

        [JsonProperty("transferIn")]
        public long TransferIn { get; set; }

        [JsonProperty("transferOut")]
        public long TransferOut { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("pendingCredit")]
        public long PendingCredit { get; set; }

        [JsonProperty("pendingDebit")]
        public long PendingDebit { get; set; }

        [JsonProperty("confirmedDebit")]
        public long ConfirmedDebit { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty("addr")]
        public string Addr { get; set; }

        [JsonProperty("script")]
        public string Script { get; set; }

        [JsonProperty("withdrawalLock")]
        public List<object> WithdrawalLock { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Wallet {\n");
            sb.Append("  Account: ").Append(Account).Append("\n");
            sb.Append("  Currency: ").Append(Currency).Append("\n");
            sb.Append("  PrevDeposited: ").Append(PrevDeposited).Append("\n");
            sb.Append("  PrevWithdrawn: ").Append(PrevWithdrawn).Append("\n");
            sb.Append("  PrevTransferIn: ").Append(PrevTransferIn).Append("\n");
            sb.Append("  PrevTransferOut: ").Append(PrevTransferOut).Append("\n");
            sb.Append("  PrevAmount: ").Append(PrevAmount).Append("\n");
            sb.Append("  PrevTimestamp: ").Append(PrevTimestamp).Append("\n");
            sb.Append("  DeltaDeposited: ").Append(DeltaDeposited).Append("\n");
            sb.Append("  DeltaWithdrawn: ").Append(DeltaWithdrawn).Append("\n");
            sb.Append("  DeltaTransferIn: ").Append(DeltaTransferIn).Append("\n");
            sb.Append("  DeltaTransferOut: ").Append(DeltaTransferOut).Append("\n");
            sb.Append("  DeltaAmount: ").Append(DeltaAmount).Append("\n");
            sb.Append("  Deposited: ").Append(Deposited).Append("\n");
            sb.Append("  Withdrawn: ").Append(Withdrawn).Append("\n");
            sb.Append("  TransferIn: ").Append(TransferIn).Append("\n");
            sb.Append("  TransferOut: ").Append(TransferOut).Append("\n");
            sb.Append("  Amount: ").Append(Amount).Append("\n");
            sb.Append("  PendingCredit: ").Append(PendingCredit).Append("\n");
            sb.Append("  PendingDebit: ").Append(PendingDebit).Append("\n");
            sb.Append("  ConfirmedDebit: ").Append(ConfirmedDebit).Append("\n");
            sb.Append("  Timestamp: ").Append(Timestamp).Append("\n");
            sb.Append("  Addr: ").Append(Addr).Append("\n");
            sb.Append("  Script: ").Append(Script).Append("\n");
            sb.Append("  WithdrawalLock: ").Append(WithdrawalLock).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
    
    public partial class WalletResponse
    {
        public static WalletResponse FromJson(string json)
        {
            return JsonConvert.DeserializeObject<WalletResponse>(json, WalletConverter.Settings);
        }
    }
    
    internal static class WalletConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
