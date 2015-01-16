﻿using CVChatbot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheCommonLibrary.Extensions;

namespace CVChatbot.Commands
{
    /// <summary>
    /// Used for editing the last completed session's review count.
    /// </summary>
    public class LastSessionEditCount : UserCommand
    {
        public override void RunCommand(ChatExchangeDotNet.Message userMessage, ChatExchangeDotNet.Room chatRoom)
        {
            using (SOChatBotEntities db = new SOChatBotEntities())
            {
                //get the last complete session
                var lastSession = db.ReviewSessions
                    .Where(x => x.RegisteredUser.ChatProfileId == userMessage.AuthorID)
                    .Where(x => x.SessionEnd != null)
                    .OrderByDescending(x => x.SessionStart)
                    .FirstOrDefault();

                if (lastSession == null)
                {
                    chatRoom.PostReply(userMessage, "You have no completed review sessions on record, so I can't edit any entries.");
                    return;
                }

                var newReviewCount = GetRegexMatchingObject()
                    .Match(GetMessageContentsReadyForRegexParsing(userMessage))
                    .Groups[1]
                    .Value
                    .Parse<int>();

                var previousReviewCount = lastSession.ItemsReviewed;
                lastSession.ItemsReviewed = newReviewCount;

                string replyMessage = @"    Review item count has been changed:
    User: {0} ({1})
    Start Time: {2}
    End Time: {3}
    Items Reviewed: {4} -> {5}
    Use the command 'last session stats' to see more details."
                    .FormatInline(
                        userMessage.AuthorName,
                        userMessage.AuthorID,
                        lastSession.SessionStart.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss 'UTC'"),
                        lastSession.SessionEnd.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss 'UTC'"),
                        previousReviewCount.HasValue
                            ? previousReviewCount.Value.ToString()
                            : "[Not Set]", 
                        lastSession.ItemsReviewed.Value);

                db.SaveChanges();
                chatRoom.PostReply(userMessage, replyMessage);
            }
        }

        public override ActionPermissionLevel GetPermissionLevel()
        {
            return ActionPermissionLevel.Registered;
        }

        protected override string GetMatchPattern()
        {
            return @"^last session edit count (\d+)$";
        }

        public override string GetCommandName()
        {
            return "Last Session Edit Count";
        }

        public override string GetCommandDescription()
        {
            return "edits the number of reviewed items in your last review session";
        }
    }
}
