using irsdkSharp;
using irsdkSharp.Models;
using irsdkSharp.Serialization;
using irsdkSharp.Serialization.Enums.Fastest;
using irsdkSharp.Serialization.Models.Data;
using irsdkSharp.Serialization.Models.Fastest;
using irsdkSharp.Serialization.Models.Session;
using irsdkSharp.Serialization.Models.Session.SessionInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;

namespace iRacingOverlaySuite
{
    public class IRData
    {
        public bool IsConnected { get; private set; }

        public int DriverId = -1;

        public string TrackName => _session?.WeekendInfo.TrackDisplayName ?? string.Empty;

        public float Brake;

        public float Speed;

        public float Throttle;

        public int CurrentLap;

        public float LapDistance;

        public bool InsideCar;

        public CarLeftRight CarLeftRight;

        public double SessionTime;

        #region

        private IRacingSDK sdk;

        private IRacingSDK sdkFile;

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
                if (IsConnected /* Check if iRacing process is open as well */)
                {
                    Data data = sdk.GetData();
                    Brake = (float)(sdk.GetData("Brake") ?? 0);
                    Speed = (float)(sdk.GetData("Speed") ?? 0);
                    Throttle = (float)(sdk.GetData("Throttle") ?? 0);
                    CurrentLap = (int)(sdk.GetData("Lap") ?? -1);
                    LapDistance = (float)(sdk.GetData("LapDist") ?? 0);
                    CarLeftRight = (CarLeftRight)sdk.GetData("CarLeftRight");
                    SessionTime = (double)(sdk.GetData("SessionTime") ?? -1);
                    InsideCar = (bool)(sdk.GetData("IsOnTrack") ?? false);
                    

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
