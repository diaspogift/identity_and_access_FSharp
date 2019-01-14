module IdentityAndAcccess.DomainApiTypes.Handlers


open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation

open IdentityAndAccess.DatabaseFunctionsInterfaceTypes
open IdentityAndAcccess.DomainApiTypes.ProvisionTenantWorflowImplementation
open FSharp.Data.Sql
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


