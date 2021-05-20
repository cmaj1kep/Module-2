using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;


namespace Module_2
{
    class CarService
    {
        private readonly CrmServiceClient svc;
        private readonly Entity entity;
        public CarService(CrmServiceClient svc, Entity entity)
        {
            this.svc = svc;
            this.entity = entity;
        }
        public void CarClassAssign()
        {
            using (OrganizationServiceContext context = new OrganizationServiceContext(svc))
            {
                var carClasses = (from _carClasses in context.CreateQuery("drn_carclass")
                                  select _carClasses)
                                   .ToArray();
                Entity rndmCarClassEntity = carClasses.ElementAt(new Random().Next(0, carClasses.Length));
                EntityReference refName = new EntityReference("drn_carclass", rndmCarClassEntity.Id);
                entity["drn_carclass"] = refName;
            }
        }

        public void CarAssign()
        {
            using (OrganizationServiceContext context = new OrganizationServiceContext(svc))
            {
                var cars = (from _cars in context.CreateQuery("drn_car")
                            where _cars["drn_carclass"].Equals(entity.Attributes["drn_carclass"])
                            select _cars
                           ).ToArray();
                EntityReference carClassRef = new EntityReference("drn_car", cars.ElementAt(new Random().Next(0, cars.Length)).Id);
                entity["drn_car"] = carClassRef;
            }
        }
    }
}
