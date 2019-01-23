// Learn more about F# at http://fsharp.org




open IdentityAndAcccess.EventStore.Implementation
open IdentityAndAcccess.EventStorePlayGround.Implementation

open IdentityAndAcccess.Serialization

open Suave.Web
open Suave.Successful
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainServicesImplementations
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.DomainServicesImplementations.Tenant
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.DomainTypes

open IdentityAndAcccess.DomainApiTypes
open IdentityAndAcccess.DomainApiTypes.Handlers
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes.ProvisionTenantWorflowImplementation
open IdentityAndAcccess.Workflow.OffertRegistrationInvitationApiTypes
open IdentityAndAcccess.Workflow.WithdrawRegistrationInvitationApiTypes
open IdentityAndAcccess.WithdrawRegistrationInvitationApiTypes.WithdrawRegistrationInvitationWorflowImplementation
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

open IdentityAndAcccess.Workflow.ProvisionGroupApiTypes
open IdentityAndAcccess.Workflow.ProvisionRoleApiTypes

open IdentityAndAcccess.Workflow.AddUserToGroupApiTypes
open IdentityAndAcccess.Workflow.AddUserToGroupApiTypes.AddUserToGroupWorfklowImplementation
open IdentityAndAcccess.Workflow.AddGroupToGroupApiTypes














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








let mutable diaspoGiftTenantId = ""
let mutable diaspoGiftAdminUserId = ""
let mutable diaspoGiftAdminRoleId = ""
let mutable diaspoGiftGroupIdForAnyDeveloperGroup = ""
let mutable diaspoGiftGroupIdForJuniorDeveloperGroup = ""
let mutable diaspoGiftGroupIdForMidDeveloperGroup = ""
let mutable diaspoGiftGroupIdForSeniorDeveloperGroup = ""
let mutable diaspoGiftRoleIdForDeveloper = ""












/// PROVISION THE DIASPO-GIFT TENANT
/// 
/// 
 

let unvalidatedDiaspoGiftTenant : UnvalidatedTenant = {
        Name = "DIASPO-GIFT"
        Description = "Diaspora Gift Platforme pour le payement et le suivi des services"
        }

let diaspoGiftTenantAdministrator : TenantAdministrator = {
    FirstName = "Felicien"
    MiddleName = "N/A"
    LastName = "Fotio"
    Email = "felicien@gmail.com"
    Address = "Douala, Cameroun"
    PrimPhone = "669262690" 
    SecondPhone = "669262691"
    }

let unvalidatedDiaspoGiftTenantProvision : UnvalidatedTenantProvision = {
        TenantInfo = unvalidatedDiaspoGiftTenant
        AdiminUserInfo = diaspoGiftTenantAdministrator
        }

type Command<'data> = {
    Data : 'data;
    TimeStamp : DateTime;
    UserId : string;
}
let provisionTenantCommand : ProvisionTenantCommand = {
        
        Data = unvalidatedDiaspoGiftTenantProvision
        TimeStamp = DateTime.Now
        UserId = "Felicien"

        } 


match  provisionTenantCommand |> ProvisionTenant.handleProvisionTenant  with  
| Ok rs -> 
        printEmptySeparatorLine(1)
        printfn " THE PROVISIONING RESULT"
        printEmptySeparatorLine(1)
        printfn " %A" rs
        printEmptySeparatorLine(1)


        match rs.Head with 
        | TenantProvisionCreated t ->

                let tenant = t.TenantProvisioned
                let role = t.RoleProvisioned
                let user = t.UserRegistered

                diaspoGiftTenantId <- tenant.TenantId |> TenantId.value
                diaspoGiftAdminUserId <- user.UserId |> UserId.value
                diaspoGiftAdminRoleId <- role.RoleId |> RoleId.value

                printfn " diaspoGiftTenantId %A"   diaspoGiftTenantId   
                printfn " diaspoGiftAdminUserId %A"   diaspoGiftAdminUserId   
                printfn " diaspoGiftAdminRoleId %A"   diaspoGiftAdminRoleId   


           

        | _    ->
               printfn " Not interrested"     
                                      
| Error error ->
        printfn " %A" error

 






printEmptySeparatorLine(5)








