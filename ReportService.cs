using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace Module_2
{
    class ReportService
    {
        private readonly CrmServiceClient svc;
        private readonly Entity entity;

        public ReportService(CrmServiceClient svc, Entity entity)
        {
            this.svc = svc;
            this.entity = entity;
        }

        public void CreatePickupReport()
        {
            Entity report = new Entity("drn_cartransferreport");
            report["drn_name"] = "Pickup_" + DateTime.Now.ToString("dd/MM - HH:mm:ss");

            EntityReference carRef = new EntityReference("drn_car", ((EntityReference)entity["drn_car"]).Id);
            report["drn_car"] = carRef;
            report["drn_type"] = new OptionSetValue(172620000);
            report["drn_date"] = entity["drn_reservedpickup"];
            report["drn_damages"] = false;

            if ((new Random().NextDouble()) < 0.05)
            {
                report["drn_damages"] = true;
                report["drn_damagedescription"] = "Damage";
            }
            Guid pickupReportGuid = svc.Create(report);
            EntityReference rentingRef = new EntityReference("drn_cartransferreport", pickupReportGuid);
            entity["drn_pickupreport"] = rentingRef;
        }

        public void CreateReturnReport()
        {
            Entity report = new Entity("drn_cartransferreport");
            report["drn_name"] = "Return_" + DateTime.Now.ToString("dd/MM-HH:mm:ss");

            EntityReference carRef = new EntityReference("drn_car", ((EntityReference)entity["drn_car"]).Id);
            report["drn_car"] = carRef;
            report["drn_type"] = new OptionSetValue(172620001);
            report["drn_date"] = entity["drn_reservedhandover"];
            report["drn_damages"] = false;
            if ((new Random().NextDouble()) < 0.05)
            {
                report["drn_damages"] = true;
                report["drn_damagedescription"] = "Damage";
            }

            Guid returnReportGuid = svc.Create(report);

            EntityReference returnRef = new EntityReference("drn_cartransferreport", returnReportGuid);
            entity["drn_returnreport"] = returnRef;
        }
    }
}
