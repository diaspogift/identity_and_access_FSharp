module IdentityAndAcccess.DomainApiTypes.ProvisionTenantWorflowImplementation

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainServices.Tenant
open IdentityAndAcccess.DomainApiTypes





//Dependencies 



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


        

///Step3 create events impl
let createEvents : CreateEvents = 

    fun aProvision ->

        let tenant, user, role =  aProvision

        let tenantProvisionCreatedEvent = {
            TenantProvisioned = tenant
            RoleProvisioned = role
            UserCreated = user
        }


        let tenantProvisionedEvent = TenantProvisionCreated tenantProvisionCreatedEvent

        [tenantProvisionedEvent]
























let ProvisionTenantWorflow: ProvisionTenantWorkflow = 

    fun unvalidatedTenantProvision ->


        let provision' = Result.bind provision
        let createEvents' = Result.map createEvents

        unvalidatedTenantProvision
        |> validateProvision
        |> provision'
        |> createEvents'
        //|> saveProvision







 
