module IdentityAndAcccess.DomainApiTypes.Handlers


open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes.ProvisionTenantWorflowImplementation
open IdentityAndAcccess.OffertRegistrationInvitationApiTypes.OffertRegistrationInvitationWorflowImplementation
open FSharp.Data.Sql
open IdentityAndAcccess.Workflow.OffertRegistrationInvitationApiTypes
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open Suave.Sockets











module ProvisionTenant = 




    let saveOneTenant = TenantDb.saveOneTenant 
    let saveOneUser = UserDb.saveOneUser
    let saveOneRole = RoleDb.saveOneRole 




    let handleProvisionTenant 
                            (saveOneTenant:TenantDb.SaveOneTenant) (saveOneUser: UserDb.SaveOneUser) (saveOneRole:RoleDb.SaveOneRole)
                            (aProvisionTenantCommand:ProvisionTenantCommand) = 

        //Extract the command data




        let aProvisionTenantCommandData = aProvisionTenantCommand.Data





        //Call into pure business logic



        let ouput = 
            aProvisionTenantCommandData 
            |> provisionTenantWorflow 



        //IO ad the edge base on the result / decision from the actual worflow



        match ouput with 
        | Ok tenantProvisionedEventList ->

            let firstEvent : TenantProvisionedEvent  = 
                tenantProvisionedEventList 
                |> List.head

            match firstEvent with
            | TenantProvisionCreated aTenantProvisionCreated->  

                let tenantToSave = aTenantProvisionCreated.TenantProvisioned
                let roleToSave = aTenantProvisionCreated.RoleProvisioned
                let userToSave = aTenantProvisionCreated.UserCreated


                printfn "-----------------------------------------------------------------------------------------"
                printfn " tenantToSave === %A"  tenantToSave
                printfn "-----------------------------------------------------------------------------------------"

                 

                let rst = 
                    tenantToSave 
                    |> saveOneTenant

                match rst  with  
                | Ok () ->

                    let rsu = 
                        userToSave 
                        |> saveOneUser


                    match rsu  with  
                    | Ok () ->

                        let rsr = 
                            roleToSave
                            |> saveOneRole 

                        match rsr  with  
                        | Ok () -> 
                            Ok tenantProvisionedEventList
                        | Error error ->
                            Error (ProvisionTenantError.DbError error)

                    | Error error ->
                        Error (ProvisionTenantError.DbError error)        
                    
                | Error error ->
                    Error (ProvisionTenantError.DbError error)  


            | ProvisionAcknowledgementSent aProvisionAcknowledgementSent ->

                Ok tenantProvisionedEventList


        | Error error ->

            Error error


    let handleProvisionTenant' = handleProvisionTenant saveOneTenant saveOneUser saveOneRole




module OffertRegistrationInvitationCommand = 


    let loadTenantById = TenantDb.loadOneTenantById 
    let saveOneTenant = TenantDb.saveOneTenant




    let handleOfferRegistrationInvitation 
                            (loadTenantById: TenantDb.LoadOneTenantById) 
                            (updateTenant: TenantDb.UpdateOneTenant) 
                            (aOfferRegistrationInvitationCommand:OfferRegistrationInvitationCommand)
                            :Result<RegistrationInvitationOfferredEvent, OfferRegistrationInvitationError> = 

        let aOfferRegistrationInvitationCommandData = aOfferRegistrationInvitationCommand.Data

        //IO at the edges


        let rsOfferInvitation = result {

            let unvalidatedRegistrationInvitationDescription:UnvalidatedRegistrationInvitationDescription = {
                TenantId = aOfferRegistrationInvitationCommandData.TenantId
                Description =  aOfferRegistrationInvitationCommandData.Description 
                }

            
            let! tenantId = aOfferRegistrationInvitationCommandData.TenantId |> TenantId.create'
            let! foundTenant = loadTenantById tenantId
      
            printfn "-----------------------------------------------------------------"
            printfn "-----------------------------------------------------------------"

            printfn "LOADED TENANT %A " foundTenant

            printfn "-----------------------------------------------------------------"
            printfn "-----------------------------------------------------------------"

            let rs = offerRegistrationInvitationWorkflow foundTenant unvalidatedRegistrationInvitationDescription


            return rs
        }

        match rsOfferInvitation with  
        | Ok rs ->

            match rs with  
            | Ok r ->


                let rsUpdateTenant = updateTenant r.Tenant


                match rsUpdateTenant with 
                | Ok () ->
                    Ok r
                | Error error ->
                    Error (OfferRegistrationInvitationError.DbError error)
                  

            | Error error ->
            Error error

        | Error error ->
            Error (OfferRegistrationInvitationError.DbError error)


    let handleOfferRegistrationInvitation' = handleOfferRegistrationInvitation loadOneTenantById updateTenant

