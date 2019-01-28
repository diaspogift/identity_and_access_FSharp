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

open IdentityAndAcccess.Commamds
open IdentityAndAcccess.Commamds.Handlers
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes.ProvisionTenantWorflowImplementation
open IdentityAndAcccess.Workflow.OffertRegistrationInvitationApiTypes
open IdentityAndAcccess.Workflow.WithdrawRegistrationInvitationApiTypes
open IdentityAndAcccess.WithdrawRegistrationInvitationApiTypes.WithdrawRegistrationInvitationWorflowImplementation
open Suave.Sockets

open IdentityAndAcccess.Commamds.Handlers
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
open IdentityAndAcccess.DomainTypes.Functions.Dto
open System.Collections
open IdentityAndAcces.Infrstructure.Queue.Impl.Queue
open IdentityAndAcces.Infrstructure.Queue.Impl
open IdentityAndAccess.RabbitMQ.FSharp.Client

open IdentityAndAcccess.Commamds.Handlers.Command
open IdentityAndAcccess.ReastApi











let unwrapToStandardGroup aGroupToAddToUnwrapp = 
    match aGroupToAddToUnwrapp with 
    | Standard aStandardGroup -> aStandardGroup
    | Internal anInternalGroup -> anInternalGroup

let printSeparatorLine(count) = 
        
        let countArray = List.init count (fun x -> x )
        countArray
        |> List.iter (fun x -> 
        
                        printfn ""
                        printfn "------------------------------------------------------------------------------------------------------------"
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
let mutable diaspoGiftGroupIdForDeveloperGroup = ""
let mutable diaspoGiftGroupIdForJuniorDeveloperGroup = ""
let mutable diaspoGiftGroupIdForMidDeveloperGroup = ""
let mutable diaspoGiftGroupIdForSeniorDeveloperGroup = ""
let mutable diaspoGiftRoleIdForDeveloper = ""












/// PROVISION THE DIASPO-GIFT TENANT
/// 
/// 
 


let provisionTenantCommand : ProvisionTenantCommand = {
        Data = {
            TenantInfo = {
                Name = "DIASPO-GIFT"
                Description = "Diaspora Gift Platforme pour le payement et le suivi des services"
                }
            AdminUserInfo = {
                FirstName = "Felicien"
                MiddleName = "N/A"
                LastName = "Fotio"
                Email = "felicien@gmail.com"
                Address = "Douala, Cameroun"
                PrimPhone = "669262690" 
                SecondPhone = "669262691"
                }
        }
        TimeStamp = DateTime.Now
        UserId = "Felicien"
        } 

match  provisionTenantCommand |> ProvisionTenant.handle  with  
| Ok rs -> 

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

    | _    -> printfn " Not interrested"                                      
| Error error -> printfn " %A" error

 

printEmptySeparatorLine(1)



// Provision DEVELOPER Group
let developerGroup : ProvisionGroupCommand = {
        Data = {
                TenantId = diaspoGiftTenantId;
                Name = "DEVELOPER"
                Description = "Groupe comprenant tous les devellopeurs (junior, mid, senior) de la boite "    
                Members = [||]
        }
        TimeStamp = DateTime.Now
        UserId = diaspoGiftAdminUserId
        } 
let resultPrvisionDeveloperGroupCommand = developerGroup |> ProvisionGroup.handle 
match  resultPrvisionDeveloperGroupCommand with  
| Ok rs -> 
    printfn " THE PROVISIONED DEVELOPER GROUP RESULT"
    printEmptySeparatorLine(2)
    printfn " %A" rs
    printEmptySeparatorLine(2)
    diaspoGiftGroupIdForDeveloperGroup <- rs.GroupId
    printfn " diaspoGiftGroupIdForDeveloperGroup %A"   diaspoGiftGroupIdForDeveloperGroup   
| Error error -> printfn " %A" error 



printEmptySeparatorLine(1)



// JUNIOR_DEVELOPER Group
let provisionJuniorDeveloperGroupCommand : ProvisionGroupCommand = {
        Data = {
                TenantId = diaspoGiftTenantId;
                Name = "JUNIOR_DEVELOPER"
                Description = "Groupe comprenant tous les devellopeurs junior de la boite"    
                Members = [||]
        }
        TimeStamp = DateTime.Now
        UserId = diaspoGiftAdminUserId
        } 

match  ProvisionGroup.handle provisionJuniorDeveloperGroupCommand with  
| Ok rs -> 
    printEmptySeparatorLine(1)
    printfn " THE PROVISIONED JUNIOR DEVELOPER GROUP RESULT"
    printEmptySeparatorLine(1)
    printfn " %A" rs
    printEmptySeparatorLine(2)
        //let ugs = unwrapGroup rs.Group
    diaspoGiftGroupIdForJuniorDeveloperGroup <- rs.GroupId
    printfn " diaspoGiftGroupIdForJuniorDeveloperGroup %A"   diaspoGiftGroupIdForJuniorDeveloperGroup   
| Error error -> printfn " %A" error 



printEmptySeparatorLine(1)



// PROVISION MID DEVELOPER Group
let midDeveloperGroup : ProvisionGroupCommand = {
        Data = {
                TenantId = diaspoGiftTenantId;
                Name = "MID_DEVELOPER"
                Description = "Groupe comprenant tous les devellopeurs mid de la boite "    
                Members = [||]
        }
        TimeStamp = DateTime.Now
        UserId = diaspoGiftAdminUserId
        } 
match   midDeveloperGroup |> ProvisionGroup.handle with  
| Ok rs -> 
    printfn " THE PROVISIONED MID GROUP RESULT"
    printEmptySeparatorLine(2)
    printfn " %A" rs
    printEmptySeparatorLine(2)
    diaspoGiftGroupIdForMidDeveloperGroup <- rs.GroupId
    printfn " diaspoGiftGroupIdForMidDeveloperGroup %A"   diaspoGiftGroupIdForMidDeveloperGroup    
| Error error -> printfn " %A" error 



printEmptySeparatorLine(1)



// SENIOR DEVELOPER Group
let seniorDeveloperGroup : ProvisionGroupCommand = 
    {
    Data = {
        TenantId = diaspoGiftTenantId;
        Name = "SENIOR_DEVELOPER"
        Description = "Groupe comprenant tous les devellopeurs senior de la boite "    
        Members = [||]
        }
    TimeStamp = DateTime.Now
    UserId = diaspoGiftAdminUserId
    } 
match  seniorDeveloperGroup |> ProvisionGroup.handle  with  
| Ok rs -> 
    printfn " THE PROVISIONED SENIOR GROUP RESULT"
    printEmptySeparatorLine(2)
    printfn " %A" rs
    printEmptySeparatorLine(2)
    diaspoGiftGroupIdForSeniorDeveloperGroup <- rs.GroupId
    printfn " diaspoGiftGroupIdForSeniorDeveloperGroup %A"   diaspoGiftGroupIdForSeniorDeveloperGroup      
| Error error -> printfn " %A" error 



printEmptySeparatorLine(2)


///ADD JUNIOR_DEVELOPER GROUP TO THE DEVELOPER GROUP
match  {
        Data = { GroupIdToAddTo = diaspoGiftGroupIdForDeveloperGroup; GroupIdToAdd = diaspoGiftGroupIdForJuniorDeveloperGroup }
        TimeStamp = DateTime.Now
        UserId = diaspoGiftAdminUserId
        } |> AddGroupToGroup.handle  with  
| Ok rs -> 
    printEmptySeparatorLine(2)
    printfn " JUNIOR DEVELOPER GROUP ADDED TO ANY DEVELOPER GROUP"
    printfn " %A" rs
    printEmptySeparatorLine(2)
| Error error -> printfn " %A" error 


printEmptySeparatorLine(2)



///ADD MID_DEVELOPER GROUP TO THE DEVELOPER GROUP

match  {   
        Data = { GroupIdToAddTo = diaspoGiftGroupIdForDeveloperGroup; GroupIdToAdd = diaspoGiftGroupIdForMidDeveloperGroup }
        TimeStamp = DateTime.Now
        UserId = diaspoGiftAdminUserId
        } |> AddGroupToGroup.handle  with  
| Ok rs -> 
    printEmptySeparatorLine(2)
    printfn " JUNIOR DEVELOPER GROUP ADDED TO ANY DEVELOPER GROUP"
    printfn " %A" rs
    printEmptySeparatorLine(2)
| Error error -> printfn " %A" error 



printEmptySeparatorLine(1)




///ADD SENIOR_DEVELOPER GROUP TO THE DEVELOPER GROUP

match  {   
        Data = {  GroupIdToAddTo = diaspoGiftGroupIdForDeveloperGroup; GroupIdToAdd = diaspoGiftGroupIdForSeniorDeveloperGroup }
        TimeStamp = DateTime.Now
        UserId = diaspoGiftAdminUserId
        } 
    |> AddGroupToGroup.handle  with  
| Ok rs -> 
    printEmptySeparatorLine(2)
    printfn " SENIOR DEVELOPER GROUP ADDED TO  DEVELOPER GROUP"
    printfn " %A" rs
    printEmptySeparatorLine(2)
| Error error -> printfn " %A" error 



printEmptySeparatorLine(1)





[<EntryPoint>]
let main argv =

   //Queue.loop 0
   startWebServer defaultConfig Rest.app
   0































































// Queue.subscribe<string> Queue.Queue.GroupCreated (
//                         fun str -> 
//                                 printfn "-----------IN subscribe ------------"
//                                 printfn "-----------------------"
//                                 printfn "-----------------------"
//                                 printfn " = ----------- %A ------------= " str
//                                 printfn "-----------------------"
//                                 printfn "-----------------------"
//                                 printfn "-----------IN subscribe ------------"
//                         )()


// Queue.enqueue Queue.Queue.GroupCreated "diaspoGiftGroupIdForJuniorDeveloperGroup"



