using Microsoft.Extensions.Configuration;
using Overkill.Core.Interfaces;
using Overkill.Core.Topics;
using Overkill.Proxies.Interfaces;
using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace Overkill.Core
{
    /// <summary>
    /// A location service for Quectel-CM GPS modems
    /// </summary>
    public class QuectelModemPositioningService : IPositioningService
    {
        private IOverkillConfiguration config;
        private IPubSubService pubSub;
        private ISerialProxy serialProxy;
        private IThreadProxy threadProxy;

        private ISerialProxy outputPort;
        private IThreadProxy workThread;

        public QuectelModemPositioningService(IOverkillConfiguration _config, IPubSubService _pubSub, IThreadProxy _threadProxy, ISerialProxy _serialProxy)
        {
            config = _config;
            pubSub = _pubSub;
            threadProxy = _threadProxy;
            serialProxy = _serialProxy;
        }

        /// <summary>
        /// Begin reading from the modem's serial port and listen for location updates
        /// </summary>
        public void Start()
        {   
            outputPort = serialProxy.Create(config.Positioning.SerialOutput, config.Positioning.SerialBaudRate);

            SendUpdateRequestSignal();
            workThread = threadProxy.Create(ProcessUpdates);
        }

        /// <summary>
        /// Sends a payload to the modem to begin receiving location updates
        /// </summary>
        public void SendUpdateRequestSignal()
        {
            var inputPort = serialProxy.Create(config.Positioning.SerialOutput, config.Positioning.SerialBaudRate);
            inputPort.Open();
            inputPort.Write("AT+QGPS=1");
            inputPort.Close();
        }

        /// <summary>
        /// Ran in a thread. Continuously reads information from the serial input buffer and, if valid GPS location data, dispatches a Topic
        /// </summary>
        void ProcessUpdates()
        {
            while (outputPort.IsOpen)
            {
                var buffer = new byte[1024];
                if (outputPort.Read(buffer, 0, 1024) > 0)
                {
                    var data = UTF8Encoding.UTF8.GetString(buffer);

                    var (success, latitude, longitude) = ParseModemData(data);

                    if (!success) continue;

                    pubSub.Dispatch(new PositionUpdateTopic()
                    {
                        Latitude = latitude,
                        Longitude = longitude
                    });
                }
            }
        }

        /// <summary>
        /// Helper function to parse text data coming from the modem into usable GPS information
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public (bool Success, float Latitude, float Longitude) ParseModemData(string data)
        {
            if (!data.Contains("$GPRMC")) return (false, 0, 0); //Location data starts with this $GPRMC tag

            var postGprmc = data.Split(new[] { "$GPRMC" }, StringSplitOptions.None)[1];
            var dataPoints = postGprmc.Split(','); //The information comes comma delimited

            //Verify the data is valid
            if (dataPoints.Length < 6) return (false, 0, 0);
            if (dataPoints[3].Length < 1 || dataPoints[5].Length < 1) return (false, 0, 0);

            var latitude = dataPoints[3];
            var longitude = dataPoints[5];

            return (
                true,
                ParseGPSCoordinate(latitude),
                ParseGPSCoordinate(longitude)
            );
        }

        /// <summary>
        /// Takes in the coordinate data format used by the modem and converts it into common latitude/longitude format
        /// </summary>
        /// <returns></returns>
        public float ParseGPSCoordinate(string coordinate)
        {
            float deg;
            float remainder;

            if (coordinate[0] == '0')
            {
                var degF = float.Parse(coordinate.Substring(1, 3));
                var remainderF = float.Parse(coordinate.Substring(3));
                deg = degF * -1;
                remainder = remainderF * -1;
            }
            else
            {
                deg = float.Parse(coordinate.Substring(0, 2));
                remainder = float.Parse(coordinate.Substring(2));
            }

            remainder /= 60;

            return deg + remainder;
        }
    }
}
