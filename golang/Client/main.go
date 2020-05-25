/*
 *
 * Copyright 2015 gRPC authors.
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
 *
 */

// Package main implements a client for Greeter service.
package main

import (
	"context"
	"log"
	"strconv"
	"time"

	pb "go-client/helloworld"

	"google.golang.org/grpc"
	"google.golang.org/grpc/keepalive"
)

const (
	address = "192.168.248.115:50051"
)

func main() {
	// Set up a connection to the server.
	timeout := time.Second * 3
	options := []grpc.DialOption{
		grpc.WithBlock(),
		grpc.WithInsecure(),
		grpc.WithKeepaliveParams(keepalive.ClientParameters{
			Time:                timeout,
			Timeout:             timeout,
			PermitWithoutStream: true,
		})}

	ctx, _ := context.WithTimeout(context.Background(), timeout)
	conn, err := grpc.DialContext(ctx, address, options...)
	if err != nil {
		log.Fatalf("did not connect: %v", err)
	}
	defer conn.Close()
	c := pb.NewGreeterClient(conn)

	for i := 0; i < 100; i++ {
		_, err := c.SayHello(context.Background(), &pb.HelloRequest{Name: strconv.Itoa(i)})
		if err != nil {
			log.Fatalf("could not greet: %v", err)
		}
		log.Printf("Sent: %d", i)
	}

}
