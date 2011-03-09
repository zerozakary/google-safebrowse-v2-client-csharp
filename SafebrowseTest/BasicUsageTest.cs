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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using SafebrowseV2;

//TODO: Fill in API key in project settings
namespace SafebrowseTest
{
    [TestClass]
    public class SafebrowseTest
    {

        private string cacheBaseDir = null;

        [TestMethod]
        public void TestBasicUsage()
        {
            ReputationEngine rep = new ReputationEngine();

            //cacheBaseDir will look something like "C:\\src\\SafebrowseCSharp\\SafeBrowsingCacheList", and should 
            //point to the SafeBrowsingCacheList*.bin files included with this test. Initialize() will attempt to do that 
            //automatically if it's not already set.
            rep.Initialize(Properties.Settings.Default.SafebrowseV2ApiKey, cacheBaseDir, 1800);

            Assert.AreEqual(SafebrowseV2.Reputation.None, rep.CheckUrl("http://google.com"));
            Assert.AreEqual(SafebrowseV2.Reputation.MalwareBlackList, rep.CheckUrl("http://gumblar.cn"));
            Assert.AreEqual(SafebrowseV2.Reputation.MalwareBlackList, rep.CheckUrl("http://goooogleadsence.biz/"));
        }

        [TestInitialize()]
        public void Initialize()
        {
            // This call registers the event source.
            // This call requires elevation, otherwise it will throw an exception.
            // It only needs to be done once per machine.
            // It should be done before running the reputation service (but not immediately before).
            //ReputationEventing reputationEventing = new ReputationEventing();
            //reputationEventing.InitializeEventing();

            // This call unregisters the event source.
            // This call requires elevation, otherwise it will throw an exception.
            // It only needs to be done once per machine.
            // It should be done after quitting the reputation service (but not immediately after).
            //reputationEventing.ShutdownEventing();

            if (cacheBaseDir == null)
            {
                //Work upward to find the directory with the cached lists (SafeBrowsingCacheList[0-4].bin) so that we have valid data when we start up
                string cwd = System.IO.Directory.GetCurrentDirectory();
                System.IO.DirectoryInfo dir = System.IO.Directory.GetParent(cwd);
                while (!dir.GetFiles().Any(file => file.Name.Equals("SafeBrowsingCacheList0.bin")))
                {
                    Assert.IsNotNull(dir.Parent, "Failed to find SafeBrowsingCacheList*.bin files which are expected to be in the solution root directory. Unit tests require cached data.");
                    dir = dir.Parent;
                }
                cacheBaseDir = dir.FullName;
            }

            Assert.IsFalse(string.IsNullOrEmpty(Properties.Settings.Default.SafebrowseV2ApiKey),
                "SafebrowseV2ApiKey must be provided in project Properties.");
        }

    }
}
