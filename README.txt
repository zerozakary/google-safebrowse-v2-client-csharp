

GOOGLE SAFEBROWSE API C# CLIENT

This is a C# client for the Google Safe Browsing v2.2 protocol (http://code.google.com/p/google-safe-browsing/)

PROVIDED FILES
----------------
- Visual Studio Solution 2010 Solution (SafebrowseV2.sln)
- SafebrowseV2 source project and unit test project (Safebrowsev2/*, SafebrowseTest/*)
- Persisted copy of a set of Safebrowse chunk updates for testing purposes (SafebrowsingCacheList[0-4].bin)
- A set of URLs for stress testing the implementation (UrlPool.dat)



USAGE
----------------
- The client is provided as a library: SafebrowseV2.dll. 
- See BasicUsageTest.cs for an executable usage sample. 

Most users should be able to get started by doing:
/***********************/
using SafebrowseV2;

<...snip...>
            ReputationEngine rep = new ReputationEngine();
            
            //cacheBaseName will look something like "C:\\src\\SafebrowseCSharp\\SafeBrowsingCacheList", and should 
            //point to the SafeBrowsingCacheList*.bin files included with this test, or . Initialize() will attempt to do that 
            //automatically if it's not already set.
			string safebrowseApiKey = ""; //PUT KEY HERE
			string cacheBaseName = "C:\\src\\SafebrowseCSharp\\SafeBrowsingCacheList"
			int pollIntervalInSeconds = 1800;
            rep.Initialize(safebrowseApiKey, cacheBaseName, 1800);

            Assert.AreEqual(SafebrowseV2.Reputation.None, rep.CheckUrl("http://google.com"));
/***********************/



TESTS:
----------------
- The SafebrowseTests project should run normally from Visual Studio
- Be sure to provide a value for SafebrowseV2ApiKey in the project settings.



IMPLEMENTATION NOTES:
----------------
- This implementation fetches list updates in a seperate thread every pollIntervalSeconds
- This implementation currently reports all errors through the Windows Application Event Log, with source="ReputationService". In order to enable logging, the ReputationEvening service should be initialized. See SafebrowseV2.ReputationEventing.
- This implementation has the ability to persist and restore chunks used between runs in order to reduce the time to obtain a complete list. The DefaultCacheDir should point to the directory containing these files. They are required for the unit tests to work, but the example files SHOULD NOT be used in production. 
- This implementation does not currently support Message Authentication Codes (MAC)



AUTHORS
----------------
Written By: Brian Cheek (bcheek@qualys.com)
Support Contact: Patrick Thomas (pthomas@qualys.com)







