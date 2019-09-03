# Context

When I came accross DDD a couple of years ago, first I thought it was the biggest revelation in my carreer. I then discover functional programing and it almost stop me brething as I could not stop wondering why the OOP is so mainstrain. It took several months year to transition from the OOP to FFP. One of my concrete realisation is converting the DDD_Sample from Von Vernon into a Functional F# Project. It's in this context that I set out to write this piece of software. 


# Indentities and Accesses Management System

Identity and access management system is a back end service that allows allow to:
* Register tenants with their defaults adminitrators users
* Invite and register users under a given tenancy
* Group them base on certain affinities
* Assign/Unassign them to/from roles
* Handle their accesses statuses 

This my first attempt to translate the original IAM DDD_Sample developed by Von Vernon in his book : Implementing Domain Driven Design into a serie of functional languages. I picked F# because it was the most accessible but I plan to follow up with a Haskell version in the upcomming months. The project use the CQRS pattern and Grey Young event store for its persitence layer. 


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


* Install Grey Young's Event Store DB

    1- curl -s https://packagecloud.io/install/repositories/EventStore/EventStore-OSS/script.deb.sh | sudo bash
    2- sudo apt-get install eventstore-oss=5.0.2-1
    3- sudo systemctl start eventstore
    
* Clone the repo

    1- git clone https://github.com/diaspogift/identity_and_access_FSharp
    
* In your terminal, navigate to the identity_and_access_FSharp folder and run dotnet and run the following commands:
    
    * dotnet build
    * dotnet restore
    * dotnet run (This will start a local server on port 8080)
    
* Install postman or any other rest API client and try running these
    
    * Provision a Tenant
        * Url:      http://localhost:8080/tenant-provisions
        * Method:   post
        * Data:    {
                  "TenantInfo" : {"Name":"Clinic le Poitier", "Description":"Hopital de reference"},
                   "AdminUserInfo" : {"FirstName" : "Felicien", "MiddleName" :  "N/A", "LastName" :  "Fotio", "Email" :  "felicien@gmail.com", "Address" :  "Douala, Cameroun", "PrimPhone" :  "669262690" ,"SecondPhone" : "669262691"}
}

    * Deactivate Tenant
        * Url: http://localhost:8080/activate-tenant
        * Method: post
        * Data {
    
    "TenantId" : "5C570B52644AA37772E26671",
    "ActivationStatus" : true,
    "Reason" : "I do not like youuuuuuu"
}

    


End with an example of getting some data out of the system or using it for a little demo

## Running the tests


N/A


## Deployment

Add additional notes about how to deploy this on a live system

## Built With

* [F#](https://fsharp.org) - The language
* [Fake](https://fake.build/) - Dependency Management
* [Suave](https://suave.io/) - Used as API server
* [EventStore](https://eventstore.org) Used as the event store

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## Authors

* **Felicien FOTIO MANFO** - *Initial work* - [PurpleBooth](https://github.com/PurpleBooth)

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc
