﻿using System;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
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
        private Database database = new Database();
        private ReaParser parser = new ReaParser();
        private DateTime ClockInfoFromSystem = DateTime.Now;
        

        public static long _chatid = 1;
        private string UserGroup;
        private long UserId;

        public void Connect()
        {
            DotNetEnv.Env.Load(@"C:\Users\Admin\Documents\GitHub\ReaParser-ScheduleParser-\ScheduleParser\ScheduleParser\.env");
            var m_AccessToken = Environment.GetEnvironmentVariable("TOKEN");

            bool groupButtonPressed = false;

            api.Authorize(new ApiAuthParams
            {
                AccessToken = $"{m_AccessToken}"
            });

            var settings = api.Groups.GetLongPollServer(215942977);

            var keyboard = new KeyboardBuilder()
                .AddButton("Расписание на сегодня", "scheduleToday", KeyboardButtonColor.Positive)
                .SetInline(false)
                .SetOneTime()
                .AddLine()
                .AddButton("Расписание на неделю", "scheduleWeek", KeyboardButtonColor.Primary)
                .AddButton("К выбору группы", "choosegroup", KeyboardButtonColor.Negative)
                .Build();
            
            var GroupManagerKeyboard = new KeyboardBuilder()
                .AddButton("Выбрать группу", "choosegroup", KeyboardButtonColor.Primary)
                .SetInline(false)
                .SetOneTime()
                .AddLine()
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
                            //button handler
                            switch (button.Message.Payload)
                            {
                                case "{\"button\":\"scheduleToday\"}":
                                    if (UserGroup != null)
                                    {
                                        api.Messages.Send(new MessagesSendParams
                                        {
                                            RandomId = rnd.Next(100000),
                                            ChatId = _chatid,
                                            UserId = api.UserId.Value,
                                            Keyboard = keyboard,
                                            Message = "Расписание группы " + UserGroup + " на сегодня: \n"
                                        });
                                        var Today = (int)(ClockInfoFromSystem.DayOfWeek + 6) % 7;

                                        foreach (var DaySchedule in parser.GetDaySchedule(UserGroup,Today+1))
                                        {
                                            api.Messages.Send(new MessagesSendParams
                                            {
                                                RandomId = rnd.Next(100000),
                                                ChatId = _chatid,
                                                UserId = api.UserId.Value,
                                                Keyboard = keyboard,
                                                Message = DaySchedule
                                            });
                                        }
                                        parser.ClearDaySchedule();
                                    }
                                    else
                                    {
                                        api.Messages.Send(new MessagesSendParams
                                        {
                                            RandomId = rnd.Next(100000),
                                            ChatId = _chatid,
                                            UserId = api.UserId.Value,
                                            Keyboard = keyboard,
                                            Message = "Пожалуйста, сначала выберите группу \n"
                                        });
                                    }
                                    break;

                                case "{\"button\":\"scheduleWeek\"}":
                                    if (UserGroup != null)
                                    {
                                        api.Messages.Send(new MessagesSendParams
                                        {
                                            RandomId = rnd.Next(100000),
                                            ChatId = _chatid,
                                            UserId = api.UserId.Value,
                                            Keyboard = keyboard,
                                            Message = "Расписание группы " + UserGroup + " на эту неделю: \n"
                                        });
                                        foreach (var WeekSchedule in parser.GetWeekSchedule(UserGroup))
                                        {
                                            api.Messages.Send(new MessagesSendParams
                                            {
                                                RandomId = rnd.Next(100000),
                                                ChatId = _chatid,
                                                UserId = api.UserId.Value,
                                                Keyboard = keyboard,
                                                Message = WeekSchedule
                                            });
                                        }
                                        parser.ClearWeekSchedule();
                                    }
                                    else
                                    {
                                        api.Messages.Send(new MessagesSendParams
                                        {
                                            RandomId = rnd.Next(100000),
                                            ChatId = _chatid,
                                            UserId = api.UserId.Value,
                                            Keyboard = keyboard,
                                            Message = "Пожалуйста, сначала выберите группу \n"
                                        });
                                    }
                                    break;

                                case "{\"button\":\"choosegroup\"}":
                                    api.Messages.Send(new MessagesSendParams
                                    {
                                        RandomId = rnd.Next(100000),
                                        ChatId = _chatid,
                                        UserId = api.UserId.Value,
                                        Message = "Пожалуйста введите номер вашей группы (Например: 15.27Д-ИСТ15/22б)"
                                    });
                                    groupButtonPressed = true;
                                    break;
                            }
                        }
                        if (groupButtonPressed)
                        {
                            GetGroupNumber();
                            async void GetGroupNumber()
                            {
                                UserGroup = null;

                                await Task.Run(() =>
                                {
                                    while (UserGroup == null)
                                    {
                                        if (element.Instance is MessageNew groupnumber)
                                        {
                                            //add distant and extramural prefixes
                                            //bug is still exists 15.any number
                                            if (groupnumber.Message.Text.StartsWith("15.") && groupnumber.Message.Text.Length < 20)
                                            {
                                                UserGroup = groupnumber.Message.Text;
                                                api.Messages.Send(new VkNet.Model.RequestParams.MessagesSendParams
                                                {
                                                    RandomId = rnd.Next(100000),
                                                    ChatId = _chatid,
                                                    UserId = api.UserId.Value,
                                                    Keyboard = keyboard,
                                                    Message = "Вы выбрали " + UserGroup + " группу \n"
                                                });
                                                //ask user to remember his group?
                                                database.AddUser(UserId.ToString(), UserGroup);
                                            }
                                        }
                                    }
                                    groupButtonPressed = false;
                                });                              
                            }
                        }
                        //exception handler
                        if (parser.UnableToGetToWebSite)
                        {
                            api.Messages.Send(new VkNet.Model.RequestParams.MessagesSendParams
                            {
                                RandomId = rnd.Next(100000),
                                ChatId = _chatid,
                                UserId = api.UserId.Value,
                                Keyboard = keyboard,
                                Message = "Ошибка: невозможно получить расписание указанной группы"
                            });
                            parser.UnableToGetToWebSite = false;
                        }

                        //Chat start
                        if (element.Instance is MessageNew messageNew)
                        {
                            Console.WriteLine(messageNew.Message.Text);
                            //works only with intaracting program user 
                            if (messageNew.Message.Text == "Начать")
                            {
                                UserId = (long)messageNew.Message.FromId;

                                if(database.isRegistred(UserId.ToString()) == "")
                                {
                                    api.Messages.Send(new VkNet.Model.RequestParams.MessagesSendParams
                                    {
                                        RandomId = rnd.Next(100000),
                                        ChatId = _chatid,
                                        UserId = api.UserId.Value,
                                        Keyboard = GroupManagerKeyboard,
                                        Message = "Для начала укажите вашу группу"
                                    });
                                }else if (database.isRegistred(UserId.ToString()) != "")
                                {
                                    //create authorized user keyboard instead of 
                                    api.Messages.Send(new VkNet.Model.RequestParams.MessagesSendParams
                                    {
                                        RandomId = rnd.Next(100000),
                                        ChatId = _chatid,
                                        UserId = api.UserId.Value,
                                        Keyboard = keyboard,
                                        Message = "Сохраненная вами группа:  "+ database.isRegistred(UserId.ToString())
                                });
                                    UserGroup = database.isRegistred(UserId.ToString());
                                }
                                
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
