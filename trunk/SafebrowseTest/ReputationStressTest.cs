/*****************************************************************************
 * Licensed to Qualys, Inc. (QUALYS) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * QUALYS licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *****************************************************************************/

// Author: Brian Cheek (Qualys) 8/2010
// Maintainer & Support Contact: Patrick Thomas (Qualys), pthomas@qualys.com 

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

using SafebrowseV2;

namespace SafebrowseTest
{
    [TestClass]
    public class ReputationStressTest
    {
        public static string[] SeedUrls;
        public static int Queries = 0;
        public static int BadUrls = 0;

        public static int NumHosts = 10000;
        public static bool StripPath = false;

        public static ReputationEngine re = new ReputationEngine();
        private string cacheBaseDir = null;
        
        [TestMethod]
        public void StressTestLocal()
        {
            re.Initialize(Properties.Settings.Default.SafebrowseV2ApiKey, cacheBaseDir, 300);

            List<string> hosts = new List<string>();
            Random r = new Random();

            for (int i = 0; i < NumHosts; ++i)
            {
                if (StripPath)
                {
                    Uri u = new Uri(SeedUrls[r.Next(0, SeedUrls.Length)]);
                    hosts.Add("http://" + u.Host);
                }
                else
                {
                    hosts.Add(SeedUrls[r.Next(0, SeedUrls.Length)]);
                }
            }

            DateTime start = DateTime.Now;
            re.CheckUrl(hosts);
            DateTime end = DateTime.Now;
            
            double checkTime = (end - start).TotalSeconds;
            Assert.IsTrue(checkTime < 1, "A check of " + NumHosts + " urls from UrlPool.dat should take less than 1 second. Actually took " + checkTime);
        }

        [TestInitialize()]
        public void Initialize()
        {
            Assert.IsFalse(string.IsNullOrEmpty(Properties.Settings.Default.SafebrowseV2ApiKey),
                "SafebrowseV2ApiKey must be provided in Properties.");

            //Work upward from default working dir to find the root of the solution directory, which contains testing data and also persisted safebrowse lists (SafeBrowsingCacheList[0-4].bin)
            string cwd = System.IO.Directory.GetCurrentDirectory();
            System.IO.DirectoryInfo dir = System.IO.Directory.GetParent(cwd);
            while (!dir.Name.Equals("SafebrowseCSharp"))
            {
                dir = dir.Parent;
            }

            if (cacheBaseDir == null)
            {
                cacheBaseDir = dir.FullName;
                Assert.IsTrue(dir.GetFiles().Any(file => file.Name.Equals("SafeBrowsingCacheList0.bin")), "Expected solution directory to contain SafeBrowsingCacheList*.bin files. Unit tests require cached data.");
            }

            //populate seed urls
            Assert.IsTrue(dir.GetFiles().Any(file => file.Name.Equals("UrlPool.dat")));
            string[] lines = File.ReadAllLines(dir.FullName + "\\UrlPool.dat");
            for (int i = 0; i < lines.Length; ++i)
            {
                lines[i] = "http://" + lines[i];
            }
            SeedUrls = lines;
            Assert.IsTrue(SeedUrls.Length > 25000);
        }
    }
}
