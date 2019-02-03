using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Core.Discord;
using Core.Entities;
using Core.Entities.Interactive;
using Core.Entities.Interview;
using Core.Exceptions;

namespace Core.Services
{
    public class InterviewService
    {
        private  readonly IDiscordMessages _discordMessages;

        public InterviewService(IDiscordMessages discordMessages)
        {
            _discordMessages = discordMessages;
        }

        public async Task<List<Answer>> ProcessInterviewAsync(BotCommandContext context, Clan clan, List<Question> questions)
        {
            List<Answer> answers = new List<Answer>();
            var messages = new List<BotMessage>();

            //add criteria
            var criteria = new Criteria<BotMessage>();
            criteria.AddCriterion(new EnsureFromUserCriterion(context.User));
            criteria.AddCriterion(new EnsureFromChannelCriterion(context.Channel));

            //go through questions
            for (int x = 0; x < questions.Count; x++)
            {
                var answer = string.Empty;
                bool responded = false;

                //set current question object
                var questionVo = questions[x];

                var title = $"**[{x + 1}/{questions.Count}]**";
                string footer = $"Use `{clan.Prefix}back` to go back a question or `{clan.Prefix}cancel` to stop the process.";
                string content = questionVo.GetContent();

                var botEmbed = new BotEmbed();
                if (questions.Count > 1) botEmbed.Title = title;
                botEmbed.Description = content + Environment.NewLine + footer;

                //ask question
                messages.Add(await _discordMessages.SendMessageAsync(context.Channel.ChannelId, string.Empty, botEmbed));

                //handle response
                do
                {
                    //wait for the reply
                    var response = await _discordMessages.NextMessageAsync(context, criteria,
                        TimeSpan.FromSeconds(questionVo.SecondsToAnswer));

                    if (response == null) throw new NoResponseException();

                    messages.Add(response);

                    /*
                     * Check for empty response content
                     */
                    if (string.IsNullOrWhiteSpace(response.Content))
                    {
                        var emptyErrorEmbed = new BotEmbed();
                        emptyErrorEmbed.Description = "Please enter a valid answer.";

                        var errorMsg = await _discordMessages.SendMessageAsync(context.Channel.ChannelId, "", emptyErrorEmbed);
                        messages.Add(errorMsg);
                        continue;
                    }

                    /*
                     * Check for the following commands:
                     * !cancel
                     * !back
                     *
                     * Otherwise ignore the command
                     */
                    if (response.Content.StartsWith(clan.Prefix))
                    {
                        //cancel application process
                        if (response.Content.Equals($"{clan.Prefix}CANCEL",
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            DeleteMessages(messages);
                            throw new ProcessCanceledException();
                        }

                        //go back a question
                        if (response.Content.Equals($"{clan.Prefix}BACK", StringComparison.InvariantCultureIgnoreCase) && x > 0)
                        {
                            responded = true;
                            x = x - 2;
                        }

                        continue;
                    }

                    //process answer
                    try
                    {
                        answer = questionVo.ProcessAnswer(response.Content);
                    }
                    catch (AnswerLengthException ex)
                    {
                        var errorEmbed = new BotEmbed();
                        errorEmbed.Description = $"Please keep your answer to a maximum of `{ex.Length}` characters.";

                        messages.Add(await _discordMessages.SendMessageAsync(context.Channel.ChannelId, string.Empty, errorEmbed));
                        continue;
                    }
                    catch (InvalidOptionException ex)
                    {
                        var errorEmbed = new BotEmbed();
                        errorEmbed.Description = $"Please choose a valid option by entering a number from `{ex.MinIndex}` to `{ex.MaxIndex}`.";

                        messages.Add(await _discordMessages.SendMessageAsync(context.Channel.ChannelId, string.Empty, errorEmbed));
                        continue;
                    }
                    catch (NumberBelowMinException ex)
                    {
                        var errorEmbed = new BotEmbed();
                        errorEmbed.Description = $"Please enter a number more than or equal to `{ex.MinValue}`.";

                        messages.Add(await _discordMessages.SendMessageAsync(context.Channel.ChannelId, string.Empty, errorEmbed));
                        continue;
                    }
                    catch (NumberAboveMaxException ex)
                    {
                        var errorEmbed = new BotEmbed();
                        errorEmbed.Description = $"Please enter a number less than or equal to `{ex.MaxValue}`.";

                        messages.Add(await _discordMessages.SendMessageAsync(context.Channel.ChannelId, string.Empty, errorEmbed));
                        continue;
                    }
                    catch (NumberParseException)
                    {
                        var errorEmbed = new BotEmbed();
                        errorEmbed.Description = $"Please enter a number.";

                        messages.Add(await _discordMessages.SendMessageAsync(context.Channel.ChannelId, string.Empty, errorEmbed));
                        continue;
                    }
                    catch (PolarAnswerException)
                    {
                        var errorEmbed = new BotEmbed();
                        errorEmbed.Description = $"Please answer this question by typing either `Y` or `N`.";

                        messages.Add(await _discordMessages.SendMessageAsync(context.Channel.ChannelId, string.Empty, errorEmbed));
                        continue;
                    }
                    catch (InvalidAnswerException)
                    {
                        var errorEmbed = new BotEmbed();
                        errorEmbed.Description = $"Please enter a valid answer.";

                        messages.Add(await _discordMessages.SendMessageAsync(context.Channel.ChannelId, string.Empty, errorEmbed));
                        continue;
                    }
                    catch (Exception ex)
                    {
                        var errorEmbed = new BotEmbed();
                        errorEmbed.Description = $"An error occurred while processing your response. {Environment.NewLine}Please try again or contact a moderator if this keeps happening.";

                        messages.Add(await _discordMessages.SendMessageAsync(context.Channel.ChannelId, string.Empty, errorEmbed));
                        continue;
                    }

                    //check for empty answer
                    if (string.IsNullOrWhiteSpace(answer))
                    {
                        var errorEmbed = new BotEmbed();
                        errorEmbed.Description = $"Please enter a valid answer.";

                        messages.Add(await _discordMessages.SendMessageAsync(context.Channel.ChannelId, string.Empty, errorEmbed));
                        continue;
                    }

                    //save answer and next question
                    answers.Add(new Answer(questionVo, answer));
                    responded = true;


                } while (responded == false);
            }

            DeleteMessages(messages);

            return answers;
        }

        private void DeleteMessages(List<BotMessage> messages)
        {
            messages.Reverse();
            foreach (var botMessage in messages)
            {
                _ = _discordMessages.DeleteMessageAsync(botMessage);
            }
        }
    }
}