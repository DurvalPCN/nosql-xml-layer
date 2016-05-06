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

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace System.Data.Sedna {
    internal static class StreamUtil {

        //--- Constants ---
        private const int MAX_MESSAGE_SIZE = 10240;
        private const int SLEEP_WAIT_CYCLE = 100;

        //--- Class Methods ---
        internal static int ReadInt(Stream stream, TimeSpan timeout) {
            byte[] buffer = ReadArray(stream, 4, timeout);
            return (int)(((uint)buffer[0] << 24) | ((uint)buffer[1] << 16) | ((uint)buffer[2] << 8) | buffer[3]);
        }

        internal static string ReadString(Stream stream, TimeSpan timeout) {

            // validate string type
            byte[] type = ReadArray(stream, 1, timeout);
            if(type[0] != 0) {
                throw new ProtocolViolationException(string.Format("Sedna Server response: expected type 0 string (C-style), but received type {0}.", type[0]));
            }

            // read string length
            int length = ReadInt(stream, timeout);
            if(length > MAX_MESSAGE_SIZE) {
                throw new ProtocolViolationException(string.Format("Sedna Server response: a length of {0} is too long for a string value.", length));
            }

            // read string contents
            string result = Encoding.UTF8.GetString(ReadArray(stream, length, timeout));
            return result;
        }

        internal static byte[] ReadArray(Stream stream, int length, TimeSpan timeout) {
            if(length > MAX_MESSAGE_SIZE) {
                throw new ArgumentException(string.Format("Requested lenght of {0} is larger than the allowed maximum of {1}.", length, MAX_MESSAGE_SIZE), "length");                
            }

            // read the data from the stream
            byte[] result = new byte[length];
            double remainder = timeout.TotalMilliseconds;
            int pos = 0;
            while((pos < length) && (remainder > 0)) {

                // TODO (steveb): this is ugly, ugly code; why can't we timeout using an asynchronous read instead?

                // check if stream is a network stream; if so, check if the data is available, otherwise sleep
                if((stream is NetworkStream) && !((NetworkStream)stream).DataAvailable) {
                    System.Threading.Thread.Sleep(SLEEP_WAIT_CYCLE);
                    remainder -= SLEEP_WAIT_CYCLE;
                } else {
                    int count = stream.Read(result, pos, length - pos);
                    remainder = timeout.TotalMilliseconds;
                    pos += count;
                }
            }
            if(remainder <= 0) {
                throw new ConnectionExcpetion(string.Format("Sedna Server failed to respond within {0}ms.", (int)timeout.TotalMilliseconds));
            }
            return result;
        }

        internal static void WriteInt(Stream stream, int value) {
            byte[] buffer = new byte[4];
            buffer[0] = (byte)(value >> 24);
            buffer[1] = (byte)(value >> 16);
            buffer[2] = (byte)(value >> 8);
            buffer[3] = (byte)value;
            stream.Write(buffer, 0, buffer.Length);
        }

        internal static void WriteString(Stream stream, string value) {
            byte[] buffer = Encoding.UTF8.GetBytes(value);
            stream.WriteByte(0); /* type 0 (C-Style) string */
            WriteInt(stream, buffer.Length);
            stream.Write(buffer, 0, buffer.Length);
        }

        internal static void WriteMessage(Stream stream, InstructionCode instruction, MemoryStream buffer) {
            WriteInt(stream, (int)instruction);
            if(buffer != null) {
                WriteInt(stream, (int)buffer.Length);
                buffer.WriteTo(stream);
            } else {
                WriteInt(stream, 0);
            }
            stream.Flush();
        }
    }
}