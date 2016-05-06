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

    /// <summary>
    /// Contains an enumeration of all the Sedna instructions.
    /// </summary>
    public enum InstructionCode {

        ErrorResponse = 100,
        StartUp = 110,
        SessionParameters = 120,
        AuthenticationParameters = 130,
        SendSessionParameters = 140,
        SendAuthParameters = 150,
        AuthenticationOK = 160,
        AuthenticationFailed = 170,

        BeginTransaction = 210,
        CommitTransaction = 220,
        RollbackTransaction = 225,
        BeginTransactionOk = 230,
        BeginTransactionFailed = 240,
        CommitTransactionOk = 250,
        RollbackTransactionOk = 255,
        CommitTransactionFailed = 260,
        RollbackTransactionFailed = 265,

        Execute = 300,
        ExcuteLong = 301,
        LongQueryEnd = 302,
        GetNextItem = 310,
        QuerySucceeded = 320,
        DebugInfo = 325,
        QueryFailed = 330,
        UpdateSucceeded = 340,
        UpdateFailed = 350,
        ItemPart = 360,
        ItemEnd = 370,
        ResultEnd = 375,

        BulkLoadError = 400,
        BulkLoadPortion = 410,
        BulkLoadEnd = 420,
        BulkLoadFileName = 430,
        BulkLoadFromStream = 431,
        BulkLoadSucceeded = 440,
        BulkLoadFailed = 450,
        ShowTime = 451,
        LastQueryTime = 452,

        CloseConnection = 500,
        CloseConnectionOk = 510,
        TransactionRollbackBeforeClose = 520,
        SetSessionOptions = 530,
        SetSessionOptionsOk = 540,
        ResetSessionOptions = 550,
        ResetSessionOptionsOk = 560
    }
}