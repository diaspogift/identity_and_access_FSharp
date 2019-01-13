module IdentityAndAcccess.DomainApiTypes.Handlers


open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation

open IdentityAndAccess.DatabaseFunctionsInterfaceTypes
open IdentityAndAcccess.DomainApiTypes.ProvisionTenantWorflowImplementation










module ProvisionTenant = 




    let saveOneTenant = TenantDb.saveOneTenant 
    let saveOneUser = UserDb.saveOneUser
    let saveOneRole = RoleDb.saveOneRole 




    let handleProvisionTenant saveOneTenant saveOneUser saveOneRole (aProvisionTenantCommand:ProvisionTenantCommand) = 

        //IO ad the edge

        let aProvisionTenantCommandData = aProvisionTenantCommand.Data

        let rsProvision = aProvisionTenantCommandData |> provisionTenantWorflow 


  







        //IO ad the edge

        saveOneTenant

        saveOneUser

        saveOneRole
