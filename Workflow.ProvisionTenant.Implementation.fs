namespace IdentityAndAcccess.Workflow.ProvisionTenantApiTypes

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Functions.Tenant
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAcccess.DomainTypes.Functions
open System
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes.Functions








type ProvisionTenantService = 
               StrongPasswordGeneratorService
                -> PasswordEncryptionService
                -> TenantName 
                -> TenantDescription 
                -> FirstName 
                -> MiddleName 
                -> LastName 
                -> EmailAddress 
                -> PostalAddress 
                -> Telephone 
                -> Telephone
                ->  Result<TenantProvision,string>









///Workflow  types
/// 
/// 



//Types for Step 1 - validation 


type UnvalidatedTenantProvision = {
    TenantInfo : UnvalidatedTenant
    AdminUserInfo : TenantAdministrator
    }
    and  UnvalidatedTenant = {
        Name : string
        Description : string
        }
    and TenantAdministrator = {
        FirstName : string
        MiddleName : string
        LastName : string
        Email : string
        Address : string
        PrimPhone : string 
        SecondPhone : string
        }


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


//- Sucess types
type TenantProvisionCreated = {
    TenantProvisioned : Tenant
    RoleProvisioned : Role
    UserRegistered : User
}

type InvitationWithdrawn = {
    TenantId : string
    Invitation : Dto.RegistrationInvitation
}

type InvitationOffered = {
    TenantId : string
    Invitation : Dto.RegistrationInvitation
}


type ProvisionAcknowledgementSent = {
        TenantId : TenantId
        Email : EmailAddress
}


type TenantProvisionedEvent = 
    | TenantProvisionCreated of TenantProvisionCreated
    | InvitationWithdrawn of InvitationWithdrawn
    | InvitationOffered of InvitationOffered
    | UserAssignedToRole of UserAssignedToRoleEvent
    | UserUnAssignedFromRole of UserUnAssignedFromRoleEvent
    | ProvisionAcknowledgementSent of ProvisionAcknowledgementSent



//- Failure types

type ProvisionTenantError = 
    | ValidationError of string
    | ProvisioningError of string
    | DbError of string



//Worflow funtions types 


//Types for Step 1 - validade
type ValidateTenantProvision = UnvalidatedTenantProvision -> Result<ValidatedTenantProvision, ProvisionTenantError>

//Types for Step 2 - provision
type ProvisionTenant = ValidatedTenantProvision -> Result<TenantProvision, ProvisionTenantError>

//Types for Step 3 - create events 
type CreateEvents = TenantProvision -> TenantProvisionedEvent list


type ProvisionTenantWorkflow = UnvalidatedTenantProvision -> Result<TenantProvisionedEvent list, ProvisionTenantError>





