﻿using ChatExchangeDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVChatbot.Bot.ChatbotActions.Commands
{
    /// <summary>
    /// A User Command is a message sent directly to the chatbot which wants the chatbot to perform an action.
    /// </summary>
    public abstract class UserCommand : ChatbotAction
    {
        /// <summary>
        /// Takes the contents from the message, strips out any "mentions", and trims the sides of the string.
        /// </summary>
        /// <param name="incommingMessage"></param>
        /// <returns></returns>
        protected override sealed string GetMessageContentsReadyForRegexParsing(Message incommingMessage)
        {
            return incommingMessage
                .GetContentsWithStrippedMentions()
                .Trim();
        }

        /// <summary>
        /// Hard-coded to return true.
        /// If you want to run a User Command it must be a reply to the chatbot.
        /// </summary>
        /// <returns></returns>
        protected override bool GetMessageIsReplyToChatbotRequiredValue()
        {
            return true;
        }
    }
}
