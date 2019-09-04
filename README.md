# Context

When I came accross Domain Driven Design (DDD) a couple of years ago, it was the biggest revelation in my software development carreer. I could relate what I had been doing previously to a **Big Ball of Mud** as Eric Evans coined it so well. After a lot of hard work pushing myself to learn DDD, I re-developped the IAM used by Vaughn Vernnon in his book [Implementing Domain Driven Design] from scratch using Java 8. I then discovered functional programing and it completely changed my outlook on programming, and I could not stop wondering why the OOP is so mainstrain. It took me several months to transition from the OOP to FFP. One of my biggest projects was converting an  [IAM System](https://github.com/diaspogift/identity-and-access) that I previously implemented in Java (using DDD) into a F# Project using the Funtional Programming Paragdim. It is in this mindset that I set out to write this piece of software as the first big venture into the world of functional programming. This is an initial version that I intend to improve in the comming months. I am currently working up to make a Haskell version and in parallel using Elm to replace the [Angular UI](https://github.com/diaspogift/identity-and-access-ui) that was implemented for the initial Java IAM. I would be happy for any feeback or construtive criticism from anyone willing to help. 


# Indentities and Accesses Management System

Identity and access management system is a back end service that allows to:
* Register tenants with their default adminitrator user accounts
* Invite and register users under a given tenancy
* Group users based on certain affinities
* Assign/Unassign them to/from roles
* Handle both users' and tenants' access statuses 

This my first attempt to translate the original IAM DDD_Sample developed by Vaughn Vernon in his book [Implementing Domain Driven Design] into a series of functional languages. I picked F# because it was the most accessible but I plan to follow up with a Haskell version in the upcomming months. The project uses the CQRS pattern and Grey Young event store for the persitence layer. 


## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

What things you need to install the software and how to install them
The following pieces of software need to be install and configured in order to deply/test the project. 

* mono: version >= 5.16.0
* dotnet: version 2.2.401
* EventStore: 5.0.2.0
* Postman: Latest version
* Operation System: Ubuntu 18.04


### Installing


* Install Grey Young's Event Store DB and start it
   * Run the follwing three commands from your terminal
      * curl -s https://packagecloud.io/install/repositories/EventStore/EventStore-OSS/script.deb.sh | sudo bash
      * sudo apt-get install eventstore-oss=5.0.2-1
      * sudo systemctl start eventstore
      * eventstored --version 
    * navigate to http://127.0.0.1:2113/web/index.html#/dashboard and click on the **Stream Browser** menu. If promtep to login, enter user the following credentials: **username: admin, password: changeit**. You can see the stream of events relative to the **Tenant, User, Group, and Role** aggregates beeing created as you play with the API through Postman. 
    
* Clone the repo

    * git clone https://github.com/diaspogift/identity_and_access_FSharp.git
    
* In your terminal, navigate to the identity_and_access_FSharp folder and run dotnet and run the following commands:
    
    * dotnet build
    * dotnet restore
    * dotnet run (This will start a local server on port 8080)
    
* Install postman or any other rest API client and try running these
    
    * Follow the instruction on this link: https://suave.io
               
               
               
## Running basic tests through Postman.

Fire up postman and try the following use cases: 





   * Provision a Tenant
        * Url:      http://localhost:8080/tenant-provisions
        * Method:   post
        * Data: 
```javascript 
            {
                  "TenantInfo" : { "Name":"Clinic le Poitier", "Description":"Hopital de reference"},
                   "AdminUserInfo" : { "FirstName" : "Felicien", 
                                       "MiddleName" : "N/A", 
                                       "LastName" : "Fotio", 
                                       "Email" : "felicien@gmail.com", 
                                       "Address" :  "Douala, Cameroun", 
                                       "PrimPhone" :  "669262690",
                                       "SecondPhone" : "669262691"
                                       }
            }
```

   * Reactivate a Tenant
        * Url: http://localhost:8080/activate-tenant
        * Method: post
        * Data: 
```javascript 
            { 
                "TenantId" : "5C570B52644AA37772E26671",
                "ActivationStatus" : true,
                "Reason" : "I do like youuuuuuu"
            }
```
    
   * Deactivate a Tenant
        * Url: http://localhost:8080/deactivate-tenant
        * Method: post
        * Data: 
```javascript 
         
             { 
                "TenantId" : "5C570B52644AA37772E26671",
                "ActivationStatus" : false,
                "Reason" : "I do not like youuuuuuu"
              }
```
         


    
   * Offert Registration Invitation
        * Url: http://localhost:8080/offert-invitations
        * Method: post
        * Data: 
```javascript 
         
            { 
                "TenantId" : "5C5752B0D389C32384152311",
                "Description" : "Invitation for Megan"
            }
```
        

                  
                  
    
   * Withdraw Registration Invitation 
        * Url: http://localhost:8080/withdraw-invitations
        * Method: post
        * Data: 
```javascript 

            { 
                "TenantId" : "5C538067026E46460ED95CC9",
                 "RegistrationInvitationId": "5C5387BD026E46460ED95D25"
            }
```


   * Register an Invited User
        * Url: http://localhost:8080/register-users
        * Method: post
        * Data: 
```javascript 

                { 
                    "TenantId" : "5C5752B0D389C32384152311",
                    "RegistrationInvitationId" : "5C575304D389C32384152319",
                    "Username" : "meg",
                    "Password" : "123456",
                    "Email" : "meg@gmail.com",
                    "Address" : "Denever, Co",
                    "PrimPhone" : "669373782",
                    "SecondPhone" : "669987645", 
                    "FirstName" : "Megan",
                    "MiddleName" : "Amanda",
                    "LastName" : "Hess"
                 }
```           
       
                    
   * Provision a Group
        * Url: http://localhost:8080/provision-group
        * Method: post
        * Data: 
```javascript 

            {   "TenantId" : "5c4f7527f8b9103cf82944bb",
                "Name" : "LEAD_DEVELOPER",
                "Description" : "Ils s occupent des developerus lideurs dans la boite",
                "Members" : []
            }
``` 
      
 

   * Provision a Role
        * Url: http://localhost:8080/provision-role
        * Method: post
        * Data: 
```javascript 

            {   
                "TenantId" : "5c4f7527f8b9103cf82944bb",
                "Name" : "Tester",
                "Description" : "Ils s occupent des developerus lideurs dans la boite"
            }
``` 
               
   * Assign User to Role
        * Url: http://localhost:8080/assign-user-to-role
        * Method: post
        * Data:
```javascript 
         
            { 
                "RoleId" : "5C538067026E46460ED95CC9",
                 "UserId": "5C5387BD026E46460ED95D25"
            }
``` 

            
               
   * Assign Group to Role
        * Url: http://localhost:8080/assign-group-to-role
        * Method: post
        * Data: 
```javascript 
        
            {   
                "GroupId" : "5C538067026E46460ED95CC9",
                "RoleId": "5C5387BD026E46460ED95D25"
            }
``` 
            
   * Add User to Group
        * Url: http://localhost:8080/withdraw-invitations
        * Method: post
        * Data: 
```javascript 

            { 
                "GroupId" : "5C538067026E46460ED95CC9",
                "UserID": "5C5387BD026E46460ED95D25"
            }
``` 


N/A


## Deployment

N/A

## Built With

* [F#](https://fsharp.org) - The language
* [Fake](https://fake.build/) - Dependency Management
* [Suave](https://suave.io/) - Used as API server
* [EventStore](https://eventstore.org) Used as the event store
* [Functional Domain Driven Design](https://eventstore.org) Used as an example for production ready code 
* [CQRS](https://eventstore.org) Used for the persistence layer

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

N/A

## Authors

* **Felicien FOTIO MANFO** 
* **Junior Didier NKALLA EHAWE** 


## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* To Vaun Vernon for its Implementing DDD book
* To Eric even for the DDD Philosophie and Book
* To Scott Wlashin for his book Domain Modeling made functional
* To Brian'O Sulivan for Real word haskell 
* To ----- for Progrmaing in Haskell
* To ----- for Learn you a great Haskell
* To Grey young for the CQRS and Event sourcing tools
