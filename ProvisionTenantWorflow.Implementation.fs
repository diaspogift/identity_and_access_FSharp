module IdentityAndAcccess.Workflow.ProvisionTenantApiTypes.ProvisionTenantWorflowImplementation

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainServicesImplementations.Tenant
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes
open IdentityAndAcccess.DomainTypes.Functions





///Dependencies 
/// 
/// 

let saveOneTenant : SaveOneTenant = TenantDb.saveOneTenant
let saveOneRole : SaveOneRole = RoleDb.saveOneRole
let saveOneUser : SaveOneUser = UserDb.saveOneUser














//validation step 

type ValidatedTenantProvision = {
    TenanName : TenantName
    TenantDescription : TenantDescription
    TenantAdministratorFirsTName : FirstName
    AdministratorMiddleName : MiddleName
    AdministratorLastName : LastName
    AdministratorEmail : EmailAddress
    AdministratorAddress : PostalAddress
    AdministratorPrimTel : Telephone
    AdministratorSecondTel : Telephone
    
}

type ValidateTenantProvision =

    UnvalidatedTenantProvision -> Result<ValidatedTenantProvision, ProvisionTenantError>







//Provision tenant step

type ProvisionTenant = 

    ValidatedTenantProvision -> Result<Provision, ProvisionTenantError>



//Create events step


type CreateEvents = Provision -> TenantProvisionedEvent list


//persistence "ad the edge steps"

type Provision = (Tenant*User*Role)


type SaveTenanProvision = Provision -> unit




///Step 1 validate tenant provision impl
let validateProvision : ValidateTenantProvision =

    fun aUnvalidatedTenantProvision ->

        result {

            let! name = 

                aUnvalidatedTenantProvision.TenantInfo.Name 
                |> TenantName.create' 
                |> Result.mapError ProvisionTenantError.ValidationError



            let! description = 

                aUnvalidatedTenantProvision.TenantInfo.Description 
                |> TenantDescription.create' 
                |> Result.mapError ProvisionTenantError.ValidationError



            let! first = 

                aUnvalidatedTenantProvision.AdiminUserInfo.FirstName 
                |> FirstName.create' 
                |> Result.mapError ProvisionTenantError.ValidationError





            let! middle = 

                aUnvalidatedTenantProvision.AdiminUserInfo.MiddleName 
                |> MiddleName.create' 
                |> Result.mapError ProvisionTenantError.ValidationError




            let! last = 

                aUnvalidatedTenantProvision.AdiminUserInfo.LastName 
                |> LastName.create' 
                |> Result.mapError ProvisionTenantError.ValidationError




            let! email = 
            
                aUnvalidatedTenantProvision.AdiminUserInfo.Email 
                |> EmailAddress.create' 
                |> Result.mapError ProvisionTenantError.ValidationError




            let! address = 

                aUnvalidatedTenantProvision.AdiminUserInfo.Address 
                |> PostalAddress.create' 
                |> Result.mapError ProvisionTenantError.ValidationError




            let! primePhone = 
            
                aUnvalidatedTenantProvision.AdiminUserInfo.PrimPhone 
                |> Telephone.create' 
                |> Result.mapError ProvisionTenantError.ValidationError




            let! secondPhone = 
            
                aUnvalidatedTenantProvision.AdiminUserInfo.SecondPhone 
                |> Telephone.create' 
                |> Result.mapError ProvisionTenantError.ValidationError





            let validTenantProvision = {

                TenanName = name
                TenantDescription = description
                TenantAdministratorFirsTName = first
                AdministratorMiddleName = middle
                AdministratorLastName = last
                AdministratorEmail = email
                AdministratorAddress = address
                AdministratorPrimTel = primePhone
                AdministratorSecondTel = secondPhone
    
            }


            return validTenantProvision


        }





///Step2 provision tenant  impl
let provision : ProvisionTenant = 

    fun  validatedProvision ->
        provisionTenantServiceImpl' validatedProvision.TenanName validatedProvision.TenantDescription validatedProvision.TenantAdministratorFirsTName  
                                                    validatedProvision.AdministratorMiddleName validatedProvision.AdministratorLastName validatedProvision.AdministratorEmail 
                                                    validatedProvision.AdministratorAddress validatedProvision.AdministratorPrimTel validatedProvision.AdministratorSecondTel
        |> Result.mapError (ProvisionTenantError.ProvisioningError)


        

///Step3 persist the provision data Tenant, Administrator User and Adminitrative Role 

let saveProvisionInfoLocal 
    (saveOneTenant:SaveOneTenant)
    (saveOneUser:SaveOneUser)
    (saveOneRole:SaveOneRole)
    (aProvision:Provision) = 


    let tenant, user, role =  aProvision



    let rsSaveOneTenant = 
        tenant 
        |> saveOneTenant 
        |> Result.mapError ProvisionTenantError.DbError

    let rsSaveOneRole = 
        role 
        |> saveOneRole    
        |> Result.mapError ProvisionTenantError.DbError 

    let rsSaveOneUser = 
        user 
        |> saveOneUser 
        |> Result.mapError ProvisionTenantError.DbError

    match rsSaveOneTenant with  
    | Ok () -> 
        match rsSaveOneRole with  
        | Ok () -> 
             match rsSaveOneUser with  
             | Ok () -> 
                Ok aProvision
             | Error error ->
                Error error
        | Error error ->
            Error error
    | Error error ->
        Error error


    

let saveProvisionInfo = saveProvisionInfoLocal saveOneTenant saveOneUser saveOneRole
        


        







///Step4 create events impl
let createEvents : CreateEvents = 

    fun aProvision ->

        let tenant, user, role, invs =  aProvision

        let tenantProvisionCreatedEvent = {
            TenantProvisioned = tenant
            RoleProvisioned = role
            UserRegistered = user
        }


        let tenantProvisionedEvent = TenantProvisionCreated tenantProvisionCreatedEvent



        let listOfOfferedRegInv = 
            invs
            |> List.map (
                
                fun aDomainRegInv -> 

                    let regInvDto = aDomainRegInv |> Dto.RegistrationInvitation.fromDomain 
                    let invOfferred : InvitationOffered = {TenantId = tenant.TenantId |> TenantId.value ; Invitation = regInvDto}
            
                    invOfferred 
                    |> TenantProvisionedEvent.InvitationOffered)
                    

        let listOfWithdrawnRegInv = 
            invs
            |> List.map (
                
                fun aDomainWithdrawnRegInv -> 

                    let regInvDto = aDomainWithdrawnRegInv |> Dto.RegistrationInvitation.fromDomain 
                    let invitationWithdrawn : InvitationWithdrawn = {TenantId = tenant.TenantId |> TenantId.value ; Invitation = regInvDto}
            
                    invitationWithdrawn 
                    |> TenantProvisionedEvent.InvitationWithdrawn)
        
        

        let r = List.append [tenantProvisionedEvent]  listOfOfferedRegInv  
        
        List.append r listOfWithdrawnRegInv




















///Provision workflow implementation
/// 
/// 
let provisionTenantWorflow: ProvisionTenantWorkflow = 

    fun unvalidatedTenantProvision ->

        let provision = Result.bind provision
        let createEvents = Result.map createEvents


        

        unvalidatedTenantProvision
        |> validateProvision
        |> provision
        |> createEvents
        







 
