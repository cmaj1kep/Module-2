using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;


namespace Module_2
{
    class Car
    {
        private readonly CrmServiceClient svc;
        private readonly Entity entity;
        public Car(CrmServiceClient svc, Entity entity)
        {
            this.svc = svc;
            this.entity = entity;
        }
        public void CarClassAssign()
        {
            using (OrganizationServiceContext context = new OrganizationServiceContext(svc))
            {
                var carClasses = (from _carClasses in context.CreateQuery("new_carclass")
                                  select _carClasses)
                                   .ToArray();
                Entity rndmCarClassEntity = carClasses.ElementAt(new Random().Next(0, carClasses.Length));
                EntityReference refName = new EntityReference("new_carclass", rndmCarClassEntity.Id);
                entity["new_carclass"] = refName;
            }
        }

        public void CarAssign()
        {
            using (OrganizationServiceContext context = new OrganizationServiceContext(svc))
            {
                var cars = (from _cars in context.CreateQuery("new_car")
                            where _cars["new_carclass"].Equals(entity.Attributes["new_carclass"])
                            select _cars
                           ).ToArray();
                EntityReference carClassRef = new EntityReference("new_car", cars.ElementAt(new Random().Next(0, cars.Length)).Id);
                entity["new_car"] = carClassRef;
            }
        }
    }
}
