using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace Module_2
{
    class Reports
    {
        private readonly CrmServiceClient svc;
        private readonly Entity entity;
        public Reports(CrmServiceClient svc, Entity entity)
        {
            this.svc = svc;
            this.entity = entity;
        }
        public void CreatePickupReport()
        {
            Entity report = new Entity("new_cartransferreport");
            report.Attributes.Add("new_name", "Pickup_" + DateTime.Now.ToString("dd/MM - HH:mm:ss"));

            EntityReference carRef = new EntityReference("new_car", ((EntityReference)entity["new_car"]).Id);
            report["new_car"] = carRef;
            report["new_type"] = new OptionSetValue(100000000);
            report["new_date"] = entity["new_reservedpickup"];
            report["new_damages"] = false;
            if ((new Random().NextDouble()) < 0.05)
            {
                report["new_damages"] = true;
                report["new_damagedescription"] = "Damage";
            }
            Guid pickupReportGuid = svc.Create(report);
            EntityReference rentingRef = new EntityReference("new_cartransferreport", pickupReportGuid);
            entity["new_pickupreport"] = rentingRef;
        }

        public void CreateReturnReport()
        {
            Entity report = new Entity("new_cartransferreport");
            report.Attributes.Add("new_name", "Return_" + DateTime.Now.ToString("dd/MM - HH:mm:ss"));

            EntityReference carRef = new EntityReference("new_car", ((EntityReference)entity["new_car"]).Id);
            report["new_car"] = carRef;
            report["new_type"] = new OptionSetValue(100000001);
            report["new_date"] = entity["new_reservedhandover"];
            report["new_damages"] = false;
            if ((new Random().NextDouble()) < 0.05)
            {
                report["new_damages"] = true;
                report["new_damagedescription"] = "Damage";
            }
            Guid returnReportGuid = svc.Create(report);
            EntityReference returnRef = new EntityReference("new_cartransferreport", returnReportGuid);
            entity["new_returnreport"] = returnRef;
        }
    }
}
