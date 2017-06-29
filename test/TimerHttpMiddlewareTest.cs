﻿using System;
using FluentlyHttpClient;
using FluentlyHttpClient.Middleware;
using FluentlyHttpClient.Test;
using RichardSzalay.MockHttp;
using Xunit;
using static FluentlyHttpClient.Test.ServiceTestUtil;

namespace Test
{
	public class TimerHttpMiddlewareTest
	{
		[Fact]
		public async void ShouldReturnContent()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When("https://sketch7.com/api/heroes/azmodan")
				.Respond("application/json", "{ 'name': 'Azmodan' }");

			var fluentHttpClientFactory = GetNewClientFactory();
			fluentHttpClientFactory.CreateBuilder("sketch7")
				.WithBaseUrl("https://sketch7.com")
				.AddMiddleware<TimerHttpMiddleware>()
				.WithMessageHandler(mockHttp)
				.Register();

			var httpClient = fluentHttpClientFactory.Get("sketch7");
			var response = await httpClient.CreateRequest("/api/heroes/azmodan")
				.ReturnAsResponse<Hero>();

			Assert.NotNull(response.Data);
			Assert.Equal("Azmodan", response.Data.Name);
			Assert.NotEqual(TimeSpan.Zero, response.GetTimeTaken());
		}
	}
}