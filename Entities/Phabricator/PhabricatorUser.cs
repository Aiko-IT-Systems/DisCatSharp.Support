using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Globalization;

namespace DisCatSharp.Support.Entities.Phabricator
{
    public class Policy
    {
        [JsonProperty("view", NullValueHandling = NullValueHandling.Ignore)]
        public string View;

        [JsonProperty("edit", NullValueHandling = NullValueHandling.Ignore)]
        public string Edit;
    }

    public class Fields
    {
        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string Username;

        [JsonProperty("realName", NullValueHandling = NullValueHandling.Ignore)]
        public string RealName;

        [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Roles;

        [JsonProperty("dateCreated", NullValueHandling = NullValueHandling.Ignore)]
        internal int _dateCreated;

        [JsonIgnore]
        public DateTimeOffset? DateCreated =>
            DateTimeOffset.TryParse(this._dateCreated.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var dto) ? dto : null;

        [JsonProperty("dateModified", NullValueHandling = NullValueHandling.Ignore)]
        internal int _dateModified;

        [JsonIgnore]
        public DateTimeOffset? DateModified =>
            DateTimeOffset.TryParse(this._dateModified.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var dto) ? dto : null;

        [JsonProperty("policy", NullValueHandling = NullValueHandling.Ignore)]
        public Policy Policy;

        [JsonProperty("custom.role", NullValueHandling = NullValueHandling.Ignore)]
        public string CustomRole;

        [JsonProperty("custom.discord", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? CustomDiscord;
    }

    public class Attachments
    {
        [JsonProperty("subscribers", NullValueHandling = NullValueHandling.Ignore)]
        public Subscribers Subscribers;
    }

    public class Subscribers
    {
        [JsonProperty("subscriberPHIDs", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> SubscriberPHIDs;

        [JsonProperty("subscriberCount", NullValueHandling = NullValueHandling.Ignore)]
        public int SubscriberCount;

        [JsonProperty("viewerIsSubscribed", NullValueHandling = NullValueHandling.Ignore)]
        public bool ViewerIsSubscribed;
    }


    public class Maps
    {
    }

    public class Query
    {
        [JsonProperty("queryKey", NullValueHandling = NullValueHandling.Ignore)]
        public object QueryKey;
    }

    public class Cursor
    {
        [JsonProperty("limit", NullValueHandling = NullValueHandling.Ignore)]
        public int Limit;

        [JsonProperty("after", NullValueHandling = NullValueHandling.Ignore)]
        public string After;

        [JsonProperty("before", NullValueHandling = NullValueHandling.Ignore)]
        public string Before;

        [JsonProperty("order", NullValueHandling = NullValueHandling.Ignore)]
        public string Order;
    }

    public class Data
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int Id;

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type;

        [JsonProperty("phid", NullValueHandling = NullValueHandling.Ignore)]
        public string Phid;

        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public Fields Fields;

        [JsonProperty("attachments", NullValueHandling = NullValueHandling.Ignore)]
        public Attachments Attachments;
    }
    public class Result
    {
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public List<Data> Data;

        [JsonProperty("maps", NullValueHandling = NullValueHandling.Ignore)]
        public Maps Maps;

        [JsonProperty("query", NullValueHandling = NullValueHandling.Ignore)]
        public Query Query;

        [JsonProperty("cursor", NullValueHandling = NullValueHandling.Ignore)]
        public Cursor Cursor;
    }

    public class UserSearch
    {
        [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)]
        public Result Result;

        [JsonProperty("error_code", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorCode;

        [JsonProperty("error_info", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorInfo;
    }

    public class QResult
    {
        [JsonProperty("phid", NullValueHandling = NullValueHandling.Ignore)]
        public string Phid;

        [JsonProperty("userName", NullValueHandling = NullValueHandling.Ignore)]
        public string UserName;

        [JsonProperty("realName", NullValueHandling = NullValueHandling.Ignore)]
        public string RealName;

        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image;

        [JsonProperty("uri", NullValueHandling = NullValueHandling.Ignore)]
        public string Uri;

        [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Roles;
    }

    public class Extended
    {
        [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)]
        public List<QResult> QResult;

        [JsonProperty("error_code", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorCode;

        [JsonProperty("error_info", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorInfo;
    }

}