/// DEACTIVATE TENANT ACTIVATION STATUS

let unvalidatedDeactivateDiaspoGiftTenant : UnvalidatedTenantActivationStatus = {
        TenantId = diaspoGiftTenantId
        ActivationStatus = true
        Reason = "Fuck that tenantt ..."
        }


let deactivateTenantActivationStatusCommand : DeactivateTenantActivationStatusCommand = {
        
        Data = unvalidatedDeactivateDiaspoGiftTenant
        TimeStamp = DateTime.Now
        UserId = "Megan"

        } 
match  deactivateTenantActivationStatusCommand |> DeactivateTenantActivationStatus.handleDeactivateTenantActivationStatus  with  
| Ok rs -> 
        printEmptySeparatorLine(1)
        printfn " THE DEACTIVATION RESULT"
        printEmptySeparatorLine(1)
        printfn " %A" rs
        printEmptySeparatorLine(1)

| Error error ->
        printfn " %A" error




printEmptySeparatorLine(5)







/// REACTIVATE TENANT ACTIVATION STATUS

let unvalidatedReactivateDiaspoGiftTenant : UnvalidatedTenantActivationStatusData = {
        TenantId = diaspoGiftTenantId
        ActivationStatus = false
        Reason = "Love that tenantt ..."
        }
let reactivateTenantActivationStatusCommand : ReactivateTenantActivationStatusCommand = { 
        Data = unvalidatedReactivateDiaspoGiftTenant
        TimeStamp = DateTime.Now
        UserId = "Megan"
        } 
match  reactivateTenantActivationStatusCommand |> ReactivateTenantActivationStatus.handleReactivateTenantActivationStatus  with  
| Ok rs -> 
        printEmptySeparatorLine(1)
        printfn " THE DEACTIVATION RESULT"
        printEmptySeparatorLine(1)
        printfn " %A" rs
        printEmptySeparatorLine(1)

| Error error ->
        printfn " %A" error



printEmptySeparatorLine(5)







 
/// PROVOSION (ANY_DEVELOPER, JUNIOR_DEVELOPER, MID_DEVELOPER, SENIOR_DEVELOPER) GROUPS FOR DIASPO GIFT TENANT 
/// 
/// 
 



// ANY_DEVELOPER Group
let unvalidatedAnyDeveloper:UnvalidatedGroup = {
        TenantId = diaspoGiftTenantId;
        Name = "ANY_DEVELOPER"
        Description = "Groupe comprenant tous les devellopeurs (junior, mid, senior) de la boite "    
        Members = [||]   
} 


let provisionAnyDeveloperGroupCommand : ProvisionGroupCommand = {
        Data = unvalidatedAnyDeveloper
        TimeStamp = DateTime.Now
        UserId = diaspoGiftAdminUserId
        } 


match  ProvisionGroupCommand.handleProvisionGroup provisionAnyDeveloperGroupCommand with  
| Ok rs -> 
        printEmptySeparatorLine(1)
        printfn " THE PROVISIONED ANY DEVELOPER GROUP RESULT"
        printEmptySeparatorLine(1)
        printfn " %A" rs
        printEmptySeparatorLine(1)


        diaspoGiftGroupIdForAnyDeveloperGroup <- rs.Group.GroupId

        printfn " diaspoGiftGroupIdForAnyDeveloperGroup %A"   diaspoGiftGroupIdForAnyDeveloperGroup   



| Error error ->
        printfn " %A" error 





printEmptySeparatorLine(5)



// JUNIOR_DEVELOPER Group
let unvalidatedJuniorDeveloper:UnvalidatedGroup = {
        TenantId = diaspoGiftTenantId;
        Name = "JUNIOR_DEVELOPER"
        Description = "Groupe comprenant tous les devellopeurs junior de la boite"    
        Members = [||]   
} 


let provisionJuniorDeveloperGroupCommand : ProvisionGroupCommand = {
        Data = unvalidatedJuniorDeveloper
        TimeStamp = DateTime.Now
        UserId = diaspoGiftAdminUserId
        } 


