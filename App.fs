// Learn more about F# at http://fsharp.org




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

let tt = ObjectId.GenerateNewId()
let aa = ObjectId.GenerateNewId()
let bb = ObjectId.GenerateNewId()


printfn "tt ==== %A" tt
printfn "aa ==== %A" aa
printfn "bb ==== %A" bb
















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













(*let unvalidatedTenant : UnvalidatedTenant = {Name = "Le Quattro"; Description = "Restauration - Mets locaux et traditionnels"}

let tenantAdministrator : TenantAdministrator = {
    FirstName = "Megan"
    MiddleName = "Amanda"
    LastName = "Hess"
    Email = "mah90@gmail.com"
    Address = "973 Ranch House Road Thousand Oaks CA, 91786"
    PrimPhone = "669262658" 
    SecondPhone = "669262659"
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

 
*)

(* let unvalidatedRegistrationInvitationDescription : UnvalidatedRegistrationInvitationDescription = {
        TenantId = "5c4353b53766624bce89cf91"; 
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
        printfn " %A" error *)




let unvalidatedDeactivateTenant : UnvalidatedTenantActivationStatusData = {
        TenantId = "5c4353b53766624bce89cf91"; 
        ActivationStatus = false
        Reason = "Fuck that userrrr ..."
        }


let reactivateTenantActivationStatusCommand : ReactivateTenantActivationStatusCommand = {
        
        Data = unvalidatedDeactivateTenant
        TimeStamp = DateTime.Now
        UserId = "Megan"

        } 




let rsDeactivateTenantActivationStatusCommand = ReactivateTenantActivationStatus.handleReactivateTenantActivationStatus reactivateTenantActivationStatusCommand


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




