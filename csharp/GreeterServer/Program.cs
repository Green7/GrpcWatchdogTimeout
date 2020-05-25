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
using Helloworld;
using System;
using System.Threading.Tasks;

namespace GreeterServer
{
	internal class GreeterImpl : Greeter.GreeterBase
	{
		private static readonly string _letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

		private static readonly Random _rng = new Random();

		// Server side handler of the SayHello RPC
		public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
		{
			Console.WriteLine($"{DateTime.Now} Received: {request.Name}");

			return Task.FromResult(new HelloReply { Message = RandString(1024 * 100) });
		}

		private string RandString(int length)
		{
			var buffer = new char[length];

			for (var i = 0; i < length; i++)
			{
				buffer[i] = _letters[_rng.Next(_letters.Length)];
			}

			return new string(buffer);
		}
	}

	internal class Program
	{
		private const string Host = "0.0.0.0";

		private const int Port = 50051;

		public static void Main(string[] args)
		{
			Server server = new Server
			{
				Services = { Greeter.BindService(new GreeterImpl()) },
				Ports = { new ServerPort(Host, Port, ServerCredentials.Insecure) }
			};
			server.Start();

			Console.WriteLine("Greeter server listening on port " + Port);
			Console.WriteLine("Press any key to stop the server...");
			Console.ReadKey();

			server.ShutdownAsync().Wait();
		}
	}
}