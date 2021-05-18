using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;

namespace Module_2
{
    class PickupHandover
    {
        private readonly Entity entity;
        readonly CrmServiceClient svc;
        public PickupHandover(CrmServiceClient svc, Entity entity)
        {
            this.entity = entity;
            this.svc = svc;
        }
        public void PickupDateAssign()
        {
            DateTime reservedPickup = new DateTime(2019, 1, 1).AddDays(new Random().Next(0, 731));
            entity.Attributes.Add("new_reservedpickup", reservedPickup);
        }

        public void HandoverDateAssign()
        {
            DateTime reservedHandover = ((DateTime)entity.Attributes["new_reservedpickup"]).AddDays(new Random().Next(0, 30));
            entity.Attributes.Add("new_reservedhandover", reservedHandover);
        }

        public void PickupReturnLocationsAssign()
        {
            RetrieveAttributeRequest request = new RetrieveAttributeRequest
            {
                EntityLogicalName = "new_rent",
                LogicalName = "new_pickuplocation",
                RetrieveAsIfPublished = true
            };

            RetrieveAttributeResponse response = (RetrieveAttributeResponse)svc.Execute(request);

            var dic = ((EnumAttributeMetadata)response.AttributeMetadata).OptionSet.Options.ToDictionary(key => key.Label.UserLocalizedLabel.Label, option => option.Value.Value);

            entity.Attributes["new_pickuplocation"] = new OptionSetValue(dic.ElementAt(new Random().Next(0, dic.Count - 1)).Value);

            //return
            RetrieveAttributeRequest request2 = new RetrieveAttributeRequest
            {
                EntityLogicalName = "new_rent",
                LogicalName = "new_returnlocation",
                RetrieveAsIfPublished = true
            };

            RetrieveAttributeResponse response2 = (RetrieveAttributeResponse)svc.Execute(request2);

            var dic2 = ((EnumAttributeMetadata)response2.AttributeMetadata).OptionSet.Options.ToDictionary(key => key.Label.UserLocalizedLabel.Label, option => option.Value.Value);

            entity.Attributes["new_returnlocation"] = new OptionSetValue(dic2.ElementAt(new Random().Next(0, dic2.Count - 1)).Value);
        }
    }
}
