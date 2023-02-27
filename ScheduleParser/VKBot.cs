using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualBasic;
using System.Net.Http;
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

        public LongPollServerResponse settings;

        public async Task VkConnectAsync()
        {
            //DotNetEnv.Env.Load(@"C:\Users\Admin\Documents\GitHub\ReaParser-ScheduleParser-\ScheduleParser\ScheduleParser\.env");
            var m_AccessToken = "3045ca193a28e30a92aa9aafd879e410a641a430b797723e54c21410a7c8dec61100f3bbc0fdc4fa4f9bf"; //Environment.GetEnvironmentVariable("TOKEN");

            string Today = date.ToString("d");
            string Tommorow = DateTime.Today.AddDays(1).ToString("d");
            bool GroupButtonPressed = false;

            ulong GroupIdentificator = 213384546;

            int RetriedConnection = 0;
            bool Connected = false;


            while (!Connected)
            {
                try
                {
                    api.Authorize(new ApiAuthParams
                    {
                        AccessToken = $"{m_AccessToken}"
                    });

                    settings = api.Groups.GetLongPollServer(GroupIdentificator);
                    Connected = true;
                }
                catch (HttpRequestException httpRequestException)
                {
                    Console.WriteLine(httpRequestException);

                    System.Threading.Thread.Sleep(3000);

                    if (RetriedConnection == 5)
                    {
                        break;
                    }
                    else
                    {
                        RetriedConnection++;
                        Console.WriteLine("Нет доступа к вк. \n Переподключение...");
                        continue;
                    }
                }
            }



            var keyboard = new KeyboardBuilder()
                .AddButton("Расписание на неделю", "scheduleWeek", KeyboardButtonColor.Primary)
                .SetInline(false)
                .SetOneTime()
                .AddLine()
                .AddButton("Расписание на сегодня", "scheduleToday", KeyboardButtonColor.Positive)
        .AddLine()
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

            while (Connected)
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
                                //check if user in db
                                if (database.GetUserGroup(messageNew.Message.PeerId.Value.ToString()) == "")
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
                                else if (database.GetUserGroup(messageNew.Message.PeerId.Value.ToString()) != "")
                                {
                                    api.Messages.Send(new VkNet.Model.RequestParams.MessagesSendParams
                                    {
                                        RandomId = rnd.Next(100000),
                                        PeerId = messageNew.Message.PeerId.Value,
                                        UserId = api.UserId.Value,
                                        Keyboard = keyboard,
                                        Message = "Сохраненная вами группа:  " + database.GetUserGroup(messageNew.Message.PeerId.Value.ToString())
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
                            //button handler - messages are not displayed
                            switch (button.Message.Payload)
                            {
                                case "{\"button\":\"scheduleToday\"}":
                                    api.Messages.Send(new MessagesSendParams
                                    {
                                        RandomId = rnd.Next(100000),
                                        PeerId = button.Message.PeerId.Value,
                                        UserId = api.UserId.Value,
                                        Keyboard = keyboard,
                                        Message = "Погоди немного, посмотрим, что тебе сегодня предстоит"
                                    });
                                    if (database.GetUserGroup(button.Message.PeerId.Value.ToString()) != null)
                                    {
                                        api.Messages.Send(new MessagesSendParams
                                        {
                                            RandomId = rnd.Next(100000),
                                            PeerId = button.Message.PeerId.Value,
                                            UserId = api.UserId.Value,
                                            Keyboard = keyboard,
                                            Message = "Расписание группы " + database.GetUserGroup(button.Message.PeerId.Value.ToString()) + " на " + Today + ": \n" + await parser.RunParser(database.GetUserGroup(button.Message.PeerId.Value.ToString()), Today)
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
                                    api.Messages.Send(new MessagesSendParams
                                    {
                                        RandomId = rnd.Next(100000),
                                        PeerId = button.Message.PeerId.Value,
                                        UserId = api.UserId.Value,
                                        Keyboard = keyboard,
                                        Message = "Сейчас поглядим что у тебя завтра, не торопись"
                                    });
                                    if (database.GetUserGroup(button.Message.PeerId.Value.ToString()) != null)
                                    {
                                        api.Messages.Send(new MessagesSendParams
                                        {
                                            RandomId = rnd.Next(100000),
                                            PeerId = button.Message.PeerId.Value,
                                            UserId = api.UserId.Value,
                                            Keyboard = keyboard,
                                            Message = "Расписание группы " + database.GetUserGroup(button.Message.PeerId.Value.ToString()) + " на " + Tommorow + ": \n" + await parser.RunParser(database.GetUserGroup(button.Message.PeerId.Value.ToString()), Tommorow)
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
                                    api.Messages.Send(new MessagesSendParams
                                    {
                                        RandomId = rnd.Next(100000),
                                        PeerId = button.Message.PeerId.Value,
                                        UserId = api.UserId.Value,
                                        Keyboard = keyboard,
                                        Message = "Да уж, ну и неделька, ща покажу"
                                    });
                                    if (database.GetUserGroup(button.Message.PeerId.Value.ToString()) != null)
                                    {
                                        api.Messages.Send(new MessagesSendParams
                                        {
                                            RandomId = rnd.Next(100000),
                                            PeerId = button.Message.PeerId.Value,
                                            UserId = api.UserId.Value,
                                            Keyboard = keyboard,
                                            Message = "Погоди немного, сейчас будет расписание этой групппы " + database.GetUserGroup(button.Message.PeerId.Value.ToString()) + " на неделю: \n" + await parser.RunParser(database.GetUserGroup(button.Message.PeerId.Value.ToString()))
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

                                    GroupButtonPressed = true;
                                    break;
                            }
                        }
                        //wait for a new message with group 
                        if (GroupButtonPressed)
                        {
                            GetGroupNumber();
                            async void GetGroupNumber()
                            {
                                await Task.Run(() =>
                                {
                                    while (GroupButtonPressed)
                                    {
                                        if (element.Instance is MessageNew groupnumber)
                                        {
                                            //add distant and extramural prefixes
                                            //exploit is still exists 15.any number
                                            if (groupnumber.Message.Text.StartsWith("15.") && groupnumber.Message.Text.Length < 20)
                                            {
                                                string UserGroup = groupnumber.Message.Text;
                                                string UserId = groupnumber.Message.PeerId.Value.ToString();

                                                api.Messages.Send(new VkNet.Model.RequestParams.MessagesSendParams
                                                {
                                                    RandomId = rnd.Next(100000),
                                                    PeerId = groupnumber.Message.PeerId.Value,
                                                    UserId = api.UserId.Value,
                                                    Keyboard = keyboard,
                                                    Message = "Вы выбрали " + UserGroup + " группу \n"
                                                });
                                                database.AddUser(UserId, UserGroup);
                                                GroupButtonPressed = false;
                                            }
                                        }
                                    }
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
                        settings = api.Groups.GetLongPollServer(GroupIdentificator);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }
    }
}