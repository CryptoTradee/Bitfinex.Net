﻿using Bitfinex.Net.Converters;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.SocketObjects;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Threading;
using Binance.Net.UnitTests.TestImplementations;
using Bitfinex.Net.UnitTests.TestImplementations;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;

namespace Bitfinex.Net.UnitTests
{
    [TestFixture]
    public class BitfinexSocketClientTests
    {
        [TestCase(Precision.PrecisionLevel0, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel1, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel2, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel3, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel0, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel1, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel2, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel3, Frequency.TwoSeconds)]
        public void SubscribingToBookUpdates_Should_SubscribeSuccessfully(Precision prec, Frequency freq)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            BitfinexOrderBookEntry[] result;
            var subTask = client.SubscribeToBookUpdatesAsync("Test", prec, freq, 10, data => result = data);

            var subResponse = new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Frequency = JsonConvert.SerializeObject(freq, new FrequencyConverter(false)),
                Length = 10,
                Pair = "Test",
                Precision = JsonConvert.SerializeObject(prec, new PrecisionConverter(false)),
                Symbol = "Test"
            };

            // act
            socket.InvokeMessage(subResponse);

            subTask.Wait(5000);

            // assert
            Assert.IsTrue(subTask.Result.Success);
        }

        [TestCase(Precision.PrecisionLevel0, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel1, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel2, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel3, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel0, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel1, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel2, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel3, Frequency.TwoSeconds)]
        public void SubscribingToBookUpdates_Should_TriggerWithBookUpdate(Precision prec, Frequency freq)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            BitfinexOrderBookEntry[] result = null;
            var subTask = client.SubscribeToBookUpdatesAsync("Test", prec, freq, 10, data => result = data);

            var subResponse = new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Frequency = JsonConvert.SerializeObject(freq, new FrequencyConverter(false)),
                Length = 10,
                Pair = "Test",
                Precision = JsonConvert.SerializeObject(prec, new PrecisionConverter(false)),
                Symbol = "Test"
            };
            socket.InvokeMessage(subResponse);
            subTask.Wait(5000);
            BitfinexOrderBookEntry[] expected = new[] { new BitfinexOrderBookEntry() };

            // act
            socket.InvokeMessage($"[1, {JsonConvert.SerializeObject(expected)}]");

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result[0], expected[0]));
        }

        [TestCase(TimeFrame.OneMinute)]
        [TestCase(TimeFrame.FiveMinute)]
        [TestCase(TimeFrame.FiveteenMinute)]
        [TestCase(TimeFrame.ThirtyMinute)]
        [TestCase(TimeFrame.OneHour)]
        [TestCase(TimeFrame.ThreeHour)]
        [TestCase(TimeFrame.SixHour)]
        [TestCase(TimeFrame.TwelveHour)]
        [TestCase(TimeFrame.OneDay)]
        [TestCase(TimeFrame.SevenDay)]
        [TestCase(TimeFrame.FourteenDay)]
        public void SubscribingToCandleUpdates_Should_SubscribeSuccessfully(TimeFrame timeframe)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            var subTask = client.SubscribeToCandleUpdatesAsync("Test", timeframe, data => { });

            var subResponse = new CandleSubscriptionResponse()
            {
                Channel = "candles",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Key = "trade:" + JsonConvert.SerializeObject(timeframe, new TimeFrameConverter(false)) + ":Test"
            };

            // act
            socket.InvokeMessage(subResponse);

            subTask.Wait(5000);

            // assert
            Assert.IsTrue(subTask.Result.Success);
        }

        [TestCase(TimeFrame.OneMinute)]
        [TestCase(TimeFrame.FiveMinute)]
        [TestCase(TimeFrame.FiveteenMinute)]
        [TestCase(TimeFrame.ThirtyMinute)]
        [TestCase(TimeFrame.OneHour)]
        [TestCase(TimeFrame.ThreeHour)]
        [TestCase(TimeFrame.SixHour)]
        [TestCase(TimeFrame.TwelveHour)]
        [TestCase(TimeFrame.OneDay)]
        [TestCase(TimeFrame.SevenDay)]
        [TestCase(TimeFrame.FourteenDay)]
        public void SubscribingToCandleUpdates_Should_TriggerWithCandleUpdate(TimeFrame timeframe)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            BitfinexCandle[] result = null;
            var subTask = client.SubscribeToCandleUpdatesAsync("Test", timeframe, data => result = data);

            var subResponse = new CandleSubscriptionResponse()
            {
                Channel = "candles",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Key = "trade:" + JsonConvert.SerializeObject(timeframe, new TimeFrameConverter(false)) + ":Test"
            };
            socket.InvokeMessage(subResponse);
            subTask.Wait(5000);
            BitfinexCandle[] expected = new[] { new BitfinexCandle() };

            // act
            socket.InvokeMessage($"[1, {JsonConvert.SerializeObject(expected)}]");

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result[0], expected[0]));
        }

        [Test]
        public void SubscribingToTickerUpdates_Should_SubscribeSuccessfully()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            var subTask = client.SubscribeToTickerUpdatesAsync("Test", data => { });

            var subResponse = new TickerSubscriptionResponse()
            {
                Channel = "ticker",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test"
            };

            // act
            socket.InvokeMessage(subResponse);

            subTask.Wait(5000);

            // assert
            Assert.IsTrue(subTask.Result.Success);
        }

        [Test]
        public void SubscribingToTickerUpdates_Should_TriggerWithTickerUpdate()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            BitfinexMarketOverview result = null;
            var subTask = client.SubscribeToTickerUpdatesAsync("Test", data => result = data);

            var subResponse = new TickerSubscriptionResponse()
            {
                Channel = "ticker",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test"
            };
            socket.InvokeMessage(subResponse);
            subTask.Wait(5000);
            BitfinexMarketOverview expected = new BitfinexMarketOverview();

            // act
            socket.InvokeMessage($"[1, {JsonConvert.SerializeObject(expected)}]");

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result, expected));
        }

        [Test]
        public void SubscribingToRawBookUpdates_Should_SubscribeSuccessfully()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            var subTask = client.SubscribeToRawBookUpdatesAsync("Test", 10, data => { });

            var subResponse = new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test",
                Frequency = "F0",
                Precision = "R0",
                Length = 10
            };

            // act
            socket.InvokeMessage(subResponse);

            subTask.Wait(5000);

            // assert
            Assert.IsTrue(subTask.Result.Success);
        }

        [Test]
        public void SubscribingToRawBookUpdates_Should_TriggerWithRawBookUpdate()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            BitfinexRawOrderBookEntry[] result = null;
            var subTask = client.SubscribeToRawBookUpdatesAsync("Test", 10, data => result = data);

            var subResponse = new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test",
                Frequency = "F0",
                Precision = "R0",
                Length = 10
            };
            socket.InvokeMessage(subResponse);
            subTask.Wait(5000);
            BitfinexRawOrderBookEntry[] expected = new []{ new BitfinexRawOrderBookEntry()};

            // act
            socket.InvokeMessage($"[1, {JsonConvert.SerializeObject(expected)}]");

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result[0], expected[0]));
        }

        [Test]
        public void SubscribingToTradeUpdates_Should_SubscribeSuccessfully()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            var subTask = client.SubscribeToTradeUpdatesAsync("Test", data => { });

            var subResponse = new TradesSubscriptionResponse()
            {
                Channel = "trades",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test"
            };

            // act
            socket.InvokeMessage(subResponse);

            subTask.Wait(5000);

            // assert
            Assert.IsTrue(subTask.Result.Success);
        }

        [Test]
        public void SubscribingToTradeUpdates_Should_TriggerWithTradeUpdate()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            BitfinexTradeSimple[] result = null;
            var subTask = client.SubscribeToTradeUpdatesAsync("Test", data => result = data);

            var subResponse = new TradesSubscriptionResponse()
            {
                Channel = "trades",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test"
            };
            socket.InvokeMessage(subResponse);
            subTask.Wait(5000);
            BitfinexTradeSimple[] expected = new[] { new BitfinexTradeSimple() };

            // act
            socket.InvokeMessage($"[1, {JsonConvert.SerializeObject(expected)}]");

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result[0], expected[0]));
        }
        
        [TestCase("ou", BitfinexEventType.OrderUpdate)]
        [TestCase("on", BitfinexEventType.OrderNew)]
        [TestCase("oc", BitfinexEventType.OrderCancel)]
        [TestCase("os", BitfinexEventType.OrderSnapshot, false)]
        public void SubscribingToOrderUpdates_Should_TriggerWithOrderUpdate(string updateType, BitfinexEventType eventType, bool single=true)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var rstEvent = new ManualResetEvent(false);
            BitfinexSocketEvent<BitfinexOrder[]> result = null;
            var expected = new BitfinexSocketEvent<BitfinexOrder[]>(eventType, new [] { new BitfinexOrder() { StatusString = "ACTIVE" }});
            client.SubscribeToTradingUpdatesAsync(data =>
            {
                result = data;
                rstEvent.Set();
            }, null, null);

            // act
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            socket.InvokeMessage(single ? new object[] {0, updateType, expected.Data[0] } : new object[] {0, updateType, new[] { expected.Data[0]} });
            rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result.Data[0], expected.Data[0]));
        }

        [TestCase("te", BitfinexEventType.TradeExecuted)]
        [TestCase("tu", BitfinexEventType.TradeExecutionUpdate)]
        public void SubscribingToTradeUpdates_Should_TriggerWithTradeUpdate(string updateType, BitfinexEventType eventType, bool single = true)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var rstEvent = new ManualResetEvent(false);
            BitfinexSocketEvent<BitfinexTradeDetails[]> result = null;
            var expected = new BitfinexSocketEvent<BitfinexTradeDetails[]>(eventType, new[] { new BitfinexTradeDetails() { } });
            client.SubscribeToTradingUpdatesAsync(null,
                data =>
                {
                    result = data;
                    rstEvent.Set();
                }, null);

            // act
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            socket.InvokeMessage(single ? new object[] { 0, updateType, new BitfinexTradeDetails() } : new object[] { 0, updateType, new[] { new BitfinexTradeDetails() } });
            rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result.Data[0], expected.Data[0]));
        }

        [TestCase("ws", BitfinexEventType.WalletSnapshot, false)]
        [TestCase("wu", BitfinexEventType.WalletUpdate)]
        public void SubscribingToWalletUpdates_Should_TriggerWithWalletUpdate(string updateType, BitfinexEventType eventType, bool single = true)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var rstEvent = new ManualResetEvent(false);
            BitfinexSocketEvent<BitfinexWallet[]> result = null;
            var expected = new BitfinexSocketEvent<BitfinexWallet[]>(eventType, new[] { new BitfinexWallet() { } });
            client.SubscribeToWalletUpdatesAsync(data =>
                {
                    result = data;
                    rstEvent.Set();
                });

            // act
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            socket.InvokeMessage(single ? new object[] { 0, updateType, new BitfinexWallet() } : new object[] { 0, updateType, new[] { new BitfinexWallet() } });
            rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result.Data[0], expected.Data[0]));
        }

        [TestCase("pn", BitfinexEventType.PositionNew)]
        [TestCase("pc", BitfinexEventType.PositionClose)]
        [TestCase("pu", BitfinexEventType.PositionUpdate)]
        [TestCase("ps", BitfinexEventType.PositionSnapshot, false)]
        public void SubscribingToPositionUpdates_Should_TriggerWithPositionUpdate(string updateType, BitfinexEventType eventType, bool single = true)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var rstEvent = new ManualResetEvent(false);
            BitfinexSocketEvent<BitfinexPosition[]> result = null;
            var expected = new BitfinexSocketEvent<BitfinexPosition[]>(eventType, new[] { new BitfinexPosition() { } });
            client.SubscribeToTradingUpdatesAsync(null, null, data =>
            {
                result = data;
                rstEvent.Set();
            });

            // act
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            socket.InvokeMessage(single ? new object[] { 0, updateType, new BitfinexPosition() } : new object[] { 0, updateType, new[] { new BitfinexPosition() } });
            rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result.Data[0], expected.Data[0]));
        }

        [TestCase("fcn", BitfinexEventType.FundingCreditsNew)]
        [TestCase("fcu", BitfinexEventType.FundingCreditsUpdate)]
        [TestCase("fcc", BitfinexEventType.FundingCreditsClose)]
        [TestCase("fcs", BitfinexEventType.FundingCreditsSnapshot, false)]
        public void SubscribingToFundingCreditsUpdates_Should_TriggerWithFundingCreditsUpdate(string updateType, BitfinexEventType eventType, bool single = true)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var rstEvent = new ManualResetEvent(false);
            BitfinexSocketEvent<BitfinexFundingCredit[]> result = null;
            var expected = new BitfinexSocketEvent<BitfinexFundingCredit[]>(eventType, new[] { new BitfinexFundingCredit() { StatusString="ACTIVE" } });
            client.SubscribeToFundingUpdatesAsync(null,data =>
            {
                result = data;
                rstEvent.Set();
            }, null);

            // act
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            socket.InvokeMessage(single ? new object[] { 0, updateType, new BitfinexFundingCredit() } : new object[] { 0, updateType, new[] { new BitfinexFundingCredit() } });
            rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result.Data[0], expected.Data[0]));
        }

        [TestCase("fln", BitfinexEventType.FundingLoanNew)]
        [TestCase("flu", BitfinexEventType.FundingLoanUpdate)]
        [TestCase("flc", BitfinexEventType.FundingLoanClose)]
        [TestCase("fls", BitfinexEventType.FundingLoanSnapshot, false)]
        public void SubscribingToFundingLoanUpdates_Should_TriggerWithFundingLoanUpdate(string updateType, BitfinexEventType eventType, bool single = true)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var rstEvent = new ManualResetEvent(false);
            BitfinexSocketEvent<BitfinexFundingLoan[]> result = null;
            var expected = new BitfinexSocketEvent<BitfinexFundingLoan[]>(eventType, new[] { new BitfinexFundingLoan() { StatusString = "ACTIVE" } });
            client.SubscribeToFundingUpdatesAsync(null, null, data =>
            {
                result = data;
                rstEvent.Set();
            });

            // act
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            socket.InvokeMessage(single ? new object[] { 0, updateType, new BitfinexFundingLoan() } : new object[] { 0, updateType, new[] { new BitfinexFundingLoan() } });
            rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result.Data[0], expected.Data[0]));
        }

        [TestCase("fon", BitfinexEventType.FundingOfferNew)]
        [TestCase("fou", BitfinexEventType.FundingOfferUpdate)]
        [TestCase("foc", BitfinexEventType.FundingOfferCancel)]
        [TestCase("fos", BitfinexEventType.FundingOfferSnapshot, false)]
        public void SubscribingToFundingOfferUpdates_Should_TriggerWithFundingOfferUpdate(string updateType, BitfinexEventType eventType, bool single = true)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var rstEvent = new ManualResetEvent(false);
            BitfinexSocketEvent<BitfinexFundingOffer[]> result = null;
            var expected = new BitfinexSocketEvent<BitfinexFundingOffer[]>(eventType, new[] { new BitfinexFundingOffer() { StatusString = "ACTIVE" } });
            client.SubscribeToFundingUpdatesAsync(data =>
            {
                result = data;
                rstEvent.Set();
            }, null, null);

            // act
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            socket.InvokeMessage(single ? new object[] { 0, updateType, new BitfinexFundingOffer() } : new object[] { 0, updateType, new[] { new BitfinexFundingOffer() } });
            rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result.Data[0], expected.Data[0]));
        }

        [Test]
        public void PlacingAnOrder_Should_SucceedIfSuccessResponse()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var expected = new BitfinexOrder()
            {
                Price = 0.1m,
                Amount = 0.2m,
                Symbol = "tBTCUSD",
                Type = OrderType.ExchangeLimit,
                ClientOrderId = 1234,
                StatusString = "ACTIVE"
            };

            // act
            var placeTask = client.PlaceOrderAsync(OrderType.ExchangeLimit, "tBTCUSD", 1, price: 1, clientOrderId: 1234);
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            Thread.Sleep(100);
            socket.InvokeMessage($"[0, \"on\", {JsonConvert.SerializeObject(expected)}]");
            var result = placeTask.Result;

            // assert
            Assert.IsTrue(result.Success);
            Assert.IsTrue(TestHelpers.AreEqual(expected, result.Data));
        }

        [Test]
        public void PlacingAnOrder_Should_FailIfErrorResponse()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);
            var order = new BitfinexOrder() {ClientOrderId = 123};

            // act
            var placeTask = client.PlaceOrderAsync(OrderType.ExchangeLimit, "tBTCUSD", 1, price: 1, clientOrderId: 123);
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            Thread.Sleep(100);
            socket.InvokeMessage($"[0, \"n\", [0, \"on-req\", 0, 0, {JsonConvert.SerializeObject(order)}, 0, \"error\", \"order placing failed\"]]");
            var result = placeTask.Result;

            // assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Error.Message.Contains("order placing failed"));
        }

        [Test]
        public void PlacingAnOrder_Should_FailIfNoResponse()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket, new BitfinexSocketClientOptions()
            {
                ApiCredentials = new ApiCredentials("Test", "Test"),
                LogVerbosity = LogVerbosity.Debug,
                SocketResponseTimeout = TimeSpan.FromMilliseconds(100)
            });

            // act
            var placeTask = client.PlaceOrderAsync(OrderType.ExchangeLimit, "tBTCUSD", 1, price: 1, clientOrderId: 123);
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            var result = placeTask.Result;

            // assert
            Assert.IsFalse(result.Success);
        }

        [Test]
        public void PlacingAnMarketOrder_Should_SucceedIfSuccessResponse()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var expected = new BitfinexOrder()
            {
                Price = 0.1m,
                Amount = 0.2m,
                Symbol = "tBTCUSD",
                Type = OrderType.ExchangeMarket,
                ClientOrderId = 1234,
                StatusString = "EXECUTED"
            };

            // act
            var placeTask = client.PlaceOrderAsync(OrderType.ExchangeMarket, "tBTCUSD", 1, price: 1, clientOrderId: 1234);
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            Thread.Sleep(100);
            socket.InvokeMessage($"[0, \"oc\", {JsonConvert.SerializeObject(expected)}]");
            var result = placeTask.Result;

            // assert
            Assert.IsTrue(result.Success);
            Assert.IsTrue(TestHelpers.AreEqual(expected, result.Data));
        }

        [Test]
        public void PlacingAnFOKOrder_Should_SucceedIfCanceledResponse()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var expected = new BitfinexOrder()
            {
                Price = 0.1m,
                Amount = 0.2m,
                Symbol = "tBTCUSD",
                Type = OrderType.ExchangeFillOrKill,
                ClientOrderId = 1234,
                StatusString = "CANCELED"
            };

            // act
            var placeTask = client.PlaceOrderAsync(OrderType.ExchangeFillOrKill, "tBTCUSD", 1, price: 1, clientOrderId: 1234);
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            Thread.Sleep(100);
            socket.InvokeMessage($"[0, \"oc\", {JsonConvert.SerializeObject(expected)}]");
            var result = placeTask.Result;

            // assert
            Assert.IsTrue(result.Success);
            Assert.IsTrue(TestHelpers.AreEqual(expected, result.Data));
        }

        [Test]
        public void ReceivingAReconnectMessage_Should_ReconnectWebsocket()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket, new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectInterval = TimeSpan.FromMilliseconds(10)
            });

            var rstEvent = new ManualResetEvent(false);
            var subTask = client.SubscribeToCandleUpdatesAsync("tBTCUSD", TimeFrame.FiveMinute, data => { });
            socket.InvokeMessage(new CandleSubscriptionResponse()
            {
                Channel = "candles",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Key = "trade:" + JsonConvert.SerializeObject(TimeFrame.FiveMinute, new TimeFrameConverter(false)) + ":Test"
            });
            var subResult = subTask.Result;

            subResult.Data.ConnectionRestored += (t) => rstEvent.Set();

            // act
            socket.InvokeMessage("{\"event\":\"info\", \"code\": 20051}");
            var triggered = rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(triggered);
        }
    }
}
