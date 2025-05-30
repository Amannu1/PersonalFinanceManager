﻿using System.Text.RegularExpressions;
using BudgetAutomation.Engine.Interface;
using BudgetAutomation.Engine.Misc;
using BudgetAutomation.Engine.Model;
using BudgetAutomation.Engine.Service;
using SharedLibrary.Dto;
using SharedLibrary.Telegram;

namespace BudgetAutomation.Engine.Handler.Command;

public partial class SpreadsheetCommand(
    ISenderGateway sender,
    IUserManagerService userManagerService,
    IExpenseLoggerApiClient expenseLoggerApiClient) : ICommand
{
    public string CommandName => StaticCommandName;
    // TODO: check if it's possible to have this static property coming from the interface somehow
    public static string StaticCommandName => "planilha";

    public async Task<Message> HandleAsync(Message message, CancellationToken cancellationToken = default)
    {
        UserManagerService.EnsureUserSignedIn();

        ArgumentNullException.ThrowIfNull(message.Text);

        try
        {
            if (!Utility.TryExtractCommandArguments(message.Text, CommandName, SpreadsheetIdRegex, out var commandArguments))
            {
                if (string.IsNullOrWhiteSpace(commandArguments))
                {
                    return await sender.ReplyAsync(message.Chat,
                        "Por favor envie o ID da sua planilha com esse comando.",
                        "User tried configuring spreadsheet id with empty arguments.",
                        logLevel: LogLevel.Information,
                        cancellationToken: cancellationToken);
                }

                return await sender.ReplyAsync(message.Chat,
                    "ID de planilha inválido, tente novamente.",
                    $"User tried configuring spreadsheet id with bad arguments: '{commandArguments}'.",
                    logLevel: LogLevel.Information,
                    cancellationToken: cancellationToken);
            }

            var validateRequest = new SpreadsheetValidatorRequest();
            validateRequest.SpreadsheetId = commandArguments;

            var validateResponse = await expenseLoggerApiClient.ValidateSpreadsheet(validateRequest);

            if (!validateResponse.Success)
            {
                return await sender.ReplyAsync(message.Chat,
                    "Não foi possível encontrar a planilha. Por favor tente novamente.",
                    "Spreadsheet not found.",
                    logLevel: LogLevel.Warning,
                
                    cancellationToken: cancellationToken);
            }
            var success = userManagerService.ConfigureSpreadsheet(commandArguments, cancellationToken);

            if (!success)
            {
                return await sender.ReplyAsync(message.Chat,
                    "Não foi possível configurar a sua planilha. Por favor tente novamente.",
                    "SpreadsheetId configuration failed.",
                    logLevel: LogLevel.Warning,
                    cancellationToken: cancellationToken);
            }

            return await sender.ReplyAsync(message.Chat,
                "Configuração da planilha realizada com sucesso!",
                "SpreadsheetId configuration successful.",
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            if (e is InvalidUserInputException or UnauthorizedAccessException)
                throw;

            return await sender.ReplyAsync(message.Chat,
                "Um erro ocorreu ao tentar configurar a planilha. Tente novamente mais tarde.",
                $"Exception during spreadsheet command: {e.Message}",
                logLevel: LogLevel.Error,
                cancellationToken: cancellationToken);
        }
    }

    public Task<Message> HandleAsync(Message message, ChatState chatState, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    [GeneratedRegex("^[a-zA-Z0-9_]+$")]
    private static partial Regex SpreadsheetIdRegex();
}