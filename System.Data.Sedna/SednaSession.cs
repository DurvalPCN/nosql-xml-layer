/*
 * Sedna .NET API - For use with the Sedna Native XML Server (http://modis.ispras.ru/sedna/index.htm)
 * 
 * Copyright (C) 2008 MindTouch, Inc.
 * www.mindtouch.com  oss@mindtouch.com
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace System.Data.Sedna {
    /// <summary>
    /// Represents a connection to the server.
    /// </summary>
    public class SednaSession : IDisposable {

        // TODO (steveb): define option constants

        //--- Constants ---
        public const int DEFAULT_SERVER_PORT = 5050;
        private const int BULKLOAD_PORTION_LENGTH = 5120;
        private const string ERROR_SESSION_IS_ALREADY_OPEN = "Sedna XML database session is already open.";
        private const string ERROR_SESSION_IS_CLOSED = "Sedna XML database session is closed.";
        private const string ERROR_TRANSACTION_IS_ALREADY_STARTED = "Sedna XML database has already an open transaction.";
        private const string ERROR_TRANSACTION_IS_NOT_STARTED = "Sedna XML database has no open transaction.";

        //--- Fields ---
        private Socket _socket;
        private NetworkStream _output;
        private NetworkStream _input;
        private bool _hasTransaction;
        private bool _hasUpdates;
        private string _database;
        private string _host;
        private int _port;
        private string _username;
        private string _password;
        private readonly TimeSpan _timeout;

        //--- Constructors ---
        public SednaSession() {
            _timeout = TimeSpan.FromSeconds(7.0);
        }

        public SednaSession(TimeSpan timeout) {
            _timeout = timeout;
        }

        ~SednaSession() {
            Dispose(false);
        }

        //--- Properties ---
        public bool IsOpen { get { return (_input != null) && (_output != null); } }
        public bool HasTransaction { get { return _hasTransaction; } }
        public bool HasUpdates { get { return _hasUpdates; } }

        //--- Methods ---

        /// <summary>
        /// Open connection to a Sedna XML Database server.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void Start(string host, int port, string database, string username, string password) {
            if(string.IsNullOrEmpty(database)) {
                throw new ArgumentNullException("database");
            }
            if(string.IsNullOrEmpty(host)) {
                throw new ArgumentNullException("host");
            }
            if(string.IsNullOrEmpty(username)) {
                throw new ArgumentNullException("username");
            }
            if(string.IsNullOrEmpty(password)) {
                throw new ArgumentNullException("password");
            }
            if(IsOpen) {
                throw new InvalidOperationException(ERROR_SESSION_IS_ALREADY_OPEN);
            }

            // store settings
            _database = database;
            _host = host;
            _port = port;
            _username = username;
            _password = password;
            _hasTransaction = false;
            _hasUpdates = false;

            try {
                // open connection to server
                try {
                   // _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                   // _socket = new Socket(IPAddress.IPv6Loopback.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                 //   _socket.Connect(new IPEndPoint(Dns.GetHostEntry(_host).AddressList[0], _port));


                    IPAddress address = IPAddress.Parse("127.0.0.1");
                    IPEndPoint ipe = new IPEndPoint(address, _port); Console.WriteLine("IP: " + ipe);
                    _socket =
                        new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    _socket.Connect(ipe);



                } catch(IOException ex) {
                    throw new ConnectionExcpetion(string.Format("Unable to connect to host {0}:{1}.", _host, _port), ex);
                }
                _output = new NetworkStream(_socket, FileAccess.Write);
                _input = new NetworkStream(_socket, FileAccess.Read);

                // send startup message: StartUp -> ErrorResponse | SendSessionParameters
                Send(InstructionCode.StartUp);
                Receive(true, InstructionCode.SendSessionParameters);

                // send session parameters: SessionParameters -> ErrorResponse | SendAuthParameters
                SendSessionParameters(3, 0, _username, _database);
                Receive(true, InstructionCode.SendAuthParameters);

                // send credentials: AuthenticationParameters -> AuthenticationFailed | AuthenticationOk
                SendData(InstructionCode.AuthenticationParameters, _password);
                Receive(true, InstructionCode.AuthenticationOK, InstructionCode.AuthenticationFailed);
            } catch {
                Dispose(true);
                throw;
            }
        }

        /// <summary>
        /// Terminate a session, rolling back any transactions that have not been committed.
        /// </summary>
        public void Close() {
            if(!IsOpen) {
                return;
            }
            try {

                // send close connection message: CloseConnection -> CloseConnectionOk | TransactionRollbackBeforeClose
                Send(InstructionCode.CloseConnection);
                Receive(true, InstructionCode.CloseConnectionOk, InstructionCode.TransactionRollbackBeforeClose);
            } finally {
                Dispose(true);
            }
        }

        /// <summary>
        /// Set one or more session options.
        /// </summary>
        /// <param name="options">Array of KeyValue&ltint, string&gt options.</param>
        public void SetOptions(params KeyValuePair<int, string>[] options) {
            if(!IsOpen) {
                throw new InvalidOperationException(ERROR_SESSION_IS_CLOSED);
            }
            if((options == null) || (options.Length == 0)) {
                return;
            }
            try {

                // send options message: SetSessionOptions -> SetSessionOptionsOk
                SendOptions(options);
                Receive(true, InstructionCode.SetSessionOptionsOk);
            } catch {
                Dispose(true);
                throw;
            }
        }

        /// <summary>
        /// Reset session options to default settings.
        /// </summary>
        public void ResetOptions() {
            if(!IsOpen) {
                throw new InvalidOperationException(ERROR_SESSION_IS_CLOSED);
            }
            try {

                // send reset options message: ResetSessionOptions -> ResetSessionOptionsOk
                Send(InstructionCode.ResetSessionOptions);
                Receive(true, InstructionCode.ResetSessionOptionsOk);
            } catch {
                Dispose(true);
                throw;
            }
        }

        /// <summary>
        /// Begins a new transaction manually
        /// </summary>
        public void BeginTransaction() {
            if(!IsOpen) {
                throw new InvalidOperationException(ERROR_SESSION_IS_CLOSED);
            }
            if(_hasTransaction) {
                throw new InvalidOperationException(ERROR_TRANSACTION_IS_ALREADY_STARTED);
            }
            try {

                // send transaction message: BeginTransaction -> BeginTransactionOk | BeginTransactionFailed
                Send(InstructionCode.BeginTransaction);
                Receive(true, InstructionCode.BeginTransactionOk, InstructionCode.BeginTransactionFailed);
                _hasUpdates = false;
                _hasTransaction = true;
            } catch {
                Dispose(true);
                throw;
            }
        }

        /// <summary>
        /// Rollback the current transaction.
        /// </summary>
        public void RollbackTransaction() {
            if(!IsOpen) {
                throw new InvalidOperationException(ERROR_SESSION_IS_CLOSED);
            }
            if(!_hasTransaction) {
                throw new InvalidOperationException(ERROR_TRANSACTION_IS_NOT_STARTED);
            }
            try {

                // send rollback transaction message: RollbackTransaction -> RollbackTransactionOk | RollbackTransactionFailed
                Send(InstructionCode.RollbackTransaction);
                Receive(true, InstructionCode.RollbackTransactionOk, InstructionCode.RollbackTransactionFailed);
                _hasTransaction = false;
                _hasUpdates = false;
            } catch {
                Dispose(true);
                throw;                
            }
        }

        /// <summary>
        /// Commit the current transaction
        /// </summary>
        public bool CommitTransaction() {
            if(!IsOpen) {
                throw new InvalidOperationException(ERROR_SESSION_IS_CLOSED);
            }
            if(!_hasTransaction) {
                throw new InvalidOperationException(ERROR_TRANSACTION_IS_NOT_STARTED);
            }
            try {
                _hasTransaction = false;
                _hasUpdates = false;

                // send commit transaction message: CommitTransaction -> CommitTransactionOk | CommitTransactionFailed
                Send(InstructionCode.CommitTransaction);
                return Receive(false, InstructionCode.CommitTransactionOk, InstructionCode.CommitTransactionFailed).Instruction == InstructionCode.CommitTransactionOk;
            } catch {
                Dispose(true);
                throw;
            }
        }

        /// <summary>
        /// Execute a query, update, or bulk load operation.
        /// </summary>
        public SednaQueryResults Execute(string query) {
            if(!IsOpen) {
                throw new InvalidOperationException(ERROR_SESSION_IS_CLOSED);
            }

            // TODO (steveb): use long query format if query exceed max buffer size, which is 10240

            // Execute | ExecuteLong+ LongQueryEnd -> QuerySucceeded | QueryFailed | DebugInfo* UpdateSucceeded | DebugInfo* UpdateFailed | BulkLoadFileName+ | BulkLoadFromStream
            // BulkLoadError -> (none)

            // check if we need to start a transaction
            bool autoTransaction = false;
            if(!HasTransaction) {
                autoTransaction = true;
                BeginTransaction();
            }

            // execute query operation
            try {
                SendByteAndData(InstructionCode.Execute, (byte)SednaXmlFormat.XML, query);
                SednaMessage response = Receive(
                    true, 
                    InstructionCode.QuerySucceeded, 
                    InstructionCode.QueryFailed,
                    InstructionCode.UpdateSucceeded, 
                    InstructionCode.UpdateFailed,
                    InstructionCode.BulkLoadFileName, 
                    InstructionCode.BulkLoadFromStream, 
                    InstructionCode.DebugInfo
                );
                switch(response.Instruction) {
                case InstructionCode.QuerySucceeded:
                    return new SednaQueryResults(this);
                case InstructionCode.UpdateSucceeded:
                    _hasUpdates = true;
                    return null;
                case InstructionCode.BulkLoadFileName:
                case InstructionCode.BulkLoadFromStream:

                    // TODO (steveb): are bulk loads transacted?
                    _hasUpdates = true;

                    BulkLoad(response);
                    return null;
                case InstructionCode.DebugInfo:

                    // TODO (steveb): what to do?
                    throw new NotImplementedException();
                default:

                    // TODO (steveb): protocol violation; should never happen!
                    throw new ProtocolViolationException("Unexpected error");
                }
            } catch(Exception) {
                _hasTransaction = false;
                _hasUpdates = false;
                throw;
            }

            // TODO (steveb): automatically commit transacations that were opened automatically
        }

        internal void Send(InstructionCode instruction) {
            StreamUtil.WriteMessage(_output, instruction, null);
        }

        internal void SendCodeAndInfo(InstructionCode instruction, int code, string info) {
            MemoryStream buffer = new MemoryStream();
            StreamUtil.WriteInt(buffer, code);
            StreamUtil.WriteString(buffer, info);
            StreamUtil.WriteMessage(_output, instruction, buffer);
        }

        internal void SendData(InstructionCode instruction, string data) {
            MemoryStream buffer = new MemoryStream();
            StreamUtil.WriteString(buffer, data);
            StreamUtil.WriteMessage(_output, instruction, buffer);
        }

        internal void SendByteAndData(InstructionCode instruction, byte code, string data) {
            MemoryStream buffer = new MemoryStream();
            buffer.WriteByte(code);
            StreamUtil.WriteString(buffer, data);
            StreamUtil.WriteMessage(_output, instruction, buffer);
        }

        internal void SendSessionParameters(byte major, byte minor, string username, string database) {
            MemoryStream buffer = new MemoryStream();
            buffer.WriteByte(major);
            buffer.WriteByte(minor);
            StreamUtil.WriteString(buffer, username);
            StreamUtil.WriteString(buffer, database);
            StreamUtil.WriteMessage(_output, InstructionCode.SessionParameters, buffer);
        }

        internal void SendOptions(params KeyValuePair<int, string>[] options) {
            MemoryStream buffer = new MemoryStream();
            foreach(KeyValuePair<int, string> option in options) {
                StreamUtil.WriteInt(buffer, option.Key);
                StreamUtil.WriteString(buffer, option.Value);
            }
            StreamUtil.WriteMessage(_output, InstructionCode.SetSessionOptions, buffer);
        }

        internal SednaMessage Receive(bool throwOnError, params InstructionCode[] accepted) {
            int code;
            string info;

            // read response instruction code & body length
            Stream stream = _input;
            int instruction = StreamUtil.ReadInt(stream, _timeout);
            int length = StreamUtil.ReadInt(stream, _timeout);

            // memorize stream
            if(length > 0) {
                stream = new MemoryStream(StreamUtil.ReadArray(stream, length, _timeout));
            }

            // check if response is acceptable
            if((instruction != (int)InstructionCode.ErrorResponse) && (accepted != null) && (accepted.Length > 0)) {
                bool found = false;
                foreach(InstructionCode acceptedInstruction in accepted) {
                    if(instruction == (int)acceptedInstruction) {
                        found = true;
                        break;
                    }
                }
                if(!found) {

                    // TODO (steveb): throw appropriate exception

                    throw new ProtocolViolationException();
                }
            }

            // process response
            switch(instruction) {

                // body: empty
            case (int)InstructionCode.SendSessionParameters:
            case (int)InstructionCode.SendAuthParameters:
            case (int)InstructionCode.AuthenticationOK:
            case (int)InstructionCode.BeginTransactionOk:
            case (int)InstructionCode.CommitTransactionOk:
            case (int)InstructionCode.RollbackTransactionOk:
            case (int)InstructionCode.QuerySucceeded:
            case (int)InstructionCode.UpdateSucceeded:
            case (int)InstructionCode.ItemEnd:
            case (int)InstructionCode.ResultEnd:
            case (int)InstructionCode.BulkLoadFromStream:
            case (int)InstructionCode.BulkLoadSucceeded:
            case (int)InstructionCode.CloseConnectionOk:
            case (int)InstructionCode.TransactionRollbackBeforeClose:
            case (int)InstructionCode.SetSessionOptionsOk:
            case (int)InstructionCode.ResetSessionOptionsOk:
                if(length != 0) {
                    throw new ProtocolViolationException(string.Format("Sedna Server response was {0} ({1}) with body length {2}, but body was expected to be empty.", (InstructionCode)instruction, instruction, length));
                }
                return new SednaSuccessMessage((InstructionCode)instruction);

                // body: code : int, info : string
            case (int)InstructionCode.AuthenticationFailed:
            case (int)InstructionCode.ErrorResponse:
            case (int)InstructionCode.BeginTransactionFailed:
            case (int)InstructionCode.CommitTransactionFailed:
            case (int)InstructionCode.RollbackTransactionFailed:
            case (int)InstructionCode.QueryFailed:
            case (int)InstructionCode.UpdateFailed:
            case (int)InstructionCode.BulkLoadFailed:
                code = StreamUtil.ReadInt(stream, _timeout);
                info = StreamUtil.ReadString(stream, _timeout);
                if(throwOnError) {
                    throw new SednaErrorException((InstructionCode)instruction, code, info);
                }
                return new SednaErrorMessage((InstructionCode)instruction, code, info);

            case (int)InstructionCode.DebugInfo:
                code = StreamUtil.ReadInt(stream, _timeout);
                info = StreamUtil.ReadString(stream, _timeout);
                return new SednaDebugInfoMessage(code, info);

                // body: data : string
            case (int)InstructionCode.ItemPart:
            case (int)InstructionCode.BulkLoadFileName:
            case (int)InstructionCode.LastQueryTime:
                info = StreamUtil.ReadString(stream, _timeout);
                return new SednaDataMessage((InstructionCode)instruction, info);

                // unrecognized server response
            default:
                throw new ProtocolViolationException(string.Format("Unrecognized Sedna Server response code {0} with body length {1}.", instruction, length));
            }
        }

        private void BulkLoad(SednaMessage msg) {

            // BulkLoadPortion+ BulkLoadEnd -> BulkLoadSucceeded | BulkLoadFailed

            // open the source stream
            TextReader reader;
            switch(msg.Instruction) {
            case InstructionCode.BulkLoadFileName:
                reader = File.OpenText(((SednaDataMessage)msg).Info);
                break;
            case InstructionCode.BulkLoadFromStream:
                reader = Console.In;
                break;
            default:

                // TODO (steveb): throw a more meaningful exception

                throw new Exception();
            }

            // read data to send
            string data;
            try {
                data = reader.ReadToEnd();
            } catch(IOException e) {
                try {

                    // TODO (steveb): what error message do we need to send here?
                    SendCodeAndInfo(InstructionCode.BulkLoadError, 0, string.Empty);
                } catch(Exception ex) {

                    // TODO (steveb): not sure what to do?!?
                    //throw new SednaErrorException(InstructionCode.BulkLoadFailed, "Unable to bulk load: IO error: " + ex.Message);
                    throw new ProtocolViolationException();
                }

                // TODO (steveb): what responses should we expect?
                msg = Receive(true);

                // TODO (steveb): should never get here since we always expect an error!
                //throw new SednaException(InstructionCode.BulkLoadFailed, msg.GetException() + " : " + e.Message);

                throw new ProtocolViolationException();
            }

            // transfer bulk content            
            for(int position = 0, length; position < data.Length; position += length) {
                length = Math.Min(data.Length - position, BULKLOAD_PORTION_LENGTH);
                SendData(InstructionCode.BulkLoadPortion, data.Substring(position, length));
            }

            // send end-of-content marker
            Send(InstructionCode.BulkLoadEnd);
            Receive(true, InstructionCode.BulkLoadSucceeded, InstructionCode.UpdateSucceeded);
        }

        private void Dispose(bool disposing) {
            _database = null;
            _host = null;
            _port = 0;
            _username = null;
            _password = null;
            _hasTransaction = false;
            _hasUpdates = false;
            try {
                if(disposing) {
                    if(_input != null) {
                        _input.Dispose();
                    }
                    if(_output != null) {
                        _output.Dispose();
                    }
                    if(_socket != null) {
                        _socket.Close();
                    }
                }
            } finally {
                _input = null;
                _output = null;
                _socket = null;
            }
        }

        //--- Interface Methods ---
        void IDisposable.Dispose() {
            Dispose(true);
        }
    }
}