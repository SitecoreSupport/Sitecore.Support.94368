using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Diagnostics;
using System.Xml;
using Sitecore.Data;

namespace Sitecore.Support.Links
{
    public class LinkProvider : Sitecore.Links.LinkProvider
    {
        private string EncodeNamesRecursive(Item item, string currResult = "")
        {
            string dispName = item.DisplayName;
            
            dispName = Sitecore.MainUtil.EncodeName(dispName);

            if (String.IsNullOrEmpty(currResult))
            {
                currResult = dispName;
            }
            else
            {
                currResult = dispName + "/" + currResult;
            }

            if (item.Parent != null && item.ParentID != Sitecore.ItemIDs.ContentRoot && item.ParentID != Sitecore.ItemIDs.RootID)
            {
                return EncodeNamesRecursive(item.Parent, currResult);
            }
            else
            {
                return currResult;
            }

        }
        public override string GetItemUrl(Item item, UrlOptions options)
        {
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNull(options, "options");
            LinkBuilder linkBuilder = this.CreateLinkBuilder(options);
            string text = linkBuilder.GetItemUrl(item);
            if (UseDisplayName)
            {
                text = EncodeNamesRecursive(item);
            }
            
            if (options.LowercaseUrls)
            {
                text = text.ToLowerInvariant();
            }
            return text;
        }
    }
}