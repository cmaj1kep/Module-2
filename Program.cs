using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;

namespace Module_2
{
    class Program
    {
        private const int recordsToCreate = 50;
        static void Main(string[] args)
        {
            string connectionString = @"AuthType=OAuth;
               Username=andrey@kravtsov2.onmicrosoft.com;
               Password=3F&$zK3XF6x5;
               Url=https://kravtsov.crm.dynamics.com/;
               AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;
               RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;";
            CrmServiceClient svc = new CrmServiceClient(connectionString);



            for (int i = 1; i <= recordsToCreate; i++)
            {
                Console.WriteLine("{0,2} of {1}", i, recordsToCreate);
                Entity rent = new Entity("new_rent");
                rent.Attributes.Add("new_name", "" + DateTime.Now.ToString("dd/MM - HH:mm:ss"));

                PickupHandover pickupHandover = new PickupHandover(svc, rent);
                pickupHandover.PickupDateAssign();
                pickupHandover.HandoverDateAssign();
                pickupHandover.PickupReturnLocationsAssign();

                Car car = new Car(svc, rent);
                car.CarClassAssign();
                car.CarAssign();

                Status rentStatus = new Status(svc, rent);
                Reports reports = new Reports(svc, rent);
                int t = i % 20;         //Status should be picked up randomly with probabilities:
                if (t <= 14)            //Returned (0.75)
                {                       //For Returned – create Pickup and Return reports and fill respective lookups
                    rentStatus.status = Status.Statuses.Returned;
                    reports.CreatePickupReport();
                    reports.CreateReturnReport();                //If Status = Returned, Paid should be set to yes 
                    if (new Random().NextDouble() <= 0.9998)     //with probability 0.9998
                        rent["new_paid"] = true;
                }

                switch (t)
                {
                    case 15:
                        rentStatus.status = Status.Statuses.Created;
                        break;
                    case 16:
                        rentStatus.status = Status.Statuses.Confirmed;
                        if ((new Random().NextDouble()) <= 0.9)   //If Status = Confirmed, Paid should be set to yes with probability 0.9
                            rent["new_paid"] = true;
                        break;
                    case 17:
                        rentStatus.status = Status.Statuses.Renting;                //For Renting – create Pickup report
                        reports.CreatePickupReport();
                        if (new Random().NextDouble() <= 0.999)   //Status = Renting, Paid should be set to yes with probability 0.999
                            rent["new_paid"] = true;
                        break;
                    case 18:
                        rentStatus.status = Status.Statuses.Canceled;
                        break;
                    case 19:
                        rentStatus.status = Status.Statuses.Canceled;
                        break;
                }
                rentStatus.StatusAssign(rentStatus.status);

                Customer customer = new Customer(svc, rent);
                customer.CustomerAssign();
                svc.Create(rent);
            }
        }

















    }
}
