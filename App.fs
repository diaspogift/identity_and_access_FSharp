﻿// Learn more about F# at http://fsharp.org




open IdentityAndAcccess.EventStore.Implementation
open IdentityAndAcccess.EventStorePlayGround.Implementation

open IdentityAndAcccess.Serialization

open Suave.Web
open Suave.Successful
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainServices
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.DomainServices.Tenant
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.DomainTypes

open IdentityAndAcccess.DomainApiTypes
open IdentityAndAcccess.DomainApiTypes.Handlers
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes.ProvisionTenantWorflowImplementation
open IdentityAndAcccess.Workflow.OffertRegistrationInvitationApiTypes
open IdentityAndAcccess.Workflow.OffertRegistrationInvitationApiTypes
open Suave.Sockets

open IdentityAndAcccess.DomainApiTypes.Handlers
open MongoDB.Driver
open MongoDB.Bson
open System


open IdentityAndAcces.Infrstructure.Queue.Impl

open IdentityAndAccess.RabbitMQ.FSharp.Client

open IdentityAndAcccess.EventStorePlayGround.Implementation.EventStorePlayGround
open EventStore.ClientAPI

open IdentityAndAcccess.DeactivateTenantActivationStatusApiTypes.DeactivateTenantActivationStatusWorflowImplementation
open IdentityAndAcccess.Workflow.DeactivateTenantActivationStatusApiTypes
open IdentityAndAcccess.Workflow.ReactivateTenantActivationStatusApiTypes
open IdentityAndAcccess.Workflow.ReactivateTenantActivationStatusApiTypes
















let unwrapToStandardGroup aGroupToAddToUnwrapp = 
        match aGroupToAddToUnwrapp with 
            | Standard aStandardGroup -> aStandardGroup
            | Internal anInternalGroup -> anInternalGroup


let printSeparatorLine(count) = 
        
        let countArray = List.init count (fun x -> x )
        countArray
        |> List.iter (fun x -> 
        
                        printfn ""
                        printfn ""
                        printfn "------------------------------------------------------------------------------------------------------------"
                        printfn ""
                        printfn ""
                     )



let printEmptySeparatorLine(count) = 
        
        let countArray = List.init count (fun x -> x)
        countArray
        |> List.iter (fun x -> 
        
                        printfn ""
                        printfn ""
                        printfn ""
                     )











(*

let unvalidatedTenant : UnvalidatedTenant = {Name = "Mobile Biller"; Description = "Mobile Biller offre les services mobile de transfert d'argent"}

let tenantAdministrator : TenantAdministrator = {
    FirstName = "Didier"
    MiddleName = "N/A"
    LastName = "Nkalla"
    Email = "didnkallaehawe@gmail.com"
    Address = "Douala, Cameroun"
    PrimPhone = "669262690" 
    SecondPhone = "669262691"
}
let unvalidatedTenantProvision : UnvalidatedTenantProvision = {TenantInfo = unvalidatedTenant; AdiminUserInfo = tenantAdministrator}

type Command<'data> = {
    Data : 'data;
    TimeStamp : DateTime;
    UserId : string;
}
let provisionTenantCommand : ProvisionTenantCommand = {
        
        Data = unvalidatedTenantProvision
        TimeStamp = DateTime.Now
        UserId = "Megan"

        } 




let rsProvisionTenantCommand = ProvisionTenant.handleProvisionTenant provisionTenantCommand



match  rsProvisionTenantCommand with  
| Ok rs -> 
        printEmptySeparatorLine(1)
        printfn " THE PROVISIONING RESULT"
        printEmptySeparatorLine(1)
        printfn " %A" rs
        printEmptySeparatorLine(1)

| Error error ->
        printfn " %A" error

 


let unvalidatedRegistrationInvitationDescription : UnvalidatedRegistrationInvitationDescription = {
        TenantId = "5c464a449c40a87f406780e8"; 
        Description = "Invitation for Megan"
        }


let offerRegistrationInvitationCommand : OfferRegistrationInvitationCommand = {
        Data = unvalidatedRegistrationInvitationDescription
        TimeStamp = DateTime.Now
        UserId = "Megan"
        } 


let rsOfferRegistrationInvitationCommand = OffertRegistrationInvitationCommand.handleOfferRegistrationInvitation offerRegistrationInvitationCommand


match  rsOfferRegistrationInvitationCommand with  
| Ok rs -> 
        printEmptySeparatorLine(1)
        printfn " THE INVITATION RESULT"
        printEmptySeparatorLine(1)
        printfn " %A" rs
        printEmptySeparatorLine(1)

| Error error ->
        printfn " %A" error 

*)


let unvalidatedDeactivateTenant : UnvalidatedTenantActivationStatus = {
        TenantId = "5c464a449c40a87f406780e8"; 
        ActivationStatus = true
        Reason = "Fuck that tenantt ..."
        }


let deactivateTenantActivationStatusCommand : DeactivateTenantActivationStatusCommand = {
        
        Data = unvalidatedDeactivateTenant
        TimeStamp = DateTime.Now
        UserId = "Megan"

        } 




let rsDeactivateTenantActivationStatusCommand = DeactivateTenantActivationStatus.handleDeactivateTenantActivationStatus deactivateTenantActivationStatusCommand


match  rsDeactivateTenantActivationStatusCommand with  
| Ok rs -> 
        printEmptySeparatorLine(1)
        printfn " THE DEACTIVATION RESULT"
        printEmptySeparatorLine(1)
        printfn " %A" rs
        printEmptySeparatorLine(1)

| Error error ->
        printfn " %A" error




[<EntryPoint>]
let main argv =

   //startWebServer defaultConfig (OK "hello")
   0




