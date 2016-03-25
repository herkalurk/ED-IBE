﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IBE.EDDN
{

    /// <summary>
    /// schema class,
    /// based on the structure from 
    /// https://github.com/jamesremuscat/EDDN/blob/master/schemas/commodity-v2.0.json
    /// </summary>
    internal partial class EDDNSchema_v2
    {
        internal class Header_Class
        {

            [JsonProperty("softwareVersion")]
            public string SoftwareVersion { get; set; }

            [JsonProperty("gatewayTimestamp")]
            public string GatewayTimestamp { get; set; }

            [JsonProperty("softwareName")]
            public string SoftwareName { get; set; }

            [JsonProperty("uploaderID")]
            public string UploaderID { get; set; }
        }

    }

    internal partial class EDDNSchema_v2
    {
        internal class Commodity_Class
        {

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("buyPrice")]
            public int BuyPrice { get; set; }

            [JsonProperty("supplyLevel")]
            public string SupplyLevel { get; set; }

            [JsonProperty("supply")]
            public int Supply { get; set; }

            [JsonProperty("demand")]
            public int Demand { get; set; }

            [JsonProperty("sellPrice")]
            public int SellPrice { get; set; }

            [JsonProperty("demandLevel")]
            public string DemandLevel { get; set; }
        }
    }

    internal partial class EDDNSchema_v2
    {
        internal class Message_Class
        {

            [JsonProperty("commodities")]
            public Commodity_Class[] Commodities { get; set; }

            [JsonProperty("timestamp")]
            public string Timestamp { get; set; }

            [JsonProperty("systemName")]
            public string SystemName { get; set; }

            [JsonProperty("stationName")]
            public string StationName { get; set; }
        }
    }

    internal partial class EDDNSchema_v2
    {

        [JsonProperty("header")]
        public Header_Class Header { get; set; }

        [JsonProperty("$schemaRef")]
        public string SchemaRef { get; set; }

        [JsonProperty("message")]
        public Message_Class Message { get; set; }

        /// changes data to a RN-importable stringarray
        internal String[] getEDDNCSVImportStrings()
        {
            int size                = this.Message.Commodities.GetUpperBound(0)+1;
            string[] csvFormatted   = new String[size];

            for (int i = 0; i < size; i++)
            {
            	Commodity_Class Commodity = this.Message.Commodities[i];

                //System;Location;Commodity_Class;Sell;Buy;Demand;DemandLevel;Supply;SupplyLevel;Date;
                csvFormatted[i] = this.Message.SystemName + ";" +
                                  this.Message.StationName + ";" +
                                  Commodity.Name + ";" +
                                  (Commodity.SellPrice == 0 ? "" : Commodity.SellPrice.ToString()) + ";" +
                                  (Commodity.BuyPrice == 0 ? "" : Commodity.BuyPrice.ToString()) + ";" +
                                  Commodity.Demand.ToString() + ";" +
                                  Commodity.DemandLevel + ";" +
                                  Commodity.Supply.ToString() + ";" +
                                  Commodity.SupplyLevel + ";" +
                                  this.Message.Timestamp.ToString() + ";"
                                  +
                                  "EDDN" + ";";		 
            }

            return csvFormatted;
        }

        /// <summary>
        /// returns, if this is a main message or only a message from the test schema
        /// </summary>
        /// <returns></returns>
        public Boolean isTest()
        { 
            return SchemaRef.EndsWith("/test");
        }


    }

}
