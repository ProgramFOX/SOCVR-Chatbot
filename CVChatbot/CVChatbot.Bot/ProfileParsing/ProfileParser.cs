﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCommonLibrary.Extensions;
using CVChatbot.Bot.Model;
using System.Threading;
using System.Net;
using System.Collections.Specialized;

namespace CVChatbot.Bot.ProfileParsing
{
    public class ProfileParser
    {
        public void GetNewReviewItemsForProfile(int profileId)
        {
            var newReviewItemIds = EnumNewCloseVoteReviewItemIdsFromProfile(profileId);

            using (CVChatBotEntities db = new CVChatBotEntities())
            {
                foreach (var reviewItemId in newReviewItemIds)
                {
                    var newReviewItem = ParseReviewItemById(reviewItemId);
                    db.ReviewItems.Add(newReviewItem);
                    db.SaveChanges();
                }
            }
        }

        private ReviewItem ParseReviewItemById(int reviewItemId)
        {
            string res;

            using (var wb = new WebClient())
            {
                var data = new NameValueCollection { { "taskTypeId", "2" } };
                res = Encoding.UTF8.GetString(wb.UploadValues("http://stackoverflow.com/review/next-task/" + reviewItemId, "POST", data));
            }


        }

        private void PopulateResults(string postResponse)
        {
            Results = new List<ReviewResult>();

            dynamic json = JsonConvert.DeserializeObject(postResponse);
            var html = (string)json["instructions"];
            var dom = CQ.Create(html);

            foreach (var i in dom[".review-results"])
            {
                var username = "";
                var timeStamp = new DateTime();
                var action = ReviewAction.LeaveOpen;

                foreach (var j in i.ChildElements)
                {
                    if (j.Attributes["href"] != null && j.Attributes["href"].StartsWith(@"/users/"))
                    {
                        username = j.InnerHTML;
                    }
                    else if (j.Attributes["title"] != null)
                    {
                        timeStamp = DateTime.Parse(j.Attributes["title"]);
                    }
                    else
                    {
                        switch (j.InnerHTML)
                        {
                            case "Close":
                                {
                                    action = ReviewAction.Close;
                                    break;
                                }

                            case "Leave Open":
                                {
                                    action = ReviewAction.LeaveOpen;
                                    break;
                                }

                            case "Edit":
                                {
                                    action = ReviewAction.Edit;
                                    break;
                                }
                        }
                    }
                }

                Results.Add(new ReviewResult(username, action, timeStamp));
            }
        }

        public IEnumerable<int> EnumNewCloseVoteReviewItemIdsFromProfile(int profileId)
        {
            int pageNumber = 1;
            var web = new HtmlWeb();

            bool doneSearching = false;

            using (CVChatBotEntities db = new CVChatBotEntities())
            {
                while (!doneSearching)
                {
                    //get the page
                    var url = BuildAjaxReviewsTabUrl(profileId, pageNumber);
                    var doc = web.Load(url);

                    var parsedItems = ParseSinglePageForReviewItemIds(profileId, doc, ref doneSearching, db);

                    foreach (var item in parsedItems)
                    {
                        yield return item;
                    }

                    Thread.Sleep(1000);
                    pageNumber += 1;
                }
            }
        }

        private List<int> ParseSinglePageForReviewItemIds(int profileId, HtmlDocument doc, ref bool doneSearching, CVChatBotEntities db)
        {
            List<int> reviewItemIds = new List<int>();

            //find the user tab contents div
            var userTabContentDiv = doc.DocumentNode
                .Descendants("div")
                .Where(x => x.Attributes["class"] != null)
                .Where(x => x.Attributes["class"].Value == "user-tab-content")
                .Single();

            //check if there are none on the page
            if (userTabContentDiv.InnerText.Trim() == "You have no reviews")
            {
                doneSearching = true;
                //break;
            }

            //get a list of all the cv reviews on the page
            var cvReviewItemIds = userTabContentDiv
                .Descendants("a")
                .Where(x => x.Attributes["class"] != null)
                .Where(x => x.Attributes["class"].Value == "reviewed-action")
                .Select(x => x.Attributes["href"].Value)
                .Where(x => x.StartsWith("/review/close/"))
                .Select(x => x.Split('/')[3])
                .Select(x => x.Parse<int>());

            foreach (var reviewItemId in cvReviewItemIds)
            {
                //check if the link is in the database already
                var entryExists = db.ReviewItems
                    .Where(x => x.RegisteredUser.ChatProfileId == profileId)
                    .Where(x => x.ReviewId == reviewItemId)
                    .Any();

                if (entryExists)
                {
                    doneSearching = true;
                    break; //entry exists, stop execution
                }

                reviewItemIds.Add(reviewItemId);
            }

            return reviewItemIds;
        }

        private string BuildAjaxReviewsTabUrl(int profileId, int pageNumber)
        {
            return "http://stackoverflow.com/ajax/users/tab/{0}?tab=activity&sort=reviews&page={1}"
                .FormatInline(profileId, pageNumber);
        }
    }
}
