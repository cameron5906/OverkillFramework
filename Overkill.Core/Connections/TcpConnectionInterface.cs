using Overkill.Core.Connections.Data;
using Overkill.Core.Connections.Initialization;
using Overkill.Core.Interfaces;
using Overkill.Core.Topics;
using Overkill.Proxies;
using Overkill.Proxies.Interfaces;
using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Overkill.Core.Connections
{
    /// <summary>
    /// TCP communication service for Vehicles that use the TCP protocol to receive/transmit data
    /// </summary>
    public class TcpConnectionInterface : IConnectionInterface
    {
        private string host;
        private int port;
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private IPubSubService pubSub;
        private IThreadProxy threadProxy;
        private IThreadProxy thread;

        public bool IsConnected
        {
            get
            {
                if(tcpClient != null && tcpClient.Client != null && tcpClient.Client.Connected)
                {
                    if(tcpClient.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buffer = new byte[1];
                        return tcpClient.Client.Receive(buffer, SocketFlags.Peek) > 0;
                    }
                    return false;
                }

                return false;
            }
        }

        public TcpConnectionInterface(IPubSubService _pubSub, IThreadProxy _threadProxy)
        {
            pubSub = _pubSub;
            threadProxy = _threadProxy;
        }

        /// <summary>
        /// Initializes the TCP socket and binds it to the appropriate local network interface
        /// </summary>
        /// <param name="parameters"></param>
        public void Initialize(IConnectionInitializer parameters)
        {
            var tcpConnection = (TcpInitialization)parameters;

            host = tcpConnection.Host;
            port = tcpConnection.Port;
            tcpClient = new TcpClient(tcpConnection.LocalEndpoint);
        }

        /// <summary>
        /// Connects to the remote host and starts the receiving thread
        /// </summary>
        public void Connect()
        {
            Console.WriteLine("Connecting to TCP interface...");
            try
            {
                tcpClient.Connect(host, port);
                tcpClient.ReceiveTimeout = 1000;
                tcpClient.SendTimeout = 1000;
                networkStream = tcpClient.GetStream();

                thread = threadProxy.Create(Run);
                thread.Start();
            } catch(Exception ex)
            {
                Console.WriteLine("Could not connect to vehicle. Retrying...");
                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    Connect();
                });
            }
        }

        /// <summary>
        /// Sends a TCP payload to the remote host
        /// </summary>
        /// <param name="payload"></param>
        public void Send(ICommunicationPayload payload)
        {
            var tcp = (TcpData)payload;
            try
            {
                networkStream.Write(tcp.Data);
                networkStream.Flush();
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Runs in a thread. Processes incoming messages from the remote host and dispatches messages via PubSub
        /// </summary>
        private void Run()
        {
            var isConnected = false;
            while(true)
            {
                if(IsConnected)
                {
                    if(!isConnected)
                    {
                        pubSub.Dispatch(new ConnectionStatusChangedTopic() { Connected = true });
                    }

                    try
                    {
                        var buffer = new byte[1024];
                        if(networkStream.Read(buffer, 0, buffer.Length) > 0)
                        {
                            pubSub.Dispatch(new VehicleDataTopic()
                            {
                                Payload = new TcpData()
                                {
                                    Data = buffer
                                }
                            });
                        }
                    } catch(Exception ex)
                    {
                        pubSub.Dispatch(new ConnectionStatusChangedTopic() { Connected = false });
                        break;
                    }
                } else if(isConnected)
                {
                    isConnected = false;
                    pubSub.Dispatch(new ConnectionStatusChangedTopic() { Connected = false });
                    break;
                }
            }
        }
    }
}
