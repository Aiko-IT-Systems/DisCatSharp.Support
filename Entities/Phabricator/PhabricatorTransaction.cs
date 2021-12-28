using Newtonsoft.Json;

using System.Collections.Generic;

namespace DisCatSharp.Support.Entities.Phabricator
{
    public class PhabricatorTransaction
    {
        [JsonProperty("transactions")]
        public List<TransactionObject> Transactions;

        [JsonProperty("objectIdentifier")]
        public string ObjectIdentifier;

        public PhabricatorTransaction(string phid)
        {
            Transactions = new();
            ObjectIdentifier = phid;
        }

        public Dictionary<string, dynamic> SerializeParameters()
        {
            Dictionary<string, dynamic> parameters = new();
            parameters.Add("transactions", this.Transactions);
            parameters.Add("objectIdentifier", this.ObjectIdentifier);

            return parameters;
        }
    }

    public class TransactionObject
    {
        [JsonProperty("type")]
        public string Type;

        [JsonProperty("value")]
        public object Value;

        public TransactionObject(string type, object value)
        {
            Type = type;
            Value = value;
        }
    }
}
