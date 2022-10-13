using System;
using System.Collections.Generic;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.FluentCommands.GroupBot;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace ScheduleParser
{
    internal class VKBot
    {
        private VkApi api = new VkApi();
        private Random rnd = new Random();
        private FluentGroupBotCommands commands = new FluentGroupBotCommands();

        public static long _chatid = 2;

        public void Connect()
        {
            api.Authorize(new ApiAuthParams
            {
                AccessToken = "vk1.a.pE-uai9Z_ikQ0A0ZIjqbBJZhD2-uGVrNn5jhlkult4jtKhDpRq3czBEBy6FoZehh8MSvsJ4NK7_AGj6c706k6FbmzRBoTmeWsbThCgdOKZeUCaANnNlHh_GTZ_zeTojHFrbeAY6rfeibzeot3MqLGFVw4PyFhW-0msTFcANTM023Pw9Eq1gne8_KEgJQSszZ"
            });
            //try this if no connection to 
            var settings = api.Groups.GetLongPollServer(215942977);

            var keyboard = new KeyboardBuilder()
                .AddButton("Расписание на сегодня", "scheduleToday", KeyboardButtonColor.Positive)
                .SetInline(false)
                .SetOneTime()
                .AddLine()
                .AddButton("Расписание на неделю", "scheduleWeek", KeyboardButtonColor.Primary)
                .AddButton("К выбору группы", "choosegroup", KeyboardButtonColor.Negative)
                .Build();
            

            
            while (true)
            {
                try
                {
                    var poll = api.Groups.GetBotsLongPollHistory(
                      new BotsLongPollHistoryParams()
                      { Server = settings.Server, Ts = settings.Ts, Key = settings.Key, Wait = 1 });

                    if (poll?.Updates == null) continue;

                    settings.Ts = poll.Ts;

                    foreach (var element in poll.Updates)
                    {
                        if (element.Instance is MessageNew button)
                        {
                            
                            switch (button.Message.Payload)
                            {
                                case "{\"button\":\"scheduleToday\"}":
                                    api.Messages.Send(new MessagesSendParams
                                    {
                                        RandomId = rnd.Next(100000),
                                        ChatId = 2,
                                        UserId = api.UserId.Value,
                                        Keyboard = keyboard,
                                        Message = "day schedule"
                                    });
                                    break;

                                case "{\"button\":\"scheduleWeek\"}":
                                    api.Messages.Send(new MessagesSendParams
                                    {
                                        RandomId = rnd.Next(100000),
                                        ChatId = 2,
                                        UserId = api.UserId.Value,
                                        Keyboard = keyboard,
                                        Message = "week schedule"
                                    });
                                    break;

                                case "{\"button\":\"choosegroup\"}":
                                    api.Messages.Send(new MessagesSendParams
                                    {
                                        RandomId = rnd.Next(100000),
                                        ChatId = 2,
                                        UserId = api.UserId.Value,
                                        Keyboard = keyboard,
                                        Message = "Choose group"
                                    });
                                    break;
                            }
                        }

                        if (element.Instance is MessageNew messageNew)
                        {

                            Console.WriteLine(messageNew.Message.Text);
                            if (messageNew.Message.Text == "Расписание на неделю")
                            {
                                api.Messages.Send(new VkNet.Model.RequestParams.MessagesSendParams
                                {
                                    RandomId = rnd.Next(100000),
                                    ChatId = 2,
                                    UserId = api.UserId.Value,
                                    Keyboard = keyboard,
                                    Message = "Расписание на неделю"
                                });
                            }
                            else if (messageNew.Message.Text == "Расписание на сегодня")
                            {
                                api.Messages.Send(new VkNet.Model.RequestParams.MessagesSendParams
                                {
                                    RandomId = rnd.Next(100000),
                                    ChatId = 2,
                                    UserId = api.UserId.Value,
                                    Keyboard = keyboard,
                                    Message = "Расписание на день"
                                });
                            }
                            else
                            {
                                api.Messages.Send(new VkNet.Model.RequestParams.MessagesSendParams
                                {
                                    RandomId = rnd.Next(100000),
                                    ChatId = 2,
                                    UserId = api.UserId.Value,
                                    Keyboard = keyboard,
                                    Message = "Расписание РЭУ"
                                });

                            }

                        }

                    }
                }
                catch (LongPollException exception)
                {
                    if (exception is LongPollOutdateException outdateException)
                        settings.Ts = outdateException.Ts;
                    else
                    {
                        settings = api.Groups.GetLongPollServer(215942977);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
