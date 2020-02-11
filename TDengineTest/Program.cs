/*
 * Copyright (c) 2019 TAOS Data, Inc. <jhtao@taosdata.com>
 *
 * This program is free software: you can use, redistribute, and/or modify
 * it under the terms of the GNU Affero General Public License, version 3
 * or later ("AGPL"), as published by the Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections;
using TDengine2Net;
namespace TDengineDriver
{
    class Program
    {
        static void Main(string[] args)
        {
                  
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine("Starting Testing...");
            Console.WriteLine("---------------------------------------------------------------");
            TDengine td = new TDengine();
            td.Connect("49.234.110.126", 6035, "root", "taosdata", "");
            //td.Connect("192.168.1.101", 6030, "root", "taosdata", "demo");

            td.CreateDB("demo");
            td.UserDB("demo");
                      
            td.CreateTable("t2", new TSDB_FIELD[] { new TSDB_FIELD("st", TSDB_DATA_TYPE.TIMESTAMP), new TSDB_FIELD("note", TSDB_DATA_TYPE.BINARY,50) });
            
            td.NonQuery(string.Format("insert into t2 values ('{0}', 'note99');" ,DateTime.UtcNow));

            td.QueryToDataTable("select * from demo.t2" );
            Console.WriteLine(td.QueryToJson("select count(note) from demo.t2"));
            //td.QueryToDataTable("SHOW DATABASES");
            //td.Query("select avg(st), max(st), min(st) from demo.t2");

            Console.ReadKey();
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine("Stop Testing...");
            Console.WriteLine("---------------------------------------------------------------");

        }
    }
   
 }

