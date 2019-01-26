namespace IdentityAndAcccess.ReastApi


open System
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave.Successful
open Suave
open Suave.Operators
open Suave.Filters


open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.WithdrawRegistrationInvitationApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.DomainApiTypes.Handlers
open RabbitMQ.Client.Impl

open IdentityAndAcccess.Workflow.OffertRegistrationInvitationApiTypes
open IdentityAndAcccess.Workflow.DeactivateTenantActivationStatusApiTypes
open IdentityAndAcccess.Workflow.ReactivateTenantActivationStatusApiTypes
open IdentityAndAcccess.Workflow.ProvisionGroupApiTypes
open IdentityAndAcccess.Workflow.ProvisionRoleApiTypes





[<AutoOpen>]
module Rest =



    type JsonString = string

    /// Very simplified version!
    type HttpResponse = {
        HttpStatusCode : int
        Body : JsonString 
        }


/// This function converts the workflow output into a HttpResponse
    let workflowResultToHttpReponse result = 
        match result with
        | Ok events ->
            // and serialize to JSON
            let json = JsonConvert.SerializeObject(events)
            let response = 
                {
                HttpStatusCode = 200
                Body = json
                }
            response
        | Error err -> 

            // turn domain errors into a dto
            //let dto = err |> PlaceOrderErrorDto.fromDomain
            // and serialize to JSON
            let json = JsonConvert.SerializeObject(err)
            let response = 
                {
                HttpStatusCode = 401
                Body = json
                }
            response




            
    ///Transform an type 'a into a webpart
    let toJsonWebPart v = 
        let settings = new JsonSerializerSettings()
        settings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
        JsonConvert.SerializeObject(v, settings)
        |> OK >=> Writers.setMimeType "application/json; charset=utf-8"

    let fromJson<'a> json = JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a
    

    let getResourceFromReq<'a> (req : HttpRequest) = 

        let data = req.rawForm 
                   |> System.Text.Encoding.UTF8.GetString 
                   |> fromJson<'a>
        data


    let toProvisionTenantCommand (up:UnvalidatedTenantProvision) : ProvisionTenantCommand =
        printfn "-----START-----"
        printfn " UnvalidatedTenantProvision =  %A " up
        printfn "-----ENDS-----" 
        let provisionTenantCommand : ProvisionTenantCommand = {
            Data = up
            TimeStamp = DateTime.Now
            UserId = "Felicien"
            }
        provisionTenantCommand
    

    let toOfferRegistrationInvitationCommand (up:UnvalidatedRegistrationInvitationDescription) : OfferRegistrationInvitationCommand =
        printfn "-----START-----"
        printfn " UnvalidatedRegistrationInvitationDescription =  %A " up
        printfn "-----ENDS-----" 
        let command : OfferRegistrationInvitationCommand = {
            Data = up
            TimeStamp = DateTime.Now
            UserId = "Felicien"
            }
        command

    let toWithdrawRegistrationInvitationCommand (uri:UnvalidatedRegistrationInvitationIdentifier) : WithdrawRegistrationInvitationCommand =
        printfn "-----START-----"
        printfn " UnvalidatedRegistrationInvitationIdentifier =  %A " uri
        printfn "-----ENDS-----" 
        let commd : WithdrawRegistrationInvitationCommand = {
            Data = uri
            TimeStamp = DateTime.Now
            UserId = "Felicien"
            }
        commd


    let toDeactivateTenantCommand (uacstat:UnvalidatedTenantActivationStatus) : DeactivateTenantActivationStatusCommand =
        printfn "-----START-----"
        printfn " UnvalidatedTenantActivationStatus =  %A " uacstat
        printfn "-----ENDS-----" 
        let cmmd : DeactivateTenantActivationStatusCommand = {
            Data = uacstat
            TimeStamp = DateTime.Now
            UserId = "Felicien"
            }
        cmmd


    let toReactivateTenantCommand (uacstatd:UnvalidatedTenantActivationStatusData) : ReactivateTenantActivationStatusCommand =
        printfn "-----START-----"
        printfn " UnvalidatedTenantActivationStatusData =  %A " uacstatd
        printfn "-----ENDS-----" 
        let cmmd : ReactivateTenantActivationStatusCommand = {
            Data = uacstatd
            TimeStamp = DateTime.Now
            UserId = "Felicien"
            }
        cmmd


    let toProvisionGroupCommand (ugpr:UnvalidatedGroup) : ProvisionGroupCommand =
        printfn "-----START-----"
        printfn " UnvalidatedGroup =  %A " ugpr
        printfn "-----ENDS-----" 
        let provisionTenantCommand : ProvisionGroupCommand = {
            Data = ugpr
            TimeStamp = DateTime.Now
            UserId = "Felicien"
            }
        provisionTenantCommand


    let toProvisionRoleCommand (ur:UnvalidatedRole) : ProvisionRoleCommand =
        printfn "-----START-----"
        printfn " UnvalidatedRole =  %A " ur
        printfn "-----ENDS-----" 
        let cmmd : ProvisionRoleCommand = {
            Data = ur
            TimeStamp = DateTime.Now
            UserId = "Felicien"
            }
        cmmd

 

    let app = choose [
        POST >=> choose [

            path "/tenant-provisions" 
                >=> request (getResourceFromReq<UnvalidatedTenantProvision> 
                >> toProvisionTenantCommand 
                >> ProvisionTenant.handleProvisionTenant 
                >> workflowResultToHttpReponse
                >> toJsonWebPart)

            path "/offert-invitations" 
                >=> request (getResourceFromReq<UnvalidatedRegistrationInvitationDescription> 
                >> toOfferRegistrationInvitationCommand 
                >> OfferRegistrationInvitationCommand.handleOfferRegistrationInvitation 
                >> workflowResultToHttpReponse
                >> toJsonWebPart)

            path "/withdraw-invitations" 
                >=> request (getResourceFromReq<UnvalidatedRegistrationInvitationIdentifier> 
                >> toWithdrawRegistrationInvitationCommand 
                >> WithdrawRegistrationInvitationCommand.handleWithdrawRegistrationInvitation 
                >> workflowResultToHttpReponse
                >> toJsonWebPart)

            path "/activate-tenant" 
                >=> request (getResourceFromReq<UnvalidatedTenantActivationStatusData> 
                >> toReactivateTenantCommand 
                >> ReactivateTenantActivationStatus.handleReactivateTenantActivationStatus 
                >> workflowResultToHttpReponse
                >> toJsonWebPart)

            path "/deactivate-tenant" 
                >=> request (getResourceFromReq<UnvalidatedTenantActivationStatus> 
                >> toDeactivateTenantCommand
                >> DeactivateTenantActivationStatus.handleDeactivateTenantActivationStatus 
                >> workflowResultToHttpReponse
                >> toJsonWebPart)           
                   ]
                ]