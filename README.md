# NRedisGraph

[![Build Status](https://github.com/tombatron/NRedisGraph/actions/workflows/dotnet.yml/badge.svg)](https://github.com/tombatron/NRedisGraph/actions/workflows/dotnet.yml)

## Overview

NRedisGraph is a series of extensions methods for the [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) library that will enable you to interact with the [Redis](https://redis.io) module [RedisGraph](https://oss.redislabs.com/redisgraph/). This is made possible by the `Execute` and `ExecuteAsync` methods already present in the StackExchange.Redis library. 

The intent with this library is to duplicate the API (as much as possible) of the [JRedisGraph](https://github.com/RedisGraph/JRedisGraph) library which extends [Jedis](https://github.com/xetorthio/jedis).

## Installation

`PM> Install-Package NRedisGraph -Version 1.6.0`

## Usage

I'm assuming that you already have the [RedisGraph](https://oss.redislabs.com/redisgraph/) module installed on your Redis server. 

You can verify that the module is installed by executing the following command:

`MODULE LIST`

If RedisGraph is installed you should see output similar to the following:

```
1) 1) "name"
   2) "graph"
   3) "ver"
   4) (integer) 20811
```

(The version of the module installed on your server obviously may vary.)

## Extras

* Adds support for the [`LOLWUT`](https://redis.io/commands/lolwut/) Redis command by introducing extension methods to the `IDatabase`.

## Examples

In this repository there are a suite of integration tests that should be sufficient to serve as examples on how to use all supported RedisGraph commands.

[Integration Tests](https://github.com/tombatron/NRedisGraph/blob/master/NRedisGraph.Tests/RedisGraphAPITest.cs)
