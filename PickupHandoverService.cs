using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;

namespace Module_2
{
    class PickupHandoverService
    {
        private readonly Entity entity;
        readonly CrmServiceClient svc;
        public PickupHandoverService(CrmServiceClient svc, Entity entity)
        {
            this.entity = entity;
            this.svc = svc;
        }
        public void PickupDateAssign()
        {
            DateTime reservedPickup = new DateTime(2019, 1, 1).AddDays(new Random().Next(0, 731));
            entity["drn_reservedpickup"] = reservedPickup;
        }

        public void HandoverDateAssign()
        {
            DateTime reservedHandover = ((DateTime)entity["drn_reservedpickup"]).AddDays(new Random().Next(0, 30));
            entity["drn_reservedhandover"] = reservedHandover;
        }

        public void PickupReturnLocationsAssign()
        {
            var pickupRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = "drn_rent",
                LogicalName = "drn_pickuplocation",
                RetrieveAsIfPublished = true
            };

            var pickupResponse = (RetrieveAttributeResponse)svc.Execute(pickupRequest);
            var pickupMetadata = (EnumAttributeMetadata)pickupResponse.AttributeMetadata;

            var optionList = (from o in pickupMetadata.OptionSet.Options
                              select new { Value = o.Value })
                             .ToList();
            var pickupRandomOptionsetValue = (optionList.ElementAt(new Random().Next(0, optionList.Count))).Value;

            entity["drn_pickuplocation"] = new OptionSetValue(pickupRandomOptionsetValue.Value);


            var handoverRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = "drn_rent",
                LogicalName = "drn_pickuplocation",
                RetrieveAsIfPublished = true
            };

            var handoverResponse = (RetrieveAttributeResponse)svc.Execute(handoverRequest);
            var handoverMetadata = (EnumAttributeMetadata)handoverResponse.AttributeMetadata;

            var handoverOptionList = (from o in handoverMetadata.OptionSet.Options
                              select new { Value = o.Value })
                             .ToList();
            var handoverRandomOptionsetValue = (handoverOptionList.ElementAt(new Random().Next(0, optionList.Count))).Value;

            entity["drn_pickuplocation"] = new OptionSetValue(handoverRandomOptionsetValue.Value);
        }
    }
}