module ProvisionTenantWorflowImplementation =



    ///Dependencies  Implementations right here ad the edge of the workflow
    /// 
    /// 

    let strongPasswordService:StrongPasswordGeneratorService = 
        fun aPassword ->
            let unwrappedPassword = Password.value aPassword
            StrongPassword.create' unwrappedPassword

    let passwordEncryptionService:PasswordEncryptionService = 
        fun aStrongPassword ->
            let unwrappedPassword = StrongPassword.value aStrongPassword
            EncrytedPassword.create' unwrappedPassword

    let provisionTenantServiceLocal : ProvisionTenantService =
                                         
            fun strongPasswordService 
                passwordEncryptionService 
                aTenantName aTenantDescription 
                anAdministorFirstName 
                anAdministorMiddleName 
                anAdministorLastName
                anEmailAddress 
                aPostalAddress 
                aPrimaryTelephone 
                aSecondaryTelephone ->
                
                result {

                    let enablementSartDate = DateTime.Now
                    let enablementEndDate = enablementSartDate.AddDays(365.0)
                    let! invitationDescription = "Invitation for User TODDDDDOOOOOOOO ..."  |> RegistrationInvitationDescription.create' 

                    let! tenantToProvision = Tenant.createFullActivatedTenant aTenantName aTenantDescription

                    let! tenantWithRegistrationInvitation, 
                         offerredInvitation =  Tenant.offerRegistrationInvitation  tenantToProvision  invitationDescription

                    let! password = "123456" |> Password.create' 
                    let!  strongPassword = password |> strongPasswordService
                    let! encryptedPassword = strongPassword |> passwordEncryptionService
                    let! unwrappedEncryptedPassword = encryptedPassword |> EncrytedPassword.value |> Password.create'
                    let! adminUsername = "Default Aministrator" |> Username.create' 
                    let! adminUserEnablement = Enablement.fullCreate enablementSartDate enablementEndDate User.EnablementStatus.Enabled

                    let! adminUser = 
                        registerUserForTenant 
                            tenantWithRegistrationInvitation 
                            offerredInvitation.RegistrationInvitationId 
                            adminUsername  
                            unwrappedEncryptedPassword
                            adminUserEnablement anEmailAddress
                            aPostalAddress aPrimaryTelephone 
                            aSecondaryTelephone anAdministorFirstName
                            anAdministorMiddleName anAdministorLastName 
                                                
                    let! rstenant, withdrawnInvitation = 
                        withdrawRegistrationInvitation 
                            tenantWithRegistrationInvitation 
                            offerredInvitation.RegistrationInvitationId


                    let! adminRoleName = "SUPER_ADMINISTRATOR" |> RoleName.create'
                    let! adminRoleDescription = "SUPER_ADMINISTRATOR is a role that have access to all tenant'rsources" |> RoleDescription.create'

                    let! adminRole = Tenant.provisionRole rstenant adminRoleName adminRoleDescription

                    let! resultAssignUserToSuperAdminRole = Role.assignUser adminRole adminUser
              
                    //IO operation kept and the end ????

                    let provision:TenantProvision = {
                        Tenant = tenantToProvision
                        AdminUser = adminUser
                        AdminRole = adminRole
                        OfferredInvitation = offerredInvitation  
                        WithdrawnInvitation = withdrawnInvitation
                        }
                    
                        

                    return provision
                }
                  //(tenantToProvision, adminUser, resultAssignUserToSuperAdminRole, tenantWithRegistrationInvitation.RegistrationInvitations)

    //Partial application right there
    let provisionTenantService = provisionTenantServiceLocal strongPasswordService passwordEncryptionService






    ///Implementtation
    /// 
    /// 

    //Step 1 validate tenant provision impl
    let validate : ValidateTenantProvision =

        fun unvalidatedTenantProvision ->
            //printfn "THE COMMAND ============================== %A" aUnvalidatedTenantProvision
            result {
                let! name = 
                    unvalidatedTenantProvision.TenantInfo.Name 
                    |> TenantName.create' 
                    |> Result.mapError ProvisionTenantError.ValidationError
                let! description = 
                    unvalidatedTenantProvision.TenantInfo.Description 
                    |> TenantDescription.create' 
                    |> Result.mapError ProvisionTenantError.ValidationError
                let! first = 
                    unvalidatedTenantProvision.AdminUserInfo.FirstName 
                    |> FirstName.create' 
                    |> Result.mapError ProvisionTenantError.ValidationError
                let! middle = 
                    unvalidatedTenantProvision.AdminUserInfo.MiddleName 
                    |> MiddleName.create' 
                    |> Result.mapError ProvisionTenantError.ValidationError
                let! last = 
                    unvalidatedTenantProvision.AdminUserInfo.LastName 
                    |> LastName.create' 
                    |> Result.mapError ProvisionTenantError.ValidationError
                let! email = 
                    unvalidatedTenantProvision.AdminUserInfo.Email 
                    |> EmailAddress.create' 
                    |> Result.mapError ProvisionTenantError.ValidationError
                let! address = 
                    unvalidatedTenantProvision.AdminUserInfo.Address 
                    |> PostalAddress.create' 
                    |> Result.mapError ProvisionTenantError.ValidationError
                let! primePhone = 
                    unvalidatedTenantProvision.AdminUserInfo.PrimPhone 
                    |> Telephone.create' 
                    |> Result.mapError ProvisionTenantError.ValidationError
                let! secondPhone = 
                    unvalidatedTenantProvision.AdminUserInfo.SecondPhone 
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





    //Step2 provision tenant  impl
    let provision : ProvisionTenant = 

        fun  validatedProvision ->

            provisionTenantService 
                validatedProvision.TenanName 
                validatedProvision.TenantDescription 
                validatedProvision.TenantAdministratorFirsTName  
                validatedProvision.AdministratorMiddleName 
                validatedProvision.AdministratorLastName 
                validatedProvision.AdministratorEmail 
                validatedProvision.AdministratorAddress 
                validatedProvision.AdministratorPrimTel 
                validatedProvision.AdministratorSecondTel
                
            |> Result.mapError (ProvisionTenantError.ProvisioningError)


            

    //Step4 create events impl
    let createEvents : CreateEvents = 

        fun aProvision ->
            //let tenant, user, role, invs =  aProvision

            let tenant = aProvision.Tenant |> Tenant.fromDomain
            let user = aProvision.AdminUser |> User.fromDomain
            let userDesc = aProvision.AdminUser |> User.toDescriptor
            let role = aProvision.AdminRole |> Role.fromDomain
            let offerredInv = aProvision.OfferredInvitation
            let withdrawnInv = aProvision.WithdrawnInvitation

            let tenantProvisionCreatedEvent = {
                TenantProvisioned = tenant 
                RoleProvisioned = role 
                UserRegistered = user 
                }

            let ioff:InvitationOffered = {
                TenantId = tenant.TenantId 
                Invitation = offerredInv |> RegistrationInvitation.fromDomain
                }
            let iowi:InvitationWithdrawn = {
                TenantId = tenant.TenantId 
                Invitation = withdrawnInv |> RegistrationInvitation.fromDomain
                }
            let uassr:UserAssignedToRoleEvent = {
                RoleId = role.RoleId
                UserId = user.UserId
                AssignedUser = userDesc |> UserDescriptor.fromDomain
                }

            let tenantProvisionedEvent = tenantProvisionCreatedEvent |> TenantProvisionedEvent.TenantProvisionCreated |> List.singleton
            let listOfOfferedRegInv = ioff |> TenantProvisionedEvent.InvitationOffered |> List.singleton
            let listOfWithdrawnRegInv = iowi |> TenantProvisionedEvent.InvitationWithdrawn |> List.singleton
            let listUserAssignedToRole = uassr |> TenantProvisionedEvent.UserAssignedToRole |> List.singleton

            tenantProvisionedEvent @ listOfOfferedRegInv @ listOfWithdrawnRegInv @ listUserAssignedToRole

            




















    ///Provision workflow implementation
    /// 
    /// 
    let provisionTenantWorflow: ProvisionTenantWorkflow = 

        fun unvalidatedTenantProvision ->

            let provision = Result.bind provision
            let createEvents = Result.map createEvents


            

            unvalidatedTenantProvision
            |> validate
            |> provision
            |> createEvents
        







 
