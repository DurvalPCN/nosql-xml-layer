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

namespace System.Data.Sedna {

    // TODO (steveb): needed exceptions
    //  * fatal exception
    //  * invalid operation exception
    //  * protocol violation

    public enum SednaXmlFormat : byte {
        XML     = 0,
        SCHEME  = 1
    }

    public class SednaErrorException : Exception {

        //--- Fields ---
        public readonly InstructionCode Instruction;
        public readonly int Code;
        public readonly string Info;

        //--- Constructors ---
        public SednaErrorException(InstructionCode instruction, int code, string info) {
            this.Instruction = instruction;
            this.Code = code;
            this.Info = info;
        }

        //--- Properties ---
        public override string Message {
            get {
                return string.Format("Sedna Server response was {0} ({1}) with code {2}: {3}.", Instruction, (int)Instruction, Code, Info);
            }
        }
    }

    public class ConnectionExcpetion : Exception {

        //--- Constructors ---
        public ConnectionExcpetion(string message) : base(message) { }
        public ConnectionExcpetion(string message, Exception innerException) : base(message, innerException) { }
    }
}