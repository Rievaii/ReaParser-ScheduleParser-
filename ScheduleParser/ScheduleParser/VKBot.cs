using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;
using VkNet.FluentCommands.GroupBot;
using VkNet.Model.Keyboard;

namespace ScheduleParser
{
    internal class VKBot
    {
        VkApi api = new VkApi();
        Random rnd = new Random();
        FluentGroupBotCommands commands = new FluentGroupBotCommands();

        public static long _chatid = 2;

        public void Connect()
        {
            

            api.Authorize(new ApiAuthParams
            {
                AccessToken = "vk1.a.pE-uai9Z_ikQ0A0ZIjqbBJZhD2-uGVrNn5jhlkult4jtKhDpRq3czBEBy6FoZehh8MSvsJ4NK7_AGj6c706k6FbmzRBoTmeWsbThCgdOKZeUCaANnNlHh_GTZ_zeTojHFrbeAY6rfeibzeot3MqLGFVw4PyFhW-0msTFcANTM023Pw9Eq1gne8_KEgJQSszZ"
            });

            var settings = api.Groups.GetLongPollServer(215942977);


            var keyboard = new KeyboardBuilder()
                .AddButton("Расписание на сегодня", "btnValue", KeyboardButtonColor.Positive)
                .SetInline(false)
                .SetOneTime()
                .AddLine()
                .AddButton("Расписание на неделю", "btnValue", KeyboardButtonColor.Primary)
                .AddButton("Назад", "btnValue", KeyboardButtonColor.Default)
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
                        if (element.Instance is MessageNew messageNew)
                        {
                            api.Messages.Send(new VkNet.Model.RequestParams.MessagesSendParams
                            {
                                RandomId = rnd.Next(100000),
                                ChatId = 2,
                                UserId = api.UserId.Value,
                                Keyboard = keyboard,
                                Message = "request params"
                            });

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
            Console.ReadLine();
        }
    }
}
