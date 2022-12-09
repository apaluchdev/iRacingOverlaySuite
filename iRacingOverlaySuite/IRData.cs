using irsdkSharp;
using irsdkSharp.Serialization;
using irsdkSharp.Serialization.Models.Fastest;
using irsdkSharp.Serialization.Models.Session;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace iRacingOverlaySuite
{
    public class IRData
    {

        public int DriverId = -1;

        public string TrackName => _session?.WeekendInfo.TrackDisplayName ?? string.Empty;

        #region

        private IRacingSDK sdk;

        private IRacingSessionModel? _session = null;

        private Data _data;

        #endregion

        public IRData()
        {
            sdk = new IRacingSDK();
            sdk.OnConnected += Sdk_OnConnected;
        }

        private void Sdk_OnConnected()
        {
            Console.WriteLine("Connected!");
        }

        public void ProcessData()
        {
            if (sdk == null)
                return;

            int lastUpdate = -1;

            var currentlyConnected = sdk.IsConnected();

            // Check if we can find the sim
            if (currentlyConnected)
            {
                _data = sdk.GetData();

                // Parse out your own driver Id
                if (DriverId == -1)
                {
                    var headers = sdk.Header;
                    DriverId = (int)sdk.GetData("PlayerCarIdx");
                }

                // Is the session info updated?
                int newUpdate = sdk.Header.SessionInfoUpdate;
                if (newUpdate != lastUpdate)
                {
                    lastUpdate = newUpdate;
                    _session = sdk.GetSerializedSessionInfo();
                }
            }
        }
    }
}