match  ProvisionGroupCommand.handleProvisionGroup provisionJuniorDeveloperGroupCommand with  
| Ok rs -> 
        printEmptySeparatorLine(1)
        printfn " THE PROVISIONED JUNIOR DEVELOPER GROUP RESULT"
        printEmptySeparatorLine(1)
        printfn " %A" rs
        printEmptySeparatorLine(1)

        diaspoGiftGroupIdForJuniorDeveloperGroup <- rs.Group.GroupId

        printfn " diaspoGiftGroupIdForJuniorDeveloperGroup %A"   diaspoGiftGroupIdForJuniorDeveloperGroup   


| Error error ->
        printfn " %A" error 




printEmptySeparatorLine(5)



// MID_DEVELOPER Group
let unvalidatedMidDeveloper:UnvalidatedGroup = {
        TenantId = diaspoGiftTenantId;
        Name = "MID_DEVELOPER"
        Description = "Groupe comprenant tous les devellopeurs au niveua intermediaire de la boite"    
        Members = [||]   
} 

let provisionMidDeveloperGroupCommand : ProvisionGroupCommand = {
        Data = unvalidatedMidDeveloper
        TimeStamp = DateTime.Now
        UserId = diaspoGiftAdminUserId
        } 


match  ProvisionGroupCommand.handleProvisionGroup provisionMidDeveloperGroupCommand with  
| Ok rs -> 
        printEmptySeparatorLine(1)
        printfn " THE PROVISIONED MID DEVELOPER GROUP RESULT"
        printEmptySeparatorLine(1)
        printfn " %A" rs
        printEmptySeparatorLine(1)


        diaspoGiftGroupIdForMidDeveloperGroup <- rs.Group.GroupId


        printfn " diaspoGiftGroupIdForJuniorDeveloperGroup %A"   diaspoGiftGroupIdForMidDeveloperGroup   



| Error error ->
        printfn " %A" error 





printEmptySeparatorLine(5)



// SENIOR_DEVELOPER Group
let unvalidatedSeniorDeveloper:UnvalidatedGroup = {
        TenantId = diaspoGiftTenantId;
        Name = "SENIOR_DEVELOPER"
        Description = "Groupe comprenant tous les devellopeurs au niveau experimente de la boite"    
        Members = [||]   
} 

let provisionSeniorDeveloperGroupCommand : ProvisionGroupCommand = {
        Data = unvalidatedSeniorDeveloper
        TimeStamp = DateTime.Now
        UserId = diaspoGiftAdminUserId
        } 


match  ProvisionGroupCommand.handleProvisionGroup provisionSeniorDeveloperGroupCommand with  
| Ok rs -> 
        printEmptySeparatorLine(1)
        printfn " THE PROVISIONED SENIOR DEVELOPER GROUP RESULT"
        printEmptySeparatorLine(1)
        printfn " %A" rs
        printEmptySeparatorLine(1)

        diaspoGiftGroupIdForSeniorDeveloperGroup <- rs.Group.GroupId

        printfn " diaspoGiftGroupIdForSeniorDeveloperGroup %A"   diaspoGiftGroupIdForSeniorDeveloperGroup   

| Error error ->
        printfn " %A" error 










printEmptySeparatorLine(5)







/// ADD JUNIOR_DEVELOPER GROUP TO ANY_DEVELOPER GROUP
let unvalidatedGroupIds:UnvalidatedGroupIds = {
        GroupIdToAddTo = diaspoGiftGroupIdForAnyDeveloperGroup
        GroupIdToAdd = diaspoGiftGroupIdForJuniorDeveloperGroup
        } 
let addJuniorDeveloperGroupToAnyDeveloperGroupCommand : AddGroupToGroupCommand = {
        Data = unvalidatedGroupIds
        TimeStamp = DateTime.Now
        UserId = diaspoGiftAdminUserId
        } 

match  AddGroupToGroupCommand.handleAddGroupToGroup addJuniorDeveloperGroupToAnyDeveloperGroupCommand with  
| Ok rs -> 
        printEmptySeparatorLine(1)
        printfn " JUNIOR DEVELOPER GROUP ADDED TO ANY DEVELOPER GROUP RESULT"
        printEmptySeparatorLine(1)
        printfn " %A" rs
        printEmptySeparatorLine(1)

| Error error ->
        printfn " %A" error 







printEmptySeparatorLine(5)







