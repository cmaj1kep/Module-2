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
        private const int recordsToCreate = 40000;
        static void Main(string[] args)
        {
            string connectionString = @"AuthType=OAuth;
               Username=andrey@kravtsov3.onmicrosoft.com;
               Password=9Nxc4uPDu;
               Url=https://kravtsov.crm4.dynamics.com;
               AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;
               RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;";
            CrmServiceClient svc = new CrmServiceClient(connectionString);

            for (int i = 1; i <= recordsToCreate; i++)
            {
                Console.WriteLine("{0,2} of {1}", i, recordsToCreate);
                Entity rent = new Entity("drn_rent");
                rent["drn_name"] = "" + DateTime.Now.ToString("dd/MM - HH:mm:ss");
                PickupHandoverService pickupHandover = new PickupHandoverService(svc, rent);
                pickupHandover.PickupDateAssign();
                pickupHandover.HandoverDateAssign();
                pickupHandover.PickupReturnLocationsAssign();       

                CarService car = new CarService(svc, rent);
                car.CarClassAssign();
                car.CarAssign();

                StatusService rentStatus = new StatusService(svc, rent);
                ReportService reports = new ReportService(svc, rent);
                int t = i % 20;         
                if (t <= 14)            
                {                       
                    rentStatus.status = StatusService.Statuses.Returned;
                    reports.CreatePickupReport();
                    reports.CreateReturnReport();               
                    if (new Random().NextDouble() <= 0.9998)    
                        rent["drn_paid"] = true;
                }

                switch (t)
                {
                    case 15:
                        rentStatus.status = StatusService.Statuses.Created;
                        break;
                    case 16:
                        rentStatus.status = StatusService.Statuses.Confirmed;
                        if ((new Random().NextDouble()) <= 0.9)  
                            rent["drn_paid"] = true;
                        break;
                    case 17:
                        rentStatus.status = StatusService.Statuses.Renting;              
                        reports.CreatePickupReport();
                        if (new Random().NextDouble() <= 0.999)   
                            rent["drn_paid"] = true;
                        break;
                    case 18:
                        rentStatus.status = StatusService.Statuses.Canceled;
                        break;
                    case 19:
                        rentStatus.status = StatusService.Statuses.Canceled;
                        break;
                }
                rentStatus.StatusAssign(rentStatus.status);

                CustomerService customer = new CustomerService(svc, rent);
                customer.CustomerAssign();
                Guid g = svc.Create(rent);
                Console.WriteLine(g);
            }
        }
    }
}
