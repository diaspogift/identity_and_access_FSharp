# Context

When I came accross DDD a couple of years ago, first I thought it was the biggest revelation in my carreer. I then discover functional programing and it almost stop me brething as I could not stop wondering why the OOP is so mainstrain. It took several months year to transition from the OOP to FFP. One of my concrete realisation is converting the DDD_Sample from Von Vernon into a Functional F# Project. It's in this context that I set out to write this piece of software. 


# Indentities and Accesses Management System

Identity and access management system is a back end service that allows allow to:
* Register tenants with their defaults adminitrators users
* Invite and register users under a given tenancy
* Group them base on certain affinities
* Assign/Unassign them to/from roles
* Handle their accesses statuses 

This my first attempt to translate the original IAM DDD_Sample developed by Von Vernon in his book : Implementing Domain Driven Design into a serie of functional languages. I picked F# because it was the most accessible but I plan to follow up with a Haskell version in the upcomming months. 


## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

What things you need to install the software and how to install them
The following pieces of software need to be install and configured in order to deply/test the project. 

* mono version >= 5.16.0
* dotnet
* Grey young Event Sourcing
* Postman 

```
Give examples
```

### Installing

A step by step series of examples that tell you how to get a development env running

Say what the step will be
* Install Grey You Event Sourcing DB
* Clone the repo
* navigate to src and run dotnet and run the command: dotnet run
* Install postman or any other rest API client


End with an example of getting some data out of the system or using it for a little demo

## Running the tests


N/A


## Deployment

Add additional notes about how to deploy this on a live system

## Built With

* [F#](https://fsharp.org) - The language
* [Fake](https://fake.build/) - Dependency Management
* [Suave](https://suave.io/) - Used as API server

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
