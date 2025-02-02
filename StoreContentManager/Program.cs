﻿using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using SemenNewsBot;
using System.Text.Json;
using System.Net;
using System.Xml;
using System.ServiceModel.Syndication;

internal static class Program
{
    // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
    private static ITelegramBotClient? _botClient;

    // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
    private static ReceiverOptions? _receiverOptions;

    [STAThread]
    static async Task Main()
    {
        Settings.Init();
        _botClient = new TelegramBotClient(Settings.Instance.TelegramBotTokenToAccess); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
        _receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
        {
            AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
            {
                UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
            }
        };

        using var cts = new CancellationTokenSource();

        // UpdateHander - обработчик приходящих Update`ов
        // ErrorHandler - обработчик ошибок, связанных с Bot API
        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота

        var me = await _botClient.GetMeAsync(); // Создаем переменную, в которую помещаем информацию о нашем боте.
        Console.WriteLine($"{me.FirstName} запущен!");

        //await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
        while (true)
        {
            
            Thread.Sleep(1000*60*10); //10минут
        }
    }

    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
        try
        {
            // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        if (Settings.Instance.TelegramAdminsUsername.Contains(update.Message.From.Username)) //"Детский Дворик"
                        {
                            if (update.Message.Text == "SelectThisChat")
                            {
                                Settings.Instance.TelegramChatId = update.Message.Chat.Id;
                                Settings.Instance.TelegramThemeId = update.Message.MessageThreadId;
                                Settings.Save();
                                botClient.DeleteMessage(Settings.Instance.TelegramChatId, update.Message.Id);
                                var mesg = botClient.SendMessage(update.Message.Chat.Id, "Чат успешно приязан!", messageThreadId: update.Message.MessageThreadId);
                            }
                        }
                        else if (update.Message.SenderChat.Title == Settings.Instance.TelegramChannelTitle)
                        {

                        }
                        return;
                    }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}
