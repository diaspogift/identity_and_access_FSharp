namespace IdentityAndAcccess.Workflow.WithdrawRegistrationInvitationApiTypes

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Functions.RegistrationInvitations
open IdentityAndAcccess.DomainTypes





///Dependencies 
/// 
/// 









///Withdraw registration invitation worflow types
/// 
/// 


//Withdraw registration invitation workflow input types
type UnvalidatedRegistrationInvitationIdentifier = {
   RegistrationInvitationId : string
   TenantId : string
}




///Ouputs of the withdraw registration invitation worflow 
//- Sucess types




type RegistrationInvitationWithdrawnEvent = {
    TenantId : Dto.TenantId
    WithdrawnInvitation : Dto.RegistrationInvitation
    }




//- Failure types

type WithdrawRegistrationInvitationError = 
    | ValidationError of string
    | WithdrawInvitationError of string
    | DbError of string


//Worflow type 

type WithdrawRegistrationInvitationWorkflow = 
    Tenant -> UnvalidatedRegistrationInvitationIdentifier -> Result<RegistrationInvitationWithdrawnEvent, WithdrawRegistrationInvitationError>



//Step 1 Validation of RegistrationInvitationDescription

type ValidatedRegistrationInvitationIdentifier = {

   RegistrationInvitationId : RegistrationInvitationId
   TenantId : TenantId
}



type ValidateRegistrationInvitationIdentifier =

    UnvalidatedRegistrationInvitationIdentifier -> Result<ValidatedRegistrationInvitationIdentifier, WithdrawRegistrationInvitationError>







//Step 2 Withdraw registration invitation

type WithdrawRegistrationInvitation = 

    Tenant -> ValidatedRegistrationInvitationIdentifier -> Result<(Tenant*RegistrationInvitation), WithdrawRegistrationInvitationError>




module WithdrawRegistrationInvitationWorflowImplementation =


    ///Step 1 validate RegistrationInvitationIdentifier  impl
    let validateRegistrationInvitationIdentifier : ValidateRegistrationInvitationIdentifier =

        fun anUnvalidatedRegistrationInvitationIdentifier ->

            result {


                let! registrationInvitationId = 
                    anUnvalidatedRegistrationInvitationIdentifier.RegistrationInvitationId
                    |> RegistrationInvitationId.create'
                    |> Result.mapError WithdrawRegistrationInvitationError.ValidationError


                let! tenantId = 
                    anUnvalidatedRegistrationInvitationIdentifier.TenantId
                    |> TenantId.create'
                    |> Result.mapError WithdrawRegistrationInvitationError.ValidationError

                let validatedRegInv : ValidatedRegistrationInvitationIdentifier = 
                    {RegistrationInvitationId = registrationInvitationId; TenantId = tenantId}

                return validatedRegInv


            }





    ///Step2 withdraw registration invitation  impl
    let withdrawRegistrationInvitation : WithdrawRegistrationInvitation = 

        fun  aTenant validatedRegInvId ->
            validatedRegInvId.RegistrationInvitationId
            |> Tenant.withdrawRegistrationInvitation aTenant
            |> Result.mapError WithdrawRegistrationInvitationError.WithdrawInvitationError 

             
            

            



    type CreateEvents = (Tenant.Tenant*RegistrationInvitation) -> RegistrationInvitationWithdrawnEvent 

            


    ///Step4 create events impl
    let createEvents : CreateEvents = 

        fun tenantAndInvitation ->

            let tenant, withdrawnInvitation =  tenantAndInvitation

            let registrationInvitationOfferredEvent : RegistrationInvitationWithdrawnEvent = {
                TenantId = tenant.TenantId |> TenantId.value
                WithdrawnInvitation = withdrawnInvitation |> Dto.RegistrationInvitation.fromDomain
            }


            registrationInvitationOfferredEvent




    ///Wthdraw registration invitation workflow implementation
    /// 
    /// 
    let withdrawRegistrationInvitationWorkflow: WithdrawRegistrationInvitationWorkflow = 

        fun aTenant anUnvalidatedRegistrationInvitationIdentifier ->

            let withdrawRegistrationInvitation = withdrawRegistrationInvitation aTenant
            let withdrawRegistrationInvitation = Result.bind withdrawRegistrationInvitation
            let createEvents = Result.map createEvents




            anUnvalidatedRegistrationInvitationIdentifier
            |> validateRegistrationInvitationIdentifier
            |> withdrawRegistrationInvitation
            |> createEvents
            


         
            







     
