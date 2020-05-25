// Copyright 2015 gRPC authors.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on
// an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.

using Grpc.Core;
using Grpc.Core.Logging;
using Helloworld;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreeterClient
{
	internal class Program
	{
		private const string Host = "192.168.248.115:50051";

		public static async Task Main(string[] args)
		{
			GrpcEnvironment.SetLogger(new ConsoleLogger());
			Environment.SetEnvironmentVariable("GRPC_TRACE", "http_keepalive");
			Environment.SetEnvironmentVariable("GRPC_VERBOSITY", "DEBUG");

			int timeout = 3000;
			var options = new List<ChannelOption>
			{
				new ChannelOption("grpc.keepalive_time_ms", timeout),
				new ChannelOption("grpc.keepalive_timeout_ms", timeout),
				new ChannelOption("grpc.keepalive_permit_without_calls", 1),
				new ChannelOption("grpc.http2.max_pings_without_data", 0),
				new ChannelOption("grpc.http2.min_time_between_pings_ms", timeout)
			};

			Channel channel = new Channel(Host, ChannelCredentials.Insecure, options);

			var client = new Greeter.GreeterClient(channel);
			for (int i = 0; i < 100; i++)
			{
				var reply = await client.SayHelloAsync(new HelloRequest { Name = $"{i}" });
				Console.WriteLine($"{DateTime.Now} Sent: {i}");
			}

			channel.ShutdownAsync().Wait();

			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}
	}
}