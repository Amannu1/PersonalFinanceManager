﻿using System.Text.Json.Serialization;
using Amazon.Lambda.APIGatewayEvents;
using SharedLibrary.Dto;

namespace ExpenseLoggerApi.AotTypes;

[JsonSerializable(typeof(LogExpenseResponse))]
[JsonSerializable(typeof(SpreadsheetValidatorRequest))]
[JsonSerializable(typeof(SpreadsheetValidatorResponse))]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}
