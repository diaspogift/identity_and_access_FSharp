namespace IdentityAndAcccess.Workflow.RegisterUserApiTypes



open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Functions.Dto
open System
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Functions.Dto
open Suave.Logging
open Suave.Logging



///Register user to tenancy worflow types
/// 
/// 


//register user to tenancy workflow input types
type UnalidatedUserTenancy = {
    TenantId : string
    RegistrationInvitationId : string
    Username : string
    Password : string
    Email : string
    Address : string
    PrimPhone : string
    SecondPhone : string 
    FirstName : string
    MiddleName : string
    LastName : string
    }


type ValidatedUserTenancy = {
    RegistrationInvitationId : RegistrationInvitationId
    Username : Username
    Password : Password
    Enablement : User.Enablement
    Email : EmailAddress
    Address : PostalAddress
    PrimPhone : Telephone
    SecondPhone : Telephone 
    FirstName : FirstName
    MiddleName : MiddleName
    LastName : LastName
    }

//- Sucess types




type UserRegisteredToTenancyEvent = { 
    UserId : string
    TenantId : string
    RegisteredUser : Dto.User
    }


//- Failure types

type RegisterUserToTenancyError = 
    | ValidationError of string
    | RegisterError of string
    | DbError of string


//Worflow type 

type RegisterUserToTenancyWorkflow = 
    Tenant.Tenant -> UnalidatedUserTenancy -> Result<UserRegisteredToTenancyEvent , RegisterUserToTenancyError>


















//Step 1 - validate tenancy 
type ValidateUserTenancy = 
    UnalidatedUserTenancy -> Result<ValidatedUserTenancy, RegisterUserToTenancyError>


//Step 1 - register user to tenancy 
type RegisterUserToTenancy = 

    Tenant.Tenant -> ValidatedUserTenancy -> Result<User.User*User.UserDescriptor, RegisterUserToTenancyError>



//Step 3 - Create events step

type CreateEvents = User.User * User.UserDescriptor -> UserRegisteredToTenancyEvent












module RegisterUserToTenancyWorfklowImplementation = 


    ///Step 1 validate group info to add to impl
    let validate : ValidateUserTenancy = fun unvalidatedUserTenancy  ->

        result {


            let! registrationInvitationId = 
                unvalidatedUserTenancy.RegistrationInvitationId 
                |> RegistrationInvitationId.create'
                |> Result.mapError RegisterUserToTenancyError.ValidationError


            let! username = 
                unvalidatedUserTenancy.Username 
                |> Username.create'
                |> Result.mapError RegisterUserToTenancyError.ValidationError

            let! password = 
                unvalidatedUserTenancy.Password 
                |> Password.create'
                |> Result.mapError RegisterUserToTenancyError.ValidationError


            let! email = 
                unvalidatedUserTenancy.Email 
                |> EmailAddress.create'
                |> Result.mapError RegisterUserToTenancyError.ValidationError

            let! address = 
                unvalidatedUserTenancy.Address 
                |> PostalAddress.create'
                |> Result.mapError RegisterUserToTenancyError.ValidationError

            let! primePhone = 
                unvalidatedUserTenancy.PrimPhone 
                |> Telephone.create'
                |> Result.mapError RegisterUserToTenancyError.ValidationError

            let! secondPhone = 
                unvalidatedUserTenancy.SecondPhone 
                |> Telephone.create'
                |> Result.mapError RegisterUserToTenancyError.ValidationError

            let! firstName = 
                unvalidatedUserTenancy.FirstName 
                |> FirstName.create'
                |> Result.mapError RegisterUserToTenancyError.ValidationError

            let! middleName = 
                unvalidatedUserTenancy.MiddleName 
                |> MiddleName.create'
                |> Result.mapError RegisterUserToTenancyError.ValidationError

            let! lastName = 
                unvalidatedUserTenancy.LastName 
                |> LastName.create'
                |> Result.mapError RegisterUserToTenancyError.ValidationError


            let enanblement:User.Enablement = {
                EnablementStatus = User.EnablementStatus.Enabled
                StartDate =  DateTime.Now
                EndDate = DateTime.Now.AddYears(1)
            } 

            let validatedUserTenancy:ValidatedUserTenancy = {
                RegistrationInvitationId = registrationInvitationId
                Username = username
                Password = password
                Enablement = enanblement
                Email = email
                Address = address 
                PrimPhone = primePhone
                SecondPhone = secondPhone
                FirstName = firstName
                MiddleName = middleName
                LastName = lastName
                }

            return validatedUserTenancy
        }
    
    ///Step2 register user to tenancy impl
    let register : RegisterUserToTenancy = fun aTenantToRegisterTo validatedUserTenancy ->
           let a = Tenant.registerUser aTenantToRegisterTo validatedUserTenancy.RegistrationInvitationId validatedUserTenancy.Username 
                    validatedUserTenancy.Password validatedUserTenancy.Enablement validatedUserTenancy.Email validatedUserTenancy.Address 
                    validatedUserTenancy.PrimPhone validatedUserTenancy.SecondPhone validatedUserTenancy.FirstName 
                    validatedUserTenancy.MiddleName  validatedUserTenancy.LastName 
            //let b = 9
            
           a|> Result.mapError RegisterUserToTenancyError.RegisterError

           

    ///Step3 create events impl
    let createEvents : CreateEvents = 
        fun (aUser, aUserDesc) ->

           let regUser =  aUser |> User.fromDomain
           
           let userRegisteredTenancyEvent : UserRegisteredToTenancyEvent = {
               UserId = aUser.UserId |> UserId.value
               TenantId = aUser.TenantId |> TenantId.value
               RegisteredUser = regUser
           }
           userRegisteredTenancyEvent

            
                        

     

    ///Register user to tenancy workflow implementation
    /// 
    /// 
    let registerUserToTenancyWorkflow: RegisterUserToTenancyWorkflow = 
        fun tenant unvalidatedUserTenancy ->
            
            let register = register tenant
            let register = Result.bind register

            let createEvents = Result.map createEvents
            let b = 
                unvalidatedUserTenancy 
                |> validate
                |> register
                |> createEvents

            b
            
            







     
