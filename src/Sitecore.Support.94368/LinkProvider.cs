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
        private string ResolveDisplayNames(Item item, LinkBuilder builder, string currResult = "")
        {
            string dispName = item.DisplayName;
            if (!String.IsNullOrEmpty(dispName))
            {
                XmlNode repNode = Configuration.Factory.GetConfigNode("/sitecore/encodeNameReplacements");
                if (repNode != null)
                {
                    foreach (XmlNode childNode in repNode.ChildNodes)
                    {
                        string mode = childNode != null ? childNode.Attributes["mode"].Value : String.Empty;
                        if (String.IsNullOrEmpty(mode))
                        {
                            mode = "on";
                        }
                        if (childNode != null && mode == "on")
                        {
                            if (String.IsNullOrEmpty(childNode.Attributes["find"].Value))
                            {
                                Log.Error("'encodeNameReplacements' Error. The value of the 'Find' attribute can't be empty", this);
                            }
                            else if (dispName.Contains(childNode.Attributes["find"].Value))
                            {
                                if (String.IsNullOrEmpty(childNode.Attributes["replaceWith"].Value))
                                {
                                    Log.Error("'encodeNameReplacements' Error. The value of the 'replaceWith' attribute can't be empty", this);
                                }
                                else
                                {
                                    dispName = dispName.Replace(childNode.Attributes["find"].Value, childNode.Attributes["replaceWith"].Value);
                                }
                            }
                        }
                    }
                    if (String.IsNullOrEmpty(currResult))
                    {
                        currResult = dispName;
                    }
                    else
                    {
                        currResult = dispName + "/" + currResult;
                    }
                }
            }
            else
            {
                if (String.IsNullOrEmpty(currResult))
                {
                    currResult = builder.GetItemUrl(item);
                }
                else
                {
                    currResult = builder.GetItemUrl(item) + "/" + currResult;
                }
            }
            if (item.Parent != null && item.ParentID != Sitecore.ItemIDs.ContentRoot && item.ParentID != Sitecore.ItemIDs.RootID)
            {
               return ResolveDisplayNames(item.Parent,builder, currResult);
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
                text = ResolveDisplayNames(item,linkBuilder);
            }
            
            if (options.LowercaseUrls)
            {
                text = text.ToLowerInvariant();
            }
            return text;
        }
    }
}