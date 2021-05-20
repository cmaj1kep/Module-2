using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace Module_2
{

    class StatusService
    {

        public enum Statuses { Created, Confirmed, Renting, Returned, Canceled };
        public Statuses status;
        private readonly CrmServiceClient svc;
        private readonly Entity entity;

        public StatusService(CrmServiceClient svc, Entity entity)
        {
            this.svc = svc;
            this.entity = entity;
        }

        public void StatusAssign(Statuses s)
        {
            switch (s)
            {
                case Statuses.Created:
                    entity["statuscode"] = new OptionSetValue(1);
                    entity["statecode"] = new OptionSetValue(0);
                    break;
                case Statuses.Confirmed:
                    entity["statuscode"] = new OptionSetValue(172620000);
                    entity["statecode"] = new OptionSetValue(0);
                    break;
                case Statuses.Renting:
                    entity["statuscode"] = new OptionSetValue(172620001);
                    entity["statecode"] = new OptionSetValue(0);
                    break;
                case Statuses.Returned:
                    entity["statuscode"] = new OptionSetValue(2);
                    entity["statecode"] = new OptionSetValue(1);
                    break;
                case Statuses.Canceled:
                    entity["statuscode"] = new OptionSetValue(172620002);
                    entity["statecode"] = new OptionSetValue(1);
                    break;
            }
        }
    }
}
