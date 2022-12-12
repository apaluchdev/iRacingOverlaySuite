using irsdkSharp;
using irsdkSharp.Serialization;
using irsdkSharp.Serialization.Enums.Fastest;
using irsdkSharp.Serialization.Models.Fastest;
using irsdkSharp.Serialization.Models.Session;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace iRacingOverlaySuite
{
    public class IRData
    {
        public bool IsConnected { get; private set; }

        public int DriverId = -1;

        public string TrackName => _session?.WeekendInfo.TrackDisplayName ?? string.Empty;

        public float Brake;

        public float Throttle;

        public CarLeftRight CarLeftRight;

        #region

        private IRacingSDK sdk;

        private IRacingSessionModel? _session = null;

        private Data _data;

        #endregion

        public IRData()
        {
            sdk = new IRacingSDK();
            sdk.OnConnected += Sdk_OnConnected;
            sdk.OnDisconnected += Sdk_OnDisconnected;
        }

        private void Sdk_OnDisconnected()
        {
            IsConnected = false;
        }

        private void Sdk_OnConnected()
        {
            IsConnected = true;
        }

        public void ProcessData()
        {
            try
            {
                if (sdk == null)
                    return;

                int lastUpdate = -1;

                // Check if we can find the sim
                if (IsConnected)
                {

                    Brake = (float) sdk.GetData("Brake");
                    Throttle = (float)sdk.GetData("Throttle");
                    CarLeftRight = (CarLeftRight)sdk.GetData("CarLeftRight");

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
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message);
                Thread.Sleep(3000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Thread.Sleep(3000);
            }
        }
    }
}
