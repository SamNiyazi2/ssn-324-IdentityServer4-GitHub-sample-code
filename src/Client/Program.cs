// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        private static async Task Main()
        {
            // discover endpoints from metadata
            var client = new HttpClient();

            // 11/11/2022 10:35 pm - SSN 
            // Must run client only from Visual Studio.  Servers must be started from the command line
            // ssn_run_servers_1.cmd
            // Changed port 5001 to 6001, otherwise it will not run.

            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:6001");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            Console.WriteLine("20221111-2236 - Requesting token:");

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret",

                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);


            Console.WriteLine("20221111-2237 - Calling identity server:");

            var response = await apiClient.GetAsync("https://localhost:5201/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("20221111-2238 - Error response");
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                Console.WriteLine("20221111-2239 - Valid response");
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            Console.WriteLine("20221111-2240 - End.");
            Console.WriteLine("\n\nHit any key.");
            Console.ReadKey();
        }
    }
}