using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;

namespace ConsoleApp2
{
    class Program
    {
          

        private const int recordsToCreate = 50;

        enum Statuses { Created, Confirmed, Renting, Returned, Canceled };
        static void Main(string[] args)
        {
            string connectionString = @"AuthType=OAuth;
               Username=andrey@kravtsov2.onmicrosoft.com;
               Password=3F&$zK3XF6x5;
               Url=https://kravtsov.crm.dynamics.com/;
               AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;
               RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;";
            CrmServiceClient svc = new CrmServiceClient(connectionString);

            Statuses status=Statuses.Created;
            for (int i = 1; i <= recordsToCreate; i++)
            {
                Console.WriteLine("{0,2} of {1}", i, recordsToCreate);
                Entity rent = new Entity("new_rent");
                rent.Attributes.Add("new_name", "" + DateTime.Now.ToString("dd/MM - HH:mm:ss"));
                PickupDateAssign(rent);
                HandoverDateAssign(rent);
                PickupReturnLocationsAssign(svc, rent);
                CarClassAssign(svc, rent);
                CarAssign(svc, rent);

                int t = i % 20;         //Status should be picked up randomly with probabilities:
                if (t <= 14)            //Returned (0.75)
                {                       //For Returned – create Pickup and Return reports and fill respective lookups
                    status = Statuses.Returned;     
                    CreatePickupReport(svc, rent);
                    CreateReturnReport(svc, rent);                //If Status = Returned, Paid should be set to yes 
                    if (new Random().NextDouble() <= 0.9998)       //with probability 0.9998
                        rent["new_paid"] = true;
                }

                switch (t)
                {
                    case 15:
                        status = Statuses.Created;
                        break;
                    case 16:
                        status = Statuses.Confirmed;
                        if ((new Random().NextDouble()) <= 0.9)    //If Status = Confirmed, Paid should be set to yes with probability 0.9
                            rent["new_paid"] = true;
                        break;
                    case 17:
                        status = Statuses.Renting;                //For Renting – create Pickup report
                        CreatePickupReport(svc, rent);
                        if (new Random().NextDouble() <= 0.999)   //Status = Renting, Paid should be set to yes with probability 0.999
                            rent["new_paid"] = true;
                        break;
                    case 18:
                        status = Statuses.Canceled;
                        break;
                    case 19:
                        status = Statuses.Canceled;
                        break;
                }
                StatusAssign(rent, status);
                CustomerAssign(svc, rent);
                svc.Create(rent);
            }
        }


        static void PickupDateAssign(Entity entity)
        {
            DateTime reservedPickup = new DateTime(2019, 1, 1).AddDays(new Random().Next(0, 731));
            entity.Attributes.Add("new_reservedpickup", reservedPickup);
        }

        static void HandoverDateAssign(Entity entity)
        {
            DateTime reservedHandover = ((DateTime)entity.Attributes["new_reservedpickup"]).AddDays(new Random().Next(0, 30));
            entity.Attributes.Add("new_reservedhandover", reservedHandover);
        }

        static void CarClassAssign(CrmServiceClient svc, Entity entity)
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

        static void CarAssign(CrmServiceClient svc, Entity entity)
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

        static void CustomerAssign(CrmServiceClient svc, Entity entity)
        {
            using (OrganizationServiceContext context = new OrganizationServiceContext(svc))
            {
                var accounts = (from _accounts in context.CreateQuery("account")
                                select _accounts
                            ).ToArray();

                EntityReference customerRef = new EntityReference("account", accounts.ElementAt(new Random().Next(0, accounts.Length)).Id);
                entity["new_customer"] = customerRef;
            }
        }

        static void PickupReturnLocationsAssign(CrmServiceClient svc, Entity entity)
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

        static void StatusAssign(Entity entity, Statuses status)
        {
            switch (status)
            {
                case Statuses.Created:
                    entity["statuscode"] = new OptionSetValue(1);
                    entity["statecode"] = new OptionSetValue(0);
                    break;
                case Statuses.Confirmed:
                    entity["statuscode"] = new OptionSetValue(100000000);
                    entity["statecode"] = new OptionSetValue(0);
                    break;
                case Statuses.Renting:
                    entity["statuscode"] = new OptionSetValue(100000001);
                    entity["statecode"] = new OptionSetValue(0);
                    break;
                case Statuses.Returned:
                    entity["statuscode"] = new OptionSetValue(2);
                    entity["statecode"] = new OptionSetValue(1);
                    break;
                case Statuses.Canceled:
                    entity["statuscode"] = new OptionSetValue(100000002);
                    entity["statecode"] = new OptionSetValue(1);
                    break;
            }
        }

        static void CreatePickupReport(CrmServiceClient svc, Entity entity)
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

        static void CreateReturnReport(CrmServiceClient svc, Entity entity)
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