/// ADD MID_DEVELOPER GROUP TO ANY_DEVELOPER GROUP
let unvalidatedGroupIds1:UnvalidatedGroupIds = {
        GroupIdToAddTo = diaspoGiftGroupIdForAnyDeveloperGroup
        GroupIdToAdd = diaspoGiftGroupIdForMidDeveloperGroup
        } 
let addMidDeveloperGroupToAnyDeveloperGroupCommand : AddGroupToGroupCommand = {
        Data = unvalidatedGroupIds1
        TimeStamp = DateTime.Now
        UserId = diaspoGiftAdminUserId
        } 

match  AddGroupToGroupCommand.handleAddGroupToGroup addMidDeveloperGroupToAnyDeveloperGroupCommand with  
| Ok rs -> 
        printEmptySeparatorLine(1)
        printfn " MID DEVELOPER GROUP ADDED TO ANY DEVELOPER GROUP RESULT"
        printEmptySeparatorLine(1)
        printfn " %A" rs
        printEmptySeparatorLine(1)

| Error error ->
        printfn " %A" error 










/// ADD MID_DEVELOPER GROUP TO ANY_DEVELOPER GROUP
let unvalidatedGroupIds2:UnvalidatedGroupIds = {
        GroupIdToAddTo = diaspoGiftGroupIdForAnyDeveloperGroup
        GroupIdToAdd = diaspoGiftGroupIdForSeniorDeveloperGroup
        } 
let addSeniorDeveloperGroupToAnyDeveloperGroupCommand : AddGroupToGroupCommand = {
        Data = unvalidatedGroupIds2
        TimeStamp = DateTime.Now
        UserId = diaspoGiftAdminUserId
        } 

match  AddGroupToGroupCommand.handleAddGroupToGroup addMidDeveloperGroupToAnyDeveloperGroupCommand with  
| Ok rs -> 
        printEmptySeparatorLine(1)
        printfn " SENIOR DEVELOPER GROUP ADDED TO ANY DEVELOPER GROUP RESULT"
        printEmptySeparatorLine(1)
        printfn " %A" rs
        printEmptySeparatorLine(1)

| Error error ->
        printfn " %A" error 




































/// PROVISON DEVELOPER ROLE FOR DIASPO-GIFT TENANT
 
let unvalidatedDeveloperRole:UnvalidatedRole = {
        TenantId = diaspoGiftTenantId
        Name = "DEVELOPER"
        Description = "Ce role est joue par tous les groupes de developeurs (junior, mid, et senior)"       
} 

let provisionDeveloperRoleCommand : ProvisionRoleCommand = {
        Data = unvalidatedDeveloperRole
        TimeStamp = DateTime.Now
        UserId = "Megan"
        } 

match  ProvisionRoleCommand.handleProvisionRole provisionDeveloperRoleCommand with  
| Ok rs -> 
        printEmptySeparatorLine(1)
        printfn " THE PROVISIONED DEVELOPER ROLE RESULT"
        printEmptySeparatorLine(1)
        printfn " %A" rs
        printEmptySeparatorLine(1)

| Error error ->
        printfn " %A" error 
 











printEmptySeparatorLine(5)














/// OFFER REGISTRATION INVITATION 
let unvalidatedDescription : UnvalidatedRegistrationInvitationDescription= {
        Description = "For Diane"
        TenantId = diaspoGiftTenantId; 
        }


let offerRegistrationInvitationCommand : OfferRegistrationInvitationCommand = {
        Data = unvalidatedDescription
        TimeStamp = DateTime.Now
        UserId = "Megan"
        } 


let rsOfferRegistrationInvitationCommand = OfferRegistrationInvitationCommand.handleOfferRegistrationInvitation offerRegistrationInvitationCommand


match  rsOfferRegistrationInvitationCommand with  
| Ok rs -> 
        printEmptySeparatorLine(1)
        printfn " THE INVITATION  RESULT"
        printEmptySeparatorLine(1)
        printfn " %A" rs
        printEmptySeparatorLine(1)

| Error error ->
        printfn " %A" error 





 

 



/// ADD USER TO GROUP



 









[<EntryPoint>]
let main argv =

   //startWebServer defaultConfig (OK "hello")
   0




