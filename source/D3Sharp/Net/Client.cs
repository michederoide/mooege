﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using D3Sharp.Net.Packets;
using D3Sharp.Net.Packets.Protocol.Authentication;
using D3Sharp.Net.Packets.Protocol.Connection;

namespace D3Sharp.Net
{
    public sealed class Client : IClient
    {
        private readonly Server _server;
        private readonly Socket _socket;
        private readonly byte[] _recvBuffer = new byte[BufferSize];
        public static readonly int BufferSize = 16*1024; // 16 KB

        public Dictionary<uint, uint> Services { get; private set; }

        public bool IsConnected
        {
            get { return _socket.Connected; }
        }

        public IPEndPoint RemoteEndPoint
        {
            get { return _socket.RemoteEndPoint as IPEndPoint; }
        }

        public IPEndPoint LocalEndPoint
        {
            get { return _socket.LocalEndPoint as IPEndPoint; }
        }

        public byte[] RecvBuffer
        {
            get { return _recvBuffer; }
        }

        public Socket Socket
        {
            get { return _socket; }
        }

        public Client(Server server, Socket socket)
        {
            if (server == null) throw new ArgumentNullException("server");
            if (socket == null) throw new ArgumentNullException("socket");

            this._server = server;
            this._socket = socket;
            this.Services = new Dictionary<uint, uint>();
        }
        
        private static uint _importedServiceCounter = 99;

        public void Process(PacketIn packet)
        {
            Console.WriteLine(packet);

            if (packet is ConnectRequest)
            {
                var response = new ConnectResponse(packet.Header.RequestID);
                this.Send(response.GetRawPacketData());
                Console.WriteLine(response);
            }
            else if (packet is BindRequest)
            {
                var requestedServiceIDs = new List<uint>();
                // supply service id's requested by client using service-hashes.
                foreach (var serviceHash in packet.Request.ImportedServiceHashList) 
                {
                    requestedServiceIDs.Add(
                        Server.Services.ContainsValue(serviceHash)
                        ? Server.Services.Where(pair => pair.Value == serviceHash).FirstOrDefault().Key
                        : _importedServiceCounter++);
                }

                if (requestedServiceIDs.Count > 0)
                {
                    var response = new BindResponse(packet.Header.RequestID, requestedServiceIDs);
                    this.Send(response.GetRawPacketData());
                    Console.WriteLine(response);
                }
                // add list of imported services supplied by client.
                foreach (var service in packet.Request.ExportedServiceList) 
                {
                    if (!this.Services.ContainsKey(service.Id))
                         this.Services.Add(service.Id, service.Hash);
                }
            }

            else if (packet is LogonRequest)
            {
                // who needs a module-check when we're already hacking lol?!
                var response = new LogonResponse(packet.Header.RequestID);
                this.Send(response.GetRawPacketData());
                Console.WriteLine(response);
            }  
        }

        #region recv-methods

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return _socket.BeginReceive(_recvBuffer, 0, BufferSize, SocketFlags.None, callback, state);
        }

        public int EndReceive(IAsyncResult result)
        {
            return _socket.EndReceive(result);
        }

        #endregion

        #region send methods

        public int Send(IEnumerable<byte> data)
        {
            if (data == null) throw new ArgumentNullException("data");
            return Send(data, SocketFlags.None);
        }

        public int Send(IEnumerable<byte> data, SocketFlags flags)
        {
            if (data == null) throw new ArgumentNullException("data");
            return _server.Send(this, data, flags);
        }

        public int Send(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            return Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        public int Send(byte[] buffer, SocketFlags flags)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            return Send(buffer, 0, buffer.Length, flags);
        }

        public int Send(byte[] buffer, int start, int count)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            return Send(buffer, start, count, SocketFlags.None);
        }

        public int Send(byte[] buffer, int start, int count, SocketFlags flags)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            return _server.Send(this, buffer, start, count, flags);
        }

        #endregion

        public void Disconnect()
        {
            if (this.IsConnected)
                _server.Disconnect(this);
        }

        public override string ToString()
        {
            if (_socket.RemoteEndPoint != null)
                return _socket.RemoteEndPoint.ToString();
            else
                return "Not Connected";
        }
    }
}
