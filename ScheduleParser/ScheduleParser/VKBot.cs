using System;
using System.Threading;
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
        DateTime date = DateTime.Now;
        private DateTime ClockInfoFromSystem = DateTime.Now;

        public void VkConnect()
        {
            DotNetEnv.Env.Load(@"C:\Users\Admin\Documents\GitHub\ReaParser-ScheduleParser-\ScheduleParser\ScheduleParser\.env");
            var m_AccessToken = Environment.GetEnvironmentVariable("TOKEN");

            bool groupButtonPressed = false;
            string Today = date.ToString("d");
            string Tommorow = DateTime.Today.AddDays(1).ToString("d");


            api.Authorize(new ApiAuthParams
            {
                AccessToken = $"{m_AccessToken}"
            });

            var settings = api.Groups.GetLongPollServer(215942977);

            var keyboard = new KeyboardBuilder()
                .AddButton("Расписание на неделю", "scheduleWeek", KeyboardButtonColor.Primary)
                .SetInline(false)
                .SetOneTime()
                .AddLine()
                .AddButton("Расписание на сегодня", "scheduleToday", KeyboardButtonColor.Positive)
                .AddButton("Расписание на завтра", "scheduleTommorow", KeyboardButtonColor.Positive)
                .AddLine()
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
                        //Chat handler
                        if (element.Instance is MessageNew messageNew)
                        {
                            Console.WriteLine(messageNew.Message.Text);

                            if (messageNew.Message.Text == "Начать")
                            {
                                //check if user is in Database
                                if (database.GetUserGroup(messageNew.Message.UserId.Value.ToString()) == "")
                                {
                                    api.Messages.Send(new VkNet.Model.RequestParams.MessagesSendParams
                                    {
                                        RandomId = rnd.Next(100000),
                                        PeerId = messageNew.Message.PeerId.Value,
                                        UserId = api.UserId.Value,
                                        Keyboard = GroupManagerKeyboard,
                                        Message = "Для начала укажите вашу группу"
                                    });
                                }
                                else if (database.GetUserGroup(messageNew.Message.UserId.Value.ToString()) != "")
                                {
                                    api.Messages.Send(new VkNet.Model.RequestParams.MessagesSendParams
                                    {
                                        RandomId = rnd.Next(100000),
                                        PeerId = messageNew.Message.PeerId.Value,
                                        UserId = api.UserId.Value,
                                        Keyboard = keyboard,
                                        Message = "Сохраненная вами группа:  " + database.GetUserGroup(messageNew.Message.UserId.Value.ToString())
                                    });
                                }
                            }

                            if (parser.UnableToGetToWebSite)
                            {
                                api.Messages.Send(new VkNet.Model.RequestParams.MessagesSendParams
                                {
                                    RandomId = rnd.Next(100000),
                                    PeerId = messageNew.Message.PeerId.Value,
                                    UserId = api.UserId.Value,
                                    Keyboard = keyboard,
                                    Message = "Ошибка: невозможно получить расписание. Проверьте правильность написания группы и попробуйте еще раз."
                                });
                                parser.UnableToGetToWebSite = false;
                            }
                        }
                        if (element.Instance is MessageNew button)
                        {
                            //button handler - messages are not displayed correctly
                            switch (button.Message.Payload)
                            {
                                case "{\"button\":\"scheduleToday\"}":

                                    if (database.GetUserGroup(button.Message.UserId.Value.ToString()) != null)
                                    {
                                        api.Messages.Send(new MessagesSendParams
                                        {
                                            RandomId = rnd.Next(100000),
                                            PeerId = button.Message.PeerId.Value,
                                            UserId = api.UserId.Value,
                                            Keyboard = keyboard,
                                            Message = "Расписание группы " + database.GetUserGroup(button.Message.UserId.Value.ToString()) + " на сегодня: \n"+ parser.RunParser(database.GetUserGroup(button.Message.UserId.Value.ToString()), Today)
                                        });                                        
                                    }
                                    else
                                    {
                                        api.Messages.Send(new MessagesSendParams
                                        {
                                            RandomId = rnd.Next(100000),
                                            PeerId = button.Message.PeerId.Value,
                                            UserId = api.UserId.Value,
                                            Keyboard = keyboard,
                                            Message = "Пожалуйста, сначала выберите группу \n"
                                        });
                                    }
                                    break;

                                case "{\"button\":\"scheduleTommorow\"}":
                                    
                                    if (database.GetUserGroup(button.Message.UserId.Value.ToString()) != null)
                                    {
                                        api.Messages.Send(new MessagesSendParams
                                        {
                                            RandomId = rnd.Next(100000),
                                            PeerId = button.Message.PeerId.Value,
                                            UserId = api.UserId.Value,
                                            Keyboard = keyboard,
                                            Message = "Расписание группы " + database.GetUserGroup(button.Message.UserId.Value.ToString()) + " на завтра: \n" + parser.RunParser(database.GetUserGroup(button.Message.UserId.Value.ToString()), Tommorow)
                                        });
                                    }
                                    else
                                    {
                                        api.Messages.Send(new MessagesSendParams
                                        {
                                            RandomId = rnd.Next(100000),
                                            PeerId = button.Message.PeerId.Value,
                                            UserId = api.UserId.Value,
                                            Keyboard = keyboard,
                                            Message = "Пожалуйста, сначала выберите группу \n"
                                        });
                                    }
                                    break;
                                
                                case "{\"button\":\"scheduleWeek\"}":
                                    if (database.GetUserGroup(button.Message.UserId.Value.ToString()) != null)
                                    {
                                        api.Messages.Send(new MessagesSendParams
                                        {
                                            RandomId = rnd.Next(100000),
                                            PeerId = button.Message.PeerId.Value,
                                            UserId = api.UserId.Value,
                                            Keyboard = keyboard,
                                            Message = "Расписание группы " + database.GetUserGroup(button.Message.UserId.Value.ToString()) + " на эту неделю: \n"+ parser.RunParser(database.GetUserGroup(button.Message.UserId.Value.ToString()))
                                        });
                                    }
                                    else
                                    {
                                        api.Messages.Send(new MessagesSendParams
                                        {
                                            RandomId = rnd.Next(100000),
                                            PeerId = button.Message.PeerId.Value,
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
                                        PeerId = button.Message.PeerId.Value,
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
                                string UserGroup = null;

                                await Task.Run(() =>
                                {
                                    while (UserGroup == null)
                                    {
                                        if (element.Instance is MessageNew groupnumber)
                                        {
                                            //add distant and extramural prefixes
                                            //exploit is still exists 15.any number
                                            if (groupnumber.Message.Text.StartsWith("15.") && groupnumber.Message.Text.Length < 20)
                                            {
                                                UserGroup = groupnumber.Message.Text;
                                                api.Messages.Send(new VkNet.Model.RequestParams.MessagesSendParams
                                                {
                                                    RandomId = rnd.Next(100000),
                                                    PeerId = groupnumber.Message.PeerId.Value,
                                                    UserId = api.UserId.Value,
                                                    Keyboard = keyboard,
                                                    Message = "Вы выбрали " + UserGroup + " группу \n"
                                                });

                                                database.AddUser(database.GetUserGroup(groupnumber.Message.UserId.Value.ToString()), UserGroup);
                                            }
                                        }
                                    }
                                    groupButtonPressed = false;
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
