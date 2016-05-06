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

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System.Data.Sedna {
    public class SednaQueryResults : IEnumerable<string>, IEnumerator<string> {
        
        //--- Fields ---
        private SednaSession _session;
        private string _current;
        private string _debug;
        private InstructionCode _last;

        //--- Constructors ---
        internal SednaQueryResults(SednaSession session) {
            if(session == null) {
                throw new ArgumentNullException("session");
            }
            _session = session;
            _last = InstructionCode.QuerySucceeded;
        }

        //--- Properties ---
        public bool HasResult {
            get {
                return _current != null;
            }
        }

        public string Current {
            get {
                if(!HasResult) {

                    // TODO (steveb): better error message
                    throw new InvalidOperationException();
                }
                return _current;
            }
        }

        public string CurrentDebugInfo {
            get {
                if(!HasResult) {

                    // TODO (steveb): better error message
                    throw new InvalidOperationException();
                }
                return _debug;
            }
        }

        //--- Methods ---
        public bool MoveNext() {
            _current = null;
            _debug = null;

            // check if any results are left
            if(_last == InstructionCode.ResultEnd) {
                return false;
            }
            try {

                // TODO (steveb): check if the session hasn't change state since the query was issued

                // check if this is NOT the first request
                if(_last != InstructionCode.QuerySucceeded) {

                    // request for the next result to be sent
                    _session.Send(InstructionCode.GetNextItem);
                }

                // TODO (steveb): read and store debug info

                // read the next result
                StringBuilder buffer = new StringBuilder();
                SednaMessage msg = _session.Receive(true, InstructionCode.ItemPart, InstructionCode.ItemEnd, InstructionCode.ResultEnd);
                while(msg.Instruction == InstructionCode.ItemPart) {
                    buffer.Append(((SednaDataMessage)msg).Info);
                    msg = _session.Receive(true, InstructionCode.ItemPart, InstructionCode.ItemEnd, InstructionCode.ResultEnd);
                }
                _current = buffer.ToString();
                _last = msg.Instruction;
            } catch {
                Dispose(true);
                _last = InstructionCode.ErrorResponse;
                throw;
            }
            return true;
        }

        private void Dispose(bool error) {
            _session = null;
            _current = null;
            _debug = null;
            _last = error ? InstructionCode.ErrorResponse : InstructionCode.ResultEnd;
        }

        //--- Interface Properties ---
        object IEnumerator.Current { get { return Current; } }

        //--- Interface Methods ---
        IEnumerator<string> IEnumerable<string>.GetEnumerator() { return this; }
        IEnumerator IEnumerable.GetEnumerator() { return this; }

        void IEnumerator.Reset() {
            Dispose(true);

            // TODO (steveb): better error message
            throw new InvalidOperationException();
        }

        void IDisposable.Dispose() {
            Dispose(false);
        }
    }
}