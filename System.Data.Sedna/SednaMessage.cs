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
    public abstract class SednaMessage {

        //--- Fields ---
        public readonly InstructionCode Instruction;
        
        //--- Constructors ---
        protected SednaMessage(InstructionCode instruction) {
            this.Instruction = instruction;
        }
    }

    public class SednaSuccessMessage : SednaMessage {

        //--- Constructors ---
        public SednaSuccessMessage(InstructionCode instruction) : base(instruction) { }
    }

    public class SednaErrorMessage : SednaMessage {

        //--- Fields ---
        public readonly int Code;
        public readonly string Info;

        //--- Constructors ---
        public SednaErrorMessage(InstructionCode instruction, int code, string info)
            : base(instruction) {
            this.Code = code;
            this.Info = info;
        }
    }

    public class SednaDebugInfoMessage : SednaMessage {

        //--- Fields ---
        public readonly int Code;
        public readonly string Info;

        //--- Constructors ---
        public SednaDebugInfoMessage(int code, string info)
            : base(InstructionCode.DebugInfo) {
            this.Code = code;
            this.Info = info;
        }
    }

    public class SednaDataMessage : SednaMessage {

        //--- Fields ---
        public readonly string Info;

        //--- Constructors ---
        public SednaDataMessage(InstructionCode instruction, string info)
            : base(instruction) {
            this.Info = info;
        }
    }
}